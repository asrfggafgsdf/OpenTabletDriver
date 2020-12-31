using System;
using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.Debugging;
using OpenTabletDriver.UX.Controls.Generic;

namespace OpenTabletDriver.UX.Windows
{
    public class TabletDebugger : Form
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
        private float reportRate;
        private DateTime lastTime = DateTime.UtcNow;

        private void HandleReport(object sender, DebugReport report)
        {
            var now = DateTime.UtcNow;
            reportRate += (float)(((now - lastTime).TotalMilliseconds - reportRate) / 50);
            lastTime = now;

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