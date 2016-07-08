using NDeployer.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NDeployer
{
    class NBuildReader
    {

        string Filename { get; set; }
        Environment environment = Environment.GetEnvironment();

        public NBuildReader(string filename)
        { 
            Filename = filename;
        }

        private XElement GetDocumentRoot()
        {
            XDocument xDoc = XDocument.Load(Filename);
            IEnumerable<XElement> elements = xDoc.Elements("xml");
            if (elements.Count() != 1)
                return null;
            return elements.First();
        }

        private void ReadProperties(XElement root)
        {
            foreach (XElement element in root.Elements("property"))
            {
                string name = element.Attribute("name").Value;
                string value = element.Value;
                environment.AddProperty(name, value);
            }
        }

        public void Execute()
        {
            XElement root = GetDocumentRoot();
            if (root == null)
            {
                Logger.error("Invalid build file. Can't find document root");
                return;
            }

            ReadProperties(root);
            bool result = PropertyEvaluator.EvalAllProperties();
            if (!result)
            {
                Logger.error("Error evaluating properties. Execution suspended.");
                return;
            }

            IEnumerable<XElement> elements = root.Elements().Where(e => !e.Name.ToString().Equals("property"));
            foreach (XElement child in elements)
            {
                string name = child.Name.ToString();
                Task t = environment.GetTask(name);

                if (t == null)
                {
                    Logger.error("Could not find any task for tag: {0}", name);
                    continue;
                }

                if (t.ProcessXml(child))
                {
                    t.Execute();
                }
            }
        }

    }
}
