using System;
using System.IO;
using System.Linq;
using System.Reflection;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer
{
	class ProgramInfo
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public string Copyright { get; set; }
	}

    class Program
    {

		private ProgramInfo GetAssemblyInfo()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = assembly.GetName();
			AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute));
			AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
			return new ProgramInfo
			{
				Name = assemblyName.Name,
				Version = assemblyName.Version.ToString(),
				Description = description.Description,
				Copyright = copyright.Copyright
			};
		}

		private void PrintHelp()
		{
			Console.WriteLine("Usage: NDeployer [option]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("    -f <build file>    Runs a build file");
			Console.WriteLine("    -help              Prints this help");
			Console.WriteLine("    -info              Prints app version and copyright information");
			Console.WriteLine();
		}

		private void RunBuildFile(string filename)
		{
			Environment environment = Environment.GetEnvironment();

			if (!File.Exists(filename))
			{
				Console.WriteLine("Error: File not found {0}", filename);
				return;
			}
			ScriptFile scriptFile = ScriptFactory.GetScriptForFilename(filename);
			if (scriptFile == null)
			{
				Console.WriteLine("Error: Invalid file type or extension {0}", filename);
				return;
			}

			TaskDef rootTaskDef = scriptFile.Parse(filename);
			if (rootTaskDef == null)
			{
				Console.WriteLine("Error: Could not parse file {0}", filename);
				return;
			}

			RootTask rootTask = new RootTask();
			rootTask.ProcessTaskDef(rootTaskDef);
			if (environment.Pipe.Error.Count() > 0)
			{
				environment.Pipe.PrintErrorPipe();
				return;
			}
			rootTask.Execute();
		}

		private void CheckOptions(string[] args, ProgramInfo programInfo)
		{
			int totalParams = args.Count();
			if (totalParams == 0)
			{
				Console.WriteLine("Error: Missing option");
				Console.WriteLine();
				PrintHelp();
				return;
			}

			string option = args[0];
			switch (option)
			{
				case "-f":
					if (totalParams < 2)
					{
						Console.WriteLine("Error: Missing build file name");
						Console.WriteLine();
						PrintHelp();
						return;
					}
					RunBuildFile(args[1]);
					break;
				
				case "-help":
					PrintHelp();
					break;

				case "-info":
					Console.WriteLine("Current version: {0}", programInfo.Version);
					Console.WriteLine(programInfo.Copyright);
					break;
				
				default:
					Console.WriteLine("Error: Bad option '{0}'", option);
					PrintHelp();
					break;
			}
		}

		public void Execute(string[] args)
		{
			ProgramInfo programInfo = GetAssemblyInfo();
			Console.WriteLine("{0} v{1}", programInfo.Name, programInfo.Version);
			Console.WriteLine(programInfo.Description);
			Console.WriteLine();
			CheckOptions(args, programInfo);
		}

        static void Main(string[] args)
        {
			Program program = new Program();
			program.Execute(args);
        }
    }
}
