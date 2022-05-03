using Windows.UI.Xaml;

namespace FaceDetect_MVVM.StateTriggers
{
    public class ControlSizeTrigger : StateTriggerBase
    {
        private double _currentWidth;
        private FrameworkElement _targetElement;

        public double MinWidth { get; set; } = -1;

        public FrameworkElement TargetElement
        {
            get => _targetElement;
            set
            {
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged -= OnTargetElementSizeChanged;
                }

                _targetElement = value;
                _targetElement.SizeChanged += OnTargetElementSizeChanged;
            }
        }

        public ControlSizeTrigger()
        {
        }

        /// <summary>
        /// A felület méretváltozását lekezelő függvény
        /// </summary>
        /// <param name="sender">A hívó felület</param>
        /// <param name="e">A megváltozott méret</param>
        private void OnTargetElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _currentWidth = e.NewSize.Width;
            UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            if (_targetElement == null || MinWidth == 0)
            {
                SetActive(false);
            }
            else
            {
                SetActive(_currentWidth >= MinWidth);
            }
        }
    }
}
