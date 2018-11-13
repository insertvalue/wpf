using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WpfJsd
{
    /// <summary>
    /// <para>校验扩展类</para>
    /// <para>扩展Object类，任务对象都可通过this进行调用其中的static方法</para>
    /// </summary>
    public static class ValidationExtension
    {
        /// <summary>
        /// 表当验证错误集合
        /// </summary>
        public static Dictionary<String, String> dataErrors = new Dictionary<String, String>();

        /// <summary>
        /// 是否验证通过
        /// </summary>
        public static bool IsValid(this IDataErrorInfo obj)
        {
            if (dataErrors != null && dataErrors.Count > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 手动校验
        /// </summary>
        /// <typeparam name="metadatatype"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string Validate<metadatatype>(this IDataErrorInfo obj, string propertyName)
        {
            return obj.ValidateProperty<metadatatype>(propertyName);
        }

        /// <summary>
        /// <para>为System.Object类派生新的方法</para>
        /// </summary>
        /// <typeparam name="metadatatype"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string ValidateProperty<metadatatype>(this object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }

            var targetType = obj.GetType();
            if (targetType != typeof(metadatatype))
            {
                // 将User对象属性关联到UserMetadata对象
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(targetType, typeof(metadatatype)), targetType);
            }

            var propertyValue = targetType.GetProperty(propertyName).GetValue(obj, null);
            var validationContext = new ValidationContext(obj, null, null)
            {
                MemberName = propertyName
            };
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

            if (validationResults.Count > 0)
            {
                AddDic(dataErrors, propertyName);
                return validationResults.First().ErrorMessage;
            }
            RemoveDic(dataErrors, propertyName);
            return string.Empty;
        }

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private static void RemoveDic(Dictionary<String, String> dics, String dicKey)
        {
            dics.Remove(dicKey);
        }

        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private static void AddDic(Dictionary<String, String> dics, String dicKey)
        {
            if (!dics.ContainsKey(dicKey)) dics.Add(dicKey, "");
        }
    }
}
