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
 * This file: Main GUI.
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

using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mandelbrot
{
	public class Fractal : Window
	{
		// GUI components.
		private DockPanel panel;
		private Menu mainMenu;
		private Grid topGrid, subGrid;
		private TextBlock optionsLabel, imgLabel;
		private TextBlock xStartLabel, yStartLabel;
		private TextBlock widthLabel, heightLabel;
		private TextBlock rowsLabel, colsLabel;
		private TextBlock xIntervalLabel, yIntervalLabel;
		private TextBlock maxItersLabel;
		private TextBlock maxModulusLabel;
		private TextBox xStartHolder, yStartHolder;
		private TextBox widthHolder, heightHolder;
		private TextBox rowsHolder, colsHolder;
		private TextBox xIntervalHolder, yIntervalHolder;
		private TextBox maxItersHolder;
		private TextBox maxModulusHolder;
		private Button genFractalBtn;
		private ScrollViewer scroll;
		private Canvas canvas;
		private Image img;
		private BitmapSource bmp;

		// Variables.
		private ComplexGrid compGrid;
		private double xStart, yStart;
		private double width, height;
		private int rows, cols;
		private int xInterval, yInterval;
		private int maxIters;
		private double maxModulus;

		public Fractal()
		{
			this.Title = "Rozenman Mandelbrot";
			this.MinWidth = this.Width = 600;
			this.MinHeight = this.Height = 480;

			panel = new DockPanel();
			this.Content = panel;

			// Build menus.
			mainMenu = new Menu();
			panel.Children.Add(mainMenu);
			DockPanel.SetDock(mainMenu, Dock.Top);

			MenuItem miFile = new MenuItem();
			miFile.Header = "File";
			mainMenu.Items.Add(miFile);
			MenuItem fLoad = new MenuItem();
			fLoad.Header = "Load grid parameters...";
			miFile.Items.Add(fLoad);
			MenuItem fSaveParams = new MenuItem();
			fSaveParams.Header = "Save grid parameters...";
			miFile.Items.Add(fSaveParams);
			MenuItem fCreate = new MenuItem();
			fCreate.Header = "Create Image";
			miFile.Items.Add(fCreate);
			MenuItem fSaveImage = new MenuItem();
			fSaveImage.Header = "Save image...";
			miFile.Items.Add(fSaveImage);

			// Load parameters from file when menu item is chosen.
			fLoad.Click += delegate(object sender, RoutedEventArgs args) {
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.DefaultExt = ".txt";            // Default file extension
				dlg.Filter = "Text (.txt)|*.txt";   // Filter files by extension

				// Show open file dialog box
				Nullable<bool> result = dlg.ShowDialog();

				// Process open file dialog box results
				if(result == true)
				{
					string fName = dlg.FileName;
					double x = 0, y = 0, w = 0, h = 0, m = 0;
					int r = 0, c = 0, i = 0;
					bool isNum, failure;
					failure = false;
					string[] input;

					// Read file.
					using(StreamReader infile = new StreamReader(fName))
					{
						input = infile.ReadLine().Split(new char[] { ' ' });
					}

					if(8 != input.Length)   // Fail if file is of improper length.
						failure = true;
					else                    // File length is correct, so try to parse values.
					{
						isNum = double.TryParse(input[0], out x);
						if(!isNum)          // Parse failed, produce error. Repeat for all params.
							failure = true;
						isNum = double.TryParse(input[1], out y);
						if(!isNum)
							failure = true;
						isNum = double.TryParse(input[2], out w);
						if(!isNum)
							failure = true;
						isNum = double.TryParse(input[3], out h);
						if(!isNum)
							failure = true;
						isNum = int.TryParse(input[4], out r);
						if(!isNum)
							failure = true;
						isNum = int.TryParse(input[5], out c);
						if(!isNum)
							failure = true;
						isNum = int.TryParse(input[6], out i);
						if(!isNum)
							failure = true;
						isNum = double.TryParse(input[7], out m);
						if(!isNum)
							failure = true;
					}

					if(failure)         // Inform user of error in case of failure.
						MessageBox.Show("The file is corrupt or is not formatted correctly. Try another file.", "Error: Could Not Read File");
					else                // Param parsing successful, update values.
					{
						xStart = x;
						yStart = y;
						width = w;
						height = h;
						rows = r;
						cols = c;
						maxIters = i;
						maxModulus = m;

						xStartHolder.Text = input[0];
						yStartHolder.Text = input[1];
						widthHolder.Text = input[2];
						heightHolder.Text = input[3];
						rowsHolder.Text = input[4];
						colsHolder.Text = input[5];
						maxItersHolder.Text = input[6];
						maxModulusHolder.Text = input[7];
					}
				}
			}; // End load grid params delegate.

			// Save parameters to file when chosen.
			fSaveParams.Click += delegate(object sender, RoutedEventArgs args) {
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.FileName = "Params";            // Default file name
				dlg.DefaultExt = ".txt";            // Default file extension
				dlg.Filter = "Text (.txt)|*.txt";   // Filter files by extension

				// Show save file dialog box
				Nullable<bool> result = dlg.ShowDialog();

				// Process save file dialog box results
				if(result == true)
				{
					string fName = dlg.FileName;
					// Write to file.
					using(StreamWriter outfile = new StreamWriter(fName))
					{
						outfile.WriteLine(
							"{0} {1} {2} {3} {4} {5} {6} {7}",
							xStart,
							yStart,
							width,
							height,
							rows,
							cols,
							maxIters,
							maxModulus
						);
					}
				}
			}; // End save grid params delegate.

			// Create image menu item. Same as "generate fractal" button.
			fCreate.Click += delegate(object sender, RoutedEventArgs args) {
				createImage();
			}; // End create image delegate.

			// Saves image to HDD as jpeg file.
			fSaveImage.Click += delegate(object sender, RoutedEventArgs args) {
				if(null == bmp)     // Display error if there is no fractal to save.
					MessageBox.Show("No image to save. Generate one first.", "Error: No Image");
				else                // Saves image.
				{
					SaveFileDialog dlg = new SaveFileDialog();
					dlg.FileName = "Image";             // Default file name
					dlg.DefaultExt = ".jpg";            // Default file extension
					dlg.Filter = "Image (.jpg)|*.jpg";  // Filter files by extension

					// Show save file dialog box
					Nullable<bool> result = dlg.ShowDialog();

					// Process save file dialog box results; save image.
					if(result == true)
					{
						string fName = dlg.FileName;
						FileStream outfile = new FileStream(fName, FileMode.Create);
						JpegBitmapEncoder encoder = new JpegBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(bmp));
						encoder.Save(outfile);
					}
				}
			}; // End save image delegate.

			// Builds GIU. Divides window into two segments.
			topGrid = new Grid();
			panel.Children.Add(topGrid);
			DockPanel.SetDock(topGrid, Dock.Bottom);
			RowDefinition topRow1 = new RowDefinition();
			RowDefinition topRow2 = new RowDefinition();
			topGrid.RowDefinitions.Add(topRow1);
			topGrid.RowDefinitions.Add(topRow2);
			ColumnDefinition topCol1 = new ColumnDefinition();
			ColumnDefinition topCol2 = new ColumnDefinition();
			topGrid.ColumnDefinitions.Add(topCol1);
			topGrid.ColumnDefinitions.Add(topCol2);
			topRow1.Height = new GridLength(50);
			topCol1.Width = new GridLength(255);

			optionsLabel = new TextBlock();
			optionsLabel.Text = "Options:";
			Grid.SetRow(optionsLabel, 0);
			Grid.SetColumn(optionsLabel, 0);
			topGrid.Children.Add(optionsLabel);
			optionsLabel.HorizontalAlignment = HorizontalAlignment.Center;
			optionsLabel.VerticalAlignment = VerticalAlignment.Center;
			imgLabel = new TextBlock();
			imgLabel.Text = "Image:";
			Grid.SetRow(imgLabel, 0);
			Grid.SetColumn(imgLabel, 1);
			topGrid.Children.Add(imgLabel);
			imgLabel.HorizontalAlignment = HorizontalAlignment.Center;
			imgLabel.VerticalAlignment = VerticalAlignment.Center;

			// Builds left segment, which contains the editable parameters.
			subGrid = new Grid();
			Grid.SetRow(subGrid, 1);
			Grid.SetColumn(subGrid, 0);
			topGrid.Children.Add(subGrid);
			for(int i = 0; i < 7; i++)
				subGrid.RowDefinitions.Add(new RowDefinition());
			for(int i = 0; i < 4; i++)
				subGrid.ColumnDefinitions.Add(new ColumnDefinition());
			subGrid.VerticalAlignment = VerticalAlignment.Top;

			// TextBlock/TextBox initializations need to be in the constructor directly;
			// making a more general method to create them results in a NullReferenceException.
			xStartLabel = new TextBlock();
			xStartLabel.Text = "x start: ";
			Grid.SetRow(xStartLabel, 0);
			Grid.SetColumn(xStartLabel, 0);
			subGrid.Children.Add(xStartLabel);
			xStartLabel.HorizontalAlignment = HorizontalAlignment.Right;
			xStartLabel.VerticalAlignment = VerticalAlignment.Center;
			yStartLabel = new TextBlock();
			yStartLabel.Text = "y start: ";
			Grid.SetRow(yStartLabel, 0);
			Grid.SetColumn(yStartLabel, 2);
			subGrid.Children.Add(yStartLabel);
			yStartLabel.HorizontalAlignment = HorizontalAlignment.Right;
			yStartLabel.VerticalAlignment = VerticalAlignment.Center;

			xStartHolder = new TextBox();
			Grid.SetRow(xStartHolder, 0);
			Grid.SetColumn(xStartHolder, 1);
			subGrid.Children.Add(xStartHolder);
			yStartHolder = new TextBox();
			Grid.SetRow(yStartHolder, 0);
			Grid.SetColumn(yStartHolder, 3);
			subGrid.Children.Add(yStartHolder);

			widthLabel = new TextBlock();
			widthLabel.Text = "width: ";
			Grid.SetRow(widthLabel, 1);
			Grid.SetColumn(widthLabel, 0);
			subGrid.Children.Add(widthLabel);
			widthLabel.HorizontalAlignment = HorizontalAlignment.Right;
			widthLabel.VerticalAlignment = VerticalAlignment.Center;
			heightLabel = new TextBlock();
			heightLabel.Text = "height: ";
			Grid.SetRow(heightLabel, 1);
			Grid.SetColumn(heightLabel, 2);
			subGrid.Children.Add(heightLabel);
			heightLabel.HorizontalAlignment = HorizontalAlignment.Right;
			heightLabel.VerticalAlignment = VerticalAlignment.Center;

			widthHolder = new TextBox();
			Grid.SetRow(widthHolder, 1);
			Grid.SetColumn(widthHolder, 1);
			subGrid.Children.Add(widthHolder);
			heightHolder = new TextBox();
			Grid.SetRow(heightHolder, 1);
			Grid.SetColumn(heightHolder, 3);
			subGrid.Children.Add(heightHolder);

			rowsLabel = new TextBlock();
			rowsLabel.Text = "rows: ";
			Grid.SetRow(rowsLabel, 2);
			Grid.SetColumn(rowsLabel, 0);
			subGrid.Children.Add(rowsLabel);
			rowsLabel.HorizontalAlignment = HorizontalAlignment.Right;
			rowsLabel.VerticalAlignment = VerticalAlignment.Center;
			colsLabel = new TextBlock();
			colsLabel.Text = "cols: ";
			Grid.SetRow(colsLabel, 2);
			Grid.SetColumn(colsLabel, 2);
			subGrid.Children.Add(colsLabel);
			colsLabel.HorizontalAlignment = HorizontalAlignment.Right;
			colsLabel.VerticalAlignment = VerticalAlignment.Center;

			rowsHolder = new TextBox();
			Grid.SetRow(rowsHolder, 2);
			Grid.SetColumn(rowsHolder, 1);
			subGrid.Children.Add(rowsHolder);
			colsHolder = new TextBox();
			Grid.SetRow(colsHolder, 2);
			Grid.SetColumn(colsHolder, 3);
			subGrid.Children.Add(colsHolder);

			xIntervalLabel = new TextBlock();
			xIntervalLabel.Text = "x interval: ";
			Grid.SetRow(xIntervalLabel, 3);
			Grid.SetColumn(xIntervalLabel, 0);
			subGrid.Children.Add(xIntervalLabel);
			xIntervalLabel.HorizontalAlignment = HorizontalAlignment.Right;
			xIntervalLabel.VerticalAlignment = VerticalAlignment.Center;
			yIntervalLabel = new TextBlock();
			yIntervalLabel.Text = "y interval: ";
			Grid.SetRow(yIntervalLabel, 3);
			Grid.SetColumn(yIntervalLabel, 2);
			subGrid.Children.Add(yIntervalLabel);
			yIntervalLabel.HorizontalAlignment = HorizontalAlignment.Right;
			yIntervalLabel.VerticalAlignment = VerticalAlignment.Center;

			xIntervalHolder = new TextBox();
			Grid.SetRow(xIntervalHolder, 3);
			Grid.SetColumn(xIntervalHolder, 1);
			subGrid.Children.Add(xIntervalHolder);
			yIntervalHolder = new TextBox();
			Grid.SetRow(yIntervalHolder, 3);
			Grid.SetColumn(yIntervalHolder, 3);
			subGrid.Children.Add(yIntervalHolder);

			maxItersLabel = new TextBlock();
			maxItersLabel.Text = "max iterations: ";
			Grid.SetRow(maxItersLabel, 4);
			Grid.SetColumn(maxItersLabel, 0);
			Grid.SetColumnSpan(maxItersLabel, 2);
			subGrid.Children.Add(maxItersLabel);
			maxItersLabel.HorizontalAlignment = HorizontalAlignment.Right;
			maxItersLabel.VerticalAlignment = VerticalAlignment.Center;

			maxItersHolder = new TextBox();
			Grid.SetRow(maxItersHolder, 4);
			Grid.SetColumn(maxItersHolder, 2);
			Grid.SetColumnSpan(maxItersHolder, 2);
			subGrid.Children.Add(maxItersHolder);

			maxModulusLabel = new TextBlock();
			maxModulusLabel.Text = "max modulus: ";
			Grid.SetRow(maxModulusLabel, 5);
			Grid.SetColumn(maxModulusLabel, 0);
			Grid.SetColumnSpan(maxModulusLabel, 2);
			subGrid.Children.Add(maxModulusLabel);
			maxModulusLabel.HorizontalAlignment = HorizontalAlignment.Right;
			maxModulusLabel.VerticalAlignment = VerticalAlignment.Center;

			maxModulusHolder = new TextBox();
			Grid.SetRow(maxModulusHolder, 5);
			Grid.SetColumn(maxModulusHolder, 2);
			Grid.SetColumnSpan(maxModulusHolder, 2);
			subGrid.Children.Add(maxModulusHolder);

			xStart = -2.0;
			xStartHolder.Text = xStart.ToString();
			yStart = -1.5;
			yStartHolder.Text = yStart.ToString();
			width = height = 3.0;
			widthHolder.Text = width.ToString();
			heightHolder.Text = height.ToString();
			rows = cols = 101;
			rowsHolder.Text = colsHolder.Text = rows.ToString();
			xInterval = yInterval = rows - 1;
			xIntervalHolder.Text = yIntervalHolder.Text = xInterval.ToString();
			maxIters = 1000;
			maxItersHolder.Text = maxIters.ToString();
			maxModulus = 1000.0;
			maxModulusHolder.Text = maxModulus.ToString();

			// TextChanged delegates validate input.
			// Whenever the user changes the value, it checks to see if it's valid.
			// If change is invalid, it discards the change;
			// If change is valid, it updates related fields as necessary.
			xStartHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				if("" == xStartHolder.Text || "-" == xStartHolder.Text)
					xStart = 0.0;
				else
				{
					double check;
					bool isNum = double.TryParse(xStartHolder.Text, out check);
					if(isNum)
						xStart = check;
					else
						xStartHolder.Text = xStart.ToString();
				}
			};

			yStartHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				if("" == yStartHolder.Text || "-" == yStartHolder.Text)
					yStart = 0.0;
				else
				{
					double check;
					bool isNum = double.TryParse(yStartHolder.Text, out check);
					if(isNum)
						yStart = check;
					else
						yStartHolder.Text = yStart.ToString();
				}
			};

			widthHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				double check;
				bool isNum = double.TryParse(widthHolder.Text, out check);
				if(isNum)
				{
					width = check;
					if(width <= 0.0)
					{
						width = 3.0;
						widthHolder.Text = width.ToString();
					}
				}
				else
					widthHolder.Text = width.ToString();
			};

			heightHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				double check;
				bool isNum = double.TryParse(heightHolder.Text, out check);
				if(isNum)
				{
					height = check;
					if(height <= 0.0)
					{
						height = 2.0;
						heightHolder.Text = height.ToString();
					}
				}

				else
					heightHolder.Text = height.ToString();
			};

			rowsHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				int check;
				bool isNum = int.TryParse(rowsHolder.Text, out check);
				if(isNum)
				{
					rows = check;
					if(rows < 2)
					{
						rows = 2;
						rowsHolder.Text = rows.ToString();
					}
					xInterval = rows - 1;
					xIntervalHolder.Text = xInterval.ToString();
				}
				else
					rowsHolder.Text = rows.ToString();
			};

			colsHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				int check;
				bool isNum = int.TryParse(colsHolder.Text, out check);
				if(isNum)
				{
					cols = check;
					if(cols < 2)
					{
						cols = 2;
						colsHolder.Text = cols.ToString();
					}
					yInterval = cols - 1;
					yIntervalHolder.Text = yInterval.ToString();
				}
				else
					colsHolder.Text = cols.ToString();
			};

			xIntervalHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				int check;
				bool isNum = int.TryParse(xIntervalHolder.Text, out check);
				if(isNum)
				{
					xInterval = check;
					if(xInterval < 1)
					{
						xInterval = 1;
						xIntervalHolder.Text = xInterval.ToString();
					}
					rows = xInterval + 1;
					rowsHolder.Text = rows.ToString();
				}
				else
					xIntervalHolder.Text = xInterval.ToString();
			};

			yIntervalHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				int check;
				bool isNum = int.TryParse(yIntervalHolder.Text, out check);
				if(isNum)
				{
					yInterval = check;
					if(yInterval < 1)
					{
						yInterval = 1;
						yIntervalHolder.Text = yInterval.ToString();
					}
					cols = yInterval + 1;
					colsHolder.Text = cols.ToString();
				}
				else
					yIntervalHolder.Text = yInterval.ToString();
			};

			maxItersHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				int check;
				bool isNum = int.TryParse(maxItersHolder.Text, out check);
				if(isNum)
				{
					maxIters = check;
					if(maxIters < 1)
					{
						maxIters = 1;
						maxItersHolder.Text = maxIters.ToString();
					}
				}
				else
					maxItersHolder.Text = maxIters.ToString();
			};

			maxModulusHolder.TextChanged += delegate(object sender, TextChangedEventArgs args) {
				double check;
				bool isNum = double.TryParse(maxModulusHolder.Text, out check);
				if(isNum)
				{
					maxModulus = check;
					if(maxModulus <= 0.0)
					{
						maxModulus = 1000.0;
						maxModulusHolder.Text = maxModulus.ToString();
					}
				}
				else
					maxModulusHolder.Text = maxModulus.ToString();
			};

			genFractalBtn = new Button();
			genFractalBtn.Content = "Generate fractal";
			Grid.SetRow(genFractalBtn, 6);
			Grid.SetColumn(genFractalBtn, 1);
			Grid.SetColumnSpan(genFractalBtn, 2);
			subGrid.Children.Add(genFractalBtn);
			// Generate fractal button. Same as "Create image" menu item.
			genFractalBtn.Click += delegate(object sender, RoutedEventArgs args) {
				createImage();
			}; // End generate fractal delegate.

			// Right side of window. Has scroll bars to fit image within window.
			scroll = new ScrollViewer();
			Grid.SetRow(scroll, 1);
			Grid.SetColumn(scroll, 1);
			topGrid.Children.Add(scroll);
			scroll.VerticalAlignment = VerticalAlignment.Center;
			scroll.HorizontalAlignment = HorizontalAlignment.Center;
			scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

			canvas = new Canvas();
			scroll.Content = canvas;
		}

		// Generates the fractal, then displays it.
		private void createImage()
		{
			compGrid = new ComplexGrid(xStart, yStart, width, height, rows, cols, maxIters, maxModulus);

			bmp = compGrid.generateImage();   // Generates image.

			// Displays image.
			img = new Image();
			canvas.Children.Add(img);
			img.Source = bmp;
			Canvas.SetTop(img, 10);
			Canvas.SetLeft(img, 10);
			canvas.Height = rows;
			canvas.Width = cols;
		}

		[STAThread]
		public static void Main(string[] args)
		{
			Fractal win = new Fractal();
			win.Show();
			Application app = new Application();
			app.Run();
		}
	}
}
