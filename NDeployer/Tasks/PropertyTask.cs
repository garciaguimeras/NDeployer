using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
    class PropertyTask : Task
    {

        string name;
        string value;
		string defValue;
		string filename;

		public PropertyTask(TaskDef rootNode) : base(rootNode)
        {
            name = null;
            value = null;
			defValue = null;
			filename = null;
        }

		public override bool IsValidTaskDef()
        {
            name = RootNode.AttributeByName("name");
			value = RootNode.AttributeByName("value");
			defValue = RootNode.AttributeByName("default");
			filename = RootNode.AttributeByName("filename");

			if (filename != null && name == null && value == null && defValue == null)
                return true;
			
			if (filename == null && name != null && value != null)
				return true;
			
            return false;
        }

		private void AddProperty(string name, string value)
		{
			environment.AddProperty(name, value);
			bool result = PropertyEvaluator.EvalProperty(name);
			if (!result)
			{
				if (defValue == null)
					environment.AddToErrorList("Error evaluating property '{0}'. Execution suspended.", name);
				else
				{
					environment.AddProperty(name, defValue);
				}
			}
		}

        public override void Execute()
        {
			// Simple case: just name and value!
			if (filename == null)
			{
				AddProperty(name, value);
				return;
			}

			// Not so simple case: Read from a properties file
			string fName = PropertyEvaluator.EvalValue(filename);
			if (!File.Exists(fName))
			{
				environment.AddToErrorList("Filename does not exist '{0}'. Execution suspended.", fName);
				return;
			}

			Dictionary<string, string> properties = PropertyFileReader.Read(fName);
			if (properties == null)
			{
				environment.AddToErrorList("Error parsing properties file '{0}'. Execution suspended.", fName);
				return;
			}

			foreach (string key in properties.Keys)
				AddProperty(key, properties[key]);
		}
    }
}
