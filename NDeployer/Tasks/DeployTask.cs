using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NDeployer.Tasks
{

    class DeployTask : Task
    {

        string deployDir;
        List<FileTask> fileTasks;

        public DeployTask()
        {
            Name = "deploy";
            deployDir = null;
            fileTasks = new List<FileTask>();
        }

        public override bool ProcessXml(XElement rootNode)
        {
            deployDir = GetAttribute(rootNode, "todir");
            if (deployDir == null)
                return false;

            foreach (XElement child in rootNode.Elements("file"))
            {
                FileTask fileTask = new FileTask();
                fileTask.ProcessXml(child);
                fileTasks.Add(fileTask);
            }

            return true;
        }

        public override void Execute()
        {
            deployDir = PropertyEvaluator.EvalValue(deployDir);
            if (deployDir == null)
            {
                Logger.error("Error evaluating attributes. Execution suspended.");
                return;
            }
            
            Logger.info("Deploying to directory: {0}", deployDir);
            if (!Directory.Exists(deployDir))
            {
                Logger.info(2, "Creating new directory: {0}", deployDir);
                Directory.CreateDirectory(deployDir);
            }

            foreach (FileTask fileTask in fileTasks)
            {
                fileTask.Execute();
                List<FileTaskData> data = (List<FileTaskData>)fileTask.GetData();
                foreach (FileTaskData fData in data)
                {
                    Logger.info(2, "Deploying file {0}", fData.FileName);

                    string fName = Path.GetFileName(fData.FileName);
                    string destDir = Path.Combine(deployDir, fData.RelativePath);
                    string destFile = Path.Combine(destDir, fName);

                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);
                    File.Copy(fData.FileName, destFile, true);
                }
            }
        }

        public override object GetData()
        {
            return null;
        }

    }
}
