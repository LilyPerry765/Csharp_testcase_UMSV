using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;

namespace Pendar.Ums.Model
{
    public static class Extensions
    {
        public static T FindParent<T>(this FrameworkElement obj) where T : FrameworkElement
        {
            FrameworkElement currentObject = obj;
            while (currentObject != null)
            {
                if (currentObject is T)
                    return currentObject as T;
                currentObject = currentObject.Parent as FrameworkElement;
            }
            return null;
        }

        public static bool IsNumber(this string str)
        {
            return Regex.IsMatch(str ?? "", @"\d+");
        }

        public static bool SetPropertyValue(this object obj, string prop, object value)
        {
            var property = obj.GetType().GetProperty(prop);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }
            return false;
        }

        public static T GetPropertyValue<T>(this object obj, string prop)
        {
            var property = obj.GetType().GetProperty(prop);
            if (property != null)
            {
                return (T)property.GetValue(obj, null);
            }
            return default(T);
        }

        public static T Clone<T>(this T source)
        {
            if (source == null)
                return default(T);
            Type type = source.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return source;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = source as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Clone(array.GetValue(i)), i);
                }
                return (T)Convert.ChangeType(copied, source.GetType());
            }
            else
            {
                object newObj = Activator.CreateInstance(source.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(source);
                    if (fieldValue != null)
                        field.SetValue(newObj, Clone(fieldValue));
                }
                return (T)newObj;
            }
        }

        public static void CopyTo<T>(this T source, T destObject) where T : class
        {
            Type type = source.GetType();
            if (type.IsArray)
            {
                Type elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = source as Array;

                for (int i = 0; i < array.Length; i++)
                {
                    ((Array)(object)destObject).SetValue(Clone(array.GetValue(i)), i);
                }
            }
            else if (type.IsClass)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(source);
                    if (fieldValue != null)
                        field.SetValue(destObject, Clone(fieldValue));
                }
            }
        }


        /// <summary>
        /// If you give it "#i" or "i", it returns i; where i is any integer.
        /// </summary>
        public static int? ToInt(this string id)
        {
            if (id == null) return null;
            int result;
            string extractedNumber = Regex.Match(id, @"^#?(?<ID>\d+)$").Groups["ID"].Value;
            if (int.TryParse(extractedNumber, out result))
                return result;
            else
                return null;
        }
    }

}
