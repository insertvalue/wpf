using EasyHttp.Http;
using MahApps.Metro.Controls;
using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using WpfJsd.Common;
using WpfJsd.Core;
using WpfJsd.Model;
using WPFNotification.Core.Configuration;
using WPFNotification.Services;

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

        /// <summary>
        /// 初始化国际化
        /// </summary>
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
                // 跳转到主界面
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
            if (!user.Validation)
            {
                // 开启校验器
                user.Validation = true;
            }
            // 用户名必填
            if (string.Empty.Equals(Username.Text))
            {
                Username.Text = string.Empty;
                Username.Focus();
                return false;
            }
            // 密码必填
            if (string.Empty.Equals(Password.Text))
            {
                Password.Text = string.Empty;
                Password.Focus();
                return false;
            }
            // 远程校验
            var resp = HttpUtil.Login(Username.Text, Password.Text);

            if (resp.login)
            {
                HttpUtil.FetchWhList();
                return true;
            }
            else {
                loginGrid.ShowDialog(resp.msg as string);
                return false;
            }
        }
    }
}
