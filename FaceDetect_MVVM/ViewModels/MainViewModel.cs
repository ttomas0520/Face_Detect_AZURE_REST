using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;

using FaceDetect_MVVM.Core.Models;
using FaceDetect_MVVM.Services;
using FaceDetectHF.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Template10.Mvvm;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace FaceDetect_MVVM.ViewModels
{
    public class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private WinUI.TwoPaneView _twoPaneView;
        private bool _isGoBackButtonVisible;
        private ICommand _goBackCommand;
        private ICommand _itemClickCommand;
        private ICommand _modeChangedCommand;

        private WinUI.TwoPaneViewPriority _twoPanePriority;


        /// <summary>
        /// A dupla ablak prior panelének tulajdonsága
        /// </summary>
        public WinUI.TwoPaneViewPriority TwoPanePriority
        {
            get => _twoPanePriority;
            set
            {
                _ = SetProperty(ref _twoPanePriority, value);
                OnPropertyChanged("TwoPanePriority");
            }
        }
        /// <summary>
        /// A Details fülön lévő bal felső visszagomb láthatósági tulajdonsága
        /// </summary>
        public bool IsGoBackButtonVisible
        {
            get => _isGoBackButtonVisible;
            set
            {
                _ = SetProperty(ref _isGoBackButtonVisible, value);
                OnPropertyChanged("IsGoBackButtonVisible");

            }
        }

        /// <summary>
        /// A detektált arcok listájában lévő elemek kattíntására elsülő esemény
        /// </summary>
        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand(OnItemClick));
        /// <summary>
        /// Az ablak mód változásának detektálásakor elsülő esemény
        /// </summary>
        public ICommand ModeChangedCommand => _modeChangedCommand ?? (_modeChangedCommand = new RelayCommand<WinUI.TwoPaneView>(OnModeChanged));
        /// <summary>
        /// Visszalépés gomb megnyomásának detektálása
        /// </summary>
        public ICommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack));

        public MainViewModel()
        {
            OpenPictureCommand = new DelegateCommand(ChoosePicture);
            TakePictureCommand = new DelegateCommand(TakePicture);
            MainImage = new BitmapImage(new Uri("ms-appx:///Assets/500.png"));
            ErrorMessage = "We didn't detect human face";
        }

        public void Initialize(WinUI.TwoPaneView twoPaneView)
        {
            _twoPaneView = twoPaneView;
        }

        /// <summary>
        /// A robosztusság érdekében ellenőrizzük, hogy csak akkor zárjuk be a Details fület ha ő az aktív panel
        /// </summary>
        /// <returns></returns>
        public bool TryCloseDetail()
        {
            if (TwoPanePriority == WinUI.TwoPaneViewPriority.Pane2)
            {
                TwoPanePriority = WinUI.TwoPaneViewPriority.Pane1;
                RefreshIsGoBackButtonVisible();
                return true;
            }

            return false;
        }
        /// <summary>
        /// Az arcok listájának kattjntás eseménye mely során ha
        /// egy paneles módban vagyunk akkor a details fület jelenítjük meg a kattíntott arc adatival feltöltve
        /// </summary>
        private void OnItemClick()
        {
            if (_twoPaneView.Mode == WinUI.TwoPaneViewMode.SinglePane)
            {
                TwoPanePriority = WinUI.TwoPaneViewPriority.Pane2;
                RefreshIsGoBackButtonVisible();
            }
        }
        /// <summary>
        /// Megjelenítési mód váltásánma kelekezeléséért felelős függvény, mely fókuszba helyezi a megfelelő paneleket.
        /// </summary>
        /// <param name="twoPaneView">A UI felületen jelenlévő dupla panel (az esemény hívója) </param>
        private void OnModeChanged(WinUI.TwoPaneView twoPaneView)
        {
            TwoPanePriority = twoPaneView.Mode == WinUI.TwoPaneViewMode.SinglePane && ClickedFace != null
                ? WinUI.TwoPaneViewPriority.Pane2
                : WinUI.TwoPaneViewPriority.Pane1;
            RefreshIsGoBackButtonVisible();
        }

        private void OnGoBack()
        {
            _ = TryCloseDetail();
        }

        /// <summary>
        /// A visszagomb láthatóságának beállító függvénye
        /// </summary>
        private void RefreshIsGoBackButtonVisible()
        {
            IsGoBackButtonVisible = _twoPaneView.Mode == WinUI.TwoPaneViewMode.SinglePane && TwoPanePriority == WinUI.TwoPaneViewPriority.Pane2;
        }

        private ImageSource mainImage;
        public ImageSource MainImage
        {
            get => mainImage;
            set
            {
                if (mainImage != value)
                {
                    mainImage = value;

                    OnPropertyChanged("MainImage");
                }
            }
        }

        private DetectedFace clickedFace;
        public DetectedFace ClickedFace
        {
            get => clickedFace;
            set
            {
                if (clickedFace != value)
                {

                    clickedFace = value;
                    if (value != null)
                    {
                        clickedFace.brush = new SolidColorBrush(Colors.Green);
                        OneFace?.Invoke(clickedFace);
                        OnPropertyChanged("ClickedFace");
                    }

                }
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }

        public void SetClicked(DetectedFace clicked)
        {
            ClickedFace = clicked;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public ObservableCollection<DetectedFace> DetectedFaces { get; } = new ObservableCollection<DetectedFace>();




        public DelegateCommand OpenPictureCommand { get; }
        public DelegateCommand TakePictureCommand { get; }
        public delegate void GotFaces(List<DetectedFace> faces, int origWidth, int origHeight, int actWidth, int actHeight);
        public delegate void HighlightOneFace(DetectedFace face);
        public event GotFaces MyEvent;
        public event HighlightOneFace OneFace;

        /// <summary>
        /// A fénykép kiválasztó gomb eseménykezelő függvénye mely során a megfelelő service függvényt meghívjuk
        /// </summary>
        private async void ChoosePicture()
        {
            var imageService = new ImageProcessService();
            byte[] img = await imageService.PicturePicker();
            SendMethod(img);
        }
        /// <summary>
        /// A fénykép készítő gomb eseménykezelő függvénye mely során a megfelelő service függvényt meghívjuk
        /// </summary>
        private async void TakePicture()
        {

            var imageService = new ImageProcessService();
            byte[] img = await imageService.PictureTaker();
            SendMethod(img);
        }

        /// <summary>
        /// A fénykép feldolgozásáért és az API kérés elindításáért felelős függvény
        /// </summary>
        /// <param name="img">A kiválasztott kép byte tömbje</param>
        private async void SendMethod(byte[] img)
        {
            if (img != null)
            {
                DetectedFaces.Clear();
                ClickedFace = null;

                var faceService = new AzureFaceService();
                var notificationService = new ToastNotificationsService();
                var faces = await faceService.MakeRequest(img);

                if (faces.Count != 0)
                {
                    ErrorMessage = "We found " + faces.Count + " face, click for info";
                }
                else
                {
                    ErrorMessage = "We didn't detect human face";
                }

                BitmapImage originalImg = await ByteToBitmapImageAsync(img);
                BitmapImage resizedImgage = ResizedImage(originalImg, 500, 500);
                foreach (var face in faces)
                {
                    DetectedFaces.Add(face);
                }
                //UI értesítése a négyzetek kirajzolására
                MyEvent?.Invoke(faces, originalImg.PixelWidth, originalImg.PixelHeight, resizedImgage.DecodePixelWidth, resizedImgage.DecodePixelHeight);
                //Felugró értesítés kiküldése
                notificationService.ShowToastNotification(errorMessage);
                MainImage = resizedImgage;
            }
        }

        /// <summary>
        /// A függvény a kiválaszott kép byte tömbjének Xaml kopatibilissá konvertálásáért felelős
        /// </summary>
        /// <param name="img">A kiválasztott kép byte tömbje</param>
        /// <returns>Az elkészített BitmapImage mely Xaml kompatibilis</returns>
        private async Task<BitmapImage> ByteToBitmapImageAsync(byte[] img)
        {
            BitmapImage bmp = new BitmapImage();


            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(img.AsBuffer());
                stream.Seek(0);
                await bmp.SetSourceAsync(stream);
            }
            return bmp;
        }


        /// <summary>
        /// A kiválaszott kép skálázását végző függvény
        /// </summary>
        /// <param name="image">A Xaml kompatibilis fénykép</param>
        /// <param name="maxWidth">A cél szélesség</param>
        /// <param name="maxHeight">A cél magasság</param>
        /// <returns></returns>
        public BitmapImage ResizedImage(BitmapImage image, int maxWidth, int maxHeight)
        {
            BitmapImage sourceImage = image;
            var origHeight = sourceImage.PixelHeight;
            var origWidth = sourceImage.PixelWidth;
            var ratioX = maxWidth / (float)origWidth;
            var ratioY = maxHeight / (float)origHeight;
            var ratio = Math.Min(ratioX, ratioY);
            var newHeight = (int)(origHeight * ratio);
            var newWidth = (int)(origWidth * ratio);

            if (origHeight > newHeight && origWidth > newWidth)
            {
                sourceImage.DecodePixelWidth = newWidth;
                sourceImage.DecodePixelHeight = newHeight;
            }

            return sourceImage;
        }


    }
}
