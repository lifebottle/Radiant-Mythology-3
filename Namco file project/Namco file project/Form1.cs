using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Namco_file_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string>
            {
                {'0', "0000"},
                {'1', "0001"},
                {'2', "0010"},
                {'3', "0011"},
                {'4', "0100"},
                {'5', "0101"},
                {'6', "0110"},
                {'7', "0111"},
                {'8', "1000"},
                {'9', "1001"},
                {'a', "1010"},
                {'b', "1011"},
                {'c', "1100"},
                {'d', "1101"},
                {'e', "1110"},
                {'f', "1111"}
            };

        int PointerNumber;
        int[] pointers;
        string fullpath;
        string[] flag;
        string[] padding;
        string debug = "off";
        string ai = "";

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Namco.bdi";
            ofd.Filter = "bdi Files|*.bdi||";
            DialogResult Result = ofd.ShowDialog();
            if (Result == DialogResult.Cancel)
                return;
            
            BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));

            br.BaseStream.Position = 0x6;
            int check = br.ReadInt32();
            if (check == 0x9AB)
            {
                toolStripStatusLabel1.Text = "File Status:";
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Loaded!";
                //Thread.Sleep(2500);
                toolStripStatusLabel2.ToolTipText = "File Name: " + (Path.GetFileName(ofd.FileName)) +
                                                    "\nFile Location: " + (Path.GetDirectoryName(ofd.FileName));
                toolStripStatusLabel2.ForeColor = Color.Brown;
                toolStripStatusLabel1.Visible = true;
                toolStripStatusLabel2.Visible = true;
                br.BaseStream.Position = 0x10004;
                PointerNumber = br.ReadInt32();

                FileStream stream1 = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                pointers = new int[PointerNumber + 1];
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                for (int i = 0; i < PointerNumber; ++i)
                {
                    byte[] data = new byte[4];
                    stream1.Position = 0x10014 + i*8;
                    stream1.Read(data, 0, data.Length);
                    int bpointer = BitConverter.ToInt32(data, 0);
                    string b2pointer = bpointer.ToString("X");
                    if (b2pointer.Count() < 8)
                        b2pointer = new string('0', 8 - b2pointer.Count()) + bpointer.ToString("X");
                    StringBuilder result = new StringBuilder();
                    foreach (char c in b2pointer)
                    {
                        result.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string epointer = result.ToString();
                    flag[i] = new string(epointer.Take(1).ToArray());
                    string pointer = new string(epointer.Skip(1).Take(20).ToArray());
                    padding[i] = new string(epointer.Skip(21).Take(11).ToArray());
                    pointers[i] = Convert.ToInt32(pointer, 2)*0x800;

                    ListViewItem lvi = new ListViewItem(new string[]{
                        i.ToString(),
                        flag[i],
                        "0x" + pointers[i].ToString("x8"),
                        Convert.ToInt32(padding[i], 2).ToString()});
                    listView1.Items.AddRange(new ListViewItem[] {lvi});
                }
                pointers[pointers.Length - 1] = (int) (stream1.Length);
                toolStripStatusLabel2.Visible = false;
                toolStripStatusLabel1.Text = "Now Pick a Folder...";
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Pick where to extract the archive's content...";
                fbd.SelectedPath = Path.GetDirectoryName(ofd.FileName);
                DialogResult rresult = fbd.ShowDialog();
                if (rresult == DialogResult.Cancel)
                    return;
                //DirectoryInfo selDir = new DirectoryInfo(fbd.SelectedPath);
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Creating 'Unpacked' Directory in chosen folder...";
                toolStripStatusLabel2.Visible = true;
                Directory.CreateDirectory(fbd.SelectedPath + "\\Unpacked");
                StreamWriter wwrite = new StreamWriter(fbd.SelectedPath + "\\Unpacked\\Do_Not_Touch_Me.txt", false,
                    Encoding.Unicode);
                for (int i = 0; i < PointerNumber; ++i)
                {
                    wwrite.WriteLine(i.ToString() + "＾" + flag[i] + "＾" + padding[i]);
                }
                wwrite.Close();
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Extracting Header...";
                byte[] header = new byte[0x2D000];
                stream1.Position = 0x0;
                stream1.Read(header, 0, header.Length);
                //FileStream fh = new FileStream()
                FileStream fh = new FileStream(fbd.SelectedPath + "\\Unpacked\\header", FileMode.Create);
                fh.Write(header, 0, header.Length);
                fh.Close();
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Header Extracted!";
                int enableext;
                if (MessageBox.Show(
                    "Would you like to add extentions to known headers ?\nWARNING:\nYou cannot repack files with extentions !\nMake sure you have all files extracted somewhere before extracting with Extentions !",
                    "Extentions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    enableext = 1;
                else
                {
                    enableext = 0;
                }
                string ext = "";
                for (int i = 0; i < PointerNumber; ++i)
                {
                    //Thread.Sleep(500);
                    toolStripStatusLabel2.Text = "Extracting " + (i + 1).ToString() + "/" + PointerNumber.ToString();

                    stream1.Position = pointers[i];
                    byte[] data = new byte[pointers[i + 1] - pointers[i] - Convert.ToInt32(padding[i], 2)];
                    stream1.Read(data, 0, data.Length);
                    stream1.Position = pointers[i];
                    if (enableext == 1)
                    {
                        byte[] checking = new byte[4];
                        stream1.Read(checking, 0, checking.Length);
                        string checksum = Encoding.ASCII.GetString(checking);
                        //int checkint = BitConverter.ToInt32(checking, 0);
                        if (checksum == "EZBI")
                            ext = ".arc";
                        else if (checksum == "PSMF")
                            ext = ".pmf";
                        else
                            ext = ".unk";
                    }
                    FileStream fs = new FileStream(fbd.SelectedPath + "\\Unpacked\\" + (i + 1).ToString() + ext,
                        FileMode.Create);
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
                stream1.Close();
                ofd.Dispose();
                //Thread.Sleep(2500);
                MessageBox.Show("Mission Accomplished sir", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel2.Text = "Extraction of all " + PointerNumber.ToString() +
                                             " files is complete! Enjoy :)";
            }
            else if (check == 0x173B)
            {
                toolStripStatusLabel1.Text = "File Status:";
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Loaded!";
                toolStripStatusLabel2.ToolTipText = "File Name: " + (Path.GetFileName(ofd.FileName)) +
                                                    "\nFile Location: " + (Path.GetDirectoryName(ofd.FileName));
                toolStripStatusLabel2.ForeColor = Color.Brown;
                toolStripStatusLabel1.Visible = true;
                toolStripStatusLabel2.Visible = true;
                br.BaseStream.Position = 0x8004;
                PointerNumber = br.ReadInt32();

                FileStream stream1 = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                pointers = new int[PointerNumber + 1];
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                for (int i = 0; i < PointerNumber; ++i)
                {
                    byte[] data = new byte[4];
                    stream1.Position = 0x8014 + i * 8;
                    stream1.Read(data, 0, data.Length);
                    int bpointer = BitConverter.ToInt32(data, 0);
                    string b2pointer = bpointer.ToString("X8");
                    //if (b2pointer.Count() < 8)
                    //    b2pointer = new string('0', 8 - b2pointer.Count()) + bpointer.ToString("X");
                    StringBuilder result = new StringBuilder();
                    foreach (char c in b2pointer)
                    {
                        result.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string epointer = result.ToString();
                    flag[i] = new string(epointer.Take(1).ToArray());
                    string pointer = new string(epointer.Skip(1).Take(20).ToArray());
                    padding[i] = new string(epointer.Skip(21).Take(11).ToArray());
                    pointers[i] = Convert.ToInt32(pointer, 2) * 0x800;

                    ListViewItem lvi = new ListViewItem(new string[]{
                        i.ToString(),
                        flag[i],
                        "0x" + pointers[i].ToString("x8"),
                        Convert.ToInt32(padding[i], 2).ToString()});
                    listView1.Items.AddRange(new ListViewItem[] { lvi });
                }
                pointers[pointers.Length - 1] = (int)(stream1.Length);
                toolStripStatusLabel2.Visible = false;
                //Thread.Sleep(2500);
                toolStripStatusLabel1.Text = "Now Pick a Folder...";
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Pick where to extract the archive's content...";
                fbd.SelectedPath = Path.GetDirectoryName(ofd.FileName);
                DialogResult rresult = fbd.ShowDialog();
                if (rresult == DialogResult.Cancel)
                    return;
                //DirectoryInfo selDir = new DirectoryInfo(fbd.SelectedPath);
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Creating 'Unpacked' Directory in chosen folder...";
                toolStripStatusLabel2.Visible = true;
                toolStripStatusLabel1.Visible = false;
                Directory.CreateDirectory(fbd.SelectedPath + "\\Unpacked");
                StreamWriter wwrite = new StreamWriter(fbd.SelectedPath + "\\Unpacked\\Do_Not_Touch_Me.txt", false,
                    Encoding.Unicode);
                for (int i = 0; i < PointerNumber; ++i)
                {
                    wwrite.WriteLine(i.ToString() + "＾" + flag[i] + "＾" + padding[i]);
                }
                wwrite.Close();
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Extracting Header...";
                byte[] header = new byte[0x2D000];
                stream1.Position = 0x0;
                stream1.Read(header, 0, header.Length);
                //FileStream fh = new FileStream()
                FileStream fh = new FileStream(fbd.SelectedPath + "\\Unpacked\\header", FileMode.Create);
                fh.Write(header, 0, header.Length);
                fh.Close();
                //Thread.Sleep(2500);
                toolStripStatusLabel2.Text = "Header Extracted!";
                int enableext;
                if (MessageBox.Show(
                    "Would you like to add extentions to known headers ?\nWARNING:\nYou cannot repack files with extentions !\nMake sure you have all files extracted somewhere before extracting with Extentions !",
                    "Extentions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    enableext = 1;
                else
                {
                    enableext = 0;
                }
                string ext = "";
                for (int i = 0; i < PointerNumber; ++i)
                {
                    //Thread.Sleep(500);
                    toolStripStatusLabel2.Text = "Extracting " + (i + 1).ToString() + "/" + PointerNumber.ToString();

                    stream1.Position = pointers[i];
                    byte[] data = new byte[pointers[i + 1] - pointers[i] - Convert.ToInt32(padding[i], 2)];
                    stream1.Read(data, 0, data.Length);
                    stream1.Position = pointers[i];
                    if (enableext == 1)
                    {
                        byte[] checking = new byte[4];
                        stream1.Read(checking, 0, checking.Length);
                        string checksum = Encoding.ASCII.GetString(checking);
                        //int checkint = BitConverter.ToInt32(checking, 0);
                        if (checksum == "EZBI")
                            ext = ".arc";
                        else if (checksum == "PSMF")
                            ext = ".pmf";
                        else
                            ext = ".unk";
                    }
                    FileStream fs = new FileStream(fbd.SelectedPath + "\\Unpacked\\" + (i + 1).ToString() + ext,
                        FileMode.Create);
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
                //Thread.Sleep(2500);
                MessageBox.Show("Mission Accomplished sir", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel2.Text = "Extraction of all " + PointerNumber.ToString() +
                                             " files is complete! Enjoy :)";
                stream1.Close();
                ofd.Dispose();
                ofd = null;
            }
            else
            {
                MessageBox.Show("The file you are trying to open is either Invalid or Corrupt!",
                        "Error: Invalid File!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Are you repacking for:\nTales of Heroes(YES)\nTales of the World 3(NO)",
                    "Pick the proper Game", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                PointerNumber = 14738;
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Pick the desired folder to repack...";
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.Cancel)
                    return;

                StreamReader sr = new StreamReader(fbd.SelectedPath + "\\Do_Not_Touch_Me.txt", Encoding.Unicode);
                while (sr.Peek() >= 0)
                {
                    string[] a2 = sr.ReadLine().Split('＾');
                    if (a2.Length == 3)
                    {
                        int aa = int.Parse(a2[0].ToString());

                        flag[aa] = a2[1];
                        padding[aa] = a2[2];
                    }
                }

                FileStream stream2 = new FileStream(fbd.SelectedPath + "\\header", FileMode.Open, FileAccess.Read);
                byte[] data1 = new byte[stream2.Length];
                stream2.Read(data1, 0, data1.Length);
                stream2.Close();

                FileStream fs = new FileStream(fbd.SelectedPath + "\\New_Namco.bdi", FileMode.Create);
                fs.Write(data1, 0, data1.Length);
                //fs.Position = 0x2D000;
                toolStripStatusLabel1.Visible = true;
                toolStripStatusLabel1.Text = "Repacking in progress...";
                for (int i = 0; i < PointerNumber; ++i)
                {

                    FileStream stream1 = new FileStream(fbd.SelectedPath + "\\" + (i + 1).ToString(), FileMode.Open,
                        FileAccess.Read);
                    byte[] data = new byte[stream1.Length];
                    stream1.Read(data, 0, data.Length);
                    long Fairsize = stream1.Length;
                    stream1.Close();

                    fs.Write(data, 0, data.Length);

                    int Fix0 = 0x800;

                    long offsadress = (fs.Position - Fairsize)/Fix0;
                    string ofadr = offsadress.ToString("X");
                    StringBuilder rresult = new StringBuilder();
                    foreach (char c in ofadr)
                    {
                        rresult.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string fpointer = rresult.ToString();
                    if (fpointer.Count() < 20)
                        fpointer = new string('0', 20 - fpointer.Count()) + rresult.ToString();
                    string npointer = flag[i] + fpointer + padding[i];
                    int npointere = Convert.ToInt32(npointer, 2);

                    fs.Position = 0x10014 + i*8;
                    byte[] data2 = BitConverter.GetBytes(npointere);
                    fs.Write(data2, 0, 4);

                    fs.Position = (offsadress*Fix0) + Fairsize;

                }
                fs.Close();
                MessageBox.Show("Mission Accomplished sir", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "Repack Complete!";
            }
            else
            {
                PointerNumber = 7344;
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = "D:\\Personal Projects\\Tales of\\RM3\\USRDIR\\bdi unpacked";
                fbd.Description = "Pick the desired folder to repack...";
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.Cancel)
                    return;

                StreamReader sr = new StreamReader(fbd.SelectedPath + "\\Do_Not_Touch_Me.txt", Encoding.Unicode);
                while (sr.Peek() >= 0)
                {
                    string[] a2 = sr.ReadLine().Split('＾');
                    if (a2.Length == 3)
                    {
                        int aa = int.Parse(a2[0].ToString());

                        flag[aa] = a2[1];
                        padding[aa] = a2[2];
                    }
                }

                FileStream stream2 = new FileStream(fbd.SelectedPath + "\\header", FileMode.Open, FileAccess.Read);
                byte[] data1 = new byte[stream2.Length];
                stream2.Read(data1, 0, data1.Length);
                stream2.Close();

                FileStream fs = new FileStream(fbd.SelectedPath + "\\New_Namco.bdi", FileMode.Create);
                fs.Write(data1, 0, data1.Length);
                //fs.Position = 0x2D000;
                toolStripStatusLabel1.Visible = true;
                toolStripStatusLabel1.Text = "Repacking in progress...";
                
                for (int i = 0; i < PointerNumber; ++i)
                {
                    string[] matchingFiles = Directory.GetFiles(fbd.SelectedPath,(i+1) + ".*");
                    FileStream stream1 = new FileStream(matchingFiles.First(), FileMode.Open,
                        FileAccess.Read);
                    byte[] data = new byte[stream1.Length];
                    stream1.Read(data, 0, data.Length);
                    long Fairsize = stream1.Length;
                    stream1.Close();

                    fs.Write(data, 0, data.Length);

                    int Fix0 = 0x800;

                    long offsadress = (fs.Position - Fairsize) / Fix0;
                    string ofadr = offsadress.ToString("X");
                    StringBuilder rresult = new StringBuilder();
                    foreach (char c in ofadr)
                    {
                        rresult.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string fpointer = rresult.ToString();
                    if (fpointer.Count() < 20)
                        fpointer = new string('0', 20 - fpointer.Count()) + rresult.ToString();
                    string npointer = flag[i] + fpointer + padding[i];
                    int npointere = Convert.ToInt32(npointer, 2);

                    fs.Position = 0x8014 + i * 8;
                    byte[] data2 = BitConverter.GetBytes(npointere);
                    fs.Write(data2, 0, 4);

                    fs.Position = (offsadress * Fix0) + Fairsize;
                    
                }
                fs.Close();
                MessageBox.Show("Mission Accomplished sir", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "Repack Complete!";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Namco.bdi Extractor/rePacker 0.5 Alpha\nby Omarrrio\nDO NOT DISTRIBUTE THIS SHIT !!!!!\n\nCopyright©2013-ZOA Soft","About this tool...",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.ExecutablePath;
            sfd.Filter = "Text Files (*.txt)|*.txt";
            sfd.Title = "Save Text file";
            sfd.FileName = "log";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            //string ss = "";
                using (StreamWriter wwrite = new StreamWriter(sfd.FileName, false, Encoding.GetEncoding("SHIFT-JIS")))
                {
                    for (int i = 0; i < 14738; ++i)
                    {
                        if (listView1.Items[i].SubItems[0].Text.Length == 1)
                        {
                            wwrite.WriteLine("0000" + i.ToString() + "|" + listView1.Items[i].SubItems[1].Text + "|" +
                                             listView1.Items[i].SubItems[2].Text + "|" +
                                             listView1.Items[i].SubItems[3].Text);
                        }
                        else if (listView1.Items[i].SubItems[0].Text.Length == 2)
                        {
                            wwrite.WriteLine("000" + i.ToString() + "|" + listView1.Items[i].SubItems[1].Text + "|" +
                                             listView1.Items[i].SubItems[2].Text + "|" +
                                             listView1.Items[i].SubItems[3].Text);
                        }
                        else if (listView1.Items[i].SubItems[0].Text.Length == 3)
                        {
                            wwrite.WriteLine("00" + i.ToString() + "|" + listView1.Items[i].SubItems[1].Text + "|" +
                                             listView1.Items[i].SubItems[2].Text + "|" +
                                             listView1.Items[i].SubItems[3].Text);
                        }
                        else if (listView1.Items[i].SubItems[0].Text.Length == 4)
                        {
                            wwrite.WriteLine("0" + i.ToString() + "|" + listView1.Items[i].SubItems[1].Text + "|" +
                                             listView1.Items[i].SubItems[2].Text + "|" +
                                             listView1.Items[i].SubItems[3].Text);
                        }
                        else if (listView1.Items[i].SubItems[0].Text.Length == 5)
                        {
                            wwrite.WriteLine(i.ToString() + "|" + listView1.Items[i].SubItems[1].Text + "|" +
                                             listView1.Items[i].SubItems[2].Text + "|" +
                                             listView1.Items[i].SubItems[3].Text);
                        }
                        /*Thread.Sleep(100);
                toolStripStatusLabel1.Text = "written " + (i + 1).ToString() + "/14738";*/
                    }
                    
                    toolStripStatusLabel1.Text = "Exporting done !";
                }
            }
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            if (debug == "off")
            {
                listView1.Visible = true;
                CheckFileBtn.Visible = true;
                Height = 395;

                Text = "Namco.bdi File Unpacker/Rebuilder 0.5 Alpha - DebugMode ON";
                debug = "on";
            }
            else if (debug == "on")
            {
                listView1.Visible = false;
                CheckFileBtn.Visible = false;
                Height = 89;

                Text = "Namco.bdi File Unpacker/Rebuilder 0.5 Alpha";
                debug = "off";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Namco.bdi";
            ofd.Filter = "bdi Files|*.bdi||";
            DialogResult Result = ofd.ShowDialog();
            if (Result == DialogResult.Cancel)
                return;
            
            BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));

            br.BaseStream.Position = 0x6;
            int check = br.ReadInt32();
            if (check == 0x9AB)
            {
                br.BaseStream.Position = 0x10004;
                PointerNumber = br.ReadInt32();

                FileStream stream1 = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                pointers = new int[PointerNumber + 1];
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                for (int i = 0; i < PointerNumber; ++i)
                {
                    byte[] data = new byte[4];
                    stream1.Position = 0x10014 + i*8;
                    stream1.Read(data, 0, data.Length);
                    int bpointer = BitConverter.ToInt32(data, 0);
                    string b2pointer = bpointer.ToString("X");
                    if (b2pointer.Count() < 8)
                        b2pointer = new string('0', 8 - b2pointer.Count()) + bpointer.ToString("X");
                    StringBuilder result = new StringBuilder();
                    foreach (char c in b2pointer)
                    {
                        result.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string epointer = result.ToString();
                    flag[i] = new string(epointer.Take(1).ToArray());
                    string pointer = new string(epointer.Skip(1).Take(20).ToArray());
                    padding[i] = new string(epointer.Skip(21).Take(11).ToArray());
                    pointers[i] = Convert.ToInt32(pointer, 2)*0x800;

                    ai = i.ToString();
                    timer1.Start();

                    ListViewItem lvi = new ListViewItem(new string[]{
                        i.ToString(),
                        flag[i],
                        "0x" + pointers[i].ToString("x8"),
                        Convert.ToInt32(padding[i], 2).ToString()});
                    listView1.Items.AddRange(new ListViewItem[] {lvi});
                }
                stream1.Close();
                ofd.Dispose();
            }
            else if (check == 0x173B)
            {
                br.BaseStream.Position = 0x8004;
                PointerNumber = br.ReadInt32();

                FileStream stream1 = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                pointers = new int[PointerNumber + 1];
                flag = new string[PointerNumber];
                padding = new string[PointerNumber];
                listView1.BeginUpdate();
                for (int i = 0; i < PointerNumber; ++i)
                {
                    byte[] data = new byte[4];
                    stream1.Position = 0x8014 + i*8;
                    stream1.Read(data, 0, data.Length);
                    int bpointer = BitConverter.ToInt32(data, 0);
                    string b2pointer = bpointer.ToString("X8");
                    //if (b2pointer.Count() < 8)
                    //    b2pointer = new string('0', 8 - b2pointer.Count()) + bpointer.ToString("X");
                    StringBuilder result = new StringBuilder();
                    foreach (char c in b2pointer)
                    {
                        result.Append(hexCharacterToBinary[char.ToLower(c)]);
                    }
                    string epointer = result.ToString();
                    flag[i] = new string(epointer.Take(1).ToArray());
                    string pointer = new string(epointer.Skip(1).Take(20).ToArray());
                    padding[i] = new string(epointer.Skip(21).Take(11).ToArray());
                    pointers[i] = Convert.ToInt32(pointer, 2)*0x800;

                    ai = i.ToString();
                    timer1.Start();

                    ListViewItem lvi = new ListViewItem(new string[]
                    {
                        i.ToString(),
                        flag[i],
                        "0x" + pointers[i].ToString("x8"),
                        Convert.ToInt32(padding[i], 2).ToString()
                    });
                    listView1.Items.AddRange(new ListViewItem[] {lvi});
                }
                listView1.EndUpdate();
                stream1.Close();
                ofd.Dispose();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel1.Text = "Adding " + ai + "/" + (PointerNumber - 1) + " Items to the list.";
        }
    }
}

