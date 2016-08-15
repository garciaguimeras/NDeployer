using System;

using NDeployer.Script;

namespace NDeployer.Lang
{
	class Property
	{
		string name;
		string value;
		string defaultValue;
		string evalValue;

		public string Name { get { return name; } }
		public string Value { get { return value; } }
		public string DefaultValue { get { return defaultValue; } }
		public string EvalValue { get { return evalValue; } }

		public Property(string name, string value, string defaultValue)
		{
			this.name = name;
			this.value = value;
			this.defaultValue = defaultValue;
			this.evalValue = null;
		}

		public static Property Load(TaskDef rootNode)
		{
			string name = rootNode.AttributeByName("name");
			string value = rootNode.AttributeByName("value");
			string defaultValue = rootNode.AttributeByName("default");
			string filename = rootNode.AttributeByName("filename");

			if (name == null)
				throw LangError.MissingAttribute("name");

			return new Property(name, value, defaultValue);
		}
	}
}

