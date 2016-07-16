using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Tasks;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class RootTask : GeneratorTask
	{

		TaskDef root;

		public RootTask() : base("xml")
		{
			root = null;
		}

		private void ReadProperties()
		{
			foreach (TaskDef element in root.TaskDefsByName("property"))
			{
				TaskFactory.CreateTaskForTag("property").ProcessTaskDef(element);
			}
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			if (root == null)
			{
				environment.Pipe.AddToErrorPipe("Invalid build file. Can't find document root");
				return false;
			}

			ReadProperties();
			bool result = PropertyEvaluator.EvalAllProperties();
			if (!result)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating properties. Execution suspended.");
				return false;
			}

			return true;
		}

		public override void ExecuteGenerator()
		{
			if (environment.Pipe.Error.Count() > 0)
			{
				environment.Pipe.PrintErrorPipe();
				return;
			}

			IEnumerable<TaskDef> elements = root.TaskDefs.Where(t => !t.Name.Equals("property"));
			ExecuteContext(elements);
		}

	}
}

