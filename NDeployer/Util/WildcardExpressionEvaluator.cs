using System;

namespace NDeployer.Util
{
	class WildcardExpressionEvaluator
	{

		public static bool EvalExpression(string pattern, string text)
		{
			string[] splitPattern = pattern.Split('*');
			for (int i = 0; i < splitPattern.Length; i++)
			{
				int pos = 0;

				if (i < splitPattern.Length - 1)
				{
					pos = text.IndexOf(splitPattern[i]);
					if (pos == -1)
						return false;
					if (i == 0)
					{
						if (pos != 0)
							return false;
					} 
				}

				if (i == splitPattern.Length - 1)
				{
					if (splitPattern[i].Equals(""))
						pos = text.Length;
					else
						pos = text.LastIndexOf(splitPattern[i]);
					if (pos == -1)
						return false;
					if (pos + splitPattern[i].Length != text.Length)
						return false;
				} 

				text = text.Substring(pos + splitPattern[i].Length);
			}
			return true;
		}

	}
}

