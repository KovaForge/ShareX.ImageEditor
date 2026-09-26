using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.ImageEditor.Presentation.Controls;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SpotlightAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation.
    /// </summary>
    public Control CreateVisual()
    {
        return new SpotlightControl
        {
            Annotation = this,
            IsHitTestVisible = false,
            Tag = this
        };
    }

    internal void UpdateVisual(SpotlightControl spotlightControl, double canvasWidth, double canvasHeight)
    {
        if (canvasWidth > 0 && canvasHeight > 0)
        {
            CanvasSize = new SKSize((float)canvasWidth, (float)canvasHeight);
        }

        spotlightControl.Annotation = this;
        Canvas.SetLeft(spotlightControl, 0);
        Canvas.SetTop(spotlightControl, 0);
        spotlightControl.Width = Math.Max(1, CanvasSize.Width);
        spotlightControl.Height = Math.Max(1, CanvasSize.Height);
        spotlightControl.InvalidateVisual();
    }

    internal Control CreatePreviewVisual()
    {
        Shape shape = IsEllipse ? new Ellipse() : new Rectangle();

        shape.Fill = Brushes.Transparent;
        shape.Stroke = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
        shape.StrokeThickness = 2;
        shape.StrokeDashArray = new AvaloniaList<double> { 6, 3 };
        shape.Tag = this;

        return shape;
    }
}
