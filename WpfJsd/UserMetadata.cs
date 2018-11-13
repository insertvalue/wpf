using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace WpfJsd
{
    /// <summary>
    /// <para>用户类</para>
    /// <para>定义绑定校验</para>
    /// </summary>
    public partial class User : IDataErrorInfo
    {

        /// <summary>
        /// 与User类属性一致，用于校验，避免污染User对象
        /// </summary>
        class UserMetadata
        {
            [MyRequireAttribute]
            public string Username { get; set; }

            [MyRequireAttribute]
            public string Password { get; set; }
        }

        /// <summary>
        /// 是否启用校验
        /// </summary>
        public bool Validation { get; set; }

        public string this[string columnName]
        {
            get
            {
                return Validation ? this.ValidateProperty<UserMetadata>(columnName) : null;
            }
        }

        public string Error => null;
    }
}
