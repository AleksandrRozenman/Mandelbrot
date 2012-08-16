/**
 * Aleksandr Rozenman
 *
 * Mandelbrot fractal generator using WPF.
 * Program will generate a Mandelbrot set fractal based on user input.
 * Input is validated while type, and invalid values will not be registered.
 * Input parameters can also be loaded from or saved to a file.
 * Once a fractal is generated, the image can be saved as well.
 * Image will appear in a scroll pane on the right side of the window.
 * 
 * This file: Complex number coordinate space. Creates the actual fractal.
 *
 * Files in project: Fractal.cs, Complex.cs, ComplexGrid.cs
 * 
 * Mandelbrot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mandelbrot
{
	class ComplexGrid
	{
		// Primary grid properties
		private double xStart, yStart, width, height;
		private int rows, cols;
		private int[,] data;
		private int maxIters;
		private double maxModulus;
		private static string fName = "out.txt";

		// Secondary properties calculated from the primary properties
		private int xIntervals, yIntervals;
		private double dx, dy;

		public int[,] Data { get { return data; } }

		public ComplexGrid(double x, double y, double w, double h, int r, int c, int i, double m)
		{
			xStart = x;
			yStart = y;
			width = w;
			height = h;
			rows = r;
			cols = c;
			data = new int[rows, cols];
			maxIters = i;
			maxModulus = m;

			// Calculated properties
			xIntervals = cols - 1;
			yIntervals = rows - 1;
			dx = width / xIntervals;
			dy = height / yIntervals;

			generateIterCounts();
			saveGrid();
		}

		// Does the math to generate the Mandelbrot grid.
		private void generateIterCounts()
		{
			for(int i = 0; i < rows; i++)
			{
				for(int j = 0; j < cols; j++)
				{
					Complex z = new Complex(0, 0);
					Complex c = new Complex(xStart + (dx * j), yStart + (dy * i));
					int iters = 0;

					while(true)
					{
						z = z.Times(z).Plus(c);
						iters++;

						// Exit while loop when either divergence is shown,
						// or enough iterations through loop happened that
						// divergence is unlikely.
						if(z.Modulus() >= maxModulus || iters >= maxIters)
							break;
					}

					if(iters >= maxIters)   // Set point to 0 to signify no divergence.
						data[i, j] = 0;
					else                    // Set point to the number of iterations required to show divergence.
						data[i, j] = iters;
				}
			}
		}

		// Saves Mandelbrot set to a text file.
		private void saveGrid()
		{
			using(StreamWriter outfile = new StreamWriter(fName))
			{
				for(int i = 0; i < rows; i++)
				{
					string test = "";
					for(int j = 0; j < cols; j++)
						test += string.Format("{0, 5} ", data[i, j]);

					outfile.WriteLine(test);
				}
			}
		}

		// Converts data array into a shaded picture (black and white).
		public BitmapSource generateImage()
		{
			byte[] pixels = new byte[rows * cols * 2];

			int pixelCount = 0;
			for(int i = 0; i < rows; i++)
			{
				for(int j = 0; j < cols; j++)
				{
					ushort count = (ushort)data[i, j];
					count *= 16;                                    // Blow up numbers to make differentiation between values easier to see.
					count += 32000;                                 // Offset to change shading (current value doesn't really do anything--I was playing around with values).
					pixels[pixelCount] = (byte)(count / 256);       // Upper 8b
					pixels[pixelCount + 1] = (byte)(count % 256);   // Lower 8b
					pixelCount += 2;
				}
			}

			BitmapSource bmp = BitmapSource.Create(cols, rows, 96, 96, PixelFormats.Gray16, null, pixels, 2 * cols);
			return bmp;
		}
	}
}
