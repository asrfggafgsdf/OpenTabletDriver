﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Plugin.Output
{
    [PluginIgnore]
    public abstract class AbsoluteOutputMode : IOutputMode
    {
        private IList<IFilter> filters, preFilters, postFilters;
        private Vector2 min, max;
        private Matrix3x2 transformationMatrix;

        public IList<IFilter> Filters
        {
            set
            {
                this.filters = value ?? Array.Empty<IFilter>();
                if (Info.Driver.InterpolatorActive)
                    this.preFilters = Filters.Where(t => t.FilterStage == FilterStage.PreTranspose).ToList();
                else
                    this.preFilters = Filters.Where(t => t.FilterStage == FilterStage.PreTranspose || t.FilterStage == FilterStage.PreInterpolate).ToList();
                this.postFilters = filters.Where(t => t.FilterStage == FilterStage.PostTranspose).ToList();
            }
            get => this.filters;
        }

        private TabletState tablet;
        public TabletState Tablet
        {
            set
            {
                this.tablet = value;
                UpdateTransformMatrix();
            }
            get => this.tablet;
        }

        private Area outputArea, inputArea;

        public Area Input
        {
            set
            {
                this.inputArea = value;
                UpdateTransformMatrix();
            }
            get => this.inputArea;
        }

        public Area Output
        {
            set
            {
                this.outputArea = value;
                UpdateTransformMatrix();
            }
            get => this.outputArea;
        }

        public abstract IAbsolutePointer Pointer { get; }

        public bool AreaClipping { set; get; }
        public bool AreaLimiting { set; get; }

        protected void UpdateTransformMatrix()
        {
            if (Input != null && Output != null && Tablet?.Digitizer != null)
                this.transformationMatrix = CalculateTransformation(Input, Output, Tablet.Digitizer);

            var halfDisplayWidth = Output?.Width / 2 ?? 0;
            var halfDisplayHeight = Output?.Height / 2 ?? 0;

            var minX = Output?.Position.X - halfDisplayWidth ?? 0;
            var maxX = Output?.Position.X + Output?.Width - halfDisplayWidth ?? 0;
            var minY = Output?.Position.Y - halfDisplayHeight ?? 0;
            var maxY = Output?.Position.Y + Output?.Height - halfDisplayHeight ?? 0;

            this.min = new Vector2(minX, minY);
            this.max = new Vector2(maxX, maxY);
        }

        protected static Matrix3x2 CalculateTransformation(Area input, Area output, DigitizerIdentifier tablet)
        {
            // Convert raw tablet data to millimeters
            var res = Matrix3x2.CreateScale(
                tablet.Width / tablet.MaxX,
                tablet.Height / tablet.MaxY);

            // Translate to the center of input area
            res *= Matrix3x2.CreateTranslation(
                -input.Position.X, -input.Position.Y);

            // Apply rotation
            res *= Matrix3x2.CreateRotation(
                (float)(-input.Rotation * System.Math.PI / 180));

            // Scale millimeters to pixels
            res *= Matrix3x2.CreateScale(
                output.Width / input.Width, output.Height / input.Height);

            // Translate output to virtual screen coordinates
            res *= Matrix3x2.CreateTranslation(
                output.Position.X, output.Position.Y);

            return res;
        }

        public virtual void Read(IDeviceReport report)
        {
            if (report is ITabletReport tabletReport)
            {
                if (Tablet.Digitizer.ActiveReportID.IsInRange(tabletReport.ReportID))
                {
                    if (Pointer is IVirtualTablet pressureHandler)
                        pressureHandler.SetPressure((float)tabletReport.Pressure / (float)Tablet.Digitizer.MaxPressure);
                        
                    if (Transpose(tabletReport) is Vector2 pos)
                        Pointer.SetPosition(pos);
                }
            }
        }

        public Vector2? Transpose(ITabletReport report)
        {
            var pos = new Vector2(report.Position.X, report.Position.Y);

            // Pre Filter
            foreach (IFilter filter in this.preFilters ??= Array.Empty<IFilter>())
                pos = filter.Filter(pos);

            // Apply transformation
            pos = Vector2.Transform(pos, this.transformationMatrix);

            // Clipping to display bounds
            var clippedPoint = Vector2.Clamp(pos, this.min, this.max);
            if (AreaLimiting && clippedPoint != pos)
                return null;

            if (AreaClipping)
                pos = clippedPoint;

            // Post Filter
            foreach (IFilter filter in this.postFilters ??= Array.Empty<IFilter>())
                pos = filter.Filter(pos);

            return pos;
        }
    }
}
