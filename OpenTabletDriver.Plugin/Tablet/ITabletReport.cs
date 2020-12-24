using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet
{
    public interface ITabletReport : IDeviceReport
    {
        uint ReportID { get; }
        Vector2 Position { get; }
        Vector2 Tilt { get; }
        uint Pressure { get; }
        bool Eraser { get; }
        bool[] PenButtons { get; }
    }
}