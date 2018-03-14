using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNet;
using System.Threading;

namespace Weather
{
    class Program
    {
        static double[,] data;
        static void Main(string[] args)
        {
            string[] temps = File.ReadAllLines("..\\..\\IDCJAC0009_066078_1800_Data.csv");
            string[] rains = File.ReadAllLines("..\\..\\IDCJAC0010_066161_1800_Data.csv");
            List<string> file = new List<string>();
            for (int i = 0; i < temps.Length; i++)
            {
                string[] t = temps[i].Split(',');
                string[] r = rains[i].Split(',');
                if (t[5] == "" || r[5] == "") continue;
                file.Add(t[2] + "," + t[3] + "," + t[4] + "," + t[5] + "," + r[5]);
            }

            File.WriteAllLines("..\\..\\Weather.csv", file.ToArray());

            string[] lines = File.ReadAllLines("..\\..\\Weather.csv");
            data = new double[lines.Length, 5];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] val = lines[i].Split(',');
                for (int j = 0; j < 5; j++)
                {
                    data[i, j] = Convert.ToDouble(val[j]);
                }
            }

            //net.FromString(File.ReadAllText("..\\..\\..\\Weather.net"));

            Thread[] threads = new Thread[1];
            for (int i = 0; i < threads.Length; i++)
            {
                NetMaker.layers = new int[] { 30, 20 };
                NetMaker.inputs = 28;
                NetMaker.outputs = 2;
                NetMaker.Rate = 0.0006666666;
                NetMaker.seed = (int)DateTime.Now.Ticks;
                Net net = NetMaker.makeNet();
                net.AverageOf = 1576;
                net.FromString(File.ReadAllText("..\\..\\..\\Weather.net"));
                {
                    int j = i;
                    threads[i] = new Thread(new ThreadStart(() => ProcessNN(net, j)));
                }   
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();  
            }
        }

        static void ProcessNN(Net net, int id)
        {
            int n = 0;
            bool loop = true;
            while (loop)
            {
                DateTime date = new DateTime((int)data[13, 0], (int)data[13, 1], (int)data[13, 2]);
                for (int i = 14; i < data.GetLength(0); i++)
                {
                    if (new DateTime((int)data[i, 0], (int)data[i, 1], (int)data[i, 2]) == date.AddDays(1))
                    {
                        date = date.AddDays(1);
                        for (int j = 0; j < 14; j++)
                        {
                            net.Inputs[j * 2] = data[i - j, 3];
                            net.Inputs[j * 2 + 1] = data[i - j, 4];
                        }
                        //net.Inputs[28] = Math.Abs(data[i, 2] / 6.0 - 1);
                        //net.Inputs[29] = 1 - Math.Abs(data[i, 2] / 6.0 - 1);
                        net.Outputs[0] = 2 / (1 + Math.Exp(-data[i, 3])) - 1;
                        net.Outputs[1] = 1 / (1 + Math.Exp((-data[i, 4] + 24)));
                        net.backPropogate();
                    }
                    else
                    {
                        int a = 1;
                        while (new DateTime((int)data[i, 0], (int)data[i, 1], (int)data[i, 2]) != date.AddDays(a)) a++;
                        date = date.AddDays(a + 13);
                        i += 13;
                        continue;
                    }
                }
                n++;
                if (n % 1 == 0)
                    Console.WriteLine(id.ToString() + ", " + n.ToString() + ", " + net.errors.Max() + ", " + net.errors.Average());
            }
            File.WriteAllText("..\\..\\..\\Weather.net", net.GetString());
        }
    }
}
