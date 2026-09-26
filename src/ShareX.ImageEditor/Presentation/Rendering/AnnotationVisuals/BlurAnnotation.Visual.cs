using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class BlurAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation.
    /// </summary>
    public Control CreateVisual()
    {
        return new Avalonia.Controls.Shapes.Rectangle
        {
            Stroke = Brushes.Transparent,
            StrokeThickness = StrokeWidth,
            Fill = new SolidColorBrush(Color.Parse("#200000FF")),
            Tag = this
        };
    }

    internal Control CreatePreviewVisual()
    {
        return new Rectangle
        {
            Fill = new SolidColorBrush(Color.Parse("#200000FF")),
            Stroke = new SolidColorBrush(Color.FromArgb(80, 0, 0, 255)),
            StrokeThickness = 1,
            Tag = this
        };
    }
}
