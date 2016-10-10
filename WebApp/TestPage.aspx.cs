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

            var flatNodes = FlatNode.ReadData(@"C:\Users\avimalch\DVAssignment2\WebApp\data\2013Data.csv");
            Node.BuildNodes(null, flatNodes);


            var temp = Node.Root;
            var json = new JavaScriptSerializer().Serialize(Node.Root);
            Testdiv.InnerHtml = json;
            //printData(temp);
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