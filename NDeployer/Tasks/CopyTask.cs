using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{

    class CopyTask : ContextTask
    {

        string deployDir;
		string baseDir;

		public CopyTask(TaskDef taskDef) : base(taskDef)
        {
            deployDir = null;
        }

		public override bool IsValidTaskDef()
        {
            deployDir = GetAttribute(RootNode, "todir");
			baseDir = GetAttribute(RootNode, "basedir");
			if (deployDir == null)
			{
				AddAttributeNotFoundError("todir");
				return false;
			}
            return true;
        }

        public override void Execute()
        {
            deployDir = PropertyEvaluator.EvalValue(deployDir);
            if (deployDir == null)
            {
                environment.AddToErrorList("Error evaluating attributes. Execution suspended.");
                return;
            }

			baseDir = baseDir != null ? PropertyEvaluator.EvalValue(baseDir) : "";
			if (baseDir != null)
				baseDir = FileUtil.FixDirectorySeparator(baseDir);
			else
				baseDir = "";
            
            // Logger.info("Deploying to directory: {0}", deployDir);
            if (!Directory.Exists(deployDir))
            {
                // Logger.info(2, "Creating new directory: {0}", deployDir);
                Directory.CreateDirectory(deployDir);
            }

			List<Dictionary<string, string>> copied = new List<Dictionary<string, string>>();

			IEnumerable<Dictionary<string, string>> input = environment.Pipe.FilterStandardPipe("exclude");
			foreach (Dictionary<string, string> data in input)
            {
				if (!data.ContainsKey("filename"))
					continue;
				
				if (!File.Exists(data["filename"])) 
				{
					string name = data.ContainsKey("filename") ? data["filename"] : "";
					environment.AddToErrorList("Filename does not exist: {0}", name);
					continue;
				}

				string filename = data["filename"];

				// Is flatten?
				bool flatten = data.ContainsKey("flatten");
				string destDir = !flatten && data.ContainsKey("relativePath") ? data["relativePath"] : ".";
				destDir = FileUtil.FixDirectorySeparator(destDir);

				// Change relative dir?
				if (destDir.Equals(baseDir))
					destDir = "";
				else
				{
					if (destDir.StartsWith(baseDir + Path.DirectorySeparatorChar))
					{
						destDir = destDir.Substring(baseDir.Length + 1);
					}
				}

                // Logger.info(2, "Deploying file {0}", filename);

				string fName = Path.GetFileName(filename);
                string fullDestDir = Path.Combine(deployDir, destDir);
				string destFile = Path.Combine(fullDestDir, fName);

				data.Add("copied-to", destDir);
				copied.Add(data);

				if (!Directory.Exists(fullDestDir))
					Directory.CreateDirectory(fullDestDir);
                File.Copy(filename, destFile, true);
            }

			// Execute children tasks
			environment.BeginContext(new Pipe(copied));
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
			environment.EndContext();
        }

    }
}
