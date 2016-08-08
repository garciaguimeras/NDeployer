using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Script;

namespace NDeployer.Tasks
{
    abstract class Task
    {

        protected Environment environment;

        public string Name { get; protected set; }
		public TaskDef RootNode { get; protected set; }

		public abstract bool IsValidTaskDef();

        public abstract void Execute();

		public Task(TaskDef rootNode)
        {
			environment = Environment.GetEnvironment();
			RootNode = rootNode;
			Name = rootNode.Name;
        }

		protected string GetAttribute(TaskDef rootNode, string attrName)
        {
            string attrValue = rootNode.AttributeByName(attrName);
            return attrValue;
        }

		protected void AddAttributeNotFoundError(string attrName)
		{
			environment.Pipe.AddToErrorPipe("{0} - Not found attribute: {1}", this.GetType().Name, attrName);
		}

		protected void InvalidAttributeValueError(string attrName, string attrValue)
		{
			environment.Pipe.AddToErrorPipe("{0} - Invalid attribute value. Attribute: {1}, Value: {2}", this.GetType().Name, attrName, attrValue);
		}

		protected void AddOneAttributeMandatoryError(params string[] attrNames)
		{
			string text = "";
			if (attrNames.Count() > 0)
				text = attrNames[0];
			for (int i = 1; i < attrNames.Count(); i++)
				text += ", " + attrNames[i];
			environment.Pipe.AddToErrorPipe("{0} - One of these attributes is mandatory: {1}", this.GetType().Name, text);
		}

    }

	abstract class ContextTask : Task
	{
	
		public ContextTask(TaskDef rootNode) : base(rootNode)
		{}

		public void LoadMetaAttributes(IEnumerable<TaskDef> children)
		{
			foreach (TaskDef element in children.Where(t => t.Name.Equals("meta-attr")))
			{
				Task t = TaskFactory.CreateTask(element);
				if (!t.IsValidTaskDef())
					environment.Pipe.AddToErrorPipe("Meta attribute incorrectly defined. Must use attributes 'name' and 'value', or at least 'name'. Execution suspended.");
				else
					t.Execute();
			}
		}

		public void LoadProperties(IEnumerable<TaskDef> children)
		{
			foreach (TaskDef element in children.Where(t => t.Name.Equals("property")))
			{
				Task t = TaskFactory.CreateTask(element);
				if (!t.IsValidTaskDef())
					environment.Pipe.AddToErrorPipe("Property incorrectly defined. Must use attributes 'name' and 'value', or else 'filename'. Execution suspended.");
				else
					t.Execute();
			}
		}

		protected void ExecuteContext(IEnumerable<TaskDef> children)
		{
			if (environment.Pipe.Error.Count() > 0)
			{
				environment.Pipe.PrintErrorPipe();
				return;
			}

			IEnumerable<TaskDef> elements = children.Where(t => !t.Name.Equals("property") && !t.Name.Equals("meta-attr"));
			foreach (TaskDef child in elements)
			{
				string name = child.Name;
				Task t = TaskFactory.CreateTask(child);
				if (t == null)
				{
					environment.Pipe.AddToErrorPipe("Could not find any task for tag: {0}", name);
					continue;
				}

				if (t.IsValidTaskDef())
				{
					t.Execute();
				}

				if (environment.Pipe.Error.Count() > 0)
				{
					environment.Pipe.PrintErrorPipe();
					return;
				}
			}
		}

	}

	enum ContextStrategy
	{
		NEW,
		KEEP,
		CLONE
	}

	abstract class GeneratorTask : ContextTask
	{

		protected ContextStrategy ContextStrategy { get; set; }

		public abstract void ExecuteGenerator();

		public GeneratorTask(TaskDef rootNode) : base(rootNode)
		{
			ContextStrategy = ContextStrategy.NEW;
		}

		public override void Execute()
		{
			// ContextStrategy.NEW
			Pipe p = new Pipe();

			switch (ContextStrategy)
			{
				case ContextStrategy.KEEP:
					p = environment.Pipe;
					break;
				case ContextStrategy.CLONE:
					p = environment.Pipe.Clone();
					break;
			}

			environment.BeginContext(p);
			ExecuteGenerator();
			environment.EndContext();
		}

	}
}
