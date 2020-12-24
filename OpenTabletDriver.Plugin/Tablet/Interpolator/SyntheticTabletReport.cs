using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public class SyntheticTabletReport : ITabletReport, ISyntheticReport
    {
        public byte[] Raw { get; set; }
        public uint ReportID { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Tilt { get; set; }
        public uint Pressure { get; set; }
        public bool Eraser { get; set; }
        public bool[] PenButtons { get; set; }

        public SyntheticTabletReport(ITabletReport report)
        {
            this.Raw = report.Raw;
            this.ReportID = report.ReportID;
            this.Position = report.Position;
            this.Tilt = report.Tilt;
            this.Pressure = report.Pressure;
            this.Eraser = report.Eraser;
            this.PenButtons = report.PenButtons;
        }
    }
}