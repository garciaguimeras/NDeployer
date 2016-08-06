using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class ChangeRelativeDirTask : Task
	{

		string path;

		public ChangeRelativeDirTask(TaskDef taskDef) : base(taskDef)
		{
			path = null;
		}

		public override bool IsValidTaskDef()
		{
			path = GetAttribute(RootNode, "path");
			if (path == null)
			{
				AddAttributeNotFoundError("path");
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Std;
			foreach (Dictionary<string, string> data in input) 
			{
				data.Add("changeRelativeDir", path);
				if (data.ContainsKey("flatten"))
					data.Remove("flatten");
			}
		}

	}
}

