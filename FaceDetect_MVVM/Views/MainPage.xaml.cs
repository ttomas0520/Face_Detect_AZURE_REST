using FaceDetect_MVVM.Core.Models;
using FaceDetect_MVVM.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace FaceDetect_MVVM.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();
        private double ratioX;
        private double ratioY;
        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(twoPaneView);
            var vm = (MainViewModel)DataContext;
            vm.MyEvent += (List<DetectedFace> faces, int origWidth, int origHeight, int actWidth, int actHeight) =>
            {
                if (actHeight == 0 || actWidth == 0)
                {
                    actHeight = origHeight;
                    actWidth = origWidth;
                }

                var recs = canvas.Children.OfType<Rectangle>().ToList();
                foreach (var rec in recs)
                {
                    canvas.Children.Remove(rec);
                }
                ratioX = actWidth / (float)origWidth;
                ratioY = actHeight / (float)origHeight;
                foreach (var face in faces)
                {
                    var dimensions = face.faceRectangle;
                    var rec = new Rectangle()
                    {
                        Width = dimensions.width * ratioX,
                        Height = dimensions.height * ratioY,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 3,
                        Name = face.faceId
                    };
                    Canvas.SetLeft(rec, dimensions.left * ratioX);
                    Canvas.SetTop(rec, dimensions.top * ratioY);
                    this.canvas.Children.Add(rec);
                }

            };
            vm.OneFace += (DetectedFace face) =>
            {
                var rec = new Rectangle()
                {
                    Width = face.faceRectangle.width * ratioX,
                    Height = face.faceRectangle.height * ratioY,
                    Stroke = face.brush,
                    StrokeThickness = 3
                };
                Canvas.SetLeft(rec, face.faceRectangle.left * ratioX);
                Canvas.SetTop(rec, face.faceRectangle.top * ratioY);


                foreach (var child in canvas.Children)
                {
                    var existRec = child as Rectangle;

                    if (existRec != null && (((SolidColorBrush)existRec.Stroke).Color == ((SolidColorBrush)face.brush).Color))
                    {
                        this.canvas.Children.Remove(existRec);
                    }
                }

                this.canvas.Children.Add(rec);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

    }
}
