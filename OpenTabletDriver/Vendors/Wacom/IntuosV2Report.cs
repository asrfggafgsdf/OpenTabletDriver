using System.Numerics;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Vendors.Wacom
{
    public struct IntuosV2TabletReport : ITabletReport, IProximityReport, ITiltReport, IEraserReport
    {
        public IntuosV2TabletReport(byte[] report)
        {
            Raw = report;

            ReportID = (uint)report[9] >> 2;
            Position = new Vector2
            {
                X = (report[3] | report[2] << 8) << 1 | ((report[9] >> 1) & 1),
                Y = (report[5] | report[4] << 8) << 1 | (report[9] & 1)
            };
            Tilt = new Vector2
            {
                X = (((report[7] << 1) & 0x7E) | (report[8] >> 7)) - 64,
                Y = (report[8] & 0x7F) - 64
            };
            Pressure = (uint)((report[6] << 3) | ((report[7] & 0xC0) >> 5) | (report[1] & 1));
            Eraser = false;     // Need to do toolID lookup for this, unfortunately
            PenButtons = new bool[]
            {
                (report[1] & (1 << 1)) != 0,
                (report[1] & (1 << 2)) != 0
            };
            NearProximity = (report[1] & (1 << 5)) != 0;
            FarProximity = (report[1] & (1 << 6)) != 0;
            HoverDistance = (uint)report[9] >> 2;
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