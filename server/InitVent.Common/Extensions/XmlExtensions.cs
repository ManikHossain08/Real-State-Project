using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InitVent.Common.Extensions
{
    public static class XmlExtensions
    {
        public static String ValueOrDefault(this XmlAttribute attribute, String defaultValue = null)
        {
            return attribute == null ? defaultValue : attribute.Value;
        }
    }
}
