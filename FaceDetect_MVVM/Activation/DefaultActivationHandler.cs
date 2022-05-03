using System;
using System.Threading.Tasks;

using FaceDetect_MVVM.Core.Helpers;
using FaceDetect_MVVM.Services;

using Windows.ApplicationModel.Activation;

namespace FaceDetect_MVVM.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<IActivatedEventArgs>
    {
        private readonly Type _navElement;

        public DefaultActivationHandler(Type navElement)
        {
            _navElement = navElement;
        }

        protected override async Task HandleInternalAsync(IActivatedEventArgs args)
        {
            object arguments = null;
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }

            NavigationService.Navigate(_navElement, arguments);

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(IActivatedEventArgs args)
        {
            return NavigationService.Frame.Content == null && _navElement != null;
        }
    }
}
