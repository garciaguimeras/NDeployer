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
		List<Task> propertyTasks;

		public RootTask() : base("xml")
		{
			root = null;
			propertyTasks = new List<Task>();
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			if (root == null)
			{
				environment.Pipe.AddToErrorPipe("Invalid build file. Can't find document root");
				return false;
			}

			foreach (TaskDef element in root.TaskDefsByName("property"))
			{
				Task t = TaskFactory.CreateTaskForTag("property");
				if (!t.ProcessTaskDef(element))
				{
					environment.Pipe.AddToErrorPipe("Property incorrectly defined. Must use  attributes 'name' and 'value', or else 'filename'. Execution suspended.");
					return false;
				}
				propertyTasks.Add(t);
			}

			return true;
		}

		public override void ExecuteGenerator()
		{
			foreach (Task t in propertyTasks)
			{
				t.Execute();
				if (environment.Pipe.Error.Count() > 0)
				{
					environment.Pipe.PrintErrorPipe();
					return;
				}
			}

			IEnumerable<TaskDef> elements = root.TaskDefs.Where(t => !t.Name.Equals("property"));
			ExecuteContext(elements);
		}

	}
}

