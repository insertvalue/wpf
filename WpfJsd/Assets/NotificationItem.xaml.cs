using System.Windows;
using System.Windows.Controls;

namespace WpfJsd.Assets
{
    /// <summary>
    /// NotificationItem.xaml 的交互逻辑
    /// </summary>
    public partial class NotificationItem : UserControl
    {
        public NotificationItem()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            this.Visibility = Visibility.Hidden;
            parentWindow.Close();
        }
    }
}
