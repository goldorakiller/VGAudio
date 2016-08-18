﻿using System;
using System.Diagnostics;
using System.IO;
using DspAdpcm.Lib.Adpcm;
using DspAdpcm.Lib.Adpcm.Formats;
using DspAdpcm.Lib.Pcm;
using DspAdpcm.Lib.Pcm.Formats;

namespace DspAdpcm.Cli
{
    public static class DspAdpcmCli
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dspadpcm <wavIn> <brstmOut>\n");
                return 0;
            }

            PcmStream wave;

            try
            {
                using (var file = new FileStream(args[0], FileMode.Open))
                {
                    wave = new Wave(file).AudioStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            AdpcmStream adpcm = Lib.Adpcm.Encode.PcmToAdpcmParallel(wave);

            watch.Stop();
            Console.WriteLine($"DONE! {adpcm.NumSamples} samples processed\n");
            Console.WriteLine($"Time elapsed: {watch.Elapsed.TotalSeconds}");
            Console.WriteLine($"Processed {(adpcm.NumSamples / watch.Elapsed.TotalMilliseconds):N} samples per millisecond.");

            var brstm = new Brstm(adpcm);

            using (FileStream stream = File.Open(args[1], FileMode.Create))
            {
                brstm.WriteFile(stream);
            }

            return 0;
        }
    }
}
