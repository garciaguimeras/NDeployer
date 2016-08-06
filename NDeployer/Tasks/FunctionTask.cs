using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class FunctionTask : Task
	{

		string functionName;

		public FunctionTask(TaskDef rootNode) : base(rootNode)
		{
			functionName = null;
		}

		public override bool IsValidTaskDef()
		{
			functionName = GetAttribute(RootNode, "name");
			if (functionName == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			environment.AddFunction(functionName);

			// TODO: Add function parameters

			environment.AddFunctionTasks(functionName, RootNode.Children);
		}
	}
}

