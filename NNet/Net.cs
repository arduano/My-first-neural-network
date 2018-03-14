using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NNet
{
    public class Net
    {
        public double[] Inputs;
        public double[] Outputs;

        public double[][] Hidden;
        public double[][,] Weights;

        public double[][,] DeltaWeights;

        public double[][] Errors;

        public double Rate;

        public double TotalError = double.PositiveInfinity;

        public int AverageOf = 100;

        public Queue<double> errors = new Queue<double>(100);

        public double[] bp_Errors;
        public double[][] bp_log_func;
        public double[][] bp_math;

        public void Propogate()
        {
            for (int i = 0; i < Inputs.Length; i++)
            {
                Hidden[0][i] = Inputs[i];
            }

            for (int l = 0; l < Weights.Length; l++)
            {
                Hidden[l][Hidden[l].Length - 1] = 1;
                for (int n = 0; n < Weights[l].GetLength(0); n++)
                {
                    double total = 0;
                    for (int pn = 0; pn < Weights[l].GetLength(1); pn++)
                    {
                        total -= Hidden[l][pn] * Weights[l][n, pn];
                    }
                    Hidden[l + 1][n] = 1 / (1 + Math.Exp(total));
                }
            }
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = Hidden[Hidden.Length - 1][i];
            }
        }

        public void backPropogate()
        {
            double[] Desired = (double[])Outputs.Clone();
            Propogate();

            for (int i = 0; i < Outputs.Length; i++)
            {
                bp_Errors[i] = Outputs[i] - Desired[i];
            }

            for (int i = 1; i < Hidden.Length; i++)
                for (int j = 0; j < Hidden[i].Length; j++)
                {
                    bp_log_func[i - 1][j] = Hidden[i][j] * (1 - Hidden[i][j]);
                }

            if (false)
            {
                for (int y = 0; y < Hidden[0].Length; y++)
                {
                    for (int z = 0; z < Hidden[1].Length - 1; z++)
                    {
                        double delta = 0;
                        for (int n = 0; n < Hidden[2].Length; n++)
                        {
                            delta += bp_Errors[n] * bp_log_func[1][n] * Weights[1][n, z] * bp_log_func[0][z] * Hidden[0][y];
                        }
                        delta *= Rate;
                        Weights[0][z, y] -= delta;
                    }
                }

                for (int y = 0; y < Hidden[1].Length; y++)
                {
                    for (int z = 0; z < Hidden[2].Length; z++)
                    {
                        double delta = bp_Errors[z] * bp_log_func[1][z] * Hidden[1][y];
                        Weights[1][z, y] -= delta * Rate;
                    }
                }
            }
            else
            {
                for (int y = 0; y < Hidden[0].Length; y++)
                {
                    for (int z = 0; z < Hidden[1].Length - 1; z++)
                    {
                        double delta = 0;
                        for (int n = 0; n < Hidden[2].Length - 1; n++)
                        {
                            for (int m = 0; m < Hidden[3].Length; m++)
                            {
                                delta += bp_Errors[m] * bp_log_func[2][m] * Weights[2][m, n] * bp_log_func[1][n] * Weights[1][n, z] * bp_log_func[0][z] * Hidden[0][y];
                            }
                        }
                        delta *= Rate;
                        Weights[0][z, y] -= delta;
                    }
                }

                for (int y = 0; y < Hidden[1].Length; y++)
                {
                    for (int z = 0; z < Hidden[2].Length - 1; z++)
                    {
                        double delta = 0;
                        for (int n = 0; n < Hidden[3].Length; n++)
                        {
                            delta += bp_Errors[n] * bp_log_func[2][n] * Weights[2][n, z] * bp_log_func[1][z] * Hidden[1][y];
                        }
                        delta *= Rate;
                        Weights[1][z, y] -= delta;
                    }
                }

                for (int y = 0; y < Hidden[2].Length; y++)
                {
                    for (int z = 0; z < Hidden[3].Length; z++)
                    {
                        double delta = bp_Errors[z] * bp_log_func[2][z] * Hidden[2][y];
                        Weights[2][z, y] -= delta * Rate;
                    }
                }
            }
            TotalError = 0;
            for (int i = 0; i < Outputs.Length; i++) TotalError += Math.Abs(Outputs[i] - Desired[i]);
            errors.Enqueue(TotalError);
            if (errors.Count > AverageOf)
            {
                errors.Dequeue();
            }
        }

        public string GetString()
        {
            string s = "";
            for (int i = 0; i < Weights.Length; i++)
                for (int j = 0; j < Weights[i].GetLength(0); j++)
                    for (int k = 0; k < Weights[i].GetLength(1); k++)
                    {
                        s += Weights[i][j, k].ToString() + ",";
                    }
            return s;
        }

        public void FromString(string str)
        {
            string[] s = str.Split(',');
            int a = 0;
            for (int i = 0; i < Weights.Length; i++)
                for (int j = 0; j < Weights[i].GetLength(0); j++)
                    for (int k = 0; k < Weights[i].GetLength(1); k++)
                    {
                        Weights[i][j, k] = Convert.ToDouble(s[a]);
                        a++;
                    }
        }
    }
}
