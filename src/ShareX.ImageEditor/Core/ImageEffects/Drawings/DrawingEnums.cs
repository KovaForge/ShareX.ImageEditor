using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public enum DrawingPlacement
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public enum DrawingImageSizeMode
{
    DontResize,
    AbsoluteSize,
    PercentageOfWatermark,
    PercentageOfCanvas
}

public enum DrawingShapeType
{
    Rectangle,
    RoundedRectangle,
    Ellipse,
    Triangle,
    Diamond
}

public enum DrawingImageRotateFlipType
{
    None = 0,
    Rotate90 = 1,
    Rotate180 = 2,
    Rotate270 = 3,
    FlipX = 4,
    Rotate90FlipX = 5,
    FlipY = 6,
    Rotate90FlipY = 7
}

public enum DrawingInterpolationMode
{
    HighQualityBicubic,
    Bicubic,
    HighQualityBilinear,
    Bilinear,
    NearestNeighbor
}

public enum DrawingCompositingMode
{
    SourceOver,
    SourceCopy
}

public enum DrawingGradientType
{
    Vertical,
    Horizontal,
    ForwardDiagonal,
    BackwardDiagonal
}

/// <summary>One gradient color stop. <see cref="Location"/> is a percentage (0-100), as in ShareX presets.</summary>
public sealed class DrawingGradientStop
{
    public SKColor Color { get; set; }
    public float Location { get; set; }

    public DrawingGradientStop()
    {
    }

    public DrawingGradientStop(SKColor color, float location)
    {
        Color = color;
        Location = location;
    }
}
