using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Json
{
    public class JsonStatus
    {
        public bool Status { get; set; } = false;

        public JsonStatusType Type { get; set; } = JsonStatusType.Success;

        public List<JsonMessage> Messages { get; set; } = new List<JsonMessage>();

        public List<JsonMessage> MessagesAdmin { get; set; } = new List<JsonMessage>();

        public void MsgAdminAdd(string source)
        {
            MessagesAdmin.Add(new JsonMessage() { Text = source });
        }

        public void MsgAdd(string source)
        {
            Messages.Add(new JsonMessage() { Text = source });
        }

        public void MsgAdd(string source, JsonMessageType type)
        {
            Messages.Add(new JsonMessage() { Text = source, Type = type });
        }

        public void MsgAddDanger(string source)
        {
            Messages.Add(new JsonMessage() { Text = source, Type = JsonMessageType.Danger });
        }
        public void MsgAddSuccess(string source)
        {
            Messages.Add(new JsonMessage() { Text = source, Type = JsonMessageType.Success });
        }

        public string MsgToString()
        {
            string ret = "";
            foreach (var msg in Messages)
            {
                ret += $"<div class=\"alert alert-info\" role=\"alert\">{msg.Text}</ div >";
            }
            return ret;
        }

        public string Meta { get; set; }
    }

    public class JsonMessage
    {
        public JsonMessageType Type { get; set; } = JsonMessageType.Info;

        public string Text { get; set; }

    }

    public enum JsonStatusType { Success, Warning, Error }

    public enum JsonMessageType { Info, Success, Danger, Warning, Primary }
}
