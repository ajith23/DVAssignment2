using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApp
{
    class Node
    {
        public Node()
        {
            children = new List<Node>();
        }
        public string name { get; set; }
        public int round { get; set; }
        public List<Node> children { get; set; }

        public string matchD { get { return new JavaScriptSerializer().Serialize(Match); } }
        MatchData Match { get; set; }
        DonutData Donut { get; set; }
        public string donutD { get { return new JavaScriptSerializer().Serialize(Donut); } }
        public static Node Root { get; set; }
        public static void BuildNodes(Node node, List<FlatNode> flatList, int year, int maxRounds)
        {
            if (node == null) //root node
            {
                var tempList = flatList.Where(n => n.Round == maxRounds && n.Year == year).ToList();
                Root = new Node() { name = tempList[0].WinnerName, round = maxRounds+1 };
                node = Root;
            }
            if (node.round == 1)
                return;
            var roundList = flatList.Where(n=>n.WinnerName == node.name && n.Round == node.round-1 && n.Year== year).ToList();
            if (roundList.Count == 0)
                return;
            node.children.Add(new Node() {name = roundList[0].WinnerName, round = roundList[0].Round });
            node.children.Add(new Node() {name = roundList[0].LoserName, round = roundList[0].Round });
            node.Match = roundList[0].Match;
            node.Donut = roundList[0].Donut;
            BuildNodes(node.children[0], flatList, year, maxRounds);
            BuildNodes(node.children[1], flatList, year, maxRounds);
        }
    }

    public class MatchData
    {
        public MatchData(int[] w, int[] l)
        {
            WinnerData = w;
            LoserData = l;
        }
        public string[] XAxis = new string[] { "FirstServe", "FirstPointWon", "SecondPointWon", "Break", "Return", "Net" };
        public int[] WinnerData { get; set; }
        public int[] LoserData { get; set; }

    }

    public class DonutData
    {
        public DonutData(int a1, int a2, int t1, int t2, int w1, int w2, int e1, int e2)
        {
            Ace1 = a1;
            Ace2 = a2;
            TotalPoints1 = t1;
            TotalPoints2 = t2;
            Winner1 = w1;
            Winner2 = w2;
            Error1 = e1;
            Error2 = e2;
        }
        public int Ace1 { get; set; }
        public int Ace2 { get; set; }
        public int TotalPoints1 { get; set; }
        public int TotalPoints2 { get; set; }
        public int Winner1 { get; set; }
        public int Winner2 { get; set; }
        public int Error1 { get; set; }
        public int Error2 { get; set; }
    }

    public class FlatNode
    {
        public string WinnerName { get; set; }
        public string LoserName { get; set; }
        public int Year { get; set; }
        public int Id { get; set; }
        public int Round { get; set; }
        public MatchData Match { get; set; }
        public DonutData Donut { get; set; }
        /*0-id,1-year,2-gender,3-tid,4-mid,5-player1,6-player2,7-country1,8-country2,9-round,
         10- firstServe1,11 -firstServe2,12- ace1, 13- ace2, 14- double1, 15- double2, 16- firstPointWon1, 17- firstPointWon2,
         18- secPointWon1, 19-secPointWon2, 20-fastServe1, 21-fastServe2, 22-avgFirstServe1, 23-avgFirstServe2, 24-avgSecServe1, 25-avgSecServe2,
         26-break1, 27-break2, 28-return1, 29-return2, 30-total1, 31-total2,
         32-winner1, 33-winner2, 34-error1, 35-error2, 36- net1, 37-net2*/
        public static List<FlatNode> ReadData(string path)
        {
            var result = new List<FlatNode>();
            var lines = System.IO.File.ReadAllLines(path);

            for (var i = 1; i < lines.Length; i++)
            {
                var d = lines[i].Split(',');
                var temp = new FlatNode
                {
                    Id = Convert.ToInt32(d[0]),
                    Year = Convert.ToInt32(d[1]),
                    WinnerName = d[5],
                    LoserName = d[6],
                    Round = Convert.ToInt32(d[9]),
                    Match = new MatchData(
                            Array.ConvertAll(new string[] { "0" + d[10].Trim('%'), "0" + d[16].Trim('%'), "0" + d[18].Trim('%'), "0" + d[26].Trim('%'), "0" + d[28].Trim('%'), "0" + d[36].Trim('%') }, int.Parse),
                            Array.ConvertAll(new string[] { "0" + d[11].Trim('%'), "0" + d[17].Trim('%'), "0" + d[19].Trim('%'), "0" + d[27].Trim('%'), "0" + d[29].Trim('%'), "0" + d[37].Trim('%') }, int.Parse)),
                    Donut = new DonutData(Convert.ToInt32("0" + d[12]), Convert.ToInt32("0" + d[13]), Convert.ToInt32("0" + d[30]), Convert.ToInt32("0" + d[31]),
                                          Convert.ToInt32("0" + d[32]), Convert.ToInt32("0" + d[33]), Convert.ToInt32("0" + d[34]), Convert.ToInt32("0" + d[35]))
                };

                result.Add(temp);
            }

            return result;
        }
    }


}
