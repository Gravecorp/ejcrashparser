using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ejcrashparser
{
    class MessageTypes
    {
        public static List<Tuple<bool, string>> GetMessageTypes()
        {
            List<Tuple<bool, string>> ret = new List<Tuple<bool, string>>();
            XmlDocument doc = OpenXMLFile();
            XmlNodeList nodeList = doc.GetElementsByTagName("type");
            foreach (XmlNode node in nodeList)
            {
                bool multiLine = false;
                string messageType = string.Empty;
                for(int i = 0; i < node.ChildNodes.Count;i++)
                {
                    XmlNode subNode = node.ChildNodes.Item(i);
                    if (subNode.Name == "name")
                    {
                        messageType = subNode.InnerText;
                    }
                    if (subNode.Name == "multiline")
                    {
                        bool.TryParse(subNode.InnerText, out multiLine);
                    }
                }
                Tuple<bool, string> tuple = new Tuple<bool, string>(multiLine, messageType);
                ret.Add(tuple);
            }
            return (ret);
        }

        private static XmlDocument OpenXMLFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("MessageTypes.xml");
            return (doc);
        }
    }
}
