using System;
using System.Collections.Generic;
using System.Text;

namespace ejcrashparser
{
    class LogMessage
    {
        public enum SortType
        {
            ByTimestamp,
            ByID
        }

        public string Timestamp
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string MessageType
        {
            get;
            set;
        }

        public string MessageID
        {
            get;
            set;
        }

        public override string ToString()
        {
            string ret = string.Empty;
            switch (Form1.SortType)
            {
                case (int)SortType.ByTimestamp:
                    ret = string.Format("Timestamp: {0}\nID: {1}\nType: {2}\nMessage: {3}", Timestamp, MessageID, MessageType, (Message != null) ? Message.Trim() : "");
                    break;
                case (int)SortType.ByID:
                    ret = string.Format("ID: {0}\nTimestamp: {1}\nType: {2}\nMessage: {3}", MessageID, Timestamp, MessageType, (Message != null) ? Message.Trim() : "");
                    break;
                default:
                    ret = base.ToString();
                    break;
            }
            return (ret);
        }
    }
}
