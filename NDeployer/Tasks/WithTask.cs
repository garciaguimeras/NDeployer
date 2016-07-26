using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class WithTask : GeneratorTask
	{

		TaskDef root;

		public WithTask(string name) : base(name)
		{
			KeepContext = true;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			return true;
		}

		public override void ExecuteGenerator()
		{
			ExecuteContext(root.TaskDefs);
		}

	}
}

