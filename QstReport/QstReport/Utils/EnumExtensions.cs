using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QstReport.Utils
{
    public static class EnumExtensions
    {
        public static string GetDisplayName<T>(this T value)
        {
            var enumMember = typeof(T).GetMember(value.ToString());

            var displayAttribute = (DisplayAttribute)enumMember[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            return displayAttribute != null ? displayAttribute.Name : value.ToString();
        }
    }
}
