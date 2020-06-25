using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChrisDB.Helpers
{
	public static class StringHelpers
	{
		public static List<List<string>> GetMatchesAndGroups(string value, string regex, bool getMatchSelf = false)
		{
			
			List<List<string>> retVal = new List<List<string>>();

			var matches = Regex.Matches(value, regex);
			
			var count = matches.Count;
			for (int i = 0; i < count; i++)
			{
				var match = matches[i];
				
				retVal.Add(new List<string>());

				for (int b = 0; b < match.Groups.Count; b++)
				{
					if (b == 0 && !getMatchSelf)
						continue;
					//Console.WriteLine("DICIDCK " + match.Groups[b]);
					retVal[retVal.Count - 1].Add(match.Groups[b].ToString());
				}
			}

			return retVal;
		}
		public static List<string> GetGroups(string value, string regex, bool getMatchSelf = false)
		{
			return GetMatchesAndGroups(value, regex, getMatchSelf)[0];
		}
		public static List<string> GetMatches(string value, string regex)
		{
			List<string> retval = new List<string>();
			var b = GetMatchesAndGroups(value, regex, true);

			foreach (var item in b)
				retval.Add(item[0]);

			return retval;
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
