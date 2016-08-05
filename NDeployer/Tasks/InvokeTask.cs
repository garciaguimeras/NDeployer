using System;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class InvokeTask : Task
	{

		string functionName;

		public InvokeTask(string name) : base(name)
		{
			functionName = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
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
			functionName = PropertyEvaluator.EvalValue(functionName);
			if (functionName == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			FunctionInfo function = environment.GetFunction(functionName);
			if (function == null)
			{
				environment.Pipe.AddToErrorPipe("Trying to invoke non-existing function {0}. Execution suspended.", functionName);
				return;
			}

			// Invoke function in a new context
			environment.BeginContext();
			ExecuteContext(function.Tasks);
			environment.EndContext();
		}
	}
}

