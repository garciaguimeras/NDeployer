using System.Collections.Generic;

using NDeployer.Script;

namespace NDeployer
{

	class FunctionInfo
	{
		string name;
		Dictionary<string, PropertyItem> parameters;
		List<TaskDef> tasks;

		public string Name { get { return name; } }
		public Dictionary<string, PropertyItem> Parameters { get { return parameters; } }
		public IEnumerable<TaskDef> Tasks { get { return tasks; } }

		public FunctionInfo(string functionName)
		{
			name = functionName;
			parameters = new Dictionary<string, PropertyItem>();
			tasks = new List<TaskDef>();
		}

		public void AddParameter(string paramName)
		{
			parameters.Add(paramName, new PropertyItem { Name = paramName, Value = "", EvalValue = null });
		}

		public void AddTasks(IEnumerable<TaskDef> taskDefs)
		{
			tasks.AddRange(taskDefs);
		}
	}

}

