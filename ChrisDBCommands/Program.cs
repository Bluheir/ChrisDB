using System;

namespace ChrisDBCommands
{
	class Program
	{
		static void Main(string[] args)
		{
			foreach (var item in args)
				Console.WriteLine("args " + item);
		}
	}
}
