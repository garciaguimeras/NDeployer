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
			environment.AddToErrorList("{0} - Not found attribute: {1}", this.GetType().Name, attrName);
		}

		protected void AddInvalidAttributeValueError(string attrName, string attrValue)
		{
			environment.AddToErrorList("{0} - Invalid attribute value. Attribute: {1}, Value: {2}", this.GetType().Name, attrName, attrValue);
		}

		protected void AddErrorEvaluatingAttribute(string attrName)
		{
			environment.AddToErrorList("{0} - Error evaluating attribute: {1}", this.GetType().Name, attrName);
		}

		protected void AddOneAttributeMandatoryError(params string[] attrNames)
		{
			string text = "";
			if (attrNames.Count() > 0)
				text = attrNames[0];
			for (int i = 1; i < attrNames.Count(); i++)
				text += ", " + attrNames[i];
			environment.AddToErrorList("{0} - One of these attributes is mandatory: {1}", this.GetType().Name, text);
		}

    }

	abstract class ContextTask : Task
	{
	
		public ContextTask(TaskDef rootNode) : base(rootNode)
		{}

		private void LoadElement(IEnumerable<TaskDef> children, string name)
		{
			foreach (TaskDef element in children.Where(t => t.Name.Equals(name)))
			{
				Task t = TaskFactory.CreateTask(element);
				if (t.IsValidTaskDef())
					t.Execute();
			}
		}

		public void LoadImports(IEnumerable<TaskDef> children)
		{
			LoadElement(children, "import");
		}

		public void LoadFunctions(IEnumerable<TaskDef> children)
		{
			LoadElement(children, "function");
		}

		public void LoadMetaAttributes(IEnumerable<TaskDef> children)
		{
			LoadElement(children, "meta-attr");
		}

		public void LoadProperties(IEnumerable<TaskDef> children)
		{
			LoadElement(children, "property");
		}

		protected void ExecuteContext(IEnumerable<TaskDef> children)
		{
			if (environment.Errors.Count() > 0)
				return;

			IEnumerable<TaskDef> elements = children.Where(t => !t.Name.Equals("property") && 
				                                                !t.Name.Equals("meta-attr") && 
				                                                !t.Name.Equals("import") &&
				                                               !t.Name.Equals("function"));
			foreach (TaskDef child in elements)
			{
				string name = child.Name;
				Task t = TaskFactory.CreateTask(child);
				if (t == null)
				{
					environment.AddToErrorList("Could not find any task for tag: {0}", name);
					return;
				}

				if (t.IsValidTaskDef())
					t.Execute();

				if (environment.Errors.Count() > 0)
					return;
			}
		}

	}

	enum ContextStrategy
	{
		NEW_PIPE,
		KEEP_PIPE,
		CLONE_PIPE	
	}

	abstract class GeneratorTask : ContextTask
	{

		protected ContextStrategy ContextStrategy { get; set; }

		public abstract void ExecuteGenerator();

		public GeneratorTask(TaskDef rootNode) : base(rootNode)
		{
			ContextStrategy = ContextStrategy.NEW_PIPE;
		}

		public override void Execute()
		{
			// ContextStrategy.NEW_PIPE
			Pipe p = new Pipe();

			switch (ContextStrategy)
			{
				case ContextStrategy.KEEP_PIPE:
					p = environment.Pipe;
					break;
			    
				case ContextStrategy.CLONE_PIPE:
					p = environment.Pipe.Clone();
					break;
			}

			environment.BeginContext(p);
			ExecuteGenerator();
			environment.EndContext();
		}

	}
}
