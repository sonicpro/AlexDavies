using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAsyncCompilerTransform
{
	class AlexsClass
	{
		public async Task<int> AlexsMethod()
		{
			int foo = 3;
			// We use non-generic Task in this demo.
			// The stub method code contains the following lines:
			// "awaiter = Task.Delay(500).GetAwaiter();"
			// and then on case: 0 (async completion):
			// "awaiter.GetResult();" - this line does not produce assigneble value,
			// but re-throws an exception if one occurred during the asynchronous operation.
			await Task.Delay(500);
			return foo;
		}
	}
}
