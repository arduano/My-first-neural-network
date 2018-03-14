using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using NNet;
using System.IO;

namespace IO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Net net;

        static Form1 form1;

        static Thread formThread;

        static void StartForm()
        {
            Application.Run(form1);
        }

        [STAThread]
        static void Main()
        {
            NetMaker.layers = new int[] { 30, 20 };
            NetMaker.inputs = 28;
            NetMaker.outputs = 2;
            NetMaker.seed = (int)DateTime.Now.Ticks;
            net = NetMaker.makeNet();

            net.FromString(File.ReadAllText("..\\..\\..\\Weather.net"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();

            formThread = new Thread(new ThreadStart(() => StartForm()));
            formThread.Start();

            while (!form1.ready) { }

            form1.DrawNN(net.Hidden, 0, 0);

            double[,] data;


            string[] lines = File.ReadAllLines("..\\..\\..\\Weather.csv");
            data = new double[lines.Length, 5];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] val = lines[i].Split(',');
                for (int j = 0; j < 5; j++)
                {
                    data[i, j] = Convert.ToDouble(val[j]);
                }
            }

            DateTime date = new DateTime((int)data[13, 0], (int)data[13, 1], (int)data[13, 2]);
            int k = 14 ;
            int skip = 0;
            //try
            //{
                for (int i = 14; i < data.GetLength(0); i++)
                {
                    date = date.AddDays(1);
                    if (new DateTime((int)data[i, 0], (int)data[i, 1], (int)data[i, 2]) == date)
                    {
                        if (skip > 0)
                        {
                            skip--;
                            k++;
                            continue;
                        }
                        for (int j = 0; j < 14; j++)
                        {
                            net.Inputs[j * 2] = data[i - j, 3];
                            net.Inputs[j * 2 + 1] = data[i - j, 4];
                        }

                    //net.Inputs[28] = Math.Abs(data[i, 2] / 6.0 - 1);
                    //net.Inputs[29] = 1 - Math.Abs(data[i, 2] / 6.0 - 1);
                    net.Propogate();
                        lines[k] += "," + (2 / (1 + Math.Exp(-data[i, 3])) - 1).ToString() + "," + net.Outputs[0].ToString()+ "," + (1 / (1 + Math.Exp((-data[i, 4] + 24)))).ToString() + "," + net.Outputs[1].ToString();
                    }
                    else
                    {
                        int a = 0;
                        while (new DateTime((int)data[i, 0], (int)data[i, 1], (int)data[i, 2]) != date.AddDays(a)) a++;
                        date = date.AddDays(a);
                        k++;
                        skip = 13;
                        continue;
                    }
                    k++;
                }
            //}
            //catch { }
            File.WriteAllLines("..\\..\\..\\Weather2.csv", lines);


            while (true)
            {
                string s = Console.ReadLine();
                try
                {       //string[] nums = s.Split(' ');
                    //net.Inputs[Convert.ToInt32(nums[0])] = Convert.ToDouble(nums[1]);
                    //net.Propogate();
                    //form1.DrawNN(net.Hidden, 0, 0);
                    int i = Convert.ToInt32(s);
                    for (int j = 0; j < 14; j++)
                    {
                        net.Inputs[j * 2] = data[i - j, 3];
                        net.Inputs[j * 2 + 1] = data[i - j, 4];
                    }
                    net.Propogate();
                    form1.DrawNN(net.Hidden, 0, 0);
                }
                catch { }
            }
        }
    }
}
