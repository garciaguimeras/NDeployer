using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer.Util
{
	static class WildcardExpressionEvaluator
	{

		private static List<string> SplitPattern(string pattern)
		{
			List<string> parts = new List<string>();

			bool finish = false;
			string txt = pattern;

			while (!finish)
			{
				int pos = txt.IndexOf("*");
				if (pos == -1)
				{
					parts.Add(txt);
					finish = true;
					continue;
				}

				parts.Add(txt.Substring(0, pos));
				parts.Add("*");

				if (pos + 1 >= txt.Length)
				{
					finish = true;
					continue;
				}
				txt = txt.Substring(pos + 1);
			}

			return parts;
		}

		private static bool EvalExpression(int pos, List<string> patterns, string text)
		{
			if (pos >= patterns.Count)
				return string.IsNullOrEmpty(text);

			string pattern = patterns.ElementAt(pos);

			// Pattern is plain text
			if (!pattern.Equals("*"))
			{
				if (!text.StartsWith(pattern))
					return false;

				string newText = pattern.Length == text.Length ? "" : text.Substring(pattern.Length);
				return EvalExpression(pos + 1, patterns, newText);
			}

			// Pattern is *

			if (pos + 1 == patterns.Count)
				return true;

			string nextPattern = patterns.ElementAt(pos + 1);
			bool finish = false;
			int searchPos = 0;
			while (!finish)
			{
				int patternPos = text.IndexOf(nextPattern, searchPos);
				if (patternPos == -1)
				{
					finish = true;
					continue;
				}

				string newText = patternPos == text.Length ? "" : text.Substring(patternPos);
				bool result = EvalExpression(pos + 1, patterns, newText);
				if (result)
					return true;

				searchPos = patternPos + 1;
			}
			return false;
		}

		public static bool EvalExpression(string pattern, string text)
		{
			List<string> parts = SplitPattern(pattern);
			return EvalExpression(0, parts, text);
		}

	}
}

