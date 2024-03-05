/*
Copyright (c) 2018-2020 Rossmann-Engineering
Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software
and associated documentation files (the "Software"),
to deal in the Software without restriction, 
including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit 
persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission 
notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Threading;
using System.Windows.Forms;

namespace EasyModbusClientExample
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainForm());

			Graco proDispense = new Graco();

			Console.Write("Connected: ");
			Console.WriteLine(proDispense.Connect("172.16.72.12", 502));

			proDispense.setSystemState(2);

			Console.Write("System State set to: ");
			Console.WriteLine(proDispense.getSystemState());

			proDispense.setRecipe(50);
            //proDispense.setRecipe(16);

            Console.Write("Current Recipe set to: ");
			Console.WriteLine(proDispense.getRecipe());

			int[] targets = proDispense.getDispenseTargets();
			Console.Write("Fill Target on port 1 = ");
			Console.WriteLine(targets[0]);
            Console.Write("Fill Target on port 2 = ");
            Console.WriteLine(targets[2]);
            Console.Write("Fill Target on port 3 = ");
            Console.WriteLine(targets[4]);
            Console.Write("Fill Target on port 4 = ");
            Console.WriteLine(targets[6]);

            Console.Write("Current Fill Value");
			int[] fills = proDispense.getCurrentFlow();
			Console.WriteLine(fills[0]);

			proDispense.disconnect();

		}
		
	}
}
