using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer
{
    class Program
    {

        private const string BUILD_FILENAME = @"/home/noel/Documents/NDeployer/NDeployer/Resources/NBuild.xml";

        static void Main(string[] args)
        {
            NBuildReader reader = new NBuildReader(BUILD_FILENAME);
            reader.Execute();
        }
    }
}
