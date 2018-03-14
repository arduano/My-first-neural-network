using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NNet
{
    public static class NetMaker
    {
        public static int[] layers = new int[]{2};
        public static int inputs = 2;
        public static int outputs = 1;

        public static double minWeight = -2;
        public static double maxWeight = 2;

        public static int seed = 0;

        public static double Rate = 1;

        public static Net makeNet()
        {
            Net net = new Net();
            net.Inputs = new double[inputs];
            net.Outputs = new double[outputs];

            net.Hidden = new double[layers.Length + 2][];
            net.Errors = new double[layers.Length + 1][];
            net.Weights = new double[layers.Length + 1][,];
            net.DeltaWeights = new double[layers.Length + 1][,];
            net.Hidden[net.Hidden.Length - 1] = new double[outputs];
            net.bp_log_func = new double[layers.Length + 1][];
            net.bp_math = new double[layers.Length + 1][];
            for (int i = 0; i <= layers.Length; i++)
            {
                int ic = inputs;
                int oc = outputs;
                if (i > 0) ic = layers[i - 1];
                if (i < layers.Length) oc = layers[i];

                net.Hidden[i] = new double[ic + 1];
                net.Errors[i] = new double[oc];
                net.Weights[i] = new double[oc, ic + 1];
                if (i < layers.Length) net.bp_log_func[i] = new double[oc + 1];
                else net.bp_log_func[i] = new double[oc];
                if (i < layers.Length) net.bp_math[i] = new double[oc + 1];
                else net.bp_math[i] = new double[oc];
                net.DeltaWeights[i] = new double[oc, ic + 1];
            }

            net.Rate = Rate;

            net.bp_Errors = new double[outputs];

            Random r = new Random(seed);

            for (int i = 0; i < net.Weights.Length; i++)
                for (int j = 0; j < net.Weights[i].GetLength(0); j++)
                    for (int k = 0; k < net.Weights[i].GetLength(1); k++)
                        net.Weights[i][j, k] = r.NextDouble() * (maxWeight - minWeight) + minWeight;

            net.errors.Enqueue(double.PositiveInfinity);

            return net;
        }
    }
}
