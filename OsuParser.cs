using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Parser;

namespace OsuCleaner
{
    public class OsuParser : IniDataParser
    {
        private int _dataCounter = 0;

        protected override void ProcessLine(string currentLine, IniData currentIniData)
        {
            currentLine = currentLine.Trim();

            if (LineContainsAComment(currentLine))
                currentLine = ExtractComment(currentLine);

            if (currentLine == String.Empty)
                return;

            if (LineMatchesASection(currentLine))
            {
                string sectionName = Configuration.SectionRegex.Match(currentLine).Value.Trim();
                sectionName = sectionName.Substring(1, sectionName.Length - 2).Trim();

                if (sectionName != string.Empty)
                    _dataCounter = 0;

                ProcessSection(currentLine, currentIniData);
                return;
            }

            if (!LineMatchesAKeyValuePair(currentLine))
            {
                currentLine = _dataCounter.ToString() + currentIniData.Configuration.KeyValueAssigmentChar.ToString() + currentLine;
                ++_dataCounter;
            }

            ProcessKeyValuePair(currentLine, currentIniData);
        }
    }
}
