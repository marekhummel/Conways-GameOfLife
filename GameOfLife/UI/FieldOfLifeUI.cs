using System.Drawing.Drawing2D;

namespace GameOfLife.UI
{
    public class FieldOfLifeUI : System.Windows.Forms.Panel
    {
        public int gridoffset;

        public FieldOfLifeUI()
        {
            DoubleBuffered = true;

            //The properties
            Attributes.AmountOfCellsPerLine = 32;

            BackgroundColor = Color.FromArgb(40, 40, 40);
            GridColor = Color.FromArgb(75, 75, 75);
            LivingCellColor = Color.LightBlue;
            HoverColor = Color.FromArgb(127, Color.Orange);

            CellStyle = FieldOfLifeUI.CellStyles.Square;
            CellFigureDistance = 1;
            GridStyle = FieldOfLifeUI.GridStyles.Continuous;

            InputEnabled = true;
        }


        public int SizePerCell => (Width - 1) / Attributes.AmountOfCellsPerLine;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool InputEnabled { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public GridStyles GridStyle { get; set; }
        public enum GridStyles
        {
            Continuous,
            Dotted,
            BrokenLines,
            OnlyEdges,
            Empty
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public CellStyles CellStyle { get; set; }
        public enum CellStyles
        {
            Square,
            Circle
        }
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int CellFigureDistance { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color BackgroundColor { get; set; }
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color GridColor { get; set; }
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color LivingCellColor { get; set; }
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color HoverColor { get; set; }

        public InputModes InputMode;
        public enum InputModes
        {
            Toggle,
            Activated,
            Deactivated
        }


        //***********************************

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawOnGfx(e.Graphics);
        }

        public void DrawOnGfx(Graphics egfx) //ref
        {
            egfx.CompositingQuality = CompositingQuality.HighQuality;

            //Draw the grid
            gridoffset = (Width - (Attributes.AmountOfCellsPerLine * SizePerCell + 1)) / 2;
            int realsize = Attributes.AmountOfCellsPerLine * SizePerCell + 1;

            var gridtexture = new Bitmap(SizePerCell, SizePerCell);
            Pen p;
            using (var gfx = Graphics.FromImage(gridtexture)) {
                gfx.Clear(BackgroundColor);
                switch (GridStyle) {
                    case (FieldOfLifeUI.GridStyles.Continuous):
                        gfx.DrawRectangle(new Pen(GridColor), 0, 0, SizePerCell, SizePerCell);
                        break;
                    case (FieldOfLifeUI.GridStyles.BrokenLines):
                        float[] dp = { SizePerCell / 4, SizePerCell / 4, SizePerCell / 8, SizePerCell / 4, SizePerCell / 8 };
                        p = new Pen(GridColor) { DashPattern = dp };
                        gfx.DrawRectangle(p, 0, 0, SizePerCell, SizePerCell);
                        break;
                    case (FieldOfLifeUI.GridStyles.Dotted):
                        p = new Pen(GridColor) { DashPattern = new float[] { 1, 1 } };
                        break;
                    case (FieldOfLifeUI.GridStyles.OnlyEdges):
                        gfx.DrawRectangle(new Pen(GridColor), 0, 0, 1, 1);
                        gfx.DrawRectangle(new Pen(GridColor), 0, SizePerCell, 1, 1);
                        gfx.DrawRectangle(new Pen(GridColor), SizePerCell, 0, 1, 1);
                        gfx.DrawRectangle(new Pen(GridColor), SizePerCell, SizePerCell, 1, 1);
                        break;
                    case (FieldOfLifeUI.GridStyles.Empty):
                        //Nothing
                        break;
                }
            }

            //Draw the background
            var tb = new TextureBrush(gridtexture);
            tb.TranslateTransform(gridoffset, gridoffset);

            egfx.FillRectangle(new SolidBrush(BackgroundColor), 0, 0, Width, Height);
            if (GridStyle == GridStyles.OnlyEdges) {
                egfx.FillRectangle(tb, gridoffset, gridoffset, realsize + 1, realsize + 1);
            }
            else {
                egfx.FillRectangle(tb, gridoffset, gridoffset, realsize, realsize);
            }


            egfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
            egfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            egfx.SmoothingMode = SmoothingMode.HighQuality;

            //Draw the (living) cells
            if (Attributes.Cells != null) {
                for (int y = 0; y <= Attributes.Cells.GetLength(0) - 1; y++) {
                    for (int x = 0; x <= Attributes.Cells.GetLength(0) - 1; x++) {
                        if (Attributes.IsCellAlive(x, y)) {
                            if (CellStyle == CellStyles.Circle) {
                                egfx.FillEllipse(new SolidBrush(LivingCellColor), IndexToRect(x, y, CellFigureDistance));
                            }
                            else if (CellStyle == CellStyles.Square) {
                                egfx.FillRectangle(new SolidBrush(LivingCellColor), IndexToRect(x, y, CellFigureDistance));
                            }
                        }
                    }
                }
            }

            //Draw the hover sign
            if (IsMouseInField(ActualMouseLocation)) {
                if (CellStyle == CellStyles.Circle) {
                    egfx.FillEllipse(new SolidBrush(HoverColor), IndexToRect(PointToIndex(ActualMouseLocation), CellFigureDistance));
                }
                else if (CellStyle == CellStyles.Square) {
                    egfx.FillRectangle(new SolidBrush(HoverColor), IndexToRect(PointToIndex(ActualMouseLocation), CellFigureDistance));
                }
            }

        }



        // Activate + Deactivate cells

        private Point ActualMouseLocation = new Point(-10, -10);
        private Point LastMouseLocation = new Point(-10, -10);
        private Point LastMouseClickLocation = new Point(-10, -10);
        private bool IsMouseDown;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsMouseDown && IsMouseInField(e.Location)) {
                ChangeCell(e);
                LastMouseClickLocation = e.Location;
            }

            LastMouseLocation = ActualMouseLocation;
            ActualMouseLocation = e.Location;
            Invalidate(IndexToRect(PointToIndex(ActualMouseLocation), 0));
            Invalidate(IndexToRect(PointToIndex(LastMouseLocation), 0));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsMouseInField(e.Location)) {
                ChangeCell(e);
            }

