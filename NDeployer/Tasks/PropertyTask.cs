using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NDeployer.Tasks
{
    class PropertyTask : Task
    {

        string name;
        string value;

		public PropertyTask(string name) : base(name)
        {
            name = null;
            value = null;
        }

        public override bool ProcessXml(XElement rootNode)
        {
            XAttribute nameAttr = rootNode.Attribute("name");
            if (nameAttr == null)
                return false;
            name = nameAttr.Value;
            value = rootNode.Value;
            environment.AddProperty(name, value);
            return true;
        }

        public override void Execute()
        {}
    }
}
