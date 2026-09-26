using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.ImageEditor.Presentation.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawBackgroundEffect : ImageEffectBase
{
    public override string Id => "draw_background";
    public override string Name => "Background";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.paint_bucket;
    public override string Description => "Draws a solid color background behind the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<DrawBackgroundEffect>("color", "Color", SKColors.Black, (e, v) => e.Color = v)
    ];

    public SKColor Color { get; set; } = SKColors.Black;

    /// <summary>Fills with <see cref="GradientStops"/> instead of <see cref="Color"/> (ShareX preset compatibility).</summary>
    public bool UseGradient { get; set; }

    public DrawingGradientType GradientType { get; set; } = DrawingGradientType.Vertical;

    public List<DrawingGradientStop> GradientStops { get; set; } = new();

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);

        using SKPaint paint = new SKPaint { IsAntialias = true, Color = Color };
        using SKShader? gradient = UseGradient ? CreateGradientShader(source.Width, source.Height) : null;
        if (gradient != null)
        {
            paint.Shader = gradient;
        }

        canvas.DrawRect(0, 0, source.Width, source.Height, paint);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private SKShader? CreateGradientShader(int width, int height)
    {
        if (GradientStops == null || GradientStops.Count < 2)
        {
            return null;
        }

        DrawingGradientStop[] stops = GradientStops.OrderBy(stop => stop.Location).ToArray();
        SKColor[] colors = stops.Select(stop => stop.Color).ToArray();
        float[] positions = stops.Select(stop => Math.Clamp(stop.Location / 100f, 0f, 1f)).ToArray();

        (SKPoint start, SKPoint end) = GradientType switch
        {
            DrawingGradientType.Horizontal => (new SKPoint(0, 0), new SKPoint(width, 0)),
            DrawingGradientType.ForwardDiagonal => (new SKPoint(0, 0), new SKPoint(width, height)),
            DrawingGradientType.BackwardDiagonal => (new SKPoint(width, 0), new SKPoint(0, height)),
            _ => (new SKPoint(0, 0), new SKPoint(0, height))
        };

        return SKShader.CreateLinearGradient(start, end, colors, positions, SKShaderTileMode.Clamp);
    }
}
