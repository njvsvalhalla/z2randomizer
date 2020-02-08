using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RandomizerApp.Helpers
{
    //https://automationrhapsody.com/efficiently-use-of-enumerations-with-string-values-in-c/
    public class StringValue : Attribute
    {
        public string Value { get; private set; }

        public StringValue(string value)
        {
            Value = value;
        }
    }

    public static class ExtensionMethods
    {
        public static string GetStringValue(this Enum value)
        {
            var stringValue = value.ToString();
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attrs = fieldInfo.
                GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs.Length > 0)
            {
                stringValue = attrs[0].Value;
            }
            return stringValue;
        }
    }
}
