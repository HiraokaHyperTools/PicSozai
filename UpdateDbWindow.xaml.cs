using PicSozai.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PicSozai {
    /// <summary>
    /// UpdateDbWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateDbWindow : Window {
        public UpdateDbWindow() {
            InitializeComponent();

            bwUpdater.DoWork += bwUpdater_DoWork;
            bwUpdater.RunWorkerCompleted += bwUpdater_RunWorkerCompleted;
            bwUpdater.WorkerSupportsCancellation = true;
            bwUpdater.RunWorkerAsync();
        }

        void bwUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error == null) {
                l.Content = "完了。";
            }
            else {
                l.Foreground = Brushes.Red;
                l.Content = "失敗しました: " + e.Error;
            }
        }

        void bwUpdater_DoWork(object sender, DoWorkEventArgs e) {
            List<string> Dirs = new List<string>();

            foreach (String k in Settings.Default.Dirs.Split('\n')) {
                if (String.IsNullOrEmpty(k)) continue;
                if (Directory.Exists(k)) {
                    Dirs.AddRange(Directory.GetDirectories(k, "*", SearchOption.AllDirectories));
                }
            }

            StringWriter writer = new StringWriter();
            foreach (String dir in Dirs) {
                writer.WriteLine("@" + dir);
                foreach (var fi in new DirectoryInfo(dir).GetFiles()) {
                    writer.WriteLine("+" + fi.Name);
                }
            }

            File.WriteAllText(App.fpdb, writer.ToString());
        }

        BackgroundWorker bwUpdater = new BackgroundWorker();

        private void Window_Closing(object sender, CancelEventArgs e) {
            if (e.Cancel = bwUpdater.IsBusy) {
                bwUpdater.CancelAsync();
            }
        }
    }
}
