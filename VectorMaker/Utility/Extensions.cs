using System;
using System.Reflection;

namespace VectorMaker.Utility
{
    /// <summary>
    /// Defines Extension for enum.
    /// </summary>
    public static class Extensions
    {
        public static string GetStringValueForEnum(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            EnumStringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(EnumStringValueAttribute), false) as EnumStringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }
    }
    public class EnumStringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }
        public EnumStringValueAttribute(string value)
        {
            this.StringValue = value;
        }

    }
}
