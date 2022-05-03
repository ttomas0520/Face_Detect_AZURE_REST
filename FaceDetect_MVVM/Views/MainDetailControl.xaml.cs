using FaceDetect_MVVM.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FaceDetect_MVVM.Views
{
    public sealed partial class MainDetailControl : UserControl
    {
        public DetectedFace SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as DetectedFace; }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(DetectedFace), typeof(MainDetailControl), new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        public MainDetailControl()
        {
            InitializeComponent();
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MainDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
