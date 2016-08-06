using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class FlattenTask : Task
	{
		
		public FlattenTask(TaskDef rootNode) : base(rootNode)
		{}

		public override bool IsValidTaskDef()
		{
			return true;
		}

		public override void Execute()
		{
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Std;
			foreach (Dictionary<string, string> data in input) 
			{
				data.Add("flatten", "");
				if (data.ContainsKey("changeRelativeDir"))
					data.Remove("changeRelativeDir");
			}
		}

	}
}

