using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Script;

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

        public override bool ProcessTaskDef(TaskDef rootNode)
        {
            name = rootNode.AttributeByName("name");
			value = rootNode.AttributeByName("value");
			if (name == null || value == null)
                return false;
            environment.AddProperty(name, value);
            return true;
        }

        public override void Execute()
        {}
    }
}
