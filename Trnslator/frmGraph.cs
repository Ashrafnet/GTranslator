using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Translator
{
    public partial class frmGraph : Form
    {
        public frmGraph()
        {
            InitializeComponent();
        }

        private void frmGraph_Load(object sender, EventArgs e)
        {
            
            DataTable dt = new DataTable();
            dt.Columns.Add("column1");
            dt.Columns.Add("column2");
            dt.Columns.Add("column3");

            dt.Rows.Add(1, "Name1",6);
            dt.Rows.Add(2, "Name2",4);
            dt.Rows.Add(3, "Name3",6);
            PieGraph pg = new PieGraph();
            pg.GraphColors = "5566AA";
            pg.GraphHeight = "125";
            pg.GraphWidth = "250";
            pg.GraphTitle = "Welcome to Test Graph";
            pg.GraphTitleColor = "112233";
            pg.GraphTitleSize = "20";
            pg.dtData = dt;

            pictureBox1.ImageLocation= pg.GenerateGraph();  
            

            
          //  p.dtData = dt;
        }
    }
}
