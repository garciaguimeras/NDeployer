using System;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class InvokeTask : ContextTask
	{

		string functionName;

		public InvokeTask(TaskDef rootNode) : base(rootNode)
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
			LoadMetaAttributes(function.Tasks);
 			LoadProperties(function.Tasks);
			ExecuteContext(function.Tasks);
			environment.EndContext();
		}
	}
}

