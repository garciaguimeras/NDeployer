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
		
		public FlattenTask(string name) : base(name)
		{}

		public override bool ProcessXml(XElement rootNode)
		{
			return true;
		}

		public override void Execute()
		{
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Input;
			foreach (Dictionary<string, string> data in input) 
			{
				data.Add("flatten", "");
				environment.Pipe.AddToOuputPipe(data);
			}
		}

	}
}