            IsMouseDown = true;
            LastMouseClickLocation = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            IsMouseDown = false;
            LastMouseClickLocation = new Point(-10, -10);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Invalidate(IndexToRect(PointToIndex(ActualMouseLocation), 0));
            Invalidate(IndexToRect(PointToIndex(LastMouseLocation), 0));
            ActualMouseLocation = new Point(-10, -10);

        }




        // Methods

        public void ChangeCell(MouseEventArgs e)
        {
            if (InputEnabled) {
                var loc = e.Location;
                if ((PointToIndex(loc).Item1 != PointToIndex(LastMouseClickLocation).Item1) ||
                    (PointToIndex(loc).Item2 != PointToIndex(LastMouseClickLocation).Item2)) {
                    int index1 = PointToIndex(loc).Item1;
                    int index2 = PointToIndex(loc).Item2;

                    switch (InputMode) {
                        case InputModes.Toggle:
                            Attributes.SetCellLife(index1, index2, !Attributes.IsCellAlive(index1, index2));
                            break;
                        case InputModes.Activated:
                            Attributes.SetCellLife(index1, index2, true);
                            break;
                        case InputModes.Deactivated:
                            Attributes.SetCellLife(index1, index2, false);
                            break;
                    }

                    Invalidate(IndexToRect(PointToIndex(loc), 2));
                }
            }
        }


        private Point GetRealLocation(Point loc) => new Point(loc.X - gridoffset, loc.Y - gridoffset);

        private Tuple<int, int> PointToIndex(Point pt)
        {
            if (IsMouseInField(pt)) {
                var realloc = GetRealLocation(pt);

                int i1 = realloc.X / SizePerCell;
                int i2 = realloc.Y / SizePerCell;

                return new Tuple<int, int>(i1, i2);
            }
            else {
                return new Tuple<int, int>(-1, -1);
            }
        }

        private Rectangle IndexToRect(Tuple<int, int> index, int offset) => IndexToRect(index.Item1, index.Item2, offset);
        private Rectangle IndexToRect(int i1, int i2, int offset) => new Rectangle(i1 * SizePerCell + offset + gridoffset, i2 * SizePerCell + offset + gridoffset, SizePerCell - 2 * offset + 1, SizePerCell - 2 * offset + 1);


        private bool IsMouseInField(Point pt) => new Rectangle(gridoffset, gridoffset, Width - 2 * gridoffset - 1, Height - 2 * gridoffset - 1).IntersectsWith(new Rectangle(pt, new Size(0, 0)));

    }
}
