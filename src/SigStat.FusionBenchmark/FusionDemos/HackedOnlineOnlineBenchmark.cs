﻿using SigStat.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SigStat.Common.Loaders;

namespace SigStat.FusionBenchmark.FusionDemos
{ 
    class HackedOnlineOnlineBenchmark
    {
        public static BenchmarkResults BenchmarkWithAllSigners(bool isOptimal, DataSetLoader offlineLoader, DataSetLoader onlineLoader)
        {
            Console.WriteLine("Hacked online - online benchmark started");

            var onlineSigners = onlineLoader.EnumerateSigners().ToList();
            var offlineSigners = offlineLoader.EnumerateSigners().ToList();

            var offlinePipeline = FusionPipelines.GetHackedOfflinePipeline();

            var benchmark = FusionPipelines.GetBenchmark(onlineSigners, isOptimal);

            foreach (var offSigner in offlineSigners)
            {
                Console.WriteLine(offSigner.ID + " started at " + DateTime.Now.ToString("h:mm:ss tt"));
                var onSigner = onlineSigners.Find(signer => signer.ID == offSigner.ID);
                Parallel.ForEach(offSigner.Signatures, offSig =>
                {
                    offlinePipeline.Transform(offSig);
                    var onSig = onSigner.Signatures.Find(sig => sig.ID == offSig.ID);
                    var onToOnPipeline = FusionPipelines.GetHackedOnToOnPipeline(offSig.GetFeature(FusionFeatures.Bounds));
                    onToOnPipeline.Transform(onSig);
                }
                );
            }

            var onlinePipeline = FusionPipelines.GetOnlinePipeline();
            foreach (var onSigner in onlineSigners)
            {
                Parallel.ForEach(onSigner.Signatures, onSig =>
                {
                    onlinePipeline.Transform(onSig);
                }
                );
            }
            return benchmark.Execute();
        }
    }
}
