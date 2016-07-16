using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer
{
    class Program
    {

        private const string BUILD_FILENAME = @"/home/noel/Projects/NDeployer/NDeployer/Resources/NBuild.xml";

        static void Main(string[] args)
        {
			ScriptFile scriptFile = ScriptFactory.GetScriptForFilename(BUILD_FILENAME);
			TaskDef rootTaskDef = scriptFile.Parse(BUILD_FILENAME);

			RootTask rootTask = new RootTask();
			rootTask.ProcessTaskDef(rootTaskDef);
			rootTask.Execute();
        }
    }
}
