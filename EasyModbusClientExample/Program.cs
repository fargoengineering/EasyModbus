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

		public static void test_library()
		{
            // Initialize modbus class
            Graco proDispense = new Graco();

            // Connect to IP / Port
            Console.Write("Connected: ");
            Console.WriteLine(proDispense.Connect("172.16.72.12", 502));

            // Set System state 2 = Standby On
            proDispense.setSystemState(2);

            // Confirm System State
            Console.Write("System State set to: ");
            Console.WriteLine(proDispense.getSystemState());

            // Set Recipe
            proDispense.setRecipe(50);

            // Confirm Recipe
            Console.Write("Current Recipe set to: ");
            Console.WriteLine(proDispense.getRecipe());

            // Get Target fills for each port
            int[] targets = proDispense.getDispenseTargets();
            Console.Write("Fill Target on port 1 = ");
            Console.WriteLine(targets[0]);
            Console.Write("Fill Target on port 2 = ");
            Console.WriteLine(targets[1]);
            Console.Write("Fill Target on port 3 = ");
            Console.WriteLine(targets[2]);
            Console.Write("Fill Target on port 4 = ");
            Console.WriteLine(targets[3]);

            // Get flow cc/min for each port
            int[] fills = proDispense.getCurrentFlow();
            Console.Write("Current Fill Value on port 1 = ");
            Console.WriteLine(fills[0]);
            Console.Write("Current Fill value on port 2 = ");
            Console.WriteLine(fills[1]);
            Console.Write("Current Fill value on port 3 = ");
            Console.WriteLine(fills[2]);
            Console.Write("Current Fill value on port 4 = ");
            Console.WriteLine(fills[3]);

            // Get Current state for each port
            int[] states = proDispense.getPortState();
            Console.Write("Current port 1 state: ");
            Console.WriteLine(states[0]);
            Console.Write("Current port 2 state: ");
            Console.WriteLine(states[1]);
            Console.Write("Current port 3 state: ");
            Console.WriteLine(states[2]);
            Console.Write("Current port 4 state: ");
            Console.WriteLine(states[3]);

            // Example: Start Fill and Monitor
            // proDispense.setSystemState(3);	// Set state to DISPENSE
            // proDispense.getCurrentFlow();	// Monitor Flow Rate
            // proDispense.getCurrentVolume();	// Monitor Job Volume

            // Disconnect from MODBUS
            proDispense.disconnect();
        }
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
            // EasyMODBUS Example
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            // FEI MODBUS CODE
            //test_library();

            Application.Run(new FEI_FORM());

        }
	}
}
