using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class PopPipeTask : Task
	{
		public PopPipeTask(string name) : base(name)
		{}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			return true;
		}

		public override void Execute()
		{
			environment.PopPipe();
		}
	}
}

