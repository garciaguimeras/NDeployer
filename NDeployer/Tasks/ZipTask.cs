using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ionic.Zip;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{

    class ZipTask : ContextTask
    {

		string zipFilename;
		string toDir;

		public ZipTask(TaskDef rootNode) : base(rootNode)
        {
			zipFilename = null;
			toDir = null;
        }

		public override bool IsValidTaskDef()
        {
			zipFilename = GetAttribute(RootNode, "filename");
			toDir = GetAttribute(RootNode, "toDir");
			if (zipFilename == null)
			{
				AddAttributeNotFoundError("filename");
				return false;
			}
            return true;
        }

        public override void Execute()
        {
			zipFilename = PropertyEvaluator.EvalValue(zipFilename);
			if (zipFilename == null)
            {
                environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
                return;
            }
			if (toDir != null)
			{
				toDir = PropertyEvaluator.EvalValue(toDir);
				if (toDir == null)
				{
					environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
					return;
				}
			}
            
			string fileDir = Path.GetDirectoryName(zipFilename);
			if (!Directory.Exists(fileDir))
				Directory.CreateDirectory(fileDir);

			if (File.Exists(zipFilename))
				File.Delete(zipFilename);

			// Create tmp dir
			string tmpDir = zipFilename + ".tmpdir";
			if (Directory.Exists(tmpDir))
				FileUtil.DeleteDirectoryRecursively(tmpDir);

			// Create destDir
			string destDir = tmpDir;
			if (toDir != null)
				destDir = Path.Combine(destDir, toDir);			
			Directory.CreateDirectory(destDir);

			List<Dictionary<string, string>> zipped = new List<Dictionary<string, string>>();

			// Copy all files to tmp dir
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.FilterStandardPipe("include", "exclude");
			foreach (Dictionary<string, string> data in input)
            {
				if (!data.ContainsKey("filename") || !File.Exists(data["filename"])) 
				{
					string name = data.ContainsKey("filename") ? data["filename"] : "";
					environment.Pipe.AddToErrorPipe("Filename does not exist: {0}", name);
					continue;
				}

				string filename = data["filename"];

				// Is flatten?
				bool flatten = data.ContainsKey("flatten") && data["flatten"].Equals("");
				string relativeDir = !flatten && data.ContainsKey("relativePath") ? data["relativePath"] : ".";
				relativeDir = FileUtil.FixDirectorySeparator(relativeDir);

				// Change relative dir?
				string cd = data.ContainsKey("changeRelativeDir") ? data["changeRelativeDir"] : "";
				cd = FileUtil.FixDirectorySeparator(cd);
				if (relativeDir.Equals(cd))
					relativeDir = "";
				else
				{
					if (relativeDir.StartsWith(cd + Path.DirectorySeparatorChar))
					{
						relativeDir = relativeDir.Substring(cd.Length + 1);
					}
				}

                // Logger.info(2, "Deploying file {0}", filename);

				string fName = Path.GetFileName(filename);
                string fullDestDir = Path.Combine(destDir, relativeDir);
				string destFile = Path.Combine(fullDestDir, fName);

				data.Add("zipped", "");
				zipped.Add(data);

				if (!Directory.Exists(fullDestDir))
					Directory.CreateDirectory(fullDestDir);
                File.Copy(filename, destFile, true);
            }

			// Zip
			ZipFile zipFile = new ZipFile(zipFilename);
			zipFile.AddDirectory(tmpDir);
			zipFile.Save();
			zipFile.Dispose();

			// Remove tmp dir
			FileUtil.DeleteDirectoryRecursively(tmpDir);

			// Execute children tasks
			environment.BeginContext(new Pipe(zipped));
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
			environment.EndContext();
        }

    }
}
