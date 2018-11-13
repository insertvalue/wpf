using System.ComponentModel.DataAnnotations;

namespace WpfJsd
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
