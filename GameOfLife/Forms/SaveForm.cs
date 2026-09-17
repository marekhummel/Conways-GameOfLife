namespace GameOfLife.Forms
{
    public partial class SaveForm : Form
    {
        public SaveForm()
        {
            InitializeComponent();

            AcceptButton = btn_save;
            CancelButton = btn_cancel;
            btn_save.DialogResult = DialogResult.OK;
            btn_cancel.DialogResult = DialogResult.Cancel;
        }

        private string? path;
        private int gen;

        private SafeTypes safetype;
        private enum SafeTypes
        {
            Bitmap,
            GIF,
            Data
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public UI.FieldOfLifeUI? AFieldOfLife { get; set; }

        // *****************

        private void SaveForm_Load(object sender, EventArgs e)
        {
            //Set the limit for the NUD
            nud_gen.Maximum = Attributes.Generation;

            //Set default path
            txt_path.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GameOfLife File.bmp");

            //Set one RadioButton
            rb_bmp.Checked = true;
        }

        private void SaveForm_Shown(object sender, EventArgs e)
        {
            if (AFieldOfLife == null) {
                Close();
            }
        }

        //Save
        private void btn_save_Click(object sender, EventArgs e)
        {
            //Check whether the file already exist

            {
                if (new System.IO.FileInfo(path).Exists) {
                    string defpath = path.Substring(0, path.LastIndexOf('.'));
                    int counter = 2;
                    string ext = path.Substring(path.LastIndexOf('.'));
                    path = defpath + " (" + counter.ToString() + ")" + ext;

                    while ((new System.IO.FileInfo(path)).Exists) {
                        counter += 1;
                        path = path.Substring(0, path.Length - 7) + "(" + counter.ToString() + ")" + ext;
                    }


                    //Info
                    string msg = "A file with the name \"" + defpath + ext + "\" does already exist. Do you want to rename the file into \"" + path + "\"?";

                    if ((MessageBox.Show(msg, "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)) {
                        return;
                    }
                }


                Bitmap bmp;

                //Save the current gen
                bool[,] currgen = Attributes.Cells;

                //Save
                switch (safetype) {
                    case SafeTypes.Bitmap:
                        if (AFieldOfLife == null) {
                            break;
                        }

                        bmp = new Bitmap(AFieldOfLife.Width, AFieldOfLife.Height);
                        Attributes.Cells = Attributes.CurrentCellStack.ToArray()[Attributes.CurrentCellStack.Count - 1 - gen];

                        AFieldOfLife.DrawOnGfx(Graphics.FromImage(bmp));
                        bmp.Save(path);

                        break;
                    case SafeTypes.GIF:
                        if (AFieldOfLife == null) {
                            break;
                        }

                        var bmplist = new List<Bitmap>();
                        foreach (bool[,] generation in Attributes.CurrentCellStack) {
                            Attributes.Cells = generation;
                            bmp = new Bitmap(AFieldOfLife.Width, AFieldOfLife.Height);
                            AFieldOfLife.DrawOnGfx(Graphics.FromImage(bmp));

                            //to get longer intervals
                            bmplist.Add(bmp);
                            bmplist.Add(bmp);
                        }

                        GifCreator.Save(bmplist, path);

                        break;
                    case SafeTypes.Data:

                        //Save the data in a textfile

                        using (var streamw = new System.IO.StreamWriter(new System.IO.FileStream(path, System.IO.FileMode.CreateNew), System.Text.Encoding.UTF8)) {
                            streamw.WriteLine(Attributes.AmountOfCellsPerLine.ToString());

                            bool[,] gentowrite = Attributes.CurrentCellStack.ToArray()[Attributes.CurrentCellStack.Count - 1 - gen];

                            for (int i = 0; i <= gentowrite.GetUpperBound(0); i++) {
                                for (int j = 0; j <= gentowrite.GetUpperBound(1); j++) {
                                    streamw.Write(Convert.ToInt32(gentowrite[j, i]).ToString() + " ");
                                }
                                streamw.WriteLine();
                            }
                        }


                        break;
                }

                //Restore last gen
                Attributes.Cells = currgen;

                Close();
            }

        }



        //Update vars

        private void txt_path_TextChanged(object sender, EventArgs e) => path = txt_path.Text;

        private void nud_gen_ValueChanged(object sender, EventArgs e) => gen = (int)nud_gen.Value;

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name) {
                case "rb_bmp":
                    safetype = SafeTypes.Bitmap;
                    txt_path.Text = new System.IO.FileInfo(txt_path.Text).FullName.Replace(new System.IO.FileInfo(txt_path.Text).Extension, ".bmp");
                    nud_gen.Enabled = true;
                    break;
                case "rb_gif":
                    safetype = SafeTypes.GIF;
                    txt_path.Text = new System.IO.FileInfo(txt_path.Text).FullName.Replace(new System.IO.FileInfo(txt_path.Text).Extension, ".gif");
                    nud_gen.Enabled = false;
                    break;
                case "rb_data":
                    safetype = SafeTypes.Data;
                    txt_path.Text = new System.IO.FileInfo(txt_path.Text).FullName.Replace(new System.IO.FileInfo(txt_path.Text).Extension, ".cgol");
                    nud_gen.Enabled = true;
                    break;
            }

        }



        //Browse the path
        private void btn_browse_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog {
                AddExtension = true,
                CheckFileExists = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Select GameOfLife Field saving path"
            };

            string defaultext = "";
            string filename = "GameOfLife State";
            string filter = "";
            switch (safetype) {
                case SafeTypes.Bitmap:
                    filename += ".bmp";
                    filter += "Bitmap (*.bmp)|*.bmp";
                    defaultext += ".bmp";
                    break;
                case SafeTypes.GIF:
                    filename += ".gif";
                    filter += "GIF-Image (*.gif)|*.gif";
                    defaultext += ".gif";
                    break;
                case SafeTypes.Data:
                    filename += ".cgol";
                    filter += "Datafile (*.cgol)|*.cgol";
                    defaultext += ".cgol";
                    break;
            }

            sfd.DefaultExt = defaultext;
            sfd.FileName = filename;
            sfd.Filter = filter + "|All Files (*.*)|*.*";


            if (sfd.ShowDialog() == DialogResult.OK) {
                //if the path is valid
                if (new System.IO.DirectoryInfo(new System.IO.FileInfo(sfd.FileName).DirectoryName ?? "").Exists) {
                    txt_path.Text = sfd.FileName;
                }
            }


        }


    }
}
