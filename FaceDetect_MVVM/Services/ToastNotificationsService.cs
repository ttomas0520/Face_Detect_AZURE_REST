using System;
using System.Threading.Tasks;

using FaceDetect_MVVM.Activation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;

namespace FaceDetect_MVVM.Services
{
    internal partial class ToastNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        /// <summary>
        /// A felugró értesítés robosztus megjelenítésért felelős függvény
        /// </summary>
        /// <param name="toastNotification">A megjelenítendő értesítés</param>
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            try
            {
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// A felugró értesítés megjelenítésért felelős függvény
        /// </summary>
        /// <param name="message">A megjeleníteni kívánt üzenet</param>
        public void ShowToastNotification(string message)
        {
            var content = new ToastContent()
            {
                Launch = "ToastContentActivationParams",
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Detection completed!"
                            },

                            new AdaptiveText()
                            {
                                 Text = message
                            }
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("OK", "ToastButtonActivationArguments")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        new ToastButtonDismiss("Cancel")
                    }
                }
            };
            var toast = new ToastNotification(content.GetXml())
            {
                Tag = "ToastTag"
            };

            ShowToastNotification(toast);
        }
    }
}
