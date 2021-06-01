using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

namespace Services.ServiceBus
{
    public class ServiceBusSrvc : IServiceBusSrvc
    {
        private readonly string _connectionString;
        private readonly string _qname;
        public ServiceBusSrvc(string connectionString, string qname)
        {
            _connectionString = connectionString;
            _qname = qname;
        }


        public async Task SendMessage(string message)
        {
            await using (ServiceBusClient client = new ServiceBusClient(_connectionString))
            {
                // create a sender for the queue 
                ServiceBusSender sender = client.CreateSender(_qname);

                // create a message that we can send
                ServiceBusMessage sbMessage = new ServiceBusMessage(message);

                // send the message
                await sender.SendMessageAsync(sbMessage);
            }
        }
    }
}
