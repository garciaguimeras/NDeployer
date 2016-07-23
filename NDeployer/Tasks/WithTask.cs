using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class WithTask : Task
	{

		TaskDef root;

		public WithTask(string name) : base(name)
		{}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			return true;
		}

		public override void Execute()
		{
			environment.PushPipe();
			ExecuteContext(root.TaskDefs);
			environment.PopPipe();
		}

	}
}

