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

	static class SystemEnvironmentProperties
	{
		public static string OS
		{ 
			get { return System.Environment.OSVersion.Platform.ToString(); }
		}

		public static string HostName
		{
			get { return System.Environment.MachineName; }
		}

		public static string UserName
		{
			get { return System.Environment.GetEnvironmentVariable("USERNAME"); }
		}

	}

    class Environment
    {
		public const string HOSTNAME = "ENV.HOST";
		public const string USERNAME = "ENV.USER";

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

			FillSystemEnvironmentProperties();
        }

		private void FillSystemEnvironmentProperties()
		{
			AddProperty(HOSTNAME, SystemEnvironmentProperties.HostName);
			AddProperty(USERNAME, SystemEnvironmentProperties.UserName);
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
			if (pipe != null)
				pipeStack.Push(pipe.Clone());
		}

		public void PopPipe()
		{
			IEnumerable<Dictionary<string, string>> errorPipe = pipe.Error;

			if (pipeStack.Count > 0)
			{
				pipe = pipeStack.Pop();
				foreach (Dictionary<string, string> data in errorPipe)
					pipe.AddToErrorPipe(data["error"]);
			}
		}

		public void NewPipe()
		{
			pipe = new Pipe();
		}

		public void NewPipe(List<Dictionary<string, string>> stdData)
		{
			pipe = new Pipe();
			foreach (Dictionary<string, string> item in stdData)
				pipe.AddToStandardPipe(item);
		}
    }
}
