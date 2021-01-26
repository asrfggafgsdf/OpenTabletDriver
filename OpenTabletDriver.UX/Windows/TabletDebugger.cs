using System;
using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.Debugging;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Timing;
using OpenTabletDriver.Tablet;
using OpenTabletDriver.UX.Controls.Generic;

namespace OpenTabletDriver.UX.Windows
{
    public class TabletDebugger : DesktopForm
    {
        public TabletDebugger()
        {
            Title = "Tablet Debugger";

            var mainLayout = new TableLayout
            {
                Width = 640,
                Height = 360,
                Spacing = new Size(5, 5),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(rawTabletBox = new TextGroup("Raw Tablet Data"), true),
                            new TableCell(tabletBox = new TextGroup("Tablet Report"), true)
                        },
                        ScaleHeight = true
                    }
                }
            };

            this.Content = new StackLayout
            {
                Padding = 5,
                Spacing = 5,
                Items =
                {
                    new StackLayoutItem(mainLayout, HorizontalAlignment.Stretch, true),
                    new StackLayoutItem(reportRateBox = new TextGroup("Report Rate"), HorizontalAlignment.Stretch)
                }
            };

            InitializeAsync();
        }

        private void InitializeAsync()
        {
            App.Driver.Instance.DebugReportEvent += HandleReport;
            App.Driver.Instance.SetTabletDebug(true);
            this.Closing += (sender, e) =>
            {
                App.Driver.Instance.DebugReportEvent -= HandleReport;
                App.Driver.Instance.SetTabletDebug(false);
            };
        }

        private TextGroup rawTabletBox, tabletBox, reportRateBox;
        private double reportRate;
        private HPETDeltaStopwatch stopwatch = new HPETDeltaStopwatch(true);

        private void HandleReport(object sender, DebugReport report)
        {
            reportRate += (stopwatch.Restart().TotalMilliseconds - reportRate) / 10.0f;

            rawTabletBox.Update(report?.Raw);
            tabletBox.Update(report?.Interpreted.Replace(", ", Environment.NewLine));
            reportRateBox.Update($"{(uint)(1000 / reportRate)}hz");
        }

        private class TextGroup : Group
        {
            public TextGroup(string title)
            {
                base.Text = title;
                base.Content = label;
            }

            private Label label = new Label
            {
                Font = Fonts.Monospace(10)
            };

            public void Update(string text)
            {
                Application.Instance.AsyncInvoke(() => label.Text = text);
            }

            protected override Color VerticalBackgroundColor => base.HorizontalBackgroundColor;
        }
    }
}
