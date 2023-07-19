using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace EvoS.Framework.Misc
{
    public static class CompilerExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrEmpty<T>(this T[] t)
        {
            return t == null || t.Length == 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> t)
        {
            return t == null || !t.Any<T>();
        }

        public static bool EqualsIgnoreCase(this string lhs, string rhs)
        {
	        return string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) == 0;
        }

		public static string GetAttribute(this XmlNode value, string attributeName)
		{
			if (value == null)
			{
				return null;
			}
			XmlAttribute xmlAttribute = value.Attributes[attributeName];
			if (xmlAttribute == null)
			{
				throw new Exception($"Could not find XML attribute '{xmlAttribute}'");
			}
			return xmlAttribute.Value;
		}

		public static string GetAttribute(this XmlNode value, string attributeName, string defaultValue)
		{
			if (value == null)
			{
				return null;
			}
			XmlAttribute xmlAttribute = value.Attributes[attributeName];
			if (xmlAttribute == null)
			{
				return defaultValue;
			}
			return xmlAttribute.Value;
		}

		public static string GetChildNodeAsString(this XmlNode value, string childNodeName)
		{
			if (value == null)
			{
				return null;
			}
			XmlNode xmlNode = value.SelectSingleNode(childNodeName);
			if (xmlNode == null)
			{
				throw new Exception($"Could not find child XML node '{childNodeName}'");
			}
			return xmlNode.InnerText;
		}

		public static string GetChildNodeAsString(this XmlNode value, string childNodeName, string defaultValue)
		{
			if (value == null)
			{
				return null;
			}
			XmlNode xmlNode = value.SelectSingleNode(childNodeName);
			if (xmlNode == null)
			{
				return defaultValue;
			}
			return xmlNode.InnerText;
		}

		public static int GetChildNodeAsInt32(this XmlNode value, string childNodeName, int? defaultValue = null)
		{
			string childNodeAsString = value.GetChildNodeAsString(childNodeName, null);
			if (childNodeAsString != null)
			{
				return Convert.ToInt32(childNodeAsString);
			}
			if (defaultValue != null)
			{
				return defaultValue.Value;
			}
			throw new Exception($"Could not find child XML node '{childNodeName}'");
		}

		public static long GetChildNodeAsInt64(this XmlNode value, string childNodeName, long? defaultValue = null)
		{
			string childNodeAsString = value.GetChildNodeAsString(childNodeName, null);
			if (childNodeAsString != null)
			{
				return Convert.ToInt64(childNodeAsString);
			}
			if (defaultValue != null)
			{
				return defaultValue.Value;
			}
			throw new Exception($"Could not find child XML node '{childNodeName}'");
		}

		public static ulong GetChildNodeAsUInt64(this XmlNode value, string childNodeName, ulong? defaultValue = null)
		{
			string childNodeAsString = value.GetChildNodeAsString(childNodeName, null);
			if (childNodeAsString != null)
			{
				return Convert.ToUInt64(childNodeAsString);
			}
			if (defaultValue != null)
			{
				return defaultValue.Value;
			}
			throw new Exception($"Could not find child XML node '{childNodeName}'");
		}
    }
}
