using System.Collections.Generic;

using NDeployer.Script;
using System;

namespace NDeployer
{
	class PropertyItem
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string EvalValue { get; set; }
	}

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

		public void AddTaskDefs(IEnumerable<TaskDef> taskDefs)
		{
			tasks.AddRange(taskDefs);
		}
	}

	class Context
	{

		Context parent;
		Dictionary<string, PropertyItem> properties;
		Dictionary<string, FunctionInfo> functions;
		Pipe pipe;

		public Context Parent { get { return parent; } }
		public Pipe Pipe { get { return pipe; } }

		public Context(Context parent, Pipe initialPipe)
		{
			this.parent = parent;

			properties = new Dictionary<string, PropertyItem>();
			functions = new Dictionary<string, FunctionInfo>();
			pipe = initialPipe;
		}

		public Context(Context parent) : this(parent, new Pipe())
		{}

		public void AddProperty(string name, string value)
		{
			if (properties.ContainsKey(name))
				properties.Remove(name);
			properties.Add(name, new PropertyItem { Name = name, Value = value, EvalValue = null });
		}

		public PropertyItem GetProperty(string name)
		{
			if (properties.ContainsKey(name))
				return properties[name];

			if (parent != null)
				return parent.GetProperty(name);

			return null;
		}

		public void AddFunction(string name)
		{
			if (functions.ContainsKey(name))
				functions.Remove(name);
			functions.Add(name, new FunctionInfo(name));
		}

		public void AddFunctionParameter(string name, string paramName)
		{
			if (!functions.ContainsKey(name))
				return;
			functions[name].AddParameter(paramName);
		}

		public void AddFunctionTasks(string name, IEnumerable<TaskDef> tasks)
		{
			if (!functions.ContainsKey(name))
				return;
			functions[name].AddTaskDefs(tasks);
		}

		public FunctionInfo GetFunction(string name)
		{
			if (functions.ContainsKey(name))
				return functions[name];

			if (parent != null)
				return parent.GetFunction(name);

			return null;
		}	

		public Dictionary<string, PropertyItem> GetProperties()
		{
			Dictionary<string, PropertyItem> fullProps = new Dictionary<string, PropertyItem>(properties);
			if (parent != null)
			{
				Dictionary<string, PropertyItem> parentProps = parent.GetProperties();
				foreach (string key in parentProps.Keys)
				{
					if (!fullProps.ContainsKey(key))
						fullProps.Add(key, parentProps[key]);
					else
						fullProps[key] = parentProps[key];
				}
			}
			return fullProps;
		}

	}
}

