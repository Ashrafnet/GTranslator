using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gapi.Language;
using System.Threading;
using Translator.Properties;
using System.IO;
using Microsoft.Win32;
using Gapi.Core;
using System.Runtime.InteropServices;
using System.Net;

namespace Translator
{
    public partial class Form1 : Form
    {
        ClipboardSaver.ClipboardEvents _clipboard;
        public Form1()
        {
            
            InitializeComponent();
            this.Icon = notifyIcon1.Icon;
            _clipboard = new ClipboardSaver.ClipboardEvents();
            _clipboard.OnUpdated += new ClipboardSaver.ClipboardEvents.ClipboardUpdate(clipboardEvents1_OnUpdated);
            thread.Container = this;
            thread.TranslateComplete += new TranslateThreadEvent(thread_TranslateComplete);
            thread.TranslationError += new TranslateThreadError(thread_TranslationError);
        }

        void thread_TranslationError(Exception error)
        {
            toolStripStatusLabel1.Text = error.Message;
            toolStripStatusLabel1.ForeColor = Color.Red;
            EnableOnTextChangeTrnslation = true;
            isFromClipBoard = false;
        }

        void thread_TranslateComplete(string result)
        {
            if (revers) textBox1.Text = result;
            else textBox2.Text = result;
            toolStripStatusLabel1.Text = "تمت الترجمة";
            toolStripStatusLabel1.ForeColor = Color.Green;
            
            if (isFromClipBoard && Form.ActiveForm == null)
                notifyIcon1.ShowBalloonTip(500, toolStripStatusLabel1.Text, result  + Environment.NewLine + "انقر هنا لترى الترجمة بشكل كامل", ToolTipIcon.Info);
            EnableOnTextChangeTrnslation = true;
            isFromClipBoard = false;
        }
        bool isFromClipBoard;
        private void clipboardEvents1_OnUpdated()
        {
            
            if (!Settings.Default.MonitorClipboard || Form.ActiveForm != null || !Clipboard.ContainsText()) return;
            isFromClipBoard = true;
            //textBox1.Text = "";
            string strClip=Clipboard.GetText().Trim();
            int len = strClip.IndexOf(" ");
            string  substring=strClip;
            if(len>-1)
              substring = strClip.Substring(0,strClip.IndexOf(" "));
            Language dl = Gapi.Language.Translator.Detect(substring);
            comboBox1.Text = dl.ToString();
            if (comboBox1.Text == "Arabic")
                comboBox2.Text = "English";
            else
                comboBox2.Text = "Arabic";
            textBox1.Text = strClip;
           if (Settings.Default.NotifyWhileProgress ) notifyIcon1.ShowBalloonTip(500, "بدء عملية الترجمة", "انقر هنا لرؤية نافذة الترجمة او انتظر حتى تنتهي عملية الترجمة", ToolTipIcon.Info);
        }
        [DllImport("user32")]
        public static extern int  GetCursorPos(ref Point lpPoint);
        [DllImport("user32")]
        public static extern int SendMessage(int  hwnd, int wMsg, int wParam,ref  int lParam);
        [DllImport("user32")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, ref  string  lParam);
        [DllImport("user32")]
        public static extern int WindowFromPoint(int xPoint, int yPoint);
        const int  WM_GETTEXT = 13;
        const int  WM_GETTEXTLENGTH = 14;
        string GetCursorText()
        {
            Point P = new Point() ;
            int  lRet;
            int  hHandle;
            string aText="";
            int lTextlen;
            int ii=0;
            lRet = GetCursorPos(ref  P);
            hHandle = WindowFromPoint(P.X, P.Y );
            lTextlen = SendMessage(hHandle, WM_GETTEXTLENGTH, 0, ref ii );
            if (lTextlen>0)
            {
                 if (lTextlen > 1024) lTextlen = 1024;
                 lTextlen += 1;
                StringBuilder xx = new StringBuilder();
                 aText =xx.Append(" ", 0, lTextlen).ToString();
                 lRet = SendMessage(hHandle, WM_GETTEXT, lTextlen,ref  aText);
                 aText = aText.Substring(lRet);
            }
            return aText;
        }

