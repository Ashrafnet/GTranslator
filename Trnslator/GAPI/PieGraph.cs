using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for PieGraph
/// </summary>
public class PieGraph
{
    //private variables
    private DataTable dtGraphData =new DataTable();
    private string GTitle;
    private string GColors;
    private string GWidth;
    private string GHeight;
    private string GTitleColor;
    private string GTitleSize;
    //Set the properties 


    /// <summary>
    /// Grpah Title to be displayed above the Graph
    /// </summary> 
    public string GraphTitle
    {
        set { GTitle = value; }
    }

    /// <summary>
    /// DataSet  for Drawing the Graph Column:2 for Labels and Column:3 for Column Data
    /// </summary> 
    public DataTable dtData
    {
        set { dtGraphData = value;}
    }

    /// <summary>
    /// Colors to be displayed in the graph one or n number of colors eg: A333EE or A333EE,A311EE,1133EE 
    /// </summary> 
    public string GraphColors
    {
        set { GColors = value; }
    }

    /// <summary>
    /// Graph Dimesions width (2:1 -|- width:height)
    /// </summary> 
    public string GraphWidth
    {
        set { GWidth = value; }
    }
    /// <summary>
    /// Graph Dimesions Height (2:1 -|- width:height)
    /// </summary> 
    public string GraphHeight
    {
        set { GHeight = value; }
    }

    /// <summary>
    /// Graph Title Color (Title Font Color eg:5533AA)
    /// </summary> 
    public string GraphTitleColor
    {
        set { GTitleColor  = value; }
    }

    /// <summary>
    /// Graph Title Size (Title Font Size in pixcels 1-20)
    /// </summary>     
    public string GraphTitleSize
    {
        set { GTitleSize = value; }
    }



   
    //Method to find the max Data in the given Data
    public int GetMaxVal(DataTable dt)
    {
        int max = 0;
        int cval = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cval = Convert.ToInt32(dr[2]);
            if (cval >= max)
            {
                max = cval;
            }
        }
        return max;
    }

    
    /// <summary>
    /// Method which returns the Url be placed in the image src tag
    /// </summary>
    /// <returns>URL String </returns>
    public string GenerateGraph()
    {
        //Get the Data to draw the graph
        DataTable dt = new DataTable();
        dt = dtGraphData;

        //Get the max vals in the Data
        int maxval = GetMaxVal(dt);

        

        //SAMPLE: http://chart.apis.google.com/chart?cht=p3&chs=400x200&chd=s:asR&chl=A|B|C
        string GReqURL = "http://chart.apis.google.com/chart?chts="+GTitleColor+","+GTitleSize+"&chtt="+GTitle+"&chco="+GColors+"&cht=p3&chs="+GWidth+"x"+GHeight;

        //chts for Title Style : color,size
        //chtt for title text 
        //chco for colors: one or many


        string simpleEncoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string chartData = string.Empty;
        string chartLabels = string.Empty;
        StringBuilder strbchartData = new StringBuilder();
        StringBuilder strbchartLabels = new StringBuilder();

        strbchartData.Append("&chd=s:");
        strbchartLabels.Append("&chl=");

        int strlen = simpleEncoding.Length - 1;
        foreach (DataRow dr in dt.Rows)
        {
            //Generate the chart DATA
            int arrVal = Convert.ToInt32(dr[2]);
            int len = strlen * arrVal / maxval;
            strbchartData.Append(simpleEncoding.Substring(len, 1));

            //Generate the Chart Labels
            strbchartLabels.Append(dr[1] + "|");
        }

        //Converting the string builder to string
        chartData = strbchartData.ToString();
        chartLabels = strbchartLabels.ToString();

        chartLabels = chartLabels.Substring(0, chartLabels.Length - 1);
        return GReqURL + chartData + chartLabels;
    }
    
    //End of the method

}
