using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Debugging
{
    public class DebugReport
    {
        public DebugReport()
        {
        }

        public DebugReport(IDeviceReport deviceReport)
        {
            this.Raw = deviceReport.GetRawFormat();
            this.Interpreted = deviceReport.GetStringFormat();
        }

        public string Raw { set; get; }
        public string Interpreted { set; get; }
    }
}