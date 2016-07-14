using NDeployer.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer
{

    class PropertyItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string EvalValue { get; set; }
    }

    class Environment
    {

        private static Environment instance = null;

        Dictionary<string, PropertyItem> properties;
		Stack<Pipe> pipeStack;
        Pipe pipe;

        public Dictionary<string, PropertyItem> Properties { get { return properties;  } }
        public Pipe Pipe { get { return pipe; } }

        public static Environment GetEnvironment()
        {
            if (instance == null)
                instance = new Environment();
            return instance;
        }

        private Environment()
        {
            properties = new Dictionary<string, PropertyItem>();
			pipeStack = new Stack<Pipe>();
            pipe = new Pipe();
        }

        public void AddProperty(string name, string value)
        {
            properties.Add(name, new PropertyItem { Name = name, Value = value, EvalValue = null });
        }

        public PropertyItem GetProperty(string name)
        {
            try
            {
                return properties[name];
            }
            catch (KeyNotFoundException)
            { }
            return null;
        }

		public void PushPipe()
		{
			pipeStack.Push(pipe.Clone());
		}

		public void PopPipe()
		{
			if (pipeStack.Count > 0)
				pipe = pipeStack.Pop();
		}

		public void ClearPipe()
		{
			pipe = new Pipe();
		}
    }
}
