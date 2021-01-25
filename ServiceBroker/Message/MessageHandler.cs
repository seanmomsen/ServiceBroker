using ServiceBroker.Message.Builder;
using ServiceBroker.Message.Reader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace ServiceBroker.Message
{
    class MessageHandler
    {
        string ConnectionString = "";

        public MessageHandler(
            string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public void SendMessage()
        {
            var xml = GetMachineEventXml(
                new XmlMessageBuilder(),
                $"Test text");

            SendMachineEvent(
                xml);
        }

        public string ReceiveMessage()
        {
            var xml = ReceiveMachineEvent();

            if(string.IsNullOrEmpty(xml))
            {
                return string.Empty;
            }

            return GetFormattedMachineEvent(
                xml);
        }

        private string GetMachineEventXml(
            XmlMessageBuilder Builder,
            string EventText)
        {
            return Builder.Build(
                "MachineEvent",
                Environment.MachineName,
                Guid.NewGuid().ToString(),
                EventText);
        }

        private List<KeyValuePair<string, string>> ParseMachineEventXml(
            XmlMessageReader Reader,
            string Xml)
        {
            return Reader.Read(
                "MachineEvent",
                new List<string>() { "MachineName", "MachineId", "EventText" },
                Xml);
        }

        private void SendMachineEvent(
            string EventXml)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Send_Message";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("MessageText", SqlDbType.NVarChar, -1);
                    command.Parameters["MessageText"].Value = EventXml;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private string ReceiveMachineEvent()
        {
            using (var scope = new TransactionScope())
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "Receive_Message";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("MessageText", SqlDbType.NVarChar, -1);
                        command.Parameters["MessageText"].Direction = ParameterDirection.Output;

                        conn.Open();
                        command.ExecuteNonQuery();
                        var text = command.Parameters["MessageText"].Value as string;
                        conn.Close();
                        scope.Complete();

                        return text;
                    }
                }
            }
        }

        private string GetFormattedMachineEvent(
            string EventXml)
        {
            var parsedValues = ParseMachineEventXml(
                new XmlMessageReader(),
                EventXml);

            var builder = new StringBuilder();

            builder.Append(
                "Received: ");

            foreach (var item in parsedValues)
            {
                builder.Append(
                    $"{item.Key}={item.Value} ");
            }

            return builder.ToString();
        }
    }
}
