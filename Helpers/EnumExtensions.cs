using System;
using System.ComponentModel;
using System.Reflection;

namespace exam.Helpers
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                DescriptionAttribute attribute = 
                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                return attribute?.Description ?? value.ToString();
            }

            return value.ToString();
        }
    }
}
