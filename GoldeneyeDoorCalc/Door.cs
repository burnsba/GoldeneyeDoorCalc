using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GoldeneyeDoorCalc
{
    public class Door
    {
        private const int MaxOpenCalcIterations = 2000;

        public Door()
        {
            Position = new PointD(double.NaN, double.NaN);
            TruncPosition = new Point(int.MinValue, int.MinValue);
        }

        public Door(LuaDoor lua)
        {
            MaxDistance = lua.MaxDisplacementPercentage;
            Acceleration = lua.Acceleration;
            Deceleration = lua.Rate;
            MaxSpeed = lua.MaxSpeed;

            ObjectKey = int.Parse(lua.Key);
            Preset = lua.Preset;

            Position = new PointD(lua.Position[0], lua.Position[1]);
            TruncPosition = new Point((int)lua.Position[0], (int)lua.Position[1]);
        }

        public int ObjectKey { get; set; }
        public int Preset { get; set; }

        public PointD Position { get; set; }
        public Point TruncPosition { get; set; }

        public double DistanceDone { get; set; } = 0.0;
        public double MaxDistance { get; set; } = 0.0;
        public double Speed { get; set; } = 0.0;
        public double Acceleration { get; set; } = 0.0;
        public double Deceleration { get; set; } = 0.0;
        public double MaxSpeed { get; set; } = 0.0;
        public int OpenFrames { get; set; } = 0;
        public bool OpenDone { get; set; } = false;
        public int ToMaxSpeedFrames { get; set; } = 0;
        public bool ToMaxSpeedDone { get; set; } = false;

        public bool? PositionCalcValid { get; set; } = null;

        public string GetObjectIdString()
        {
            return $"{ObjectKey}.{Preset}.{TruncPosition.X}.{TruncPosition.Y}";
        }

        public string GetPresetIdString()
        {
            return $"{Preset}.{TruncPosition.X}.{TruncPosition.Y}";
        }

        public string GetLocationIdString()
        {
            return $"{TruncPosition.X}.{TruncPosition.Y}";
        }

        public decimal GetOpenFrameTimeSeconds(SystemVersion version)
        {
            decimal frameSeconds = 0;

            if (version == SystemVersion.PAL)
            {
                frameSeconds = OpenFrames / 50.0m;
            }
            else
            {
                frameSeconds = OpenFrames / 60.0m;
            }

            return Math.Round(frameSeconds, 2);
        }

        public decimal GetToMaxSpeedFrameSeconds(SystemVersion version)
        {
            decimal toMaxSpeedFrameSeconds = 0;

            if (version == SystemVersion.PAL)
            {
                toMaxSpeedFrameSeconds = ToMaxSpeedFrames / 50.0m;
            }
            else
            {
                toMaxSpeedFrameSeconds = ToMaxSpeedFrames / 60.0m;
            }

            return Math.Round(toMaxSpeedFrameSeconds, 2);
        }

        public void ApplySpeed()
        {
            var localSpeed = Speed;

            var limit = localSpeed * localSpeed * 0.5 / Deceleration;
            var distRemaining = MaxDistance - DistanceDone;

            if (distRemaining > 0.0)
            {
                if (localSpeed > 0.0 && distRemaining <= limit)
                {
                    // Slow down for end
                    localSpeed -= Deceleration;

                    if (localSpeed < Deceleration)
                    {
                        localSpeed = Deceleration;
                    }
                }
                else if (localSpeed < MaxSpeed)
                {
                    // Accelerate
                    if (localSpeed < 0.0)
                    {
                        localSpeed += Deceleration;
                    }
                    else
                    {
                        localSpeed += Acceleration;
                    }

                    if (localSpeed >= MaxSpeed)
                    {
                        localSpeed = MaxSpeed;
                        ToMaxSpeedDone = true;
                    }

                    if (!ToMaxSpeedDone)
                    {
                        ToMaxSpeedFrames++;
                    }
                }

                if (localSpeed >= distRemaining)
                {
                    DistanceDone = MaxDistance;
                    OpenDone = true;
                }

                DistanceDone += localSpeed;

                if (!OpenDone)
                {
                    OpenFrames++;
                }
            }
            else
            {
                if (localSpeed < 0.0 && -distRemaining <= limit)
                {
                    localSpeed += Deceleration;

                    if (localSpeed > -Deceleration)
                    {
                        localSpeed = -Deceleration;
                    }
                }
                else if (localSpeed > -MaxSpeed)
                {
                    if (localSpeed > 0.0)
                    {
                        localSpeed -= Deceleration;
                    }
                    else
                    {
                        localSpeed -= Acceleration;
                    }

                    if (localSpeed < -MaxSpeed)
                    {
                        localSpeed = -MaxSpeed;
                    }
                }

                if (localSpeed <= distRemaining)
                {
                    DistanceDone = MaxDistance;
                    //break;
                }

                DistanceDone += localSpeed;
            }

            Speed = localSpeed;
        }

        public void CalcOpen()
        {
            Speed = 0;
            DistanceDone = 0;
            OpenFrames = 0;
            ToMaxSpeedFrames = 0;

            OpenDone = false;
            ToMaxSpeedDone = false;
            PositionCalcValid = null;

            for (var i = 0; i < MaxOpenCalcIterations; i++)
            {
                ApplySpeed();
                if (OpenDone && ToMaxSpeedDone)
                {
                    PositionCalcValid = true;
                    break;
                }
            }

            if (PositionCalcValid != true)
            {
                PositionCalcValid = false;
            }
        }

        public string GetCalcOpenInfo(SystemVersion version)
        {
            var sb = new StringBuilder();

            sb.Append($"Preset=0x{Preset:x}, ");
            sb.Append($"obj=0x{ObjectKey:x}, ");
            sb.Append($"pos=({TruncPosition.X}, {TruncPosition.Y}), ");

            if (PositionCalcValid == true)
            {
                decimal frameSeconds = GetOpenFrameTimeSeconds(version);
                decimal toMaxSpeedFrameSeconds = GetToMaxSpeedFrameSeconds(version);

                sb.Append($"{nameof(ToMaxSpeedFrames)}={ToMaxSpeedFrames}={toMaxSpeedFrameSeconds} s, ");
                sb.Append($"{nameof(OpenFrames)}={OpenFrames}={frameSeconds} s");
            }
            else
            {
                sb.Append("invalid");
            }

            return sb.ToString();
        }
    }
}
