using System;
using System.Collections.Generic;
using System.Xml;

namespace ServiceBroker.Message.Reader
{
    class XmlMessageReader : IMessageReader
    {
        public string RootElementName { get; set; }

        public List<KeyValuePair<string, string>> Read(
            string RootElementName,
            List<string> ChildElementNames,
            string Xml)
        {
            if (string.IsNullOrEmpty(RootElementName))
            {
                if (RootElementName == null)
                {
                    throw new ArgumentNullException("RootElementName");
                }

                throw new ArgumentException("RootElementName");
            }

            if (string.IsNullOrEmpty(Xml))
            {
                if (Xml == null)
                {
                    throw new ArgumentNullException("Xml");
                }

                throw new ArgumentException("Xml");
            }

            if (ChildElementNames == null)
            {
                throw new ArgumentNullException("ChildElementNames");
            }

            var doc = new XmlDocument();

            doc.LoadXml(
                CleanXmlNewLine(Xml));

            return GetChildElementsByName(
                doc,
                RootElementName,
                ChildElementNames);
        }

        private string CleanXmlNewLine(
            string Xml)
        {
            return Xml.Replace(
                "\\n",
                "\n");
        }

        private List<KeyValuePair<string, string>> GetChildElementsByName(
            XmlDocument Doc,
            string RootElementName,
            List<string> ChildElementNames)
        {
            var items = new List<KeyValuePair<string, string>>();

            foreach(var name in ChildElementNames)
            {
                var item = Doc.SelectSingleNode(
                    $"/{RootElementName}/{name}/text()");

                if(item != null)
                {
                    items.Add(
                        new KeyValuePair<string, string>(name, item.Value));
                }
            }

            return items;
        }
    }
}
