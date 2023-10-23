using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static STMRRPP_PT2.MainWindow;

namespace STMRRPP_PT2
{
    public partial class MainWindow : Window
    {
        public class Computer
        {
            public int _X { get; set; }
            public int _Y { get; set; }
            public string _Name { get; set; }

            public Computer() { }

            public Computer(Computer computer)
            {
                _X = computer._X;
                _Y = computer._Y;
                _Name = computer._Name;
            }

            public Computer(int x, int y, string name)
            {
                _X = x;
                _Y = y;
                _Name = name;
            }            

            public bool EqualPos(Computer[] array, int size)
            {
                for (int i = 0; i < size; i++)
                {
                    if (_X == array[i]._X && _Y == array[i]._Y)
                        return true;                    
                }
                return false;
            }

            public bool IsEqual(Computer compare)
            {
                if (_X == compare._X && _Y == compare._Y && _Name == compare._Name)
                    return true;
                return false;
            }
        }

        public class Pair : IComparable<Pair>
        {
            public Computer _C1 { get; set; }
            public Computer _C2 { get; set; }
            public int _Weight { get; set; }

            public Pair() { }

            public Pair(Pair pair)
            {
                _C1 = pair._C1;
                _C2 = pair._C2;
                _Weight = pair._Weight;
            }

            public Pair(Computer c1, Computer c2, int weight)
            {
                _C1 = c1;
                _C2 = c2;
                _Weight = weight;
            }
            public int CompareTo(Pair other)
            {
                if (other == null) 
                    return 1;
                return _Weight.CompareTo(other._Weight);
            }

            public bool IsEqual(Pair other)
            {
                if (_C1.IsEqual(other._C1) && _C2.IsEqual(other._C2) && _Weight == other._Weight)
                    return true;
                return false;
            }
        }
        public Pair[] PairSort(Pair[] pair, int size)
        {
            Pair[] sortedPair = pair;
            Pair tmp = new Pair();

            for (int i = 0; i < size - 1; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    if (sortedPair[i]._Weight > sortedPair[j]._Weight)
                    {
                        tmp = sortedPair[i];
                        sortedPair[i] = sortedPair[j];
                        sortedPair[j] = tmp;
                    }
                }
            }
            
            return sortedPair;
        }

        public class Set
        {
            public List<Pair> _Set;
            public List<String> _Vertexes;

            public Set(Pair pair)
            {
                _Set = new List<Pair>();
                _Set.Add(pair);

                _Vertexes = new List<string>();
                _Vertexes.Add(pair._C1._Name);
                _Vertexes.Add(pair._C2._Name);
            }

            public void Union(Set set, Pair edge)
            {
                foreach (Pair pair in set._Set)
                    _Set.Add(pair);
                _Vertexes.AddRange(set._Vertexes);
                _Set.Add(edge); 
            }

            public void AddEdge(Pair edge)
            {
                _Set.Add(edge);
                _Vertexes.Add(edge._C1._Name);
                _Vertexes.Add(edge._C2._Name);
            }

            public bool Contains(string vertex)
            {
                return _Vertexes.Contains(vertex);
            }
        }

        public class SystemOfDisjointSets
        {
            public List<Set> _Sets;
            public SystemOfDisjointSets()
            {
                _Sets = new List<Set>();
            }

            public void AddEdgeInSet(Pair edge)
            {
                Set setA = Find(edge._C1._Name);
                Set setB = Find(edge._C2._Name);

                if (setA != null && setB == null)
                {
                    setA.AddEdge(edge);
                }
                else if (setA == null && setB != null)
                {
                    setB.AddEdge(edge);
                }
                else if (setA == null && setB == null)
                {
                    Set set = new Set(edge);
                    _Sets.Add(set);
                }
                else if (setA != null && setB != null)
                {
                    if (setA != setB)
                    {
                        setA.Union(setB, edge);
                        _Sets.Remove(setB);
                    }
                }
            }

