using System.Collections.Generic;

using NDeployer.Script;

namespace NDeployer
{

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

		private Context context;
		private MetaAttribute metaAttribute;

        public Pipe Pipe { get { return context.Pipe; } }
		public Dictionary<string, PropertyItem> Properties { get { return context.GetProperties();  } }
		public Dictionary<string, string> MetaAttributes { get { return metaAttribute.MetaAttributes; } }

        public static Environment GetEnvironment()
        {
            if (instance == null)
                instance = new Environment();
            return instance;
        }

        private Environment()
        {
			context = new Context(null);
			metaAttribute = new MetaAttribute();
			FillSystemEnvironmentProperties();
        }

		private void FillSystemEnvironmentProperties()
		{
			context.AddProperty(HOSTNAME, SystemEnvironmentProperties.HostName);
			context.AddProperty(USERNAME, SystemEnvironmentProperties.UserName);
		}

		public void AddMetaAttribute(string key, string value)
		{
			metaAttribute.AddMetaAttribute(key, value);
		}

		public string GetMetaAttribute(string key)
		{
			return metaAttribute.GetMetaAttribute(key);
		}

        public void AddProperty(string name, string value)
        {
			context.AddProperty(name, value);
        }

        public PropertyItem GetProperty(string name)
        {
			return context.GetProperty(name);
        }

		public void AddFunction(string name)
		{
			context.AddFunction(name);
		}

		public void AddFunctionParameter(string name, string paramName)
		{
			context.AddFunctionParameter(name, paramName);
		}

		public void AddFunctionTasks(string name, IEnumerable<TaskDef> tasks)
		{
			context.AddFunctionTasks(name, tasks);
		}

		public FunctionInfo GetFunction(string name)
		{
			return context.GetFunction(name);
		}

		public void BeginContext()
		{
			Context newContext = new Context(context);
			context = newContext;
		}

		public void BeginContext(Pipe initialPipe)
		{
			Context newContext = new Context(context, initialPipe);
			context = newContext;
		}

		public void EndContext()
		{
			context = context.Parent;
		}

    }
}
