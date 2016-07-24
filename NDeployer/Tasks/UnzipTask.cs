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

		private string UnzipFile(string filename)
		{
			string baseDir = filename + ".tmpdir";

			if (Directory.Exists(baseDir))
				FileUtil.DeleteDirectoryRecursively(baseDir);
						
			ZipFile zipFile = new ZipFile(filename);
			foreach (ZipEntry entry in zipFile.Entries)
			{
				Console.WriteLine(entry.FileName);

				string extractedFilename = baseDir + Path.DirectorySeparatorChar + entry.FileName;
				if (entry.IsDirectory)
				{
					Directory.CreateDirectory(extractedFilename);
					continue;
				}

				//Path.GetDirectoryName(extractedFilename)
				int size = 0;
				byte[] buffer = new byte[1024];
				Stream inStream = entry.OpenReader();
				FileStream f = File.OpenWrite(extractedFilename);

				while ((size = inStream.Read(buffer, 0, buffer.Length)) > 0)
					f.Write(buffer, 0, size);

				f.Close();
				inStream.Close();

				// Add filename + relativePath
				string relativePath = FileUtil.GetRelativePath(extractedFilename, baseDir);
				AddToStandardPipe(extractedFilename, relativePath);
			}
			zipFile.Dispose();

			return baseDir;
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

			// Execute tasks in context
			ExecuteContext(root.TaskDefs);

			// Remove temp directory
			FileUtil.DeleteDirectoryRecursively(tmpDirName);
		}
	}
}

