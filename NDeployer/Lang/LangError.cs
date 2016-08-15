using System;

namespace NDeployer.Lang
{
	class LangError : Exception
	{
		public LangError(string message) : base(message)
		{}

		public static LangError MissingAttribute(string name)
		{
			return new LangError(string.Format("Missing attribute: {0}", name));
		}
	}
}

