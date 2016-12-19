using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Gapi.Core;

namespace Translator
{
    public partial class frmSMS : Form
    {
        public frmSMS()
        {
            InitializeComponent();
        }

        private void frmSMS_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.google.com/accounts/ServiceLogin?service=mail&passive=true&rm=false&continue=https://mail.google.com/mail/%3Fui%3Dhtml%26zy%3Dl&bsv=1eic6yu9oa4y3&ss=1&scc=1&ltmpl=default&ltmplcache=2&hl=en");
            
        }

        void SendSMS()
        {
            string Url = "https://mail.google.com/mail/channel/bind?VER=8&at=AF6bupNTnwsIiVX_Wy6-lVL4VgqyTKTuPA&it=59&SID=D55D05770A8D9504&RID=53566&AID=42&zx=zfryyp11n76w&t=1&count=2&ofs=25&req0__sc=c&req0_cmd=a&req0_jid=32ac8414894dd058%40contact.talk.google.com&req0_type=c&req1__sc=c&req1_chatstate=active&req1_iconset=goomoji&req1_id=D55D05770A8D9504_0&req1_smsnumber=0599594047&req1_text=helloman&req1_to=32ac8414894dd058%40contact.talk.google.com&req1_type=m";
           string yy= CoreHelper.PerformRequest(Url, GetCookieContainer());
            //webBrowser1.Navigate(Url);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GetCookieContainer();
            SendSMS();
        }

        public CookieContainer GetCookieContainer()
        {
            CookieContainer container = new CookieContainer();

            foreach (string cookie in webBrowser1.Document.Cookie.Split(';'))
            {
                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string path = "/";
                string domain = ".google.com"; //change to your domain name
                container.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
            }

            return container;
        }
    }
}
