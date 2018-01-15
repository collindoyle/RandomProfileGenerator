using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Text;
using System;

namespace RandomProfileGenerator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, List<string>> data;
        public const string datafile = @"data.csv";
        private Random rnd = new Random(System.DateTime.Now.Millisecond);
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            string localpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string datafilepath = localpath + "\\" + datafile;
            if (File.Exists(datafilepath))
            {
                StreamReader datareader = new StreamReader(datafilepath);
                string line;
                while((line = datareader.ReadLine()) != null)
                {
                    string[] linetags = line.Split('$');
                    switch (linetags[0])
                    {
                        case "t":
                            if (data == null)
                            {
                                data = new Dictionary<string, List<string>>();
                            }
                            if (!data.ContainsKey(linetags[1]))
                            {
                                data.Add(linetags[1], new List<string>());
                            }
                            break;
                        case "i":
                            if (!data.ContainsKey(linetags[1]))
                            {
                                data.Add(linetags[1], new List<string>());
                            }
                            data[linetags[1]].Add(linetags[2]);
                            break;
                        default:
                            break;
                    }
                }
                datareader.Close();
            }
            foreach (var item in data.Keys)
            {
                TabItem ti = new TabItem();
                ti.Header = item;
                tabControl.Items.Add(ti);
                TabPage tp = new TabPage();
                ti.Content = tp;
                tp.dg.ItemsSource = data[item];
               
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.textresult.Text = GenerateResultText(NameInput.Text);
            UpdateLayout();
        }

        private string GenerateResultText(string input)
        {
            string result = null;
            foreach (var item in tabControl.Items)
            {
                if ((item as TabItem).TabIndex == 0)
                {
                    continue;
                }
                TabPage tp= (item as TabItem).Content as TabPage;
                if (tp.dg.Items.Count == 0)
                {
                    continue;
                }
                int index = rnd.Next() % tp.dg.Items.Count;
                string tmp = tp.dg.Items[index].ToString();
                result = result + tmp;
            }

            return result;
        }

        private void WriteData()
        {
            string localpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string datafilepath = localpath + "\\" + datafile;
            if (File.Exists(datafilepath))
                File.Delete(datafilepath);
            StreamWriter streamWriter = new StreamWriter(datafilepath);
            foreach (var item in data.Keys)
            {
                string tmp = @"t$" + item;
                streamWriter.WriteLine(tmp);
            }
            foreach(var item in data.Keys)
            {
                foreach (var value in data[item])
                {
                    string tmp = @"i$" + item + "$" + value;
                    streamWriter.WriteLine(tmp);
                }
            }
            streamWriter.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteData();
        }
    }
}
