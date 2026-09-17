using System.Drawing.Drawing2D;

namespace GameOfLife.UI
{
    public partial class RuleButtonRow : System.Windows.Forms.Control
    {

        private bool[] Active = new bool[9];

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string Rule {
            get {
                string ret = string.Empty;
                for (int i = 0; i <= 8; i++) {
                    if (Active[i]) {
                        ret += i.ToString();
                    }
                }
                return ret;
            }
            set {
                Active = new bool[9];
                foreach (char c in value) {
                    Active[int.Parse(c.ToString())] = true;
                }
                Invalidate();
            }
        }

        //Draw
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;


            var rect = new Rectangle(0, 0, 20, 20);


            for (int i = 0; i <= 8; i++) {
                //The background (depending on status)
                Pen p;
                Brush brsh;
                if (!Rule.Contains(i.ToString())) {
                    brsh = new SolidBrush(Color.LightBlue);
                    p = Pens.DarkGray;
                }
                else {
                    brsh = new SolidBrush(Color.Blue);
                    p = Pens.Black;
                }
                e.Graphics.FillRectangle(brsh, rect);
                e.Graphics.DrawRectangle(p, rect);


                //Text
                var sz = new SizeF(e.Graphics.MeasureString(i.ToString(), Font));
                e.Graphics.DrawString(i.ToString(), Font, Brushes.White, rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2);

                rect.Offset(rect.Width + 4, 0);
            }
        }


        //Handle clicks
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int x = e.X;

            if (x % (20 + 4) <= 20) {
                int clickedbutton = x / (20 + 4);

                Active[clickedbutton] = !Active[clickedbutton];
                Invalidate(new Rectangle(clickedbutton * 24, 0, 20, 20));
            }
        }

        //Disallow resizes
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Size != new System.Drawing.Size(213, 22)) {
                Size = new System.Drawing.Size(213, 22);
            }
        }

    }
}
