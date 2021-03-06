﻿using System;

namespace EWork.Models.Json
{
    public class JsonMessage
    {
        public JsonMessage(JsonUser sender, JsonUser receiver, string text, DateTime sendDate)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendDate = sendDate;
        }

        public JsonUser Sender { get; }
        public JsonUser Receiver{ get; }
        public string Text { get; set; }
        public DateTime SendDate { get; }
    }
}
