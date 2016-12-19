using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Translator.Properties;

namespace Translator
{
    public partial class SubTitleTranslator : Form
    {
        public SubTitleTranslator()
        {
            InitializeComponent();
        }
        private bool _IsSaved = true;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Close();
        }
        string fname = "";
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            fname=openFileDialog1.FileName ;
            string[] alllines= File.ReadAllLines(fname, Encoding.Default);

            SrtItemCollection xx = new SrtItemCollection(File.ReadAllText(fname, Encoding.Default));
            listView1.Items.Clear();
            foreach (var item in xx.SrtItems)
            {
                ListViewItem lvi = new ListViewItem(item.ID+"");
                lvi.Tag = item;
                lvi.ImageIndex = 0;
                lvi.SubItems.Add(item.FromTime +"");
                lvi.SubItems.Add(item.EndTime  + "");
                lvi.SubItems.Add(item.OrginalSRT + "");
                listView1.Items.Add(lvi);
            }
            _IsSaved = true;
        }
        bool StopOperation = false;
        bool IsRunning = false;
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {

                toolStripProgressBar1.Visible = true;
                Application.DoEvents();
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = listView1.Items.Count;

                if (IsRunning)
                {
                    StopOperation = true;
                    return;
                }
                
                StopOperation = false;
                toolStripButton4.Image = Resources.PauseRecord;
                toolStripButton4.Text = "ايقاف الترجمة";
                foreach (ListViewItem item in listView1.Items)
                {
                    Application.DoEvents();
                    if (item.ImageIndex == 1) continue;
                    string srt = ((SrtItem)item.Tag ).OrginalSRT + "";
                    string r = Gapi.Language.Translator.Translate(srt, Gapi.Language.Language.English, Gapi.Language.Language.Arabic);

                    if (StopOperation) 
                        break;
                    IsRunning = true;
                    toolStripProgressBar1.Value++;
                    ((SrtItem)item.Tag).TranslatedSRT = r;
                    item.SubItems.Add(r );
                    item.ImageIndex = 1;
                    Application.DoEvents();
                }
                IsRunning = false;
            }
            catch
            {
                IsRunning = false;
            }
            finally
            {
               // IsRunning = false;
               // StopOperation = false;
                toolStripProgressBar1.Visible = false;
                toolStripButton4.Text = "استئناف الترجمة";
                toolStripButton4.Image = Resources.PlayHS ;
            }
            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(fname) + "_ar.srt";
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                toolStripProgressBar1.Visible = true ;
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = listView1.Items.Count;
                string sfname = "";
                sfname = saveFileDialog1.FileName;
                string srtText = "";
                foreach (ListViewItem item in listView1.Items)
                {
                    SrtItem m = ((SrtItem)item.Tag);
                    toolStripProgressBar1.Value++;

                   

                    srtText += m.ID + Environment.NewLine + m.FromTime +"-->"+ m.EndTime +
                        Environment.NewLine + m.TranslatedSRT  + Environment.NewLine ;

                }
                File.WriteAllText(sfname, srtText, Encoding.Default);
            }
            catch
            {
            }
            finally
            {
                toolStripProgressBar1.Visible = false;
            }
        }

      
    }


    class SrtItemCollection
    {
        private static Regex unit = new Regex(
            @"(?<sequence>\d+)\r\n(?<start>\d{2}\:\d{2}\:\d{2},\d{3}) --\> (?<end>\d{2}\:\d{2}\:\d{2},\d{3})\r\n(?<text>[\s\S]*?\r\n\r\n)",
            RegexOptions.Compiled | RegexOptions.ECMAScript);
        
        public SrtItemCollection(string srtAsString)
        {
            foreach (Match item in unit.Matches(srtAsString))
            {
                SrtItem SrtItem = new SrtItem();
                SrtItem.ID = int.Parse(item.Groups["sequence"].Value.Trim());
                SrtItem.FromTime = item.Groups["start"].Value.Replace(",", ".").Trim();
                SrtItem.EndTime = item.Groups["end"].Value.Replace(",", ".").Trim();
                SrtItem.OrginalSRT = item.Groups["text"].Value;

                SrtItems.Add(SrtItem);
            }
        }
        public List<SrtItem> SrtItems = new List<SrtItem>();
        
    }
    class SrtItem
    {
        public SrtItem()
        {
        }               
        public int ID { get; set; }
        public string  FromTime{get;set;}
        public string  EndTime { get; set; }
        public string  OrginalSRT { get; set; }
        public string TranslatedSRT { get; set; }
    }
}
