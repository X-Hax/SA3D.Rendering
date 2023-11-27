using SA3D.Texturing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace SA3D.Rendering.UI.Debugging
{
	/// <summary>
	/// Provides methods for generating debug background sprites that look like the stage select panel in SA2.
	/// </summary>
	public static class DebugBackgroundGenerator
	{
		private static readonly ColorStop[] _borderGradient;
		private static readonly ColorStop[] _backgroundGradient;
		private static readonly ColorStop[] _frameGradient;
		private static readonly ColorStop[] _lineGradient;
		private static readonly Color _lineStart;
		private static readonly Color _lineLeftBevel;
		private static readonly Color _lineRightBevel;
		private static readonly Color _leftBevel;
		private static readonly Color _rightBevel;

		static DebugBackgroundGenerator()
		{
			_backgroundGradient = new ColorStop[] {
				new(0, new Rgb24(0, 180, 183)),
				new(0.25f, new Rgb24(0, 138, 143)),
				new(0.5f, new Rgb24(0, 111, 116)),
				new(0.75f, new Rgb24(0, 88, 99)),
				new(1, new Rgb24(0, 72, 87))
			};

			_borderGradient = new ColorStop[] {
				new(0, new Rgb24(0, 0, 0)),
				new(1, new Rgb24(0, 56, 57))
			};

			_frameGradient = new ColorStop[] {
				new(0, new Rgb24(8, 214, 241)),
				new(0.18f, new Rgb24(7, 215, 249)),
				new(0.35f, new Rgb24(0, 241, 232)),
				new(0.8f, new Rgb24(0, 43, 46)),
				new(1, new Rgb24(0, 46, 55))
			};

			_lineGradient = new ColorStop[] {
				new(0, new Rgb24(16, 214, 238)),
				new(0.5f, new Rgb24(32, 145, 150)),
				new(1, new Rgb24(32, 99, 114))
			};

			_lineStart = new Rgba32(16, 214, 238, 128);
			_lineLeftBevel = new Rgb24(18, 184, 183);
			_lineRightBevel = new Rgb24(64, 153, 175);

			_leftBevel = new Rgb24(0, 125, 90);
			_rightBevel = new Rgb24(19, 92, 109);
		}

		/// <summary>
		/// Generates a debug background texture. Looks like Stage select panel in SA2.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static unsafe Texture GenerateTexture(int width, int height)
		{
			// creating the line area first
			int lineCount = (int)Math.Ceiling((height - 17) / 6f) - 1;
			Image<Rgba32> lineImage = new(width - 5, lineCount * 6);

			LinearGradientBrush lineGradient = new(
				new(9, 0),
				new(lineImage.Width - 6, 0),
				GradientRepetitionMode.None,
				_lineGradient);

			for(int i = 0; i < lineCount; i++)
			{
				int yPos = 4 + (i * 6);

				Rectangle line = new(2, yPos, lineImage.Width - 2, 2);
				lineImage.Mutate(x => x.Fill(lineGradient, line));

				Rectangle lineStart = new(0, yPos, 2, 2);
				lineImage.Mutate(x => x.Fill(_lineStart, lineStart));

				Rectangle lineLeftBevel = new(5, yPos, 2, 2);
				lineImage.Mutate(x => x.Fill(_lineLeftBevel, lineLeftBevel));

				Rectangle lineRightBevel = new(lineImage.Width - 4, yPos, 2, 2);
				lineImage.Mutate(x => x.Fill(_lineRightBevel, lineRightBevel));
			}

			lineImage.Mutate(x => x.Resize(lineImage.Width, height - 17 - 4));

			// assembling everything
			Rectangle borderArea = new(width / 2, 2, width / 2, height - 5);
			LinearGradientBrush borderGradient = new(
				new(borderArea.X, 0),
				new(borderArea.X + borderArea.Width - 3, 0),
				GradientRepetitionMode.None,
				_borderGradient);

			Rectangle backgroundArea = new(2, 3, width - 5, height - 6);
			LinearGradientBrush backgroundGradient = new(
				new(backgroundArea.X, 0),
				new(backgroundArea.X + backgroundArea.Width, 0),
				GradientRepetitionMode.None,
				_backgroundGradient);

			RectangularPolygon outerFrameArea = new(9, 8, width - 16, height - 17);
			RectangularPolygon innerFrameArea = new(outerFrameArea.X + 2, outerFrameArea.Y + 2, outerFrameArea.Width - 4, outerFrameArea.Height - 4);
			IPath frameArea = outerFrameArea.Clip(innerFrameArea);

			Rectangle leftBevel = new((int)outerFrameArea.X - 2, (int)outerFrameArea.Y, 2, (int)outerFrameArea.Height);
			Rectangle rightBevel = new((int)outerFrameArea.X + (int)outerFrameArea.Width, (int)outerFrameArea.Y, 2, (int)outerFrameArea.Height);
			Rectangle bottomBevel = new(2, 6, lineImage.Width, 2);
			Rectangle topBevel = new(2, height - 9, lineImage.Width, 2);

			LinearGradientBrush frameGradient = new(
				new(innerFrameArea.X, 0),
				new(innerFrameArea.X + innerFrameArea.Width, 0),
				GradientRepetitionMode.None,
				_frameGradient);

			Image<Rgba32> image = new(width, height);
			image.Mutate(x => x.Clear(Color.Black));
			image.Mutate(x => x.Fill(new Rgb24(0, 246, 255), new Rectangle(0, 3, width, height - 3)));
			image.Mutate(x => x.Fill(borderGradient, borderArea));
			image.Mutate(x => x.Fill(backgroundGradient, backgroundArea));
			image.Mutate(x => x.Fill(_leftBevel, leftBevel));
			image.Mutate(x => x.Fill(_rightBevel, rightBevel));
			image.Mutate(x => x.Fill(lineGradient, bottomBevel));
			image.Mutate(x => x.Fill(lineGradient, topBevel));
			image.Mutate(x => x.DrawImage(lineImage, new Point(2, 8), 1));
			image.Mutate(x => x.Fill(frameGradient, frameArea));
			image.Mutate(x => x.Flip(FlipMode.Vertical));

			return image.ToTexture();
		}
	}
}
