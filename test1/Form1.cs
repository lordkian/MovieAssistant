using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.MovieAssistant.core;

namespace test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModleTree modleTree = new ModleTree(3);
            var root = modleTree.GetRoot() as BrancheModle;
            root.Next[0] = new LeafModle() { Type = LeafType.data, Name = "Name", Xpath = "//div[@class='search-result']//ul/li//a", IsUnique = false };
            root.Next[1] = new LeafModle() { Type = LeafType.data, Name = "Count", Xpath = "//div[@class='search-result']//ul/li/div[@class='subtle count']", IsUnique = false };
            root.Next[2] = new BrancheModle(2) { Xpath = "//div[@class='search-result']//ul/li//a/@href" };
            var br = root.Next[2] as BrancheModle;
            br.Next[0] = new LeafModle() { Type = LeafType.data, Name = "Subtitle Name", Xpath = @"//tbody//tr/td[1]/a/span[1]", IsUnique = false };
            br.Next[1] = new BrancheModle(1) { Xpath = @"//tbody//tr/td[1]/a/@href" };
            br = br.Next[1] as BrancheModle;
            br.Next[0] = new LeafModle() { Type = LeafType.downloadable, Xpath = @"//div[@class='download']/a/@href", Name = "subtitle.zip", IsUnique = true };

        }
    }
}
