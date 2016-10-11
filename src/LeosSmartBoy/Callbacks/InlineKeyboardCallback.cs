using System;
using Newtonsoft.Json;

namespace LeosSmartBoy.Callbacks
{
    public class InlineKeyboardCallback
    {
        public string Command;
        public string Data;

        static InlineKeyboardCallback CreateWithDataObject(string command, Object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            return new InlineKeyboardCallback
            {
                Command = command,
                Data = jsonString
            };
        }
    }
}