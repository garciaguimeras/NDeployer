using System.Collections.Generic;

using NDeployer.Script;
using NDeployer.Lang;

namespace NDeployer
{
	class PropertyItem
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string EvalValue { get; set; }
	}

	class Context
	{

		Context parent;
		Dictionary<string, PropertyItem> properties;
		Dictionary<string, FunctionInfo> functions;
		Dictionary<string, string> metaAttributes;
		Pipe pipe;

		public Context Parent { get { return parent; } }
		public Pipe Pipe { get { return pipe; } }

		public Context(Context parent, Pipe initialPipe)
		{
			this.parent = parent;

			properties = new Dictionary<string, PropertyItem>();
			functions = new Dictionary<string, FunctionInfo>();
			metaAttributes = new Dictionary<string, string>();
			pipe = initialPipe;
		}

		public Context(Context parent) : this(parent, new Pipe())
		{}

		public void AddMetaAttribute(string name, string value)
		{
			if (metaAttributes.ContainsKey(name))
				metaAttributes.Remove(name);
			metaAttributes.Add(name, value);
		}

		public string GetMetaAttribute(string name)
		{
			if (metaAttributes.ContainsKey(name))
				return metaAttributes[name];

			if (parent != null)
				return parent.GetMetaAttribute(name);

			return null;
		}

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

		public Dictionary<string, string> GetMetaAttributes()
		{
			Dictionary<string, string> fullMetaAttrs = new Dictionary<string, string>(metaAttributes);
			if (parent != null)
			{
				Dictionary<string, string> parentMetaAttrs = parent.GetMetaAttributes();
				foreach (string key in parentMetaAttrs.Keys)
				{
					if (!fullMetaAttrs.ContainsKey(key))
						fullMetaAttrs.Add(key, parentMetaAttrs[key]);
					else
						fullMetaAttrs[key] = parentMetaAttrs[key];
				}
			}
			return fullMetaAttrs;
		}

		public Dictionary<string, FunctionInfo> GetFunctions()
		{
			Dictionary<string, FunctionInfo> fullFunctions = new Dictionary<string, FunctionInfo>(functions);
			if (parent != null)
			{
				Dictionary<string, FunctionInfo> parentFunctions = parent.GetFunctions();
				foreach (string key in parentFunctions.Keys)
				{
					if (!fullFunctions.ContainsKey(key))
						fullFunctions.Add(key, parentFunctions[key]);
					else
						fullFunctions[key] = parentFunctions[key];
				}
			}
			return fullFunctions;
		}

	}
}

