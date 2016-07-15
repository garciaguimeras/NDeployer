using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NDeployer.Tasks
{
    abstract class Task
    {

        protected Environment environment;

        public string Name { get; protected set; }

        public abstract bool ProcessXml(XElement rootNode);

        public abstract void Execute();

        public Task(string name)
        {
			environment = Environment.GetEnvironment();
			Name = name;
        }

        protected string GetAttribute(XElement rootNode, string attrName)
        {
            XAttribute attr = rootNode.Attribute(attrName);
            if (attr == null)
                return null;
            return attr.Value;
        }

		protected void AddAttributeNotFoundError(string attrName)
		{
			environment.Pipe.AddToErrorPipe("{0} - Not found attribute: {1}", this.GetType().Name, attrName);
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

		protected void ExecuteContext(IEnumerable<XElement> elements)
		{
			foreach (XElement child in elements)
			{
				if (environment.Pipe.Error.Count() > 0)
				{
					environment.Pipe.PrintErrorPipe();
					return;
				}

				string name = child.Name.ToString();
				Task t = TaskFactory.CreateTaskForTag(name);
				if (t == null)
				{
					environment.Pipe.AddToErrorPipe("Could not find any task for tag: {0}", name);
					continue;
				}

				if (t.ProcessXml(child))
				{
					t.Execute();
				}
			}
		}

    }

	abstract class GeneratorTask : Task
	{

		public abstract void ExecuteGenerator();

		public GeneratorTask(string name) : base(name)
		{}

		public override void Execute()
		{
			environment.PushPipe();
			environment.NewPipe();
			ExecuteGenerator();
			environment.PopPipe();
		}

	}
}
