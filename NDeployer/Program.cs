using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Tasks;

namespace NDeployer
{
    class Program
    {

        private const string BUILD_FILENAME = @"/home/noel/Projects/NDeployer/NDeployer/Resources/NBuild.xml";

        static void Main(string[] args)
        {
			RootTask rootTask = new RootTask(BUILD_FILENAME);
			rootTask.Execute();
        }
    }
}
