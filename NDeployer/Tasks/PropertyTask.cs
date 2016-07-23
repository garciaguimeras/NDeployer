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
		string filename;

		public PropertyTask(string name) : base(name)
        {
            name = null;
            value = null;
			filename = null;
        }

        public override bool ProcessTaskDef(TaskDef rootNode)
        {
            name = rootNode.AttributeByName("name");
			value = rootNode.AttributeByName("value");
			filename = rootNode.AttributeByName("filename");

			if (filename != null && name == null && value == null)
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
				environment.Pipe.AddToErrorPipe("Error evaluating property '{0}'. Execution suspended.", name);
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
				environment.Pipe.AddToErrorPipe("Filename does not exist '{0}'. Execution suspended.", fName);
				return;
			}

			Dictionary<string, string> properties = PropertyFileReader.Read(fName);
			if (properties == null)
			{
				environment.Pipe.AddToErrorPipe("Error parsing properties file '{0}'. Execution suspended.", fName);
				return;
			}

			foreach (string key in properties.Keys)
				AddProperty(key, properties[key]);
		}
    }
}
