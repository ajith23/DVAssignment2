using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace WebApp
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //UtilityLocal.TransformFile(@"c:\users\avimalch\documents\visual studio 2015\Projects\AWAssignment1\WebApp\data\class_click.csv",
            //    @"c:\users\avimalch\documents\visual studio 2015\Projects\AWAssignment1\WebApp\data\class_click_transformed.csv");
            //UtilityLocal.CleanTransformedFile(@"c:\users\avimalch\documents\visual studio 2015\Projects\AWAssignment1\WebApp\data\class_click_transformed.csv", @"c:\users\avimalch\documents\visual studio 2015\Projects\AWAssignment1\WebApp\data\class_click_transformed_cleaned.csv");

            var dirPath = @"C:\Users\avimalch\DVAssignment2\WebApp\data";
            for (var year = 2003; year <= 2013; year++)
            {
                var flatNodes = FlatNode.ReadData(dirPath + @"\USOpenR.csv");
                Node.BuildNodes(null, flatNodes, year, year == 2005 ? 5 :7);
                var json = new JavaScriptSerializer().Serialize(Node.Root);
                //File.WriteAllText(dirPath+@"\"+year+".json", json);

                Testdiv.InnerHtml = json;
            }
            
            
            //printData(Node.Root);
        }

        void printData(Node node)
        {
            if (node.children.Count == 0)
                return;

            Testdiv.InnerHtml += "Round:" + node.round + " " + node.children[0].name + " vs " + node.children[1].name + " <br/>";
            //Console.WriteLine("Round:" + node.Round + " " + node.Children[0].Name + " vs " + node.Children[1].Name);
            printData(node.children[0]);
            printData(node.children[1]);
        }
    }
}