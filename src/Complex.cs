/**
 * Programmer: Aleksandr Rozenman
 *
 * Mandelbrot fractal generator using WPF.
 * Program will generate a Mandelbrot set fractal based on user input.
 * Input is validated while type, and invalid values will not be registered.
 * Input parameters can also be loaded from or saved to a file.
 * Once a fractal is generated, the image can be saved as well.
 * Image will appear in a scroll pane on the right side of the window.
 * 
 * This file: Helper class for dealing with complex numbers.
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

using System;

namespace Mandelbrot
{
	class Complex
	{
		// Variables
		private double real;
		private double imaginary;

		// Constructors
		public Complex()
		{
			real = imaginary = 0.0;
		}

		public Complex(double r, double i)
		{
			real = r;
			imaginary = i;
		}

		// Properties.
		public double Real
		{
			get { return real; }
		}

		public double Imaginary
		{
			get { return imaginary; }
		}

		// Performs a complex number addition.
		public Complex Plus(Complex a)
		{
			Complex sum = new Complex(real + a.Real, imaginary + a.Imaginary);
			return sum;
		}

		// Performs a complex number subtraction.
		public Complex Minus(Complex a)
		{
			Complex difference = new Complex(real - a.Real, imaginary - a.Imaginary);
			return difference;
		}

		// Performs a complex number multiplication.
		public Complex Times(Complex a)
		{
			double rProd = (real * a.Real) - (imaginary * a.Imaginary);
			double iProd = (imaginary * a.Real) + (real * a.Imaginary);

			Complex product = new Complex(rProd, iProd);
			return product;
		}

		// Performs a complex number division.
		public Complex Divide(Complex a)
		{
			double rQuot, iQuot, denom;
			denom = Math.Pow(a.Real, 2) + Math.Pow(a.Imaginary, 2);
			rQuot = ((real * a.Real) + (imaginary * a.Imaginary)) / denom;
			iQuot = ((imaginary * a.Real) - (real * a.Imaginary)) / denom;

			Complex quotient = new Complex(rQuot, iQuot);
			return quotient;
		}

		// Returns the modulus (absolute value) of a complex number.
		public double Modulus()
		{
			double mod = Math.Sqrt(Math.Pow(real, 2) + Math.Pow(imaginary, 2));
			return mod;
		}

		public override string ToString()
		{
			return string.Format("{0} + {1}i", real, imaginary);
		}
	}
}
