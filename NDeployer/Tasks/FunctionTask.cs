﻿using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class FunctionTask : Task
	{

		string functionName;

		public FunctionTask(string name) : base(name)
		{
			functionName = null;
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

