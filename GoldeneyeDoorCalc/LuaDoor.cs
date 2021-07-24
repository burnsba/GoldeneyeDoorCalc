using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GoldeneyeDoorCalc
{
    public class LuaDoor
    {
        public string Key { get; set; }

        [JsonProperty("preset")]
        public int Preset { get; set; }

        [JsonProperty("flags_1")]
        public UInt32 Flags { get; set; }

        [JsonProperty("preset_points")]
        public List<List<double>> PresetPoints { get; set; }

        [JsonProperty("collectible")]
        public bool Collectible { get; set; }

        [JsonProperty("points")]
        public List<List<double>> Points { get; set; }

        [JsonProperty("height_range")]
        public List<double> HeightRange { get; set; }

        [JsonProperty("tile")]
        public int Tile { get; set; }

        [JsonProperty("position")]
        public List<double> Position { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("health")]
        public double Health { get; set; }

        [JsonProperty("door_type")]
        public int DoorType { get; set; }

        [JsonProperty("max_displacement_percentage")]
        public int MaxDisplacementPercentage { get; set; }

        [JsonProperty("walkthrough_distance")]
        public int WalkthroughDistance { get; set; }

        [JsonProperty("acceleration")]
        public double Acceleration { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("max_speed")]
        public double MaxSpeed { get; set; }

        [JsonProperty("max_displacement")]
        public double MaxDisplacement { get; set; }

        [JsonProperty("displacement_percentage")]
        public double DisplacementPercentage { get; set; }

        [JsonProperty("speed_percentage")]
        public double SpeedPercentage { get; set; }

        [JsonProperty("hinges")]
        public List<List<double>> Hinges { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
