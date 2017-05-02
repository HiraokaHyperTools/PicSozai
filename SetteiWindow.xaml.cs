using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PicSozai.Properties;
using System.Diagnostics;
using System.IO;

namespace PicSozai {
    /// <summary>
    /// SetteiWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SetteiWindow : Window {
        public SetteiWindow() {
            InitializeComponent();

            DataContext = Settings.Default;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

        }

        private void bSave_Click(object sender, RoutedEventArgs e) {
            Settings.Default.Save();

            if (MessageBox.Show("高速検索用のデータベースを更新?", Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                UpdateDbWindow win = new UpdateDbWindow();
                win.Show();
            }
        }

        private void bMS_Click(object sender, RoutedEventArgs e) {
            Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=35825");
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Process.Start("" + ((Button)sender).Content);

        }

        String Pics { get { return Environment.ExpandEnvironmentVariables("%APPDATA%\\PicSozai\\Pics"); } }

        private void bVS2005_Click(object sender, RoutedEventArgs e) {
            // C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\VS2005ImageLibrary
            {
                String dir = Environment.GetEnvironmentVariable("VS80COMNTOOLS");
                if (dir != null && Directory.Exists(dir)) {
                    dir = Path.GetFullPath(dir + "\\..\\VS2005ImageLibrary");
                    if (dir != null && Directory.Exists(dir)) {
                        Process.Start(dir);
                    }
                }
            }
        }

        private void bNear_Click(object sender, RoutedEventArgs e) {
            SortedDictionary<string, string> d = new SortedDictionary<string, string>();
            foreach (string k in Settings.Default.Dirs.Split('\n')) d[k] = null;
            d[Pics] = null;
            d.Remove("");
            Directory.CreateDirectory(Pics);
            Settings.Default.Dirs = String.Join("\n", d.Keys.ToArray());
            Process.Start(Pics);
        }
    }
}
