﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Library.DataStructure.Model;

namespace Library.DataStructure.DataGrab
{
    public class DataGrab
    {
        readonly Model.Model model;
        readonly Tree<DataNode> tree = new Tree<DataNode>();
        readonly string keyword;
        List<string> filterXpaths = new List<string>();
        List<Guid> filterIds = new List<Guid>();
        public Action<Guid, string, string[]> onFilter { get; set; }
        public Action<DataGrab> onFinish { get; set; }
        public DataGrab(Model.Model model, string keyword)
        {
            this.model = model;
            this.keyword = keyword;
            var root = new DataNode();
            root.ModelNodes.Add(model.Root);
            tree.Add(root, null);
        }
        public void SetFilter(params Guid[] guids)
        {
            filterIds.Clear();
            filterIds.AddRange(guids);
        }
        public void SetFilter(params string[] xpathes)
        {
            filterXpaths.Clear();
            filterXpaths.AddRange(xpathes);
        }
        public void Start()
        {
            var root = RootGrabData();
            tree.Add(root, null);

            var list = new List<DataNode>() { root };
            var list2 = new List<DataNode>();
            while (list.Count > 0)
            {
                foreach (var item in list)
                    list2.AddRange(GrabData(item));
                list.Clear();
                list.AddRange(list2);
                list2.Clear();
            }
            onFinish(this);
        }
        private DataNode RootGrabData()
        {
            var html = MethodProcess(model.Root.GrabMethode, keyword);
            var dn = new DataNode();
            dn.Datas.Add(html);
            dn.ModelNodes.Add(model.Root);
            return dn;
        }
        private List<DataNode> GrabData(DataNode dataNode)
        {
            var res = new List<DataNode>();

            for (int i = 0; i < dataNode.ModelNodes.Count; i++)
            {
                if (dataNode.ModelNodes[i] is Leaf)
                    continue;
                var html = dataNode.Datas[i];
                var children = model.GetChildren(dataNode.ModelNodes[i]);
                var xpaths = new List<string>();
                foreach (var item in children)
                    xpaths.Add(item.Xpath);
                var res2 = LoadDataFromHTML(html, xpaths.ToArray());
                res.AddRange(ParsData(res2, children));

                dataNode.Datas[i] = "";
            }

            return res;
        }
        private List<DataNode> ParsData(List<List<string>> data, List<ModelNode> children)
        {
            var res = new List<DataNode>();

            return res;
        }
        private string MethodProcess(Method method, string xpathResult)
        {
            var url = BuildString(method.URL, xpathResult);
            var c = method.Keys.Count;
            if (c == 0)
                return LoadData(url);
            var nvc = new NameValueCollection();
            for (int i = 0; i < c; i++)
                nvc.Add(BuildString(method.Keys[i], xpathResult), BuildString(method.Values[i], xpathResult));
            return LoadData(url, nvc);
        }
        private string BuildString(List<string> list, string xpathResult)
        {
            var res = "";
            foreach (var item in list)
            {
                var value = item.Remove(0, 3);
                switch (item.Substring(0, 3))
                {
                    case "str":
                        res += value;
                        break;
                    case "xpa":
                        res += xpathResult;
                        break;
                    case "bur":
                        res += model.BaseURL;
                        break;
                    default:
                        break;
                }
            }
            return res;
        }
        private static string LoadData(string URL, NameValueCollection data)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var res = client.UploadValues(URL, "post", data);
            return Encoding.UTF8.GetString(res);
        }
        private static string LoadData(string URL)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(URL);
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
