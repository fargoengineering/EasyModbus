using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyModbusClientExample
{
    public partial class FEI_FORM : Form
    {
        public Graco fluid_fill;
        string boxText;
        bool filling = false;
        int numPorts = 8;
        public FEI_FORM()
        {
            InitializeComponent();
            textBox1.Text = "172.16.72.12";
            textBox2.Text = "502";
            textBox4.Text = "8";
            label5.Text = "Uninitialized";
            label6.Text = "Recipe: ";
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // CLEAR BUTTON
            richTextBox1.Clear();
        }

        private void updateText()
        {
            while (true)
            {
                int[] currentFills = fluid_fill.getCurrentVolume();
                int[] targets = fluid_fill.getDispenseTargets();
                int systemState = fluid_fill.getSystemState();
                int recipe = fluid_fill.getRecipe();

                string text = $"Port\tCurrent Fill\tTarget\t\n-------------------------------------------------------------------------------------------------------------\n\n";

                for (int i = 1; i <= numPorts; i++)
                {
                    text += $"Port{i}:\t{currentFills[i - 1]}\t\t{targets[i - 1]}\n\n";
                }

                //richTextBox1.Text = text;
                string stateStr = "";

                if (systemState == 1)
                {
                    stateStr = "Standby Off";
                }else if (systemState == 2)
                {
                    stateStr = "Standby On";
                }else if (systemState == 3)
                {
                    stateStr = "Dispensing";
                }
                else
                {
                    stateStr = "Uninitialized";
                }

                this.Invoke(new MethodInvoker(delegate ()
                {
                    label5.Text = stateStr;
                    label6.Text = "Recipe: " + recipe;
                    richTextBox1.Text = text;
                }));
                Console.WriteLine("Thread spin");
                Thread.Sleep(1000);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // DISCONNECT BUTTON
            fluid_fill.disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // RUN BUTTON
            string hostname = textBox1.Text;
            int port = int.Parse(textBox2.Text);
            int recipe = int.Parse(textBox3.Text);
            numPorts = int.Parse(textBox4.Text);

            fluid_fill = new Graco();

            bool connected = fluid_fill.Connect(hostname, port);
            Console.Write(connected);
            fluid_fill.setSystemState(2);
            bool set = fluid_fill.setRecipe(recipe);
            Console.WriteLine(set);

            // update text box thread
            Task.Run(() =>
            {
                updateText();
            });



            Console.WriteLine("READY");


        }

        private void button4_Click(object sender, EventArgs e)
        {
            // START/STOP FILL BUTTON
            if (!filling)
            {
                fluid_fill.setSystemState(3);
                button4.Text = "Stop Fill";
                filling = true;
            }
            else
            {
                fluid_fill.setSystemState(2);
                button4.Text = "Start Fill";
                filling = false;
            }
        }
    }
}
