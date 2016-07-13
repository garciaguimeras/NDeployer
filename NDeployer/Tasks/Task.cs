using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NDeployer.Tasks
{
    abstract class Task
    {

        protected Environment environment;

        public string Name { get; protected set; }

        public abstract bool ProcessXml(XElement rootNode);

        public abstract void Execute();

        public Task(string name)
        {
			environment = Environment.GetEnvironment();
			Name = name;
        }

        protected string GetAttribute(XElement rootNode, string attrName)
        {
            XAttribute attr = rootNode.Attribute(attrName);
            if (attr == null)
            {
                environment.Pipe.AddToErrorPipe("{0}: Not found attribute '{1}'", this.GetType().Name, attrName);
                return null;
            }
            if (string.IsNullOrEmpty(attr.Value))
            {
				environment.Pipe.AddToErrorPipe("{0}: Attribute is empty: '{1}'", this.GetType().Name, attrName);
                return null;
            }
            return attr.Value;
        }

    }
}
