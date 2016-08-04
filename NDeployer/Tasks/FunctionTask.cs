using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class FunctionTask : Task
	{

		TaskDef root;
		string functionName;

		public FunctionTask(string name) : base(name)
		{
			root = null;
			functionName = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			functionName = GetAttribute(rootNode, "name");
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

			environment.AddFunctionTasks(functionName, root.TaskDefs);
		}
	}
}

