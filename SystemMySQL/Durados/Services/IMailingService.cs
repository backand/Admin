using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Services
{
    public interface IMailingService
    {
        bool UseFieldOrder { get; set; }
        string SubscribeFieldName { get; set; }
        Dictionary<string, string> SubscribeBatch(DataTable subscribers);
        Dictionary<string, string> SubscribeBatch(DataTable subscribers, string listId, string apiKey);
    }
}