        public static bool StartupWithWindows
        {
            get
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (rkApp.GetValue("Translator") == null)
                {
                   
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value )
                {                    
                    rkApp.SetValue("Translator", Application.ExecutablePath.ToString() + " -min");
                }
                else
                {                    
                    rkApp.DeleteValue("Translator", false);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolStripManager.Renderer = new Office2007Renderer.Office2007Renderer();
            comboBox1.DataSource =Enum.GetNames(typeof(Gapi.Language.Language));
            comboBox2.DataSource = Enum.GetNames(typeof(Gapi.Language.Language));

            comboBox1.Text = "Arabic";
            comboBox2.Text = "English";
            comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            comboBox2.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            textBox1.Text = "اختبار";
            textBox1.SelectAll();
            textBox1.ShortcutsEnabled = true;
            textBox2.ShortcutsEnabled = true;

            if (Environment.CommandLine.ToLower().Contains("-min"))
            {
            }
            else
            {
                showWindowToolStripMenuItem.PerformClick();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Translate(false);
            
        }
        bool revers;
        bool EnableOnTextChangeTrnslation = true;
        TranslateThread thread=new TranslateThread();
        void Translate(bool revers)
        {
            try
            {
                
                toolStripStatusLabel1.Text = "تتم الان عملية الترجمة";
                toolStripStatusLabel1.ForeColor = Color.Blue;
                Application.DoEvents();
                Language SourceLanguage = (Language)Enum.Parse(typeof(Language), comboBox1.Text);
                Language targetLanguage = (Language)Enum.Parse(typeof(Language), comboBox2.Text);
                thread.Aboart();
                if (!revers) thread.SetInputs(textBox1.Text, SourceLanguage, targetLanguage);
                else thread.SetInputs(textBox2.Text, targetLanguage, SourceLanguage);
                this.revers = revers;
                thread.Start();
            }
            catch(Exception er)
            {
                thread_TranslationError(er);
                
            }
        }

       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

          if(EnableOnTextChangeTrnslation)  Translate(false );
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isFromClipBoard) return;
            Translate(false );
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag + "")
            {
                case "1":
                    if (splitContainer1.Orientation == Orientation.Horizontal)
                    {
                        splitContainer1.Orientation = Orientation.Vertical;
                        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                    }

                    else
                    {
                        splitContainer1.Orientation = Orientation.Horizontal;
                        splitContainer1.SplitterDistance = splitContainer1.Height  / 2;
                    }
                    break;
                case "2":
                    string xx = comboBox1.Text;
                    comboBox1.Text = comboBox2.Text;
                    comboBox2.Text = xx;
                    break;
                case "3":
                    EnableOnTextChangeTrnslation = false;
                    Translate(true);
                    break;
                case "4":
                    Hide();
                    break;
                case "5":
                    AboutBox1 frmabout = new AboutBox1();
                    frmabout.ShowDialog();
                    break;
                case "6":
                    SubTitleTranslator frm = new SubTitleTranslator();
                    frm.ShowDialog();
                    break;
                case "10":
                    //frmSMS x = new frmSMS();
                    //x.ShowDialog();
                    Application.Exit();
                    //string yy = CoreHelper.PerformRequest(@"https://mail.google.com/mail/channel/bind?VER=8&at=AF6bupOR88qtyup05a2FUH3Rv767EEk4lw&it=62&SID=25CB4B2200E38B5A&RID=59498&AID=61&zx=4fy9qz-2eqrle&t=1&count=1&ofs=82&req0__sc=c&req0_chatstate=active&req0_iconset=goomoji&req0_id=25CB4B2200E38B5A_2&req0_smsnumber=599594047&req0_text=Ashraf&req0_to=144623978b96da61%40contact.talk.google.com&req0_type=m&gmail=net.ashraf&pass=ashrafco");
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                notifyIcon1.Visible = true;
                Hide();
            }
            else thread.Aboart();
                

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
           // InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo(comboBox1.Text));

        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            showWindowToolStripMenuItem.PerformClick();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            enableClipboardToolStripMenuItem.Checked = Settings.Default.MonitorClipboard;
            toolStripMenuItem1.Checked = Settings.Default.NotifyWhileProgress ;
            startWithWindowsToolStripMenuItem.Checked = StartupWithWindows;
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag + "")
            {
                case "0": this.ShowInTaskbar = true; this.WindowState = FormWindowState.Normal; this.Show(); this.Activate();
                    break;
                case "1": Settings.Default.MonitorClipboard = !Settings.Default.MonitorClipboard;
                    Settings.Default.Save();
                    break;
                case "3": Settings.Default.NotifyWhileProgress  = !Settings.Default.NotifyWhileProgress ;
                    Settings.Default.Save();
                    break;
                case "2": Application.Exit(); break;
                case "4":
                    startWithWindowsToolStripMenuItem.Checked = !startWithWindowsToolStripMenuItem.Checked;
                    StartupWithWindows = startWithWindowsToolStripMenuItem.Checked;
                    break;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
                showWindowToolStripMenuItem.PerformClick();
        }

        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            contextMenuStrip1.RightToLeft = RightToLeft.Yes;
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag + "")
            {
                case "1":
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                       textBox1.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.Default);
                    }
                    break;
                case "2":
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                         File.WriteAllText(saveFileDialog1.FileName,textBox1.Text, Encoding.Default);
                    }
                    break;
                case "3":
                    if (fontDialog1.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Font = fontDialog1.Font;
                    }
                    break;
                case "4": // Speech

                    TextToSpeech.TextToSpeechByGoole("مرحبا شباب", Language.Arabic);
                    //SpeechLib.SpVoiceClass sp = new SpeechLib.SpVoiceClass();
                    //sp.Speak(textBox1.Text, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                    break;
                  
            }
        }

        private void toolStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag + "")
            {
                case "1":
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.Default);
                    }
                    break;
                case "2":
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog1.FileName, textBox2.Text, Encoding.Default);
                    }
                    break;
                case "3":
                    if (fontDialog1.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Font = fontDialog1.Font;
                    }
                    break;
                case "4":
                    SpeechLib.SpVoiceClass sp = new SpeechLib.SpVoiceClass();
                    sp.Speak(textBox2.Text, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                    break;

            }
        }


    }
}
