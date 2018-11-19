using System.ComponentModel.DataAnnotations;
using WpfJsd.Common;

namespace WpfJsd.Core
{
    class MyRequireAttribute : RequiredAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            string labelKey = LocaleUtil.GetString("Label" + name);
            string msgKey = LocaleUtil.GetString("RequireMsg");
            return labelKey + msgKey;
        }
    }
}
