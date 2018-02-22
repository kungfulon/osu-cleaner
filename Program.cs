using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace OsuCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var beatmapFolders = Directory.GetDirectories(".\\Songs");
                
                foreach (var beatmapFolder in beatmapFolders)
                {
                    Console.WriteLine("Parsing " + beatmapFolder);
                    var osuFiles = Directory.GetFiles(beatmapFolder, "*.osu");
                    var ignoredFiles = new HashSet<string>();

                    foreach (var osu in osuFiles)
                    {
                        var parser = new FileIniDataParser(new OsuParser());
                        parser.Parser.Configuration.CommentString = "//";
                        parser.Parser.Configuration.KeyValueAssigmentChar = ':';
                        parser.Parser.Configuration.SkipInvalidLines = true;
                        var data = parser.ReadFile(osu);
                        ignoredFiles.Add(osu);
                        ignoredFiles.Add(Path.Combine(beatmapFolder, data["General"]["AudioFilename"]));

                        try
                        {
                            var ev = data["Events"]["0"].Split(',');

                            if (ev[0] == "Video")
                                ev = data["Events"]["1"].Split(',');

                            ignoredFiles.Add(Path.Combine(beatmapFolder, ev[2].Substring(1, ev[2].Length - 2)));
                        }
                        catch (Exception)
                        {

                        }
                    }

                    var files = Directory.GetFiles(beatmapFolder, "*", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        try
                        {
                            if (!ignoredFiles.Contains(file))
                                File.Delete(file);
                        }
                        catch
                        {
                            Console.WriteLine("Warning: Cannot delete " + file);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
