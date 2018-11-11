using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Configuration;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;

namespace WpfJsd
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // 轮询间隔
        private int INTERVAL = Convert.ToInt16(ConfigurationManager.AppSettings["PollInterval"]) * 60 * 1000;
        // 新拣货任务查询接口
        private string URL_NEW_TASK = "http://out.jd.id:12345/f/out/pick/notify/hasNewTask";
        // 延迟拣货任务查询接口
        private string URL_DELAY_TASK = "http://out.jd.id:12345/f/out/pick/notify/hasDelayTask";
        // 仓库列表
        private List<Warehouse> warehouseList = new List<Warehouse>();
        public MainWindow()
        {
            InitializeComponent();
            // 初始化国际化
            InitLocale();
            // 初始化线程，扫描拣货单
            PollNotify();
            // 初始化UI配置控件
            InitConf();
            // 初始化通知栏图标
            ToolIcon();
        }

        /// <summary>
        /// 初始化UI配置控件
        /// </summary>
        private void InitConf()
        {
            IsRepeat.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsRepeat"]);
            IsNew.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsNew"]);
            IsDelay.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDelay"]);
            warehouseList.Add(new Warehouse { Name = "--请选择门店--", Value = "" });
            warehouseList.Add(new Warehouse { Name = "极速达仓库一", Value = "10005" });
            warehouseList.Add(new Warehouse { Name = "极速达仓库二", Value = "10006" });
            warehouseList.Add(new Warehouse { Name = "极速达仓库三", Value = "10007" });
            warehouseComboBox.ItemsSource = warehouseList;
            warehouseComboBox.DisplayMemberPath = "Name";//显示出来的值
            warehouseComboBox.SelectedValuePath = "Value";//实际选中后获取的结果的值
            warehouseComboBox.SelectedIndex = 0;
        }


        /// <summary>
        /// 初始化国际化
        /// </summary>
        private void InitLocale()
        {
            LocUtil.SetDefaultLanguage(this);

            foreach (System.Windows.Controls.MenuItem item in menuItemLanguages.Items)
            {
                if (item.Tag.ToString().Equals(LocUtil.GetCurrentCultureName(this)))
                {
                    item.IsChecked = true;
                }
            }

        }

        /// <summary>
        /// 重写OcClosing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            ShowTipDialogOnClose();
        }

        /// <summary>
        /// 关闭窗口时提示窗口
        /// </summary>
        private async void ShowTipDialogOnClose()
        {
            MetroDialogSettings settings = new MetroDialogSettings
            {
                NegativeButtonText = LocUtil.GetString("No"),
                AffirmativeButtonText = LocUtil.GetString("Yes")
            };
            MessageDialogResult clickresult = await this.ShowMessageAsync(LocUtil.GetString("MsgLvlNotice"), LocUtil.GetString("MsgExit"), MessageDialogStyle.AffirmativeAndNegative, settings);
            if (clickresult == MessageDialogResult.Negative)
            {
                Hide();
                notifyIcon.ShowBalloonTip(1000);
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 后台线程，轮询获取拣货单
        /// </summary>
        public void PollNotify()
        {
            Thread thread = new Thread(start: () =>
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                MetroDialogSettings settings = new MetroDialogSettings
                {
                    AnimateHide = true
                };
                while (true)
                {
                    string result = HttpGet(URL_NEW_TASK, "whId=10005");
                    Console.WriteLine(result);
                    if (Convert.ToBoolean(result))
                    {
                        synth.SpeakAsync(LocUtil.GetString("TipNewMsg"));
                    }
                    Thread.Sleep(3000);
                }
            })
            {
                // 设置为true，关闭窗口后才会自动关闭线程
                IsBackground = true
            };
            thread.Start();
        }

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// CheckBox事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigCheckBox(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            bool? isChecked = checkBox.IsChecked;
            cfa.AppSettings.Settings[checkBox.Name].Value = isChecked.ToString();
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchLang(Object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.MenuItem item in menuItemLanguages.Items)
            {
                item.IsChecked = false;
            }
            System.Windows.Controls.MenuItem mi = sender as System.Windows.Controls.MenuItem;
            mi.IsChecked = true;
            LocUtil.SwitchLanguage(this, mi.Tag.ToString());
            ToolIcon();
        }

        /// <summary>
        /// ToggleSwitch开关事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSwitch(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            bool? isChecked = toggleSwitch.IsChecked;
            cfa.AppSettings.Settings[toggleSwitch.Name].Value = isChecked.ToString();
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 切换门店事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectWarehouse(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
            Console.WriteLine(comboBox.SelectedValue);

        }

        NotifyIcon notifyIcon = null;

        /// <summary>
        /// 最小化到通知栏
        /// </summary>
        private void ToolIcon()
        {
            string path = Path.GetFullPath(@"Resources\favicon.ico");
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Dispose();
                this.notifyIcon = null;
            }
            this.notifyIcon = new NotifyIcon
            {
                BalloonTipText = LocUtil.GetString("MsgBubble"), //设置程序启动时显示的文本
                Text = LocUtil.GetString("ServiceName"),//最小化到托盘时，鼠标点击时显示的文本
                Icon = new System.Drawing.Icon(path),//程序图标
                Visible = true
            };
            //右键菜单--打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem(LocUtil.GetString("Open"));
            open.Click += new EventHandler(ShowWindow);
            //右键菜单--退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem(LocUtil.GetString("Exit"));
            exit.Click += new EventHandler(CloseWindow);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            //this.notifyIcon.ShowBalloonTip(1000);
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            /*
             * 这一段代码需要解释一下:
             * 窗口正常时双击图标执行这段代码是这样一个过程：
             * this.Show()-->WindowState由Normail变为Minimized-->Window_StateChanged事件执行(this.Hide())-->WindowState由Minimized变为Normal-->窗口隐藏
             * 窗口隐藏时双击图标执行这段代码是这样一个过程：
             * this.Show()-->WindowState由Normail变为Minimized-->WindowState由Minimized变为Normal-->窗口显示
             */
            this.Show();
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            //窗口最小化时隐藏任务栏图标
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindow(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideWindow(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }

    public class Warehouse
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
