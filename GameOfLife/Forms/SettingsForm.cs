namespace GameOfLife.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            AcceptButton = btn_Save;
            CancelButton = btn_Cancel;
            btn_Save.DialogResult = DialogResult.OK;
            btn_Cancel.DialogResult = DialogResult.Cancel;
        }

        private UISettingsFile? uisetts;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public UISettingsFile? UISettings {
            get => uisetts;
            set {
                uisetts = value;

                gridstyle = uisetts.GridStyle;
                cellstyle = uisetts.CellStyle;
                distance = uisetts.Distance;
                backgroundcolor = uisetts.BackgroundColor;
                gridcolor = uisetts.GridColor;
                livingcolor = uisetts.LivingColor;
                hovercolor = uisetts.HoverColor;

                if (UISettingsChanged != null) {
                    UISettingsChanged();
                }
            }
        }
        public delegate void EmptyEventHandler();
        public event EmptyEventHandler? UISettingsChanged;


        //Temp variables
        private int fieldsize;
        private UI.FieldOfLifeUI.GridStyles gridstyle;
        private UI.FieldOfLifeUI.CellStyles cellstyle;
        private int distance;
        private Color backgroundcolor;
        private Color gridcolor;
        private Color livingcolor;
        private Color hovercolor;
        private int refreshrate;
        private bool torusactive;

        private string? ruletoborn;
        private string? ruletosurvive;



        //------------------



        //Load
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            //AmountOfCellsPerLine
            fieldsize = Attributes.AmountOfCellsPerLine;
            tb_fieldsize.Value = fieldsize;
            lbl_fieldsizevalue.Text = tb_fieldsize.Value.ToString();

            //Gridstyle
            cb_gridstyle.DataSource = System.Enum.GetValues(typeof(UI.FieldOfLifeUI.GridStyles));
            cb_gridstyle.Text = UISettings.GridStyle.ToString();

            //cellstyle
            cb_cellstyle.DataSource = System.Enum.GetValues(typeof(UI.FieldOfLifeUI.CellStyles));
            cb_cellstyle.Text = UISettings.CellStyle.ToString();

            //Distance
            nud_distance.Minimum = 0;
            nud_distance.Maximum = UISettings.SizePerCell / 2;
            nud_distance.Value = Math.Min(UISettings.Distance, nud_distance.Maximum);

            //Torus
            torusactive = Attributes.TorusActivated;
            cb_torus.Checked = torusactive;

            //Colors
            btn_backcolor.BackColor = UISettings.BackgroundColor;
            btn_gridcolor.BackColor = UISettings.GridColor;
            btn_livingcolor.BackColor = UISettings.LivingColor;
            btn_hovercolor.BackColor = UISettings.HoverColor;

            //Refreshrate
            refreshrate = Attributes.Refreshrate;
            nud_refreshrate.Minimum = 10;
            nud_refreshrate.Maximum = 2500;
            nud_refreshrate.Value = Attributes.Refreshrate; //Not refreshrate, cause the valuechanged event gets fired two lines before

            //Rules
            ruletoborn = Attributes.RuleToBeBorn;
            ruletosurvive = Attributes.RuleToSurvive;
            btnrow_survive.Rule = ruletosurvive;
            btnrow_born.Rule = ruletoborn;

            CheckRules();
        }

        //Quit
        private void btn_Save_Click(object sender, EventArgs e)
        {
            Attributes.AmountOfCellsPerLine = fieldsize;
            UISettings.GridStyle = gridstyle;
            UISettings.CellStyle = cellstyle;
            UISettings.Distance = distance;
            UISettings.BackgroundColor = backgroundcolor;
            UISettings.GridColor = gridcolor;
            UISettings.LivingColor = livingcolor;
            UISettings.HoverColor = hovercolor;
            Attributes.TorusActivated = torusactive;
            Attributes.Refreshrate = refreshrate;
            Attributes.RuleToBeBorn = ruletoborn ?? "";
            Attributes.RuleToSurvive = ruletosurvive ?? "";
            UISettingsChanged?.Invoke();
        }



        //Chance settings
        private void cb_gridstyle_SelectedIndexChanged(object sender, EventArgs e) => gridstyle = (UI.FieldOfLifeUI.GridStyles)System.Enum.Parse(typeof(UI.FieldOfLifeUI.GridStyles), cb_gridstyle.Text);

        private void cb_cellstyle_SelectedIndexChanged(object sender, EventArgs e) => cellstyle = (UI.FieldOfLifeUI.CellStyles)System.Enum.Parse(typeof(UI.FieldOfLifeUI.CellStyles), cb_cellstyle.Text);

        private void nud_distance_ValueChanged(object sender, EventArgs e) => distance = (int)nud_distance.Value;

        private void btn_backcolor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog {
                Color = backgroundcolor,
                FullOpen = true
            };


            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                backgroundcolor = cd.Color;
            }

            btn_backcolor.BackColor = backgroundcolor;
        }

        private void btn_gridcolor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog {
                Color = gridcolor,
                FullOpen = true
            };

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                gridcolor = cd.Color;
            }

            btn_gridcolor.BackColor = gridcolor;
        }

        private void btn_livingcolor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog {
                Color = livingcolor,
                FullOpen = true
            };

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                livingcolor = cd.Color;
            }

            btn_livingcolor.BackColor = livingcolor;
        }

        private void btn_hovercolor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog {
                Color = hovercolor,
                FullOpen = true
            };

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                hovercolor = cd.Color;
            }

            btn_hovercolor.BackColor = hovercolor;
        }

        private void nud_refreshrate_ValueChanged(object sender, EventArgs e) => refreshrate = (int)nud_refreshrate.Value;

        private void tb_fieldsize_ValueChanged(object sender, EventArgs e)
        {
            fieldsize = tb_fieldsize.Value;
            lbl_fieldsizevalue.Text = tb_fieldsize.Value.ToString();

            nud_distance.Maximum = (Width / fieldsize) / 2;
        }

        private void cb_torus_CheckedChanged(object sender, EventArgs e) => torusactive = cb_torus.Checked;



        //Bind the CB and the RuleButtonRows
        private void btnrow_Click(object sender, EventArgs e)
        {
            ruletoborn = btnrow_born.Rule;
            ruletosurvive = btnrow_survive.Rule;

            CheckRules();
        }

        private void cb_rules_SelectedIndexChanged(object sender, EventArgs e)
        {
            string itm = cb_rules.SelectedItem.ToString();

            if (itm != "Custom") {
                ruletosurvive = itm.Substring(itm.IndexOf('[') + 1, itm.IndexOf('|') - itm.IndexOf('[') - 1);
                ruletoborn = itm.Substring(itm.IndexOf('|') + 1, itm.IndexOf(']') - itm.IndexOf('|') - 1);

                btnrow_survive.Rule = ruletosurvive;
                btnrow_born.Rule = ruletoborn;
            }
        }

        private void CheckRules()
        {
            string surviverule = btnrow_survive.Rule;
            string bornrule = btnrow_born.Rule;

            int index = 0;
            foreach (string item in cb_rules.Items) {
                if (item.Contains("[" + surviverule + "|" + bornrule + "]")) {
                    cb_rules.SelectedIndex = index;
                    return;
                }

                index++;
            }

            cb_rules.SelectedIndex = cb_rules.Items.Count - 1;
        }





        //Reset
        private void btn_reset_Click(object sender, EventArgs e)
        {
            Attributes.AmountOfCellsPerLine = 16;
            gridstyle = UI.FieldOfLifeUI.GridStyles.Continuous;
            cellstyle = UI.FieldOfLifeUI.CellStyles.Square;
            distance = 1;
            backgroundcolor = Color.FromArgb(40, 40, 40);
            gridcolor = Color.FromArgb(75, 75, 75);
            livingcolor = Color.LightBlue;
            hovercolor = Color.FromArgb(127, 255, 165, 0);
            torusactive = false;
            refreshrate = 100;
            ruletosurvive = "23";
            ruletoborn = "3";

            tb_fieldsize.Value = Attributes.AmountOfCellsPerLine;
            cb_gridstyle.Text = gridstyle.ToString();
            cb_cellstyle.Text = cellstyle.ToString();
            nud_distance.Value = Math.Min(distance, nud_distance.Maximum);
            btn_backcolor.BackColor = backgroundcolor;
            btn_gridcolor.BackColor = gridcolor;
            btn_livingcolor.BackColor = livingcolor;
            btn_hovercolor.BackColor = hovercolor;
            nud_refreshrate.Value = refreshrate;
            cb_torus.Checked = torusactive;
            cb_rules.SelectedIndex = 0;

            Invalidate();
        }



    }
}
