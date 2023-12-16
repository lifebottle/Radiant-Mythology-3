using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Windows.Forms.VisualStyles;
using MLB_Text_File_Editor.Properties;

namespace MLB_Text_File_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int MasterPointers;
        int[] Pointernumber;
        int[] Masters;
        int[] Pointers;
        string[] Strings;
        string[] Updated;
        string[] Old;
        string[] Pntrs;
        string file;
        string path;
        string[] ASTR;
        int curr_pointer;
        int writendx = 0;
        string folder;
        int countie;
        string filter = "off";


        private void menuItem2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open MLB file...";
            ofd.Filter = "MLB Files|*.mlb||";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            file = Path.GetFileNameWithoutExtension(ofd.FileName);
            folder = Path.GetDirectoryName(ofd.FileName);
            path = ofd.FileName;
            toolStripStatusLabel1.Text = path;
            toolStripStatusLabel1.Visible = true;
            menuItem3.Enabled = true;
            FileStream stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));
            br.BaseStream.Position = 0x3;
            MasterPointers = br.ReadByte();
            Masters = new int[MasterPointers + 1];
            for (int i = 0; i < MasterPointers; i++)
            {
                stream.Position = 0x4 + i * 0x4;
                byte[] data = new byte[4];
                stream.Read(data, 0, data.Length);
                Masters[i] = BitConverter.ToInt32(data, 0);
            }
            Masters[Masters.Length - 1] = (int) stream.Length;
            Pointernumber = new int[MasterPointers];
            for (int i = 0; i < MasterPointers; i++)
            {
                stream.Position = Masters[i];
                byte[] data = new byte[4];
                stream.Read(data, 0, data.Length);
                Pointernumber[i] = BitConverter.ToInt32(data, 0);
            }
            int id = 0;
            for (int i = 0; i < MasterPointers; i++)
            {
                Pointers = new int[Pointernumber[i] + 1];
                Strings = new string[Pointernumber[i]];
                int pos = Masters[i] + 0x4;
                for (int j = 0; j < Pointernumber[i]; j++)
                {
                    stream.Position = pos + j * 4;
                    byte[] data = new byte[4];
                    stream.Read(data, 0, data.Length);
                    Pointers[j] = BitConverter.ToInt32(data, 0);
                }
                Pointers[Pointers.Length - 1] = (int)(Masters[i + 1]);
                int var = 0;
                for (int j = 0; j < Pointers.Length - 1; j++)
                {
                    stream.Position = Pointers[j];
                    if (Pointers[j + 1] < Pointers[j])
                        var = 300;
                    else
                    {
                        var = Pointers[j + 1] - Pointers[j];
                    }
                    byte[] data = new byte[var];
                    stream.Read(data, 0, data.Length);
                    Strings[j] = Encoding.GetEncoding("EUC-JP").GetString(data).Split('\0')[0];
                    
                    ListViewItem item = new ListViewItem(new string[]
                    {
                        (j + id).ToString(),
                        Pointers[j].ToString("X2"),
                        Strings[j],
                        Strings[j]
                    });
                    listView1.Items.AddRange(new ListViewItem[] { item });
                }
                id += Pointernumber[i];
            }
            Updated = new string[listView1.Items.Count];
            Old = new string[listView1.Items.Count];
            Pntrs = new string[listView1.Items.Count];
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                Updated[i] = listView1.Items[i].SubItems[2].Text;
                Old[i] = listView1.Items[i].SubItems[3].Text;
                Pntrs[i] = listView1.Items[i].SubItems[1].Text;
            }
            toolStripStatusLabel1.Text = "File accessed:";
            //toolStripStatusLabel2.Enabled = true;
            toolStripStatusLabel2.ForeColor = Color.DarkRed;
            toolStripStatusLabel2.Text = (Path.GetFileName(path));
            toolStripStatusLabel2.ToolTipText = "Click to open Directory";
            toolStripStatusLabel2.Visible = true;

            //enabling some stuff
            textBox2.Enabled = true;
            toolStrip1.Enabled = true;
            menuItem7.Enabled = true;
            menuItem8.Enabled = true;

            countie = listView1.Items.Count;
            stream.Dispose();
            stream.Close();
            stream = null;
            br.Dispose();
            br.Close();
            br = null;
            ofd.Dispose();
            ofd = null;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.Text = new string(listView1.SelectedItems[0].SubItems[3].Text.Skip(1).ToArray());
                //textBox2.Text = listView1.SelectedItems[0].Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].SubItems[2].Text = "@" + textBox2.Text;
            Updated[Int32.Parse(listView1.SelectedItems[0].Text)] = "@" + textBox2.Text;
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.InitialDirectory = Application.ExecutablePath;
            sfd.Filter = "Text Files (*.txt)|*.txt";
            sfd.Title = "Save Text file";
            sfd.FileName = file;
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            StreamWriter wwrite = new StreamWriter(sfd.FileName, false, Encoding.GetEncoding("EUC-JP"));
            for (int i = 0; i < listView1.Items.Count; ++i)
            {
                wwrite.WriteLine((i + 1).ToString() + "＾" + listView1.Items[i].SubItems[2].Text.Replace("\x0A","<NL>"));
            }
            wwrite.Close();
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt";
            ofd.Title = "Open Text file";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            StreamReader sr = new StreamReader(ofd.FileName, Encoding.GetEncoding("EUC-JP"));
                while (sr.Peek() >= 0)
                {
                    string[] a2 = sr.ReadLine().Split('＾');
                    if (a2.Length == 2)
                    {
                        int aa = int.Parse(a2[0].ToString());

                        listView1.Items[aa - 1].SubItems[2].Text = a2[1];
                    }
                }
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            string namewe = Path.GetFileNameWithoutExtension(path);
            folder = Path.GetDirectoryName(path);
            BinaryWriter bw = new BinaryWriter(File.Create(folder + "\\" + namewe + "_new.mlb"));
            Encoding enc = Encoding.GetEncoding("EUC-JP");

            bw.Write(enc.GetBytes("MLT"));
            bw.Write(MasterPointers);
            for (int i = 0; i < MasterPointers; i++)
            {
                curr_pointer = Masters[i] + (Pointernumber[i]*4) + 4;
                bw.BaseStream.Position = (MasterPointers*i) + 4;
                bw.Write(Masters[i]);
                bw.BaseStream.Position = Masters[i];
                bw.Write(Pointernumber[i]);
                for (int j = 0; j < Pointernumber[i]; j++)
                {
                    bw.BaseStream.Position = Masters[i] + 4 + j*4;
                    bw.Write(curr_pointer);
                    bw.BaseStream.Position = curr_pointer;
                    bw.Write(enc.GetBytes(listView1.Items[writendx + j].SubItems[2].Text.Replace("<NL>", "\x0A") + '\0'));
                    curr_pointer += enc.GetByteCount(listView1.Items[writendx + j].SubItems[2].Text.Replace("<NL>", "\x0A")) + 1;
                }
                //MessageBox.Show(curr_pointer.ToString("x8"), i.ToString());
                writendx += Pointernumber[i];
                Masters[i + 1] = curr_pointer;
                //MessageBox.Show(Masters[i + 1].ToString("X8"), i.ToString());
            }
            bw.Write(0xffff);
            toolStripStatusLabel1.Text = "New file created:";
            //toolStripStatusLabel2.Enabled = true;
            toolStripStatusLabel2.ForeColor = Color.ForestGreen;
            toolStripStatusLabel2.Text = namewe + "_new.mlb";
            toolStripStatusLabel2.ToolTipText = "Click to open Directory";
            toolStripStatusLabel2.Visible = true;
            bw.Flush();
            bw.Close();
            bw = null;
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "By Omarrrio 2013 blablablabla just do me a fucking favor\nand don't redestribute this plz, ok ? thank you :)",
                "bout dis sheit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            if (folder != null)
                System.Diagnostics.Process.Start(folder);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripButton2.BackColor = Color.SkyBlue;
            toolStripTextBox2.Text = textBox1.Font.Size.ToString();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.SelectAll();
                textBox1.SelectionAlignment = HorizontalAlignment.Left;
                textBox2.SelectAll();
                textBox2.SelectionAlignment = HorizontalAlignment.Left;
                toolStripButton3.BackColor = SystemColors.Control;
                toolStripButton4.BackColor = SystemColors.Control;
                toolStripButton2.BackColor = Color.SkyBlue;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.SelectAll();
                textBox1.SelectionAlignment = HorizontalAlignment.Center;
                textBox2.SelectAll();
                textBox2.SelectionAlignment = HorizontalAlignment.Center;
                toolStripButton2.BackColor = SystemColors.Control;
                toolStripButton4.BackColor = SystemColors.Control;
                toolStripButton3.BackColor = Color.SkyBlue;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.SelectAll();
                textBox1.SelectionAlignment = HorizontalAlignment.Right;
                textBox2.SelectAll();
                textBox2.SelectionAlignment = HorizontalAlignment.Right;
                toolStripButton2.BackColor = SystemColors.Control;
                toolStripButton3.BackColor = SystemColors.Control;
                toolStripButton4.BackColor = Color.SkyBlue;
            }
        }

        private void toolStripTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox2.Text.Count() > 0)
            {
                textBox1.Font = new Font("Microsoft Sans Serif", Int32.Parse(toolStripTextBox2.Text));
                textBox2.Font = new Font("Microsoft Sans Serif", Int32.Parse(toolStripTextBox2.Text));
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text != "")
            {
                listView1.Items.Clear();
                filter = "on";
                for (int i = 0; i < Updated.Length; i++)
                {
                    if (Updated[i].ToLower().Contains((toolStripTextBox1.Text.ToLower())))
                    {
                        ListViewItem item = new ListViewItem(new string[]
                            {
                                i.ToString(),
                                Pntrs[i],
                                Updated[i],
                                Old[i]
                            });
                        listView1.Items.AddRange(new ListViewItem[] { item });
                    }
                }
            }
            else if (toolStripTextBox1.Text == "" && listView1.Items.Count < Updated.Length)
            {
                filter = "off";
                listView1.Items.Clear();
                for (int i = 0; i < Updated.Length; i++)
                {
                    ListViewItem item = new ListViewItem(new string[]
                    {
                        i.ToString(),
                        Pntrs[i],
                        Updated[i],
                        Old[i]
                    });
                    listView1.Items.AddRange(new ListViewItem[] { item });
                }
                toolStripButton1.Enabled = false;
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Count() == 0 && listView1.Items.Count < countie)
            {
                toolStripButton1.Image = Resources.arrow_refresh;
                toolStripButton1.ToolTipText = "Reset Items on the list.";
            }
            else if (toolStripTextBox1.Text.Count() > 0)
            {
                toolStripButton1.Enabled = true;
                toolStripButton1.Image = Resources.zoom;
                toolStripButton1.ToolTipText = "Filter.";
            }
            else if (toolStripTextBox1.Text.Count() == 0 && listView1.Items.Count == countie)
            {
                toolStripButton1.Enabled = false;
                toolStripButton1.Image = Resources.arrow_refresh;
                toolStripButton1.ToolTipText = "Reset Items on the list.";
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string languagePair = "jp|en";

            string translation = "";
            string[] laynes = textBox1.Text.Split('\n');
            if (laynes.Length > 1)
            {
                string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string result = webClient.DownloadString(url);
                string check = "";
                Clipboard.SetText(result);
                for (int i = 0; i < laynes.Length; i++)
                {
                    result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
                    if (i != laynes.Length - 1)
                    {
                        result = result.Substring(result.IndexOf(">") + 101);
                        /*if (check.StartsWith("<br>"))
                        {
                            result = result.Substring(result.IndexOf(">") + 1);
                        }
                        else
                        {
                            result = result.Substring(result.IndexOf(">") + 101);
                        }*/
                    }
                    else
                    {
                        result = result.Substring(result.IndexOf(">") + 1);
                        /*if (check.StartsWith("<br>"))
                        {
                            result = result.Substring(result.IndexOf(">") + 1);
                        }
                        else
                        {
                            result = result.Substring(result.IndexOf(">") + 101);
                        }*/
                    }
                    translation += result.Substring(0, result.IndexOf("</span>"));
                }
                //result = WebUtility.HtmlDecode(result.Trim()); <---- i didn't need this one since i translated from Japanese to English
                MessageBox.Show(translation.Replace("<br>", "\n"));
            }
            else
            {
                string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string result = webClient.DownloadString(url);
                result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
                result = result.Substring(result.IndexOf(">") + 1);
                result = result.Substring(0, result.IndexOf("</span>"));
                //result = WebUtility.HtmlDecode(result.Trim());
                MessageBox.Show(result.Replace("<br>", "\n"));
            }
            
        }
    }
}
