using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class NewFileTask : ContextTask
	{

		string filename;

		public NewFileTask(TaskDef taskDef) : base(taskDef)
		{
			filename = null;
		}

		public override bool IsValidTaskDef()
		{
			filename = GetAttribute(RootNode, "name");
			if (filename == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			// Evaluate filename property
			filename = PropertyEvaluator.EvalValue(filename);
			if (filename == null)
			{
				AddErrorEvaluatingAttribute("name");
				return;
			}

			environment.BeginContext();
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);

			using (StreamWriter writer = new StreamWriter(File.OpenWrite(filename)))
			{
				foreach (Dictionary<string, string> data in environment.Pipe.Std)
				{
					if (data.ContainsKey("print"))
					{
						writer.WriteLine(data["print"]);
					}
				}
			}

			environment.EndContext();
		}

	}
}

