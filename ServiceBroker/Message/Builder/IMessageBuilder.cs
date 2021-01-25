using System;
using System.Collections.Generic;

namespace ServiceBroker.Message.Builder
{
    interface IMessageBuilder
    {
        string RootElementName { get; set; }
        List<KeyValuePair<string, string>> NamedValues { get; set; }

        string Build(
            string RootElementName,
            List<KeyValuePair<string, string>> NamedValues);
    }
}
