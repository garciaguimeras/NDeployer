using System;

using NDeployer.Script;

namespace NDeployer.Lang
{
	class MetaAttribute
	{
		string name;
		string value;

		public string Name { get { return name; } }
		public string Value { get { return value; } }

		public MetaAttribute(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		public static MetaAttribute Load(TaskDef rootNode)
		{
			string name = rootNode.AttributeByName("name");
			string value = rootNode.AttributeByName("value");

			if (name == null)
				throw LangError.MissingAttribute("name");

			return new MetaAttribute(name, value);
		}
	}
}

