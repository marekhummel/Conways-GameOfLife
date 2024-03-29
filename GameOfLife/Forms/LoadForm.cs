﻿using System.IO;
using System.Xml;

namespace GameOfLife.Forms
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
        }


        private void LoadForm_Load(object sender, EventArgs e)
        {
            //Center the butons
            btn_add.Location = new Point(gb_add.Width / 2 - btn_add.Width / 2, btn_add.Location.Y);
            //btn_add.Location = new Point((int)(this.Width / 2) - (int)(btn_apply.Width / 2), btn_apply.Location.Y);

            //Add itmes to the treeview
            foreach (TreeNode node in tree_data.Nodes) {
                node.Nodes.Add("Static Objects");
                node.Nodes.Add("Oscillate Objects");
                node.Nodes.Add("Spaceships and Gliders");
            }
        }

        private void tree_data_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            var xmlWriterSettings = new XmlWriterSettings {
                NewLineOnAttributes = true,
                Indent = true
            };

            using (var writer = XmlWriter.Create(Path.Combine(Attributes.SettingsDirectory, "presettings.xml"), xmlWriterSettings)) {
                writer.WriteStartDocument();
                writer.WriteStartElement("CellLists");
                int count = 1;

                foreach (TreeNode node in tree_data.Nodes) {
                    writer.WriteStartElement("Node" + count.ToString());
                    writer.WriteElementString("Text", node.Text);

                    int item = 1;
                    foreach (TreeNode subnode in node.Nodes) {
                        writer.WriteElementString("Content" + item.ToString(), subnode.Text);
                        item++;
                    }

                    writer.WriteEndElement();

                    count++;
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
