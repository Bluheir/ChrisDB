using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChrisDB.Helpers
{
	public static class StringHelpers
	{
		public static List<List<string>> GetMatchesAndGroups(string value, string regex)
		{
			Regex r = new Regex(regex);
			List<List<string>> retVal = new List<List<string>>();

			var matches = r.Match(value);
			var count = matches.Captures.Count;
			for (int i = 0; i < count; i++)
			{
				GroupCollection match;
				if (i == 0)
					match = matches.Groups;
				else
					match = matches.NextMatch().Groups;
				retVal.Add(new List<string>());

				for(int b = 0; b < match.Count; b++)
				{
					if (b == 0)
						continue;
					retVal[retVal.Count - 1].Add(match[b].ToString());
				}
			}

			return retVal;
		}
		public static byte[] ToByteArray(this string value, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.UTF8;

			return encoding.GetBytes(value);
		}
		public static string FromByteArray(this byte[] value, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.UTF8;

			return encoding.GetString(value);
		}
	}
}
