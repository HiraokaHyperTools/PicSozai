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
using System.Windows.Navigation;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using PicSozai.Properties;

namespace PicSozai {
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();

            bwS.DoWork += new DoWorkEventHandler(bwS_DoWork);
            bwS.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwS_RunWorkerCompleted);
        }

        void bwS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            wpRes.IsEnabled = true;
            pbWIP.Visibility = Visibility.Hidden;
        }

        BackgroundWorker bwS = new BackgroundWorker();

        bool fastSearch;

        private void bSearch_Click(object sender, RoutedEventArgs e) {
            if (bwS.IsBusy) return;

            wpRes.Children.Clear();
            wpRes.IsEnabled = false;
            pbWIP.Visibility = Visibility.Visible;

            fastSearch = object.ReferenceEquals(sender, bFastSearch);

            bwS.RunWorkerAsync(tbKws.Text);
        }

        void bwS_DoWork(object sender, DoWorkEventArgs e) {
            String kws = Convert.ToString(e.Argument);

            if (fastSearch) {
                String dir = null;
                foreach (String row in File.ReadAllLines(App.fpdb)) {
                    String fp;
                    if (row.StartsWith("@")) {
                        dir = row.Substring(1);
                        continue;
                    }
                    else if (row.StartsWith("+")) {
                        fp = dir + "\\" + row.Substring(1);
                    }
                    else {
                        continue;
                    }
                    int lastPeriod = fp.LastIndexOf('.');
                    if (lastPeriod < 0) {
                        continue;
                    }
                    var fext = fp.Substring(lastPeriod);
                    if (fext == ".png" || fext == ".gif" || fext == ".ico") {
                        var fi = new FileInfo(fp);
                        if (fi.Exists) {
                            String fn = fi.Name;
                            if (MUt.Match(fn, kws)) {
                                PicFound(fi);
                            }
                        }
                    }
                }
            }
            else {
                List<string> Dirs = new List<string>();

                foreach (String k in Settings.Default.Dirs.Split('\n')) {
                    if (String.IsNullOrEmpty(k)) continue;
                    if (Directory.Exists(k)) {
                        Dirs.AddRange(Directory.GetDirectories(k, "*", SearchOption.AllDirectories));
                    }
                }

                foreach (String dir in Dirs) {
                    foreach (var fi in new DirectoryInfo(dir).GetFiles()) {
                        String fext = fi.Extension.ToLowerInvariant();
                        if (fext == ".png" || fext == ".gif" || fext == ".ico") {
                            String fn = fi.Name;
                            if (MUt.Match(fn, kws)) {
                                PicFound(fi);
                            }
                        }
                    }
                }
            }
        }

        private void PicFound(FileInfo fi) {
            var fext = fi.Extension;
            var fp = fi.FullName;
            var src = new BitmapImage(new Uri(fp));
            src.Freeze();
            wpRes.Dispatcher.Invoke((Action)(() => {
                Image i = new Image();
                i.Width = (src.PixelWidth);
                i.Height = (src.PixelHeight);
                i.Stretch = Stretch.Uniform;
                i.HorizontalAlignment = HorizontalAlignment.Center;
                i.VerticalAlignment = VerticalAlignment.Center;
                i.Source = src;
                i.Tag = fp;
                i.MaxHeight = 64;
                i.MaxWidth = 64;
                i.MouseDown += new MouseButtonEventHandler(i_MouseDown);
                i.ContextMenu = FindResource("cms1") as ContextMenu;
                Grid g = new Grid();
                g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(64) });
                g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                g.Children.Add(i);
                {
                    TextBlock l = new TextBlock();
                    l.Text = String.Format("{0}{1}", src.PixelWidth, fext);
                    l.HorizontalAlignment = HorizontalAlignment.Right;
                    l.VerticalAlignment = VerticalAlignment.Bottom;
                    l.FontSize = 9;
                    l.Margin = new Thickness(0, 0, 2, 0);
                    l.Foreground = Brushes.YellowGreen;
                    l.SetValue(Grid.RowProperty, 1);
                    g.Children.Add(l);
                }
                Border b = new Border();
                b.Width = 64;
                b.Height = 80;
                b.Child = g;
                b.BorderBrush = Brushes.Aqua;
                b.BorderThickness = new Thickness(0.25);
                wpRes.Children.Add(b);
            }), null);
        }

        Image lasti = null;

        void i_MouseDown(object sender, MouseButtonEventArgs e) {
            lasti = (Image)sender;
            if (e.ChangedButton == MouseButton.Left) {
                Process.Start("explorer.exe", " /select,\"" + ((Image)sender).Tag + "\"");
            }
            else if (e.ChangedButton == MouseButton.Right) {

            }
        }

        class MUt {
            internal static bool Match(string fn, string kws) {
                fn = fn.ToLowerInvariant();
                kws = kws.ToLowerInvariant();
                foreach (string kw in Regex.Split(kws, "\\s+")) {
                    if (fn.IndexOf(kw.Trim()) < 0)
                        return false;
                }
                return true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            tbKws.Focus();
        }

        private void tbKws_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                e.Handled = true;
                bSearch_Click((0 != (Keyboard.Modifiers & ModifierKeys.Shift)) ? bFastSearch : bSearch, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Process.Start("" + ((Button)sender).Content);
        }

        private void bSettei_Click(object sender, RoutedEventArgs e) {
            SetteiWindow form = new SetteiWindow();
            form.Show();
        }

        private void mOpenfp_Click(object sender, RoutedEventArgs e) {
            Process.Start(lasti.Tag + "");
        }

        private void mCopyfp_Click(object sender, RoutedEventArgs e) {
            try {
                Clipboard.SetText(lasti.Tag + "");
                MessageBox.Show("パスをコピーした。");
            }
            catch (Exception err) {
                MessageBox.Show("失敗：" + err.Message);
            }
        }

        private void mOpendir_Click(object sender, RoutedEventArgs e) {
            Process.Start("explorer.exe", " /select,\"" + lasti.Tag + "\"");
        }

        private void mExplode_Click(object sender, RoutedEventArgs e) {
            String fp = lasti.Tag + "";

            String dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sender2, e2) => {
                foreach (var res in EIUt.LoadIcos(new String[] { fp })) {
                    res.pic.Save(Path.Combine(dir, res.fn + ".png"));
                }
            };
            bw.RunWorkerCompleted += (sender2, e2) => {
                if (e2.Error != null)
                    MessageBox.Show("エラー\n\n" + e2.Error);
                Process.Start(dir);
            };
            bw.RunWorkerAsync();
        }

        private void mSplit_Click(object sender, RoutedEventArgs e) {
            String fp = lasti.Tag + "";

            String dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sender2, e2) => {
                int n = 1;
                foreach (var pic in UtSplit.Split(fp)) {
                    pic.Save(Path.Combine(dir, n + ".png"));
                    n++;
                }
            };
            bw.RunWorkerCompleted += (sender2, e2) => {
                if (e2.Error != null)
                    MessageBox.Show("エラー\n\n" + e2.Error);
                Process.Start(dir);
            };
            bw.RunWorkerAsync();
        }
    }
}
