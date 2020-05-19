using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieAssistant_Mini
{
    public delegate void voider(List<StringTuple> tuples);
    public partial class Form1 : Form
    {
        List<StringTuple> stringTuples = new List<StringTuple>();
        DownloadManager downloadManager = new DownloadManager();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var res = folderBrowserDialog.ShowDialog();
            if (res != DialogResult.Cancel)
                textBox1.Text = folderBrowserDialog.SelectedPath;
        }
        private void Block()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            checkedListBox1.Enabled = false;
            checkedListBox1.Items.Clear();
        }
        private void Unblock(List<StringTuple> tuples)
        {
            if (InvokeRequired)
                Invoke(new voider(Unblock), tuples);
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                checkedListBox1.Enabled = true;
                if (tuples.Count > 0)
                {
                    var strs = from t in tuples select t.name;
                    checkedListBox1.Items.AddRange(strs.Distinct().ToArray());
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Block();
            button2.Click -= button2_Click;
            button2.Click += button2_Click2;
            Task.Run(() =>
            {
                var res = Search(textBox2.Text);
                stringTuples.Clear();
                stringTuples.AddRange(res);
                Unblock(res);
            });
        }
        private void button2_Click2(object sender, EventArgs e)
        {
            button2.Click -= button2_Click2;
            button2.Click += button2_Click3;
            var ch = new List<string>();
            foreach (object item in checkedListBox1.CheckedItems)
                if (item is string)
                    ch.Add(item as string);
            Block();
            Task.Run(() =>
            {
                var urls = (from t in stringTuples where ch.Contains(t.name) select t.url).Distinct().ToList();
                var res = getSubLinks(urls);
                stringTuples.Clear();
                stringTuples.AddRange(res);
                Unblock(res);
            });
        }
        private void button2_Click3(object sender, EventArgs e)
        {
            button2.Click -= button2_Click3;
            button2.Click += button2_Click;
            var ch = new List<string>();
            foreach (object item in checkedListBox1.CheckedItems)
                if (item is string)
                    ch.Add(item as string);
            Block();
            Task.Run(() =>
            {
                var urls = (from t in stringTuples where ch.Contains(t.name) select t.url).Distinct().ToList();
                DownloadSubs(urls);
                stringTuples.Clear();
                Unblock(new List<StringTuple>());
            });
        }
        public void DownloadSubs(List<string> URLs)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            foreach (var item in URLs)
            {
                var html = client.DownloadString(item);
                var res = LoadDataFromHTML(html, "//div [@class='download']/a/@href");
                downloadManager.AddDownloadItem(textBox1.Text, "https://subscene.com" + res[0][0]);
            }
            downloadManager.Start();
        }
        public List<StringTuple> getSubLinks(List<string> URLs)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var ret = new List<StringTuple>();
            foreach (var item in URLs)
            {
                var html = client.DownloadString(item);
                var res = LoadDataFromHTML(html, "//td[@class='a1']/a/@href");
                foreach (var item2 in res)
                    ret.Add(new StringTuple() { url = "https://subscene.com" + item2[0], name = item2[0].Split('/')[3] });
            }
            return ret;
        }
        public List<StringTuple> Search(string keyWord)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var data = new NameValueCollection();
            data.Add("query", keyWord);
            var res = client.UploadValues("https://subscene.com/subtitles/searchbytitle", "post", data);
            var html = Encoding.UTF8.GetString(res);
            var res2 = LoadDataFromHTML(html, "//div[@class='title']/a", "//div[@class='title']/a/@href");
            var ret = new List<StringTuple>();
            foreach (var item in res2)
                ret.Add(new StringTuple() { name = item[0], url = "https://subscene.com" + item[1] });
            return ret;
        }
        private static List<List<string>> LoadDataFromHTML(string HTML, params string[] xPathes)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HTML);
            var firstDraft = new List<List<string>>();
            int rowsCount = xPathes.Length;
            HtmlNodeCollection nodes = null;
            var regex = new Regex(@"/@\w+$");

            for (int i = 0; i < rowsCount; i++)
            {
                var list = new List<string>();
                var m = regex.Match(xPathes[i]);
                nodes = doc.DocumentNode.SelectNodes(xPathes[i]);
                if (nodes == null)
                    list.Add("");
                else
                {
                    if (!m.Success)
                        foreach (var node in nodes)
                            list.Add(node.InnerText.Trim());

                    else
                    {
                        string attribute = m.Value.Substring(2);
                        foreach (var node in nodes)
                            list.Add(node.GetAttributeValue(attribute, "Attribute not found !"));
                    }
                }
                firstDraft.Add(list);
            }

            //Transpose
            var grouped = new List<List<string>>();
            var len = firstDraft.First().Count;
            for (int i = 0; i < len; i++)
            {
                var list = new List<string>();
                foreach (var item in firstDraft)
                    if (item.Count > 0)
                    {
                        list.Add(item.First());
                        item.RemoveAt(0);
                    }
                    else
                        list.Add("");
                grouped.Add(list);
            }
            return grouped;
        }
    }
}
