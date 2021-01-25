using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ServiceBroker.Message.Builder
{
    class XmlMessageBuilder : IMessageBuilder
    {
        public string RootElementName { get; set; }
        public List<KeyValuePair<string, string>> NamedValues { get; set; }

        public string Build(
            string RootElementName,
            List<KeyValuePair<string, string>> NamedValues)
        {
            if (string.IsNullOrEmpty(RootElementName))
            {
                if (RootElementName == null)
                {
                    throw new ArgumentNullException("RootElementName");
                }

                throw new ArgumentException("RootElementName");
            }

            if (NamedValues == null)
            {
                throw new ArgumentNullException("NamedValues");
            }

            this.RootElementName = RootElementName;
            this.NamedValues = NamedValues;

            var machineEvent = CreateMachineEvent();

            var doc = CreateDocument(
                machineEvent);

            AddDocumentDeclaration(
                doc);

            return doc.OuterXml;
        }

        public string Build(
            string RootElementName,
            string MachineName,
            string MachineId,
            string EventText)
        {
            var namedValues = BuildXmlNamedValues(
                MachineName,
                MachineId,
                EventText);

            return Build(
                RootElementName,
                namedValues);
        }

        private List<KeyValuePair<string, string>> BuildXmlNamedValues(
            string MachineName,
            string MachineId,
            string EventText)
        {
            var items = new List<KeyValuePair<string, string>>();

            items.Add(
                new KeyValuePair<string, string>("MachineName", MachineName));

            items.Add(
                new KeyValuePair<string, string>("MachineId", MachineId));

            items.Add(
                new KeyValuePair<string, string>("EventText", EventText));

            return items;
        }

        private XmlDocument CreateDocument(
            XElement Data)
        {
            var doc = new XmlDocument();

            doc.LoadXml(
                Data.ToString());

            return doc;
        }

        private XmlDeclaration CreateDocumentDeclaration(
            XmlDocument Doc)
        {
            return Doc.CreateXmlDeclaration(
                "1.0",
                "UTF-16",
                null);
        }

        private XElement CreateMachineEvent()
        {
            var machineEvent = new XElement(RootElementName);

            foreach(var item in NamedValues)
            {
                machineEvent.Add(
                    new XElement(item.Key, item.Value));
            }

            return machineEvent;
        }

        private void AddDocumentDeclaration(
            XmlDocument Doc)
        {
            var xmlDecl = CreateDocumentDeclaration(
                Doc);

            var root = Doc.DocumentElement;

            Doc.InsertBefore(
                xmlDecl,
                root);
        }
    }
}
