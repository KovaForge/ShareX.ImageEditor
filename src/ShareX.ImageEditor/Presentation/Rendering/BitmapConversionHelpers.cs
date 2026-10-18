#region License Information (GPL v3)

/*
    ShareX.ImageEditor - The UI-agnostic Editor library for ShareX
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering;

/// <summary>Converts bitmap pixels without assuming the renderer's channel or alpha layout.</summary>
public static class BitmapConversionHelpers
{
    public static SKBitmap ToSKBitmap(Bitmap avaloniaBitmap)
    {
        ArgumentNullException.ThrowIfNull(avaloniaBitmap);
        if (avaloniaBitmap is WriteableBitmap writable)
        {
            using var locked = writable.Lock();
            if (TryGetInfo(locked.Size, locked.Format, locked.AlphaFormat, out var info))
            {
                var result = new SKBitmap(info);
                try
                {
                    CopyRows(locked.Address, locked.RowBytes, result.GetPixels(), result.RowBytes, info.RowBytes, info.Height);
                    return result;
                }
                catch { result.Dispose(); throw; }
            }
        }
        else if (avaloniaBitmap.Format is { } format && avaloniaBitmap.AlphaFormat is { } alpha &&
                 TryGetInfo(avaloniaBitmap.PixelSize, format, alpha, out var info))
        {
            var result = new SKBitmap(info);
            try
            {
                avaloniaBitmap.CopyPixels(new PixelRect(avaloniaBitmap.PixelSize), result.GetPixels(), result.ByteCount, result.RowBytes);
                return result;
            }
            catch (NotSupportedException) { result.Dispose(); }
            catch { result.Dispose(); throw; }
        }

        // Render targets and uncommon/transcoded formats may not support direct readback.
        // PNG preserves their declared colours and alpha without guessing a byte layout.
        using var encoded = new System.IO.MemoryStream();
        avaloniaBitmap.Save(encoded, PngBitmapEncoderOptions.Default);
        encoded.Position = 0;
        return SKBitmap.Decode(encoded) ?? throw new InvalidOperationException("Could not decode bitmap pixels.");
    }

    public static Bitmap ToAvaloniBitmap(SKBitmap skBitmap)
    {
        ArgumentNullException.ThrowIfNull(skBitmap);
        var result = new WriteableBitmap(new PixelSize(skBitmap.Width, skBitmap.Height),
            new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        try
        {
            using var locked = result.Lock();
            if (!TryGetInfo(locked.Size, locked.Format, locked.AlphaFormat, out var destination))
                throw new NotSupportedException($"Unsupported destination pixel format: {locked.Format}");
            using var pixels = skBitmap.PeekPixels();
            if (pixels == null || !pixels.ReadPixels(destination, locked.Address, locked.RowBytes, 0, 0))
                throw new InvalidOperationException("Could not convert bitmap pixels.");
            return result;
        }
        catch { result.Dispose(); throw; }
    }

    private static bool TryGetInfo(PixelSize size, PixelFormat format, AlphaFormat alpha, out SKImageInfo info)
    {
        var colour = format == PixelFormat.Bgra8888 ? SKColorType.Bgra8888 :
            format == PixelFormat.Rgba8888 ? SKColorType.Rgba8888 : SKColorType.Unknown;
        var alphaType = alpha switch
        {
            AlphaFormat.Opaque => SKAlphaType.Opaque,
            AlphaFormat.Unpremul => SKAlphaType.Unpremul,
            _ => SKAlphaType.Premul
        };
        info = new SKImageInfo(size.Width, size.Height, colour, alphaType);
        return colour != SKColorType.Unknown;
    }

    private static unsafe void CopyRows(IntPtr source, int sourceStride, IntPtr destination, int destinationStride, int rowBytes, int height)
    {
        for (int y = 0; y < height; y++)
            Buffer.MemoryCopy((byte*)source + y * sourceStride, (byte*)destination + y * destinationStride, rowBytes, rowBytes);
    }
}
