using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace TheAsyncCompilerTransform
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				AsyncContext.Run(async () =>
				{
					var inst = new AlexsClass();
					Console.WriteLine(await inst.AlexsMethod());
				});
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}
