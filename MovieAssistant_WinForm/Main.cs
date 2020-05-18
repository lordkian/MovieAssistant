using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XpathDataCrawler.DataGrab;
using XpathDataCrawler.DataStructure.Model;

namespace MovieAssistant_WinForm
{
    public partial class Main : Form
    {
        private DataGrab dataGrab = null;
        Guid guid;
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog() { SelectedPath = textBox1.Text };
            var res = folderBrowserDialog.ShowDialog();
            if (res != DialogResult.Cancel)
                textBox1.Text = folderBrowserDialog.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Model model = new Model()
            {
                BaseURL = "https://subscene.com",
                SiteNmae = "subscene"
            };
            var m = new Method();
            m.URL.Add("strhttps://subscene.com/subtitles/searchbytitle");
            m.Keys.Add(new List<string>() { "strquery" });
            m.Values.Add(new List<string>() { "xpath" });
            var rootGuid = model.SetRoot(m);

            var f1Guid = model.AddItem(rootGuid, "//div[@class='title']/a", "Name", LeafType.Data, false);
            model.AddItem(rootGuid, @"//div[@class='subtle count']", "Count", LeafType.Data, false);
            var guid1 = model.AddXpath(rootGuid, "//div[@class='title']/a/@href");

            model.AddItem(guid1, @"//td[@class='a1']/a/span[not(@class)]", "Subtitle Name", LeafType.Data, false);
            var f2Guid = model.AddItem(guid1, "//td[@class='a1']/a/span[@class='l r positive-icon']", "Language", LeafType.Data, false);
            var guid2 = model.AddXpath(guid1, "//td[@class='a1']/a/@href");

            model.AddItem(guid2, "//div [@class='download']/a/@href", "subtitle.zip", LeafType.Downloadable, true);

            dataGrab = new DataGrab(model, textBox2.Text);
            dataGrab.SetFilter(f1Guid, f2Guid);
            dataGrab.onFilter = Filter;
            dataGrab.onFinish = Finish;
            dataGrab.Start();

            button2.Click -= button2_Click;
        }
        private void button2_Click2(object sender, EventArgs e)
        {
            var data = new List<string>();
            foreach (var item in checkedListBox1.SelectedItems)
            {
                data.Add(item.ToString());
            }
            dataGrab.Filter(guid, true, data.ToArray());
            button2.Click -= button2_Click2;
            dataGrab.Continue();
        }
        private void Filter(Guid id, string xpath, string[] data)
        {
            guid = id;
            if (button2.Text != "continue")
                button2.Text = "continue";
            button2.Click += button2_Click2;
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.AddRange(data.Distinct().ToArray());
        }
        private void Finish(DataGrab dataGrab)
        {
            if (!Directory.Exists(textBox1.Text))
                Directory.CreateDirectory(textBox1.Text);
            dataGrab.Download(textBox1.Text);
        }
    }
}
