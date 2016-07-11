using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace NDeployer.Tasks
{
	class FlattenTask : Task
	{
		
		public FlattenTask(Environment environment) : base(environment)
		{
			Name = "flatten";
		}

		public override bool ProcessXml(XElement rootNode)
		{
			return true;
		}

		public override void Execute()
		{
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Input;
			foreach (Dictionary<string, string> data in input) 
			{
				if (data.ContainsKey("relativePath")) 
				{
					data["relativePath"] = ".";
				}
				environment.Pipe.AddToOuputPipe(data);
			}
		}

	}
}

