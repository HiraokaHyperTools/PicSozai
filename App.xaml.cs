using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace PicSozai {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        public static string fpdb {
            get {
                return Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%\\PicSozai\\db.txt"));
            }
        }
    }
}
