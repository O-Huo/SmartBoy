using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeosSmartBoy.Callbacks
{
    public enum Button
    {
        [EnumMember(Value = "Back")]
        Back,
        [EnumMember(Value = "Ok")]
        Ok,
        [EnumMember(Value = "Dot")]
        Dot,

    }
    public class InlineKeyboardCallback
    {
        public string Command;
        public object Data;

        [JsonConstructor]
        public InlineKeyboardCallback(string command, string data)
        {
            Command = command;
            Data = data;
        }

        public InlineKeyboardCallback(string command, Button data)
        {
            Command = command;
            Data = data.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}