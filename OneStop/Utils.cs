using System;
using System.Collections.Generic;
using System.Text;

namespace OneStopHelper
{
    public class Utils
    {
        /*
         * input: an array of original GPA thresholds like [36.9, 20.2, 18.6, 10.3, 8.1, 5.6, 0.3, 0, 0]
         * output: an array of the match possibility intervals for highly recommend, recommend, 
         * neutral or not recommend, like [3.5, 3.25, 3, 3]
         * the first and last element in output array are never null.
         */
        public static double?[] EstimateFromGpa(double?[] input)
        {
            double[] arr = new double[]
            {
                4, 3.75, 3.5, 3.25, 3.0, 2.5, 2, 1, 0
            };
            double?[] convert = new double?[input.Length];
            convert[0] = 100;
            for(int i = 1; i < input.Length; i++)
            {
                if (input[i - 1] == null)
                    convert[i] = 100;
                else
                    convert[i] = convert[i - 1] - input[i - 1];
            }

            double? hr = null, r = null, n = null;
            for (int i = 0; i < convert.Length; i++)
            {
                if (convert[i] >= 40)
                {
                    hr = arr[i];
                } else if (convert[i] >= 20)
                {
                    r = arr[i];
                } else if (convert[i] >= 10)
                {
                    n = arr[i];
                }
            }

            double? nr;
            if (n != null)
                nr = n;
            else if (r != null)
                nr = r;
            else
                nr = hr;
            return new double?[] { hr, r, n, nr };
        }
        
    }
}
