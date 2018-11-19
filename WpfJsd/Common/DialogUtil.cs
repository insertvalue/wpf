using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfJsd.Core;
using WpfJsd.Model;
using WPFNotification.Core.Configuration;
using WPFNotification.Services;

namespace WpfJsd.Common
{
    public static class DialogUtil
    {
        private static readonly INotificationDialogService dailogService = new NotificationDialogService();
        private static readonly NotificationConfiguration configuration = new NotificationConfiguration(TimeSpan.Zero, 200, 100, Constants.MyNotificationTemplateName, NotificationFlowDirection.RightBottom);

        public static void ShowDialog(this FrameworkElement element, string key) {
            element.Dispatcher.Invoke(() =>
            {
                var newNotification = new MyNotification()
                {
                    Title = LocaleUtil.GetString("Tips"),
                    Content = LocaleUtil.GetString(key)
                };

                dailogService.ShowNotificationWindow(newNotification, configuration);
            });
        }
    }
}
