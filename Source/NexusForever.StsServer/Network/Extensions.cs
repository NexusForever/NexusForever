using System;
using System.Xml;

namespace NexusForever.StsServer.Network
{
    public static class Extensions
    {
        public static T GetValue<T>(this XmlNode node)
        {
            if (node == null)
                return default;

            if (node.NodeType != XmlNodeType.Element)
                return default;

            XmlNode valueNode = node.FirstChild;
            return (T)Convert.ChangeType(valueNode.Value, typeof(T));
        }
    }
}
