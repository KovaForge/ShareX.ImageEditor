using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class MagnifyAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation.
    /// </summary>
    public Control CreateVisual()
    {
        return new Avalonia.Controls.Shapes.Rectangle
        {
            Stroke = Brushes.Transparent,
            StrokeThickness = 0,
            Fill = Brushes.Transparent,
            Tag = this
        };
    }

    internal Control CreatePreviewVisual()
    {
        return new Rectangle
        {
            Fill = new SolidColorBrush(Color.FromArgb(30, 211, 211, 211)),
            Stroke = new SolidColorBrush(Color.FromArgb(80, 100, 100, 100)),
            StrokeThickness = 1,
            Tag = this
        };
    }
}
