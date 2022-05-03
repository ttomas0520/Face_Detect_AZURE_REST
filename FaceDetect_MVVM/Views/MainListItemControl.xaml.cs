using System;

using FaceDetect_MVVM.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FaceDetect_MVVM.Views
{
    public sealed partial class MainListItemControl : UserControl
    {
        public DetectedFace Item
        {
            get { return (DetectedFace)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(DetectedFace), typeof(MainListItemControl), new PropertyMetadata(null, OnItemPropertyChanged));

        public MainListItemControl()
        {
            InitializeComponent();
        }

        private static void OnItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
