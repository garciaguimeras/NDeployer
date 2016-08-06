using System;

using NDeployer.Script;

namespace NDeployer.Tasks
{
	class MetaAttributeTask : Task
	{
		string name;
		string value;

		public MetaAttributeTask(TaskDef rootNode) : base(rootNode)
		{
			name = null;
			value = null;
		}

		public override bool IsValidTaskDef()
		{
			name = RootNode.AttributeByName("name");
			value = RootNode.AttributeByName("value");
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

