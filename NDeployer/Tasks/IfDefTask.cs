using System;

using NDeployer.Script;
using NDeployer.Tasks;
using NDeployer.Util;

namespace NDeployer.Tasks
{

	enum IfDefType
	{
		none,
		property,
		metaattribute,
		function
	}

	abstract class BasicIfTask : GeneratorTask
	{

		protected string name;
		protected IfDefType type;

		public BasicIfTask(TaskDef taskDef) : base(taskDef)
		{
			ContextStrategy = ContextStrategy.KEEP;

			name = null;
			type = IfDefType.none;
		}

		public override bool IsValidTaskDef()
		{
			name = GetAttribute(RootNode, "name");
			string t = GetAttribute(RootNode, "type");
			if (name == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
			if (t == null)
			{
				AddAttributeNotFoundError("type");
				return false;
			}

			try
			{
				type = (IfDefType)Enum.Parse(typeof(IfDefType), t);
			}
			catch (Exception)
			{
				type = IfDefType.none;
			}

			if (type == IfDefType.none)
			{
				InvalidAttributeValueError("type", t);
				return false;
			}

			return true;
		}

		protected abstract bool CheckCondition();

		public override void ExecuteGenerator()
		{
			name = PropertyEvaluator.EvalValue(name);
			if (name == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			bool condition = CheckCondition();

			if (condition)
			{	
				LoadMetaAttributes(RootNode.Children);
				LoadProperties(RootNode.Children);
				ExecuteContext(RootNode.Children);
			}
		}
	}

	class IfDefTask : BasicIfTask
	{

		public IfDefTask(TaskDef taskDef) : base(taskDef)
		{}

		protected override bool CheckCondition()
		{
			bool defined = false;

			switch (type)
			{
				case IfDefType.property:
					defined = environment.GetProperty(name) != null;
					break;

				case IfDefType.metaattribute:
					defined = environment.GetMetaAttribute(name) != null;
					break;

				case IfDefType.function:
					defined = environment.GetFunction(name) != null;
					break;
			}

			return defined;
		}

	}

	class IfNotDefTask : BasicIfTask
	{

		public IfNotDefTask(TaskDef taskDef) : base(taskDef)
		{}

		protected override bool CheckCondition()
		{
			bool defined = true;

			switch (type)
			{
				case IfDefType.property:
					defined = environment.GetProperty(name) == null;
					break;

				case IfDefType.metaattribute:
					defined = environment.GetMetaAttribute(name) == null;
					break;

				case IfDefType.function:
					defined = environment.GetFunction(name) == null;
					break;
			}

			return defined;
		}

	}

}

