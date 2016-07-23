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
	class UnzipTask : GeneratorTask
	{
		string filename;
		TaskDef root;

		public UnzipTask(string name) : base(name)
		{
			filename = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			root = rootNode;
			filename = GetAttribute(rootNode, "filename");
			if (filename == null)
			{
				AddAttributeNotFoundError("filename");
				return false;
			}
			return true;
		}

		private void AddToStandardPipe(string filename, string relativePath)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("filename", filename);
			data.Add("relativePath", relativePath);
			environment.Pipe.AddToStandardPipe(data);
		}

		private void ReadDirectory(string path)
		{
			IEnumerable<string> files = FileUtil.ReadDirectoryRecursively(path);
			foreach (string f in files)
			{
				// Add filename + relativePath
				string relativePath = FileUtil.GetRelativePath(f, path);
				AddToStandardPipe(f, relativePath);
			}
		}

		private string UnzipFile(string filename)
		{
			string dirName = filename + ".tmpdir";

			if (Directory.Exists(dirName))
				FileUtil.DeleteDirectoryRecursively(dirName);
						
			ZipFile zipFile = new ZipFile(filename);
			zipFile.ExtractAll(dirName);
			zipFile.Dispose();

			return dirName;
		}

		public override void ExecuteGenerator()
		{
			// Evaluate filename property
			filename = PropertyEvaluator.EvalValue(filename);
			if (filename == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			if (!File.Exists(filename))
			{
				// Add an error :(
				environment.Pipe.AddToErrorPipe("File does not exist: {0}", filename);
				return;
			}

			if (!ZipFile.IsZipFile(filename))
			{
				// Add an error :(
				environment.Pipe.AddToErrorPipe("File is not a zipfile: {0}", filename);
				return;
			}

			// Unzip file into temp directory and add to pipe
			string tmpDirName = UnzipFile(filename);
			ReadDirectory(tmpDirName);

			// Execute tasks in context
			ExecuteContext(root.TaskDefs);

			// Remove temp directory
			FileUtil.DeleteDirectoryRecursively(tmpDirName);
		}
	}
}

