using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для PerseveranceTest.xaml
    /// </summary>
    public partial class PerseveranceTest : Page, INotifyPropertyChanged
    {
        PreseveranceTestDictionary dictionary { get; set; }
        System.Windows.Threading.DispatcherTimer timer { get; set; }
        double time = 0;

        private int previousNumber = 0;
        bool numberUp = true;

        public int PreviousNumber
        {
            get => previousNumber+1;
            set
            {
                previousNumber = value-1;
                OnPropertyChanged("PreviousNumber");
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
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            time+=0.01;
        }


        private void CreatePicture(int imageNumber, bool createMouseDown = true)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(dictionary.NumberSources[imageNumber]);
            bitmap.EndInit();

            Image newImage = new Image();
            //newImage.Source = image;
            newImage.Source = bitmap;
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
            if (previousNumber == 0)
                timer.Start();
            if (Convert.ToInt32((sender as Image).Name.Replace("img", "")) == previousNumber)
            {
                PreviousNumber++;

                if ((ThisGrid.Background as SolidColorBrush).Color.G < (ThisGrid.Background as SolidColorBrush).Color.R)
                    (ThisGrid.Background as SolidColorBrush).Color = Color.FromRgb(30, 55, 30);

                if (numberUp)
                {
                    if ((ThisGrid.Background as SolidColorBrush).Color.G >= 55 && (ThisGrid.Background as SolidColorBrush).Color.G <= 235)
                    {
                        ThisGrid.Background = new SolidColorBrush(Color.FromRgb(
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.R + 10),
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.G + 20),
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.B + 10)));
                    }
                    else
                    {
                        numberUp = false;
                        PreviousNumber--;
                        Image_MouseDown(sender, e);
                    }

                }
                else 
                {
                    if ((ThisGrid.Background as SolidColorBrush).Color.G >= 75 && (ThisGrid.Background as SolidColorBrush).Color.G <= 255)
                    {
                        ThisGrid.Background = new SolidColorBrush(Color.FromRgb(
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.R - 10),
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.G - 20),
                            (byte)((ThisGrid.Background as SolidColorBrush).Color.B - 10)));
                    }
                    else
                    {
                        numberUp = true;
                        PreviousNumber--;
                        Image_MouseDown(sender, e);
                    }
                }

                if (PreviousNumber == dictionary.NumberImages.Count)
                {
                    timer.Stop();
                    WpfMessageBox.Show(Math.Round(time, 2).ToString()+" секунд на решение");

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
                text += Math.Round(image.ActualWidth / (double)800, 6) + ";";
                text += Math.Round(image.ActualHeight / (double)450, 6) + ";";
                text += Math.Round(image.Margin.Left / (double)800, 6) + ";";
                text += Math.Round(image.Margin.Top / (double)450, 6) + ");\n";
            }
            text = text.Remove(text.Length - 2);
            text += "\n}";

            text = text.Replace(',', '.');
            text = text.Replace(';', ',');

            File.WriteAllText("E:\\User\\Downloads\\sdf.txt", text);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ParceVisualToTxt();
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
            if (dictionary.NumberImages.Count > 0)
            {
                CreatePicture(dictionary.NumberImages.Count - 1, false);
                for (int i = 0; i < dictionary.NumberImages.Count - 1; i++)
                {
                    CreatePicture(i);
                }
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
