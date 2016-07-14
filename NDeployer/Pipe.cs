using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NDeployer
{

	[Serializable]
    class Pipe
    {

		List<Dictionary<string, string>> input;
		List<Dictionary<string, string>> output;
		List<Dictionary<string, string>> error;

		public IEnumerable<Dictionary<string, string>> Input { get { return input; } }
		public IEnumerable<Dictionary<string, string>> Output { get { return output; } }
		public IEnumerable<Dictionary<string, string>> Error { get { return error; } }

        public Pipe()
        {
			input = new List<Dictionary<string, string>>();
			output = new List<Dictionary<string, string>>();
			error = new List<Dictionary<string, string>>();
        }

		public Pipe Clone()
		{
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, this);
				stream.Seek(0, SeekOrigin.Begin);
				return (Pipe)formatter.Deserialize(stream);
			}
		}

        public void SwitchPipes()
        {
            input = output;
			output = new List<Dictionary<string, string>>();
			error = new List<Dictionary<string, string>>();
        }

		public void KeepPipe()
		{
			output = input;
		}

		public void AddToOuputPipe(Dictionary<string, string> data)
        {
            output.Add(data);
        }

		public void AddToErrorPipe(string error, params string[] extra)
		{
			Dictionary<string, string> data = new Dictionary<string, string> ();
			error = string.Format(error, extra);
			data.Add("error", error);
			output.Add(data);
		}

        public void PrintErrorPipe()
        {
			foreach (Dictionary<string, string> data in Error)
            {
				if (data.ContainsKey("error"))
					Logger.error(data["error"]);
            }
        }

    }
}
