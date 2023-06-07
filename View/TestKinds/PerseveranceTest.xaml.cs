using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для PerseveranceTest.xaml
    /// </summary>
    public partial class PerseveranceTest : Page, INotifyPropertyChanged
    {
        PreseveranceTestDictionary dictionary { get; set; }
        System.Windows.Threading.DispatcherTimer timer { get; set; }
        int time = 0;

        private int nextNumber = 0;
        bool numberUp = true;

        public int NextNumber
        {
            get => nextNumber + 1;
            set
            {
                nextNumber = value - 1;
                OnPropertyChanged("NextNumber");
            }
        }

        public PerseveranceTest()
        {
            InitializeComponent();
            MainViewModel.MainWindow.Title = this.Title;
            MainViewModel.MouseHover(BackButton);
            DataContext = this;
            dictionary = new PreseveranceTestDictionary();
            timer = new();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            time += 1;
        }


        private void CreatePicture(int imageNumber, bool createMouseDown = true)
        {
            Image newImage = new Image();
            newImage.Source = MainViewModel.GetBitmap(dictionary.NumberSources[imageNumber]);
            newImage.Name = $"img{imageNumber}";
            ImageSizeChange(newImage);
            newImage.Stretch = Stretch.Fill;
            newImage.VerticalAlignment = VerticalAlignment.Top;
            newImage.HorizontalAlignment = HorizontalAlignment.Left;
            if (createMouseDown)
                newImage.MouseDown += (s, e) => { Image_MouseDown(s, e); };
            ThisGrid.Children.Add(newImage);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (nextNumber == 0)
                timer.Start();
            if (Convert.ToInt32((sender as Image).Name.Replace("img", "")) == nextNumber)
            {
                NextNumber++;
                SolidColorBrush gridBgColor = ThisGrid.Background as SolidColorBrush;

                if (gridBgColor.Color.G < gridBgColor.Color.R)
                    gridBgColor.Color = Color.FromRgb(30, 55, 30);

                if (numberUp)
                {
                    if (gridBgColor.Color.G >= 55 && gridBgColor.Color.G <= 235)
                    {
                        ThisGrid.Background = new SolidColorBrush(Color.FromRgb(
                            (byte)(gridBgColor.Color.R + 10),
                            (byte)(gridBgColor.Color.G + 20),
                            (byte)(gridBgColor.Color.B + 10)));
                    }
                    else
                    {
                        numberUp = false;
                        NextNumber--;
                        Image_MouseDown(sender, e);
                    }
                }
                else
                {
                    if (gridBgColor.Color.G >= 75 && gridBgColor.Color.G <= 255)
                    {
                        ThisGrid.Background = new SolidColorBrush(Color.FromRgb(
                            (byte)(gridBgColor.Color.R - 10),
                            (byte)(gridBgColor.Color.G - 20),
                            (byte)(gridBgColor.Color.B - 10)));
                    }
                    else
                    {
                        numberUp = true;
                        NextNumber--;
                        Image_MouseDown(sender, e);
                    }
                }

                if (NextNumber == dictionary.NumberImages.Count && timer.IsEnabled)
                {
                    timer.Stop();
                    NextNumber = NextNumber - 1;
                    foreach (Image c in ThisGrid.Children) 
                    {
                        c.IsEnabled = false;
                    }
                    int minutes = (int)(time / 60);
                    int seconds = (int)(time % 60);
                    WpfMessageBox.Show($"{minutes} мин. {seconds} сек. на решение");
                }
            }
            else
                ThisGrid.Background = new SolidColorBrush(Color.FromRgb(200, 50, 50));
        }

        private void ImageSizeChange(Image newImage)
        {
            var imageDimentions = dictionary.NumberImages[Convert.ToInt32(newImage.Name.Replace("img", ""))];
            newImage.Width = imageDimentions.WidthToWidthRatio * (ThisGrid.ActualWidth - 20);
            newImage.Height = imageDimentions.HeightToHeightRatio * (ThisGrid.ActualHeight - 20);
            newImage.Margin = new Thickness(imageDimentions.XtoXRatio * (ThisGrid.ActualWidth - 20) + 10,
                              imageDimentions.YtoYRatio * (ThisGrid.ActualHeight - 20) + 10, 10, 10);
            newImage.RenderTransform = new TransformGroup();
            (newImage.RenderTransform as TransformGroup).Children.Add(new RotateTransform(imageDimentions.Angle));
            if (imageDimentions.Angle != 0)
                newImage.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void ParceVisualToTxt()
        {
            string text = "public List<NumberImageClass> NumberImages = new List<NumberImageClass>(){\n";
            TransformGroup nullAngle = new TransformGroup();
            nullAngle.Children.Add(new RotateTransform(0));

            foreach (Image image in MainViewModel.GetVisualChilds<Image>(ThisGrid as DependencyObject))
            {
                text += "new NumberImageClass(";
                foreach (var item in (image.RenderTransform as TransformGroup)?.Children ?? nullAngle.Children)
                {
                    if (item is RotateTransform)
                    {
                        text += (item as RotateTransform).Angle + ";";
                        break;
                    }
                }
                text += Math.Round(image.ActualWidth / ActualWidth, 6) + ";";
                text += Math.Round(image.ActualHeight / ActualHeight, 6) + ";";
                text += Math.Round(image.Margin.Left / ActualWidth, 6) + ";";
                text += Math.Round(image.Margin.Top / ActualHeight, 6) + ");\n";
            }
            text = text.Remove(text.Length - 2);
            text += "\n}";

            text = text.Replace(',', '.');
            text = text.Replace(';', ',');

            File.WriteAllText("E:\\User\\Downloads\\1.txt", text);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                foreach (Image image in MainViewModel.GetVisualChilds<Image>(this.Content as DependencyObject))
                {
                    ImageSizeChange(image);
                }
            }
            catch (Exception)
            {

            }

        }
        private void ThisGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dictionary.NumberImages.Count > 0)
                {
                    CreatePicture(dictionary.NumberImages.Count - 1, false);
                    for (int i = 0; i < dictionary.NumberImages.Count - 1; i++)
                    {
                        CreatePicture(i);
                    }
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.ToString(), WpfMessageBox.MessageBoxType.Error);
                MainViewModel.Back();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ThisGrid.Children.Count; i++)
            {
                (ThisGrid.Children[0] as Image).Source = null;
                (ThisGrid.Children[0] as Image).UpdateLayout();
                ThisGrid.Children.RemoveAt(0);
                GC.Collect();
            }
            MainViewModel.Back();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
