﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SigStat.Common.Model
{
    public class ApproximateLimit
    {
        private readonly IClassification pipeline;

        public ApproximateLimit(IClassification pipeline)
        {
            this.pipeline = pipeline;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sigs">List of genuine signatures</param>
        /// <returns></returns>
        public double Calculate(List<Signature> sigs)
        {
            //calc each pair
            List<double> pairingResults = new List<double>();
            for (int i = 0; i < sigs.Count - 1; i++)
                for (int j = i + 1; j < sigs.Count; j++)
                    pairingResults.Add(pipeline.Pair(sigs[i], sigs[j]));

            //calc average cost
            double avg = 0;
            foreach (double v in pairingResults)
                avg += v;
            avg /= pairingResults.Count;

            //calc standard deviation
            double dev = 0;
            foreach (double v in pairingResults)
                dev += Math.Pow(v - avg, 2);
            dev = Math.Sqrt(dev / (pairingResults.Count - 1));

            //limit = average + deviation
            return avg + dev;


        }
    }
}
