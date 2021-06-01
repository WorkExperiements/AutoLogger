using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceBus
{
    public interface IServiceBusSrvc
    {
        Task SendMessage(string message);
    }
}
