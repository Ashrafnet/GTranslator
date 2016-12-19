using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Translator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 xx=new Form1();
            //xx.Visible = false;
            
            Application.Run(xx);
        }
    }
}
