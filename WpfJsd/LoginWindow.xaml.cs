using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfJsd
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : MetroWindow, ILocale
    {

        public LoginWindow()
        {
            InitializeComponent();
            InitLocale();
            InitBinding();
        }

        /// <summary>
        /// 初始化用户登录输入框绑定
        /// </summary>
        private void InitBinding()
        {

            User user = new User();
            loginGrid.DataContext = user;
        }

        public void InitLocale()
        {
            LocaleUtil.InitLocale(this, menuItemLanguages);
        }

        /// <summary>
        /// 语言切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SwitchLang(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                item.IsChecked = false;
            }
            System.Windows.Controls.MenuItem mi = sender as System.Windows.Controls.MenuItem;
            mi.IsChecked = true;
            LocaleUtil.SwitchLanguage(this, mi.Tag.ToString());
        }

        /// <summary>
        /// 登录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLogin(object sender, RoutedEventArgs e)
        {
            if (IsValidate())
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow windows = new MainWindow();
                    this.Hide();
                    windows.Show();
                }));
            }            
        }

        /// <summary>
        /// 触发校验
        /// </summary>
        /// <returns></returns>
        private bool IsValidate()
        {
            User user = loginGrid.DataContext as User;
            if (!user.Validation) {
                user.Validation = true;
            }
            
            if (string.Empty.Equals(Username.Text)) {
                Username.Text = string.Empty;
                Username.Focus();
                return false;
            }
            if (string.Empty.Equals(Password.Text)) {
                Password.Text = string.Empty;
                Password.Focus(); return false;
            }
            return true;
        }
    }
}
