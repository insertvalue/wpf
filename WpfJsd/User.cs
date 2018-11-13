using System.ComponentModel;

namespace WpfJsd
{
    /// <summary>
    /// <para>用户类</para>
    /// <para>partial修饰符可将一个类的定义分散在多个文件中参见<see cref="UserMetadata"/></para>
    /// <para>在此处将定义与绑定校验分离</para>
    /// </summary>
    public partial class User : INotifyPropertyChanged
    {
        /// <summary>
        /// 用户名
        /// </summary>
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged("Username");
                }
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
