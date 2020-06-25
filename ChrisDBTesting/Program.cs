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
			data = new ChrisDatabase(@"C:\Users\User\Desktop\ChrisDBTest");
			await data.LoadTablesAsync();
			Console.WriteLine(await data.ExecuteCreateAsync(data.ParseCreate("CREATE dickish {dick:string,dicke:int};key=dick;clustering=dicke;orderby=asc;"), true));
		}
	}
}
