using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class MetaAttributeTask : Task
	{
		string name;
		string value;

		public MetaAttributeTask(string name) : base(name)
		{
			name = null;
			value = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			name = rootNode.AttributeByName("name");
			value = rootNode.AttributeByName("value");
			if (name == null)
				return false;
			return true;
		}

		public override void Execute()
		{
			environment.AddMetaAttribute(name, value);
		}
	}
}

