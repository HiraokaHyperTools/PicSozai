using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PicSozai {
    class UtSplit {
        public static IEnumerable<Bitmap> Split(String fp) {
            using (Bitmap pic = new Bitmap(fp)) {
                int cy = pic.Height, cx = pic.Width;
                bool[] scanned = new bool[cx * cy];
                Color bk = pic.GetPixel(0, 0);
                for (int y = 0; y < cy; y++) {
                    for (int x = 0; x < cx; x++) {
                        if (scanned[x + cx * y]) continue;
                        var clr = pic.GetPixel(x, y);
                        if (clr == bk) continue;
                        Bitmap p1 = Scan(pic, x, y, cx, cy, scanned, bk);
                        if (p1 != null) yield return p1;
                    }
                }
            }
        }

        private static Bitmap Scan(Bitmap pic, int starx, int starty, int cx, int cy, bool[] scanned, Color bk) {
            int x0 = starx, x1 = starx, y0 = starty, y1 = starty;
            Stack<Point> pts = new Stack<Point>();
            pts.Push(new Point(starx, starty));
            while (pts.Count != 0) {
                var pt = pts.Pop();
                var fg = pic.GetPixel(pt.X, pt.Y);
                if (fg == bk) continue;
                x0 = Math.Min(x0, pt.X); x1 = Math.Max(x1, pt.X);
                y0 = Math.Min(y0, pt.Y); y1 = Math.Max(y1, pt.Y);

                if (CanMove2(pt.X - 1, pt.Y, cx, cy, scanned)) { pts.Push(new Point(pt.X - 1, pt.Y)); }
                if (CanMove2(pt.X + 1, pt.Y, cx, cy, scanned)) { pts.Push(new Point(pt.X + 1, pt.Y)); }
                if (CanMove2(pt.X, pt.Y - 1, cx, cy, scanned)) { pts.Push(new Point(pt.X, pt.Y - 1)); }
                if (CanMove2(pt.X, pt.Y + 1, cx, cy, scanned)) { pts.Push(new Point(pt.X, pt.Y + 1)); }
            }

            int px = x1 - x0 + 1, py = y1 - y0 + 1;
            if (px < 2 || py < 2) return null;
            Bitmap p2 = new Bitmap(px, py);
            for (int a = 0; a < px; a++) {
                for (int b = 0; b < py; b++) {
                    p2.SetPixel(a, b, pic.GetPixel(x0 + a, y0 + b));
                }
            }
            return p2;
        }

        private static bool CanMove2(int x, int y, int cx, int cy, bool[] scanned) {
            if (x < 0 || y < 0 || x >= cx || y >= cy)
                return false;
            if (scanned[x + cx * y])
                return false;
            scanned[x + cx * y] |= true;
            return true;
        }
    }
}
