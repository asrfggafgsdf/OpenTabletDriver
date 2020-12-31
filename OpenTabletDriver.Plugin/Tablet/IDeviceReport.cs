using System;

namespace OpenTabletDriver.Plugin.Tablet
{
    public interface IDeviceReport
    {
        byte[] Raw { get; }
        string GetRawFormat() => BitConverter.ToString(Raw).Replace('-', ' ');
        string GetStringFormat();
    }
}