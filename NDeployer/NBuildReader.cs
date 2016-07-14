using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using NDeployer.Tasks;
using NDeployer.Util;

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
                TaskFactory.CreateTaskForTag("property").ProcessXml(element);
            }
        }

        private void ValidateDocument(XElement root)
        {
            if (root == null)
            {
                environment.Pipe.AddToErrorPipe("Invalid build file. Can't find document root");
                return;
            }

            ReadProperties(root);
            bool result = PropertyEvaluator.EvalAllProperties();
            if (!result)
            {
                environment.Pipe.AddToErrorPipe("Error evaluating properties. Execution suspended.");
                return;
            }
        }

        public void Execute()
        {
            XElement root = GetDocumentRoot();
            ValidateDocument(root);
            if (environment.Pipe.Error.Count() > 0)
            {
                environment.Pipe.PrintErrorPipe();
                return;
            }

            IEnumerable<XElement> elements = root.Elements().Where(e => !e.Name.ToString().Equals("property"));
            foreach (XElement child in elements)
            {

				Console.WriteLine(child.Name);

                if (environment.Pipe.Error.Count() > 0)
                {
                    environment.Pipe.PrintErrorPipe();
                    return;
                }
                environment.Pipe.SwitchPipes();

                string name = child.Name.ToString();
                Task t = TaskFactory.CreateTaskForTag(name);
                if (t == null)
                {
                    environment.Pipe.AddToErrorPipe("Could not find any task for tag: {0}", name);
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
