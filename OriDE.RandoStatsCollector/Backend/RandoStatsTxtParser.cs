using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OriDE.RandoStatsCollector.Enums;
using OriDE.RandoStatsCollector.GUI;

namespace OriDE.RandoStatsCollector.Backend
{
    public class RandoStatsTxtParser
    {
        private IEnumerable<string> statsTxt;

        public RandoStatsTxtParser(string path)
        {
            statsTxt = File.ReadLines(path);
        }

        public Dictionary<DictEnums, Dictionary<StatParserEnums, string>> ParseStatsTxt()
        {
            Dictionary<DictEnums, Dictionary<StatParserEnums, string>> returnDict = new()
            {
                {DictEnums.SeedParamsDict, ParseSeed()},
                {DictEnums.PPMDict, ParsePPM()},
                {DictEnums.PickupsDict, ParsePickups()}
            };

            return returnDict;
        }

        private Dictionary<StatParserEnums, string> ParseSeed()
        {
            Dictionary<StatParserEnums, string> returnDict = new();

            // Seed related stuff is in the first line
            string firstLine = statsTxt.First();

            // Regex that matches the commas then the | because why would it be consistent and only use commas hahahahahahahsaEBHADBNHJADSHDSAHGHADSGHJ
            Match match = Regex.Match(firstLine, @"^(?:(.*),){5}(?:pool=(.*)\|)(.*)$");

            // This is an unrolled loop because I didn't want to use Enum.GetValues() and make it enum order dependant.
            returnDict.Add(StatParserEnums.LogicMode, match.Groups[1].Captures[1].Value); // Capture 0 is server game number which is useless
            returnDict.Add(StatParserEnums.KeyMode, match.Groups[1].Captures[2].Value);
            returnDict.Add(StatParserEnums.GoalMode, match.Groups[1].Captures[3].Value);
            returnDict.Add(StatParserEnums.FillAlg, match.Groups[1].Captures[4].Value);
            returnDict.Add(StatParserEnums.Pool, match.Groups[2].Captures[0].Value);
            returnDict.Add(StatParserEnums.Seed, match.Groups[3].Captures[0].Value);

            return returnDict;
        }

        private Dictionary<StatParserEnums, string> ParsePPM()
        {
            Dictionary<StatParserEnums, string> returnDict = new();
            
            IEnumerator<string> relevantInfo = statsTxt.SkipWhile(x => !Regex.IsMatch(x, @"^Zone\s+Deaths\s+Time\s+Pickups\s+PPM\s*$")).Skip(1).GetEnumerator(); // Skip until the wanted data and the header of it
            IEnumerator<StatParserEnums> PPMEnums = Enum.GetValues<StatParserEnums>().Where(x => (int) x is >= 100 and  < 200).GetEnumerator(); // Don't wanna unroll loop this it's too big and we only ever use one group
            Match match;
            
            while (relevantInfo.MoveNext() && PPMEnums.MoveNext() && (match = Regex.Match(relevantInfo.Current, @"^(\w+)\s+(\d+|N/A)\s+(\d+:\d+|N/A)\s+(\d+/\d+)\s+(\d+\.*\d*|N/A)\s*$")) != Match.Empty)
            {
                returnDict.Add(PPMEnums.Current, match.Groups[5].Value != "N/A"?match.Groups[5].Value:null); // Transform the N/As to nulls
            }

            PPMEnums.Dispose();
            relevantInfo.Dispose();
            
            return returnDict;
        }
        
        private Dictionary<StatParserEnums, string> ParsePickups()
        {
            Dictionary<StatParserEnums, string> returnDict = new();

            // Skip until we find the pickup lines
            IEnumerable<string> relevantInfo = statsTxt.SkipWhile(x => !Regex.IsMatch(x, @"Item\s*Found At\s*Zone\s*")).Skip(1); // One more (header)

            foreach (string line in relevantInfo.Where(x => x != "\n")) // This is the end of the file so I just iterate to the end instead of bothering with an Iterator
            {
                Match matchedLine = Regex.Match(line, @"^((?:\w|\s)+):\s*(\d+:\d+|N/A)\s*((?:\w|\s*)+)$");

                string skill = matchedLine.Groups[1].Value.Replace(" ", "");
                string area = matchedLine.Groups[3].Value != "Unknown"?matchedLine.Groups[3].Value:null;

                returnDict.Add(Enum.Parse<StatParserEnums>(skill), area);
            }
            
            return returnDict;

        }
        
    }
    
}