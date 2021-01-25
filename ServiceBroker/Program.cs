using ServiceBroker.Message;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceBroker
{
    class Program
    {
        /////////////////////////////////////////////////////////////////////////////////////
        /*
         * Use CreateDatabase.sql in the root of solution directory to create database
         * Update <SET_CONNECTION_STRING> to match the configuration in your environment
        */
        /////////////////////////////////////////////////////////////////////////////////////

        static readonly string ConnectionString = "<SET_CONNECTION_STRING>";
        static bool IsReceiverStarted = false;

        static bool ContainsArgument(
            string[] Arguments,
            string SearchArgument,
            bool IgnoreCase)
        {
            if(Arguments.Length == 0)
            {
                return false;
            }

            foreach(var arg in Arguments)
            {
                var cleanArg = RemoveArgumentDelimiters(
                    arg);

                if(string.Compare(cleanArg, SearchArgument, IgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        static string RemoveArgumentDelimiters(
            string Argument)
        {
            return Argument.TrimStart(new char[] { '/', '-' });
        }

        static void Wait(
            int WaitMs)
        {
            var delay = Task.Delay(
                WaitMs);

            delay.Wait();
        }

        static void StartReceiver()
        {
            Process.Start(
                Assembly.GetExecutingAssembly().Location,
                "-recv");
        }

        static void Main(string[] args)
        {
            if(ConnectionString == "<SET_CONNECTION_STRING>")
            {
                Console.WriteLine(
                    "Please create database and set connection string before running");

                Wait(
                    5000);

                return;
            }

            Console.WriteLine(
                "Press any key to quit...");

            var handler = new MessageHandler(
                ConnectionString);

            while(Console.KeyAvailable == false)
            {
                if (ContainsArgument(args, "recv", true))
                {
                    var received = handler.ReceiveMessage();

                    if (string.IsNullOrEmpty(received) == false)
                    {
                        Console.WriteLine(
                            received);
                    }
                    else
                    {
                        Console.WriteLine(
                            "No messages");
                    }
                }
                else
                {
                    if(IsReceiverStarted == false)
                    {
                        StartReceiver();
                        IsReceiverStarted = true;
                    }

                    Console.WriteLine(
                        "Sending message...");

                    handler.SendMessage();
                }

                Wait(
                    1000);
            }
        }
    }
}
