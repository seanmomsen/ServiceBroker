using System;
using System.Collections.Generic;

namespace ServiceBroker.Message.Reader
{
    interface IMessageReader
    {
        string RootElementName { get; set; }

        List<KeyValuePair<string, string>> Read(
            string RootElementName,
            List<string> ChildElementNames,
            string Xml);
    }
}
