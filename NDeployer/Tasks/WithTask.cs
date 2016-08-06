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

		public WithTask(TaskDef taskDef) : base(taskDef)
		{
			KeepContext = true;
		}

		public override bool IsValidTaskDef()
		{
			return true;
		}

		public override void ExecuteGenerator()
		{
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
		}

	}
}

