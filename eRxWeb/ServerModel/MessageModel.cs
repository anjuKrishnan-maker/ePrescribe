using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public enum MessageIcon
    {
        Success = 0,
        Error=1,
        Information=2
    }
    public class MessageModel
    {
        public MessageModel()
        {
            ShowCloseButton = true;
        }

        public MessageModel(string message, MessageIcon icon = MessageIcon.Success)
        {
            Message = message;
            Icon = icon;
            ShowCloseButton = true;
        }

        public MessageModel(string message, MessageIcon icon, bool showCloseButton)
        {
            Message = message;
            Icon = icon;
            ShowCloseButton = showCloseButton;
        }

        public string Message { get; set; }
        public MessageIcon Icon { get; set; }
        public string Tag { get; set; }
        public bool ShowCloseButton { get; set; }
    }
   
}