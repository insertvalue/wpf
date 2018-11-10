﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Configuration;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.ComponentModel;

namespace WpfJsd
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 轮询间隔
        private int interval = Convert.ToInt16(ConfigurationManager.AppSettings["PollInterval"]) * 60 * 1000;


        public MainWindow()
        {

            PollNotify();
            InitializeComponent();
            IsRepeat.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsRepeat"]);
            IsNew.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsNew"]);
            IsDelay.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDelay"]);
            ToolIcon();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("确定是否关闭当前应用程序？", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.ShowBalloonTip(1000);
            }

        }

        public void PollNotify()
        {
            Thread thread = new Thread(start: () =>
            {
                while (true)
                {
                    string result = HttpGet("http://www.baidu.com", "");
                    Console.WriteLine(result);
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.SpeakAsync("您有新的拣货单，请及时处理！");
                    Thread.Sleep(interval);
                }
            })
            {
                // 设置为true，关闭窗口后才会自动关闭线程
                IsBackground = true
            };
            thread.Start();
        }

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

        private void ConfigCheckBox(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            bool? isChecked = checkBox.IsChecked;
            cfa.AppSettings.Settings[checkBox.Name].Value = isChecked.ToString();
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        NotifyIcon notifyIcon = null;

        private void ToolIcon()
        {
            string path = Path.GetFullPath(@"Resources\favicon.ico");
            this.notifyIcon = new NotifyIcon
            {
                BalloonTipText = "京小秘正在运行", //设置程序启动时显示的文本
                Text = "京小秘服务",//最小化到托盘时，鼠标点击时显示的文本
                Icon = new System.Drawing.Icon(path),//程序图标
                Visible = true
            };
            //右键菜单--打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("打开");
            open.Click += new EventHandler(ShowWindow);
            //右键菜单--退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
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

        private void ShowWindow(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}