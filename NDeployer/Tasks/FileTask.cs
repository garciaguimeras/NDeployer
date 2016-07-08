using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace NDeployer.Tasks
{

    class FileTaskData
    {
        public string FileName { get; set; }
        public string RelativePath { get; set; }
    }

    class FileTask : Task
    {

        string filename;
        List<FileTaskData> data;

        public FileTask()
        {
            Name = "file";
            filename = null;
            data = new List<FileTaskData>();
        }

        public override bool ProcessXml(XElement rootNode)
        {
            filename = GetAttribute(rootNode, "name");
            if (filename == null)
                return false;
            return true;
        }

        private void ReadDirectory(string path, string relativePath)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
                data.Add(new FileTaskData { FileName = f, RelativePath = relativePath });

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string dName = Path.GetFileName(dir);
                string dRelativePath = relativePath + Path.DirectorySeparatorChar + dName;
                ReadDirectory(dir, dRelativePath);
            }
        }

        public override void Execute()
        {
            if (File.Exists(filename))
            {
                data.Add(new FileTaskData { FileName = filename, RelativePath = "." });
                return;
            }

            if (Directory.Exists(filename))
            {
                ReadDirectory(filename, ".");
                return;
            }

            Logger.error("File or directory does not exist: {0}", filename);
        }

        public override object GetData()
        {
            return data;
        }
    }
}
