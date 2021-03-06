Programmer: Aleksandr Rozenman

Mandelbrot fractal generator using WPF.
Program will generate a Mandelbrot set fractal based on user input.
Input is validated while type, and invalid values will not be registered.
Input parameters can also be loaded from or saved to a file.
Once a fractal is generated, the image can be saved as well.
Image will appear in a scroll pane on the right side of the window.

Files in project:
Fractal.cs:       Main GUI.
Complex.cs:       Helper class for dealing with complex numbers.
ComplexGrid.cs:   Complex number coordinate space. Creates the actual fractal.

Mandelbrot is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.