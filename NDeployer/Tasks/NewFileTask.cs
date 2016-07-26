using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class NewFileTask : Task
	{

		string filename;
		TaskDef root;

		public NewFileTask(string name) : base(name)
		{
			filename = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			filename = GetAttribute(rootNode, "name");
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
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			environment.PushPipe();
			environment.NewPipe();
			ExecuteContext(root.TaskDefs);

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

			environment.PopPipe();
		}

	}
}

