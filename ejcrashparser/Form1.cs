﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ejcrashparser
{
    public partial class Form1 : Form
    {
        private string targetPath = string.Empty;

        private TreeNode[] savedNodes = null;

        public static int SortType = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                targetPath = folderBrowserDialog1.SelectedPath;
            }
            ReadLogFiles();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            targetPath = Application.StartupPath;
            ReadLogFiles();
        }

        private void ReadLogFiles()
        {
            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
            string[] files = Directory.GetFiles(targetPath, "*.log");
            int i = 0;
            foreach (string file in files)
            {
                treeView2.Nodes.Add(file);
                //if (i < 2)
                //{
                ParseLogFile(file);
                //    i++;
                //}
                //else
                //{
                //    break;
                //}
            }
            savedNodes = new TreeNode[treeView1.Nodes.Count];
            treeView1.Nodes.CopyTo(savedNodes, 0);
        }

        private void ParseLogFile(string file)
        {
            TextReader a = new StreamReader(file);
            bool msgStart = false;
            LogMessage current = new LogMessage();
            string line = null;
            while((line = a.ReadLine()) != null)
            {
                if (line.Trim().Length < 1)
                {
                    msgStart = false;
                    continue;
                }
                if (line.Trim().StartsWith("="))
                {
                    msgStart = true;
                    if (current.MessageID != null)
                    {
                        AddMessage(current);
                    }
                    current = new LogMessage();
                    ParseTimeStamp(current, line);
                    continue;
                }
                if (line.Trim().StartsWith("D"))
                {
                    int start = line.IndexOf("(");
                    int end = line.IndexOf(")");
                    string id = line.Substring(start, end - start);
                    current.MessageID = id;
                    line = line.Substring(end);
                    Tuple<bool, string> tuple = GetMessageType(line);
                    current.MessageType = tuple.Item2;
                    if (!tuple.Item1)
                    {
                        continue;
                    }
                    else
                    {
                        line = line.Substring(line.IndexOf(tuple.Item2) + tuple.Item2.Length);
                    }
                }
                current.Message = current.Message + line;
            }
            UpdateToolStrips();
        }

        private void UpdateToolStrips()
        {
            toolStripStatusLabel1.Text = string.Format("Files: {0}", treeView2.Nodes.Count);
            toolStripStatusLabel2.Text = string.Format("Messages: {0}", treeView1.Nodes.Count);
        }

        private List<Tuple<bool, string>> messageTypes = MessageTypes.GetMessageTypes();

        private Tuple<bool, string> GetMessageType(string line)
        {
            Tuple<bool, string> ret = new Tuple<bool, string>(false, string.Empty);
            foreach (Tuple<bool, string> t in messageTypes)
            {
                if (line.Contains(t.Item2))
                {
                    ret = t;
                    break;
                }
            }
            return (ret);
        }

        private void ParseTimeStamp(LogMessage message, string line)
        {
            line = line.Replace("=INFO REPORT==== ", "");
            line = line.Replace("===", "");
            line = line.Trim();
            message.Timestamp = line;
        }

        private void AddMessage(LogMessage log)
        {
            TreeNode node = new TreeNode();
            node.Tag = log;
            node.Text = log.ToString();
            node.ToolTipText = node.Text;
            TreeNode timestampNode = node.Nodes.Add(string.Format("Timestamp: {0}", log.Timestamp));
            timestampNode.ToolTipText = timestampNode.Text;
            TreeNode idNode = node.Nodes.Add(string.Format("MessageID: {0}", log.MessageID));
            idNode.ToolTipText = idNode.Text;
            TreeNode typeNode = node.Nodes.Add(string.Format("MessageType: {0}", log.MessageType));
            typeNode.ToolTipText = typeNode.Text;
            TreeNode messageNode = node.Nodes.Add(string.Format("Message: {0}", (log.Message!= null)?log.Message.Trim():""));
            messageNode.ToolTipText = messageNode.Text;
            treeView1.Nodes.Add(node);
        }

        private void Resort()
        {
            switch(Form1.SortType)
            {
                case (int)LogMessage.SortType.ByTimestamp:
                    SortType = (int)LogMessage.SortType.ByID;
                    break;
                case (int)LogMessage.SortType.ByID:
                    SortType = (int)LogMessage.SortType.ByTimestamp;
                    break;
            }
            //Form1.SortType = (int)LogMessage.SortType.ByID;
            foreach (TreeNode node in treeView1.Nodes)
            {
                node.Text = ((LogMessage)node.Tag).ToString();
            }
            treeView1.Sort();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Resort();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (savedNodes == null)
            {
                ReadLogFiles();
            }
            else
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.AddRange(savedNodes);
                
            }
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    SearchByTimestamp();
                    break;
                case 1:
                    SearchByMessageID();
                    break;
                case 2:
                    SearchByMessageType();
                    break;
                case 3:
                    SearchByMessage();
                    break;
            }
            UpdateToolStrips();
        }

        private void SearchByTimestamp()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (msg.Timestamp.Contains(textBox1.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByMessageID()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (msg.MessageID.Contains(textBox1.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByMessageType()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (msg.MessageType.Contains(textBox1.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByMessage()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (msg.Message.Contains(textBox1.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void RepopulateWithList(List<TreeNode> list)
        {
            treeView1.Nodes.Clear();
            foreach (TreeNode node in list)
            {
                treeView1.Nodes.Add(node);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (savedNodes == null)
            {
                ReadLogFiles();
            }
            else
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.AddRange(savedNodes);

            }
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    SearchByTimestampRegEx();
                    break;
                case 1:
                    SearchByMessageIDRegEx();
                    break;
                case 2:
                    SearchByMessageTypeRegEx();
                    break;
                case 3:
                    SearchByMessageRegEx();
                    break;
            }
            UpdateToolStrips();
        }

        private void SearchByMessageRegEx()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (Regex.IsMatch(msg.Message, textBox2.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByMessageTypeRegEx()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (Regex.IsMatch(msg.MessageType, textBox2.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByMessageIDRegEx()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (Regex.IsMatch(msg.MessageID, textBox2.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }

        private void SearchByTimestampRegEx()
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                LogMessage msg = (LogMessage)node.Tag;
                if (Regex.IsMatch(msg.Timestamp, textBox2.Text))
                {
                    list.Add(node);
                }
            }
            RepopulateWithList(list);
        }
    }
}
