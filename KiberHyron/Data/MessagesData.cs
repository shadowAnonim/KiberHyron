using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiberHyron.Data
{
    public class MessagesData : JsonData
    {
        public List<ReactableMessage> messages = new List<ReactableMessage>();
    }
}
