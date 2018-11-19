using Microsoft.Win32;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfJsd.Common
{
    public static class LocaleUtil
    {
        private static ResourceDictionary rd;

        /// <summary>
        /// 初始化国际化
        /// </summary>
        /// <param name="element"></param>
        /// <param name="menuItem"></param>
        public static void InitLocale(FrameworkElement element, MenuItem menuItem)
        {
            LocaleUtil.SetDefaultLanguage(element);

            foreach (MenuItem item in menuItem.Items)
            {
                if (item.Tag.ToString().Equals(GetCurrentCultureName()))
                {
                    item.IsChecked = true;
                }
            }
        }

        /// <summary>  
        /// 获取应用名称
        /// </summary>  
        /// <param name="element"></param>  
        /// <returns></returns>  
        private static string GetAppName(FrameworkElement element)
        {
            var elType = element.GetType().ToString();
            var elNames = elType.Split('.');
            return elNames[0];
        }

        /// <summary>  
        /// 获取当前语言
        /// </summary>  
        /// <returns></returns>  
        public static string GetCurrentCultureName()
        {
            return ConfigurationManager.AppSettings["Lang"];
        }

        /// <summary>  
        /// 设置语言 
        /// </summary>  
        /// <param name="element"></param>  
        public static void SetDefaultLanguage(FrameworkElement element)
        {
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(GetCurrentCultureName()));
        }

        /// <summary>  
        /// 从语言文件加载到字典
        /// </summary>  
        public static void SwitchLanguage(FrameworkElement element, string lang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(lang));
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["Lang"].Value = lang;
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 获取词条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetString(string id)
        {
            if (rd.Contains(id))
            {
                return rd[id] as string;
            }
            else
            {
                return id;
            }
        }

        /// <summary>  
        /// 获取国际化文件
        /// </summary>  
        /// <param name="lang"></param>  
        /// <returns></returns>  
        public static string GetLocXAMLFilePath(string lang)
        {
            string locXamlFile = lang + ".xaml";
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, "i18N", locXamlFile);
        }

        /// <summary>  
        /// 设置国际化文件字典
        /// </summary>  
        /// <param name="inFile"></param>  
        private static void SetLanguageResourceDictionary(FrameworkElement element, String inFile)
        {
            if (File.Exists(inFile))
            {
                // 从文件读取到字典
                var languageDictionary = new ResourceDictionary
                {
                    Source = new Uri(inFile)
                };
                // 移除已加载的字典
                int langDictId = -1;
                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var md = element.Resources.MergedDictionaries[i];
                    // ResourceDictionaryName以"Loc-"开头 
                    if (md.Contains("ResourceDictionaryName") && md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                    {
                        langDictId = i;
                        break;
                    }
                }
                if (langDictId == -1)
                {
                    // 加入字典集合
                    element.Resources.MergedDictionaries.Add(languageDictionary);
                }
                else
                {
                    // 重置语言字典
                    element.Resources.MergedDictionaries[langDictId] = languageDictionary;
                    rd = languageDictionary;
                }
            }
            else
            {
                MessageBox.Show("'" + inFile + "' not found.");
            }
        }
    }
}