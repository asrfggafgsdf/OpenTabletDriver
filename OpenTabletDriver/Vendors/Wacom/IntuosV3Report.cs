using System.Numerics;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Vendors.Wacom
{
    public class IntuosV3Report : ITabletReport, IProximityReport, ITiltReport, IEraserReport
    {
        public IntuosV3Report(byte[] report)
        {
            Raw = report;

            if (report.Length < 10)
            {
                // Discard first tablet report or whenever report length is insufficient
                ReportID = 0;
                Position = new Vector2(0, 0);
                Pressure = 0;
                PenButtons = new bool[] { false, false };
                return;
            }

            ReportID = report[0];
            Position = new Vector2
            {
                X = (report[2] | (report[3] << 8) | (report[4] << 16)),
                Y = (report[5] | (report[6] << 8) | (report[7] << 16))
            };
            Tilt = new Vector2
            {
                X = (sbyte)report[10],
                Y = (sbyte)report[11]
            };
            Pressure = (uint)(report[8] | (report[9] << 8));
            Eraser = (report[1] & (1 << 4)) != 0;
            PenButtons = new bool[]
            {
                (report[1] & (1 << 1)) != 0,
                (report[1] & (1 << 2)) != 0
            };
            NearProximity = (report[1] & (1 << 5)) != 0;
            FarProximity = (report[1] & (1 << 6)) != 0;
            HoverDistance = report[16];
        }
        
        public byte[] Raw { private set; get; }
        public uint ReportID { private set; get; }
        public Vector2 Position { private set; get; }
        public Vector2 Tilt { private set; get; }
        public uint Pressure { private set; get; }
        public bool Eraser { private set; get; }
        public bool[] PenButtons { private set; get; }
        public bool FarProximity { private set; get; }
        public bool NearProximity { private set; get; }
        public uint HoverDistance { private set; get; }
    }
}