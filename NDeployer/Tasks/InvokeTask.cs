using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class InvokeTask : Task
	{
		public InvokeTask(string name) : base(name)
		{
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			return true;
		}

		public override void Execute()
		{
		}
	}
}

