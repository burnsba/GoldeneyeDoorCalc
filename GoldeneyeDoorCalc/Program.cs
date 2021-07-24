using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoldeneyeDoorCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var levels = new List<string>() { "archives", "aztec", "bunker_1", "bunker_2", "caverns", "control", "cradle", "dam", "depot", "egyptian", "facility", "frigate", "jungle", "runway", "silo", "statue", "streets", "surface_1", "surface_2", "train" };

            var versions = new List<SystemVersion>() { SystemVersion.NTSC, SystemVersion.PAL };

            string basePath = "../../../../../GE_Wiki_Maps/data/";
            string outputFile = "out.txt";
            string line;

            using (var fs = new StreamWriter(outputFile, false))
            {
                foreach (var level in levels)
                {
                    var ntscDoors = new List<Door>();
                    var palDoors = new List<Door>();

                    foreach (var version in versions)
                    {
                        string filename;

                        if (version == SystemVersion.PAL)
                        {
                            filename = $"{level}_pal.json";
                        }
                        else
                        {
                            filename = $"{level}.json";
                        }

                        var filePath = basePath + filename;

                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        var seenDoors = new HashSet<string>();
                        var doors = new List<Door>();

                        var content = File.ReadAllText(filePath);

                        var jobj = JObject.Parse(content);

                        var levelObjects = jobj.GetValue("objects");
                        foreach (var entry in levelObjects.Values())
                        {
                            var jobjentry = entry as JObject;
                            if (jobjentry.ContainsKey("type"))
                            {
                                var objectType = jobjentry.GetValue("type").ToString();
                                if (objectType == "door")
                                {
                                    var ldoor = jobjentry.ToObject<LuaDoor>();

                                    ldoor.Key = ((JProperty)entry.Parent).Name;

                                    var door = new Door(ldoor);
                                    var locationId = door.GetObjectIdString();

                                    if (!seenDoors.Contains(locationId))
                                    {
                                        doors.Add(door);
                                        seenDoors.Add(locationId);
                                    }

                                    if (version == SystemVersion.PAL)
                                    {
                                        palDoors.Add(door);
                                    }
                                    else
                                    {
                                        ntscDoors.Add(door);
                                    }
                                }
                            }
                        }

                        Parallel.ForEach(doors, d =>
                        {
                            d.CalcOpen();
                        });

                        line = $"{level} ({version})";

                        Console.WriteLine("");
                        fs.WriteLine("");

                        Console.WriteLine("");
                        fs.WriteLine("");

                        Console.WriteLine(line);
                        fs.WriteLine(line);

                        Console.WriteLine("");
                        fs.WriteLine("");

                        foreach (var d in doors)
                        {
                            line = d.GetCalcOpenInfo(version);

                            Console.WriteLine(line);
                            fs.WriteLine(line);
                        }
                    }

                    Console.WriteLine("");
                    fs.WriteLine("");

                    line = "Level summary:";
                    Console.WriteLine(line);
                    fs.WriteLine(line);

                    var seenDoorFrameTimes = new HashSet<int>();
                    var frameTimeDoorVersions = new List<(Door, Door)>();

                    foreach (var door in ntscDoors)
                    {
                        if (door.PositionCalcValid != true)
                        {
                            continue;
                        }

                        var locationId = door.GetLocationIdString();
                        var palDoor = palDoors.FirstOrDefault(x => x.GetLocationIdString() == locationId);

                        if (object.ReferenceEquals(null, palDoor))
                        {
                            line = "$Could not find matching PAL door for {locationId}";
                            Console.WriteLine(line);
                            fs.WriteLine(line);

                            continue;
                        }

                        if (!seenDoorFrameTimes.Contains(door.OpenFrames))
                        {
                            seenDoorFrameTimes.Add(door.OpenFrames);

                            frameTimeDoorVersions.Add((door, palDoor));
                        }
                    }

                    decimal maxDelta = decimal.MinValue;
                    decimal minDelta = decimal.MaxValue;

                    foreach (var doorTuples in frameTimeDoorVersions)
                    {
                        var ntscDoor = doorTuples.Item1;
                        var palDoor = doorTuples.Item2;

                        decimal ntscOpenTime = ntscDoor.GetOpenFrameTimeSeconds(SystemVersion.NTSC);
                        decimal palOpenTime = palDoor.GetOpenFrameTimeSeconds(SystemVersion.PAL);
                        decimal delta = Math.Abs(ntscOpenTime - palOpenTime);

                        if (delta < minDelta)
                        {
                            minDelta = delta;
                        }

                        if (delta > maxDelta)
                        {
                            maxDelta = delta;
                        }

                        line = $"Doors with (NTSC) OpenFrames={ntscDoor.OpenFrames} => {ntscOpenTime} s NTSC, {palOpenTime} s PAL, delta = {delta} s";
                        Console.WriteLine(line);
                        fs.WriteLine(line);
                    }

                    line = $"Max door delta time = {maxDelta} s";
                    Console.WriteLine(line);
                    fs.WriteLine(line);

                    line = $"Min door delta time = {minDelta} s";
                    Console.WriteLine(line);
                    fs.WriteLine(line);

                    line = "====================================================================================================";
                    Console.WriteLine(line);
                    fs.WriteLine(line);

                    Console.WriteLine("");
                    fs.WriteLine("");
                }
            }
        }
    }
}
