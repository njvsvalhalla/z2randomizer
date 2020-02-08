using CommandLine;
using Newtonsoft.Json;
using RandomizerApp;
using RandomizerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Serilog;
using Z2Randomizer;
using static RandomizerApp.Enums;

namespace Zelda2RandomizerCore
{
    public class Options
    {
        [Option('r', "rom", Required = true, HelpText = "Path to rom file")]
        public string RomPath { get; set; }

        //[Option('o', "output", Required = false, HelpText = "Output directory path - if none given, generates in the same as input")]
        //public string OutputPath { get; set; }

        [Option('j', "json", Required = false, HelpText = "Path to json file")]
        public string JsonPath { get; set; }

        [Option('p', "preset", Required = false, HelpText = "Use preset. 0-3 are valid. See documentation.")]
        public int? Preset { get; set; }

        [Option('s', "Seed", Required = false, HelpText = "Generate random Seed (instead of json)")]
        public bool Seed { get; set; }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Z2Randomizer Console", Color.Green);

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       ValidateOptions(o);

                       if (!string.IsNullOrWhiteSpace(o.JsonPath))
                           GenerateFromJson(o);

                       if (o.Preset != null)
                           GeneratePreset(o);
                   });
        }
        
        public static void GenerateFromJson(Options o)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<RandomizerSettings>(File.ReadAllText(o.JsonPath));
                if (o.Seed)
                    settings.Seed = RandomizerSettings.GenerateSeed();

                GenerateRom(settings, o);
            }
            catch (Exception e)
            {
                Error("Error generating file from json. You might want to double check the json file.", e);
            }
        }

        public static void GeneratePreset(Options o)
        {
            try
            {
                var settings = RandomizerSettingsFactory.Generate((Presets)o.Preset.Value);
                GenerateRom(settings, o);
            }
            catch (Exception e)
            {
                Error("Error generating file from preset.", e);
            }
        }

        public static void GenerateRom(RandomizerSettings settings, Options o)
        {
            Console.WriteLine("Generating...", Color.Blue);
            //TODO we can remove the Seed param
            var romGenerator = new Hyrule(settings.GetRandomizerProperties(o.RomPath));
            romGenerator.GenerateRom();
            Console.WriteLine("Done...", Color.Green);
        }

        private static void ValidateOptions(Options o)
        {
            if (o.Preset != null && o.Preset > 3)
                Error("Invalid preset");

            if (string.IsNullOrWhiteSpace(o.JsonPath) && o.Preset == null)
                Error("You must specify a json file or preset");

            if (!string.IsNullOrWhiteSpace(o.JsonPath) && o.Preset != null)
                Error("You can't specify both a preset and json!");

            if (string.IsNullOrWhiteSpace(o.RomPath))
                Error("Please specify a rom path.");
            else if (!new FileInfo(o.RomPath).Exists)
                Error("Rom path not found.");

            if (!string.IsNullOrWhiteSpace(o.JsonPath) && !new FileInfo(o.JsonPath).Exists)
                Error("Json path not found");

            //if (!string.IsNullOrWhiteSpace(o.OutputPath) && !new DirectoryInfo(o.OutputPath).Exists)
            //    Error("Output directory is not valid or does not exist. Ensure it exists and this is not a file.");
        }
        private static void Error(string message, Exception e = null)
        {
            Console.WriteLine(message, Color.Red);
            if (e != null)
            {
                Console.WriteLine("Exception is listed below. Please send to developer if you believe this to be an error.", Color.Red);
                Console.WriteLine($"{e}", Color.Red);
            }
            Environment.Exit(0);
        }
    }
}
