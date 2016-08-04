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

	enum ProgramFlag
	{
		EXECUTE,
		INFO
	}

    class Program
    {

		const string LICENSE = "Released under GNU General Public License";

		private ProgramInfo GetAssemblyInfo()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = assembly.GetName();
			object[] description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
			object[] copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			return new ProgramInfo
			{
				Name = assemblyName.Name,
				Version = assemblyName.Version.ToString(),
				Description = description.Length > 0 ? (description[0] as AssemblyDescriptionAttribute).Description : "",
				Copyright = copyright.Length > 0 ? (copyright[0] as AssemblyCopyrightAttribute).Copyright : "",
			};
		}

		private void PrintHelp()
		{
			Console.WriteLine("Usage: NDeployer [option]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("    -f <build file> <flag>   Runs a build file");
			Console.WriteLine("    -help                    Prints this help");
			Console.WriteLine("    -info                    Prints app version and copyright information");
			Console.WriteLine();
			Console.WriteLine("Flags:");
			Console.WriteLine("    -e                       Execute build file (by default)");
			Console.WriteLine("    -i                       Print meta attributes defined on build file");
			Console.WriteLine();
		}

		private void PrintMetaAttributes()
		{
			Environment environment = Environment.GetEnvironment();
			foreach (string k in environment.MetaAttributes.Keys)
			{
				Console.WriteLine("{0}: {1}", k, environment.MetaAttributes[k]);
			}
			Console.WriteLine();
		}

		private void RunBuildFile(string filename, ProgramFlag flag)
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

			rootTask.LoadMetaAttributes();
			if (flag == ProgramFlag.INFO)
			{
				PrintMetaAttributes();
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

					ProgramFlag flag = ProgramFlag.EXECUTE;
					if (totalParams > 2)
					{
						switch (args[2])
						{
							case "-i":
								flag = ProgramFlag.INFO;
								break;
							case "-e":
								flag = ProgramFlag.EXECUTE;
								break;
						}
					}

					RunBuildFile(args[1], flag);
					break;
				
				case "-help":
					PrintHelp();
					break;

				case "-info":
					Console.WriteLine(programInfo.Copyright);
					Console.WriteLine(LICENSE);
					Console.WriteLine();
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
