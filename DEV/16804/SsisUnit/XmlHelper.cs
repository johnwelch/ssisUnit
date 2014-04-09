namespace SsisUnit
{
    public class XmlHelper
    {
        public static string EscapeAttributeValue(string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return attributeValue;

            return attributeValue.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string UnescapeAttributeValue(string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return attributeValue;

            return attributeValue.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
        }
    }
}