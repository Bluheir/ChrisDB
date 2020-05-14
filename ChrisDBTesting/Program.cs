using ChrisDB;
using ChrisDB.Helpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChrisDBTesting
{
	class Program
	{
		private ChrisDatabase data;

		public static void Main()
		=> new Program().MainAsync().GetAwaiter().GetResult();

		private async Task MainAsync()
		{
			data = new ChrisDatabase("");

			(string a, string b) = data.SetPrivateKey(File.ReadAllText("C:/Users/User/Desktop/privatekey.txt"));


			string sex = "fuck that bitch yeah fuck that bitcw9787yt97qyre8ytfg87ya87sygf7sdyfuih";
			byte[] sexBytes = sex.ToByteArray();

			byte[] enc = data.Encrypt(sexBytes);

			string sexCypher = enc.FromByteArray();
			Console.WriteLine(sexCypher);

			byte[] decrypt = data.Decrypt(enc);
			string text = decrypt.FromByteArray();

			Console.WriteLine(text);
			//*/

			//RegexHelpers.GetMatchesAndGroups("CREATE dick {asdfasdfasdf}", "(?:CREATE )((?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*) (.*)")[0].ForEach(Console.WriteLine);

			await data.ExecuteCreate("CREATE dick {asdfasdfasdf}");
		}
	}
}