            public Set Find(string vertex)
            {
                foreach (Set set in _Sets)
                {
                    if (set.Contains(vertex)) 
                        return set;
                }
                return null;
            }
        }

        private void DrawVertex(Computer computer)
        {
            int size = 8;

            Ellipse vertex = new Ellipse();
            vertex.Width = size;
            vertex.Height = size;
            vertex.Fill = Brushes.LightCoral;            
            vertex.Stroke = Brushes.Black;
            vertex.StrokeThickness = 1;

            Canvas.SetLeft(vertex, computer._X * 10 + 1);
            Canvas.SetTop(vertex, computer._Y * 10 + 1);
            cnvs_Main.Children.Add(vertex);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = computer._Name;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = 12;

            Canvas.SetLeft(textBlock, computer._X * 10 + 10);
            Canvas.SetTop(textBlock, computer._Y * 10 - 5);
            cnvs_Main.Children.Add(textBlock);
        }

        private void DrawWeights(Pair pair, SolidColorBrush brush, double stroke)
        {
            Line weight = new Line();
            weight.X1 = pair._C1._X * 10 + 3;
            weight.Y1 = pair._C1._Y * 10 + 3;
            weight.X2 = pair._C2._X * 10 + 3;
            weight.Y2 = pair._C2._Y * 10 + 3;
            weight.StrokeThickness = stroke;
            weight.Stroke = brush;

            cnvs_Main.Children.Add (weight);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = pair._Weight.ToString();
            textBlock.FontSize = 10;

            Canvas.SetLeft(textBlock, 10 + (pair._C1._X * 10 + pair._C2._X * 10) / 2);
            Canvas.SetTop(textBlock, 10 + (pair._C1._Y * 10 + pair._C2._Y * 10) / 2);
            cnvs_Main.Children.Add(textBlock);
        }

        public MainWindow()
        {   
            InitializeComponent();
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            cnvs_Main.Children.Clear();

            Random rnd = new Random();
            //int vertexAmount = rnd.Next(10, 20);
            int vertexAmount = 5;
            Computer[] vertexes = new Computer[vertexAmount];

            vertexes[0] = new Computer(rnd.Next(0, 49), rnd.Next(0, 49), "1");

            int pairAmount = 0;

            for (int i = 1; i < vertexAmount; i++)
            {
                do
                {
                    vertexes[i] = new Computer(rnd.Next(0, 49), rnd.Next(0, 49), (i+1).ToString());
                } while (vertexes[i].EqualPos(vertexes, i));

                pairAmount += i;
            }

            int k = 0;
            Pair[] pairs = new Pair[pairAmount];
            
            for (int i = 0; i < vertexAmount - 1; i++)
            {
                for (int j = i + 1; j < vertexAmount; j++)
                {
                    pairs[k] = new Pair(vertexes[i], vertexes[j], rnd.Next(1, 10));
                    k++;
                }
            }

            if (cmb_Item_1.IsSelected == true)
            {
                //Связный граф
                for (int i = 0; i < pairAmount; i++)
                {
                    DrawWeights(pairs[i], Brushes.Black, 0.5);
                }
            }
            else
            {
                //Алгоритм Крускала
                Pair[] sortedPairs = new Pair[pairAmount];
                sortedPairs = PairSort(pairs, pairAmount);

                var disjointSets = new SystemOfDisjointSets();
                foreach (Pair edge in sortedPairs)
                {
                    disjointSets.AddEdgeInSet(edge);
                }

                var treeTemp = new List<Pair>();
                treeTemp = disjointSets._Sets.First()._Set;

                Pair[] tree = new Pair[vertexAmount - 1];

                for (int i = 0; i < vertexAmount - 1; i++)
                {
                    tree[i] = treeTemp[i];
                    DrawWeights(tree[i], Brushes.Black, 0.5);
                }
            }            

            for (int i = 0; i < vertexAmount; i++)
            {
                DrawVertex(vertexes[i]);
            }
        }
    }
}
