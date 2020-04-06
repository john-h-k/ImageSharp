// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Effects;
using Xunit;

namespace SixLabors.ImageSharp.Tests.Processing.Processors.Effects
{
    public partial class PixelShaderTest
    {
        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void FullImageWithValueDelegate<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunValidatingProcessorTest(
                x => x.ProcessPixelRowsAsVector4<PixelAverageProcessor>(),
                appendPixelTypeToFileName: false);
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void InBoxWithValueDelegate<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunRectangleConstrainedValidatingProcessorTest(
                (x, rect) => x.ProcessPixelRowsAsVector4<PixelAverageProcessor>(rect));
        }

        private readonly struct PixelAverageProcessor : IPixelRowDelegate
        {
            public void Invoke(Span<Vector4> span)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    Vector4 v4 = span[i];
                    float avg = (v4.X + v4.Y + v4.Z) / 3f;
                    span[i] = new Vector4(avg);
                }
            }
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void PositionAwareFullImageWithValueDelegate<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunValidatingProcessorTest(
                c => c.ProcessPositionAwarePixelRowsAsVector4<TrigonometryProcessor>(),
                appendPixelTypeToFileName: false);
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void PositionAwareInBoxWithValueDelegate<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunRectangleConstrainedValidatingProcessorTest(
                (c, rect) => c.ProcessPositionAwarePixelRowsAsVector4<TrigonometryProcessor>(rect));
        }

        private readonly struct TrigonometryProcessor : IPixelRowDelegate<Point>
        {
            public void Invoke(Span<Vector4> span, Point value)
            {
                int y = value.Y;
                int x = value.X;
                for (int i = 0; i < span.Length; i++)
                {
                    float
                        sine = MathF.Sin(y),
                        cosine = MathF.Cos(x + i),
                        sum = sine + cosine,
                        abs = MathF.Abs(sum),
                        a = 0.5f + (abs / 2);

                    Vector4 v4 = span[i];
                    float avg = (v4.X + v4.Y + v4.Z) / 3f;
                    var gray = new Vector4(avg, avg, avg, a);

                    span[i] = Vector4.Clamp(gray, Vector4.Zero, Vector4.One);
                }
            }
        }
    }
}