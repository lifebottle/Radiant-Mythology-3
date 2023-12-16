using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace EZBIND_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string fullPath;
        string[] Filename;
        int[] nameindex;
        int[] pointer;
        int[] size;
        private int PntrNumber;

        private void menuItem2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            openFileDialog1.Filter = "EZBIND|*.ezb;*.arc;*.bin";
            openFileDialog1.InitialDirectory = Application.ExecutablePath;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            fullPath = Path.GetDirectoryName(openFileDialog1.FileName);

            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel1.Text = openFileDialog1.FileName;
            
            Openezb(openFileDialog1.FileName);

            for (int i = 0; i < PntrNumber; ++i)
            {
               ListViewItem item = new ListViewItem(new string[] { 
               i.ToString(),
               pointer[i].ToString("X"), 
               Filename[i],
               size[i].ToString("X")});
               listView1.Items.AddRange(new ListViewItem[] { item });
            }
        }

        private void Openezb(string FileName)
        {
            FileStream stream1 = new FileStream(FileName, FileMode.Open, FileAccess.Read);

            byte[] dat = new byte[4];
            stream1.Position = 0x8;
            stream1.Read(dat, 0, dat.Length);
            PntrNumber = BitConverter.ToInt32(dat, 0); ;

            Filename = new string[PntrNumber];
            pointer = new int[PntrNumber];
            size = new int[PntrNumber];
            nameindex = new int[PntrNumber + 1];
            //stream1.Position = 0x10;

            for (int j = 0; j < PntrNumber; j++)
            {
                stream1.Position = 0x10 + (j * 0x10);
                byte[] data = new byte[4];
                stream1.Read(data, 0, data.Length);
                nameindex[j] = BitConverter.ToInt32(data, 0);
                stream1.Read(data, 0, data.Length);
                size[j] = BitConverter.ToInt32(data, 0);
                stream1.Read(data, 0, data.Length);
                pointer[j] = BitConverter.ToInt32(data, 0);
            }
            nameindex[nameindex.Length - 1] = pointer[0];
            for (int k = 0; k < PntrNumber; k++)
            {
                stream1.Position = nameindex[k];
                byte[] name = new byte[nameindex[k + 1] - nameindex[k]];
                stream1.Read(name, 0, name.Length);
                Filename[k] = Encoding.GetEncoding("GBK").GetString(name).Split('\0')[0];
            }
            stream1.Close();
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = listView1.SelectedIndices[0];

            saveFileDialog1.Filter = "*.*|*.*||";
            saveFileDialog1.InitialDirectory = fullPath;
            saveFileDialog1.FileName = Filename[i];
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            Export(i, saveFileDialog1.FileName, openFileDialog1.FileName);
            toolStripStatusLabel1.Text = "Export Complete!";
        }

        private void Export(int i, string FileName, string fileName)
        {
            FileStream stream1 = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            stream1.Position = pointer[i];
            byte[] data = new byte[size[i]];
            stream1.Read(data, 0, data.Length);

            FileStream fs = new FileStream(FileName, FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();
            stream1.Close();
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = fullPath;
            folderBrowserDialog1.Description = "Pick where to extract the files...";
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            for (int i = 0; i < listView1.Items.Count; ++i)
            {
                Export(i, folderBrowserDialog1.SelectedPath + "\\" + Filename[i], openFileDialog1.FileName);
            }
            toolStripStatusLabel1.Text = "Export Complete!";
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = fullPath;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            FileStream stream2 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            byte[] data1 = new byte[pointer[0]];
            stream2.Read(data1, 0, data1.Length);
            stream2.Close();

            FileStream fs = new FileStream(openFileDialog1.FileName + "_new", FileMode.Create);
            fs.Write(data1, 0, data1.Length);

            for (int i = 0; i < PntrNumber; ++i)
            {

                FileStream stream1 = new FileStream(folderBrowserDialog1.SelectedPath + "\\" + Filename[i], FileMode.Open, FileAccess.Read);
                byte[] data = new byte[stream1.Length];
                stream1.Read(data, 0, data.Length);
                long Fairsize = stream1.Length;
                stream1.Close();

                fs.Write(data, 0, data.Length);

                long offsadress = fs.Position - Fairsize;

                fs.Position = 0x14 + i * 0x10;
                byte[] data2 = BitConverter.GetBytes(Fairsize);
                fs.Write(data2, 0, 4);

                //fs.Position = nameindex[i] + Filename[i].Length + 1;
                data2 = BitConverter.GetBytes(offsadress);
                fs.Write(data2, 0, 4);

                fs.Position = offsadress + Fairsize;

            }
            fs.Close();
            toolStripStatusLabel1.Text = "Import Complete!";
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = fullPath;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            DirectoryInfo selDir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
            Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + "\\Exported");
            foreach (FileInfo d in selDir.GetFiles())
            {
                if (d.Extension.ToLower() == ".arc")
                {
                    Openezb(d.FullName);
                    Application.DoEvents();
                    for (int i = 0; i < PntrNumber; ++i)
                    {
                        Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + "\\Exported\\" + d.Name);
                        Export(i, folderBrowserDialog1.SelectedPath + "\\Exported\\" + d.Name + "\\" + Filename[i],
                            d.FullName);
                    }
                }

            }
            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel1.Text = "Batch Extraction Complete!";
        }

        private void menuItem12_Click(object sender, EventArgs e)
        {
            
        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            openFileDialog1.Filter = "EZTEXT Binary Files|*.bin";
            openFileDialog1.InitialDirectory = Application.ExecutablePath;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            fullPath = Path.GetDirectoryName(openFileDialog1.FileName);

            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel1.Text = openFileDialog1.FileName;

            FileStream stream1 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);

            byte[] dat = new byte[4];
            stream1.Position = 0x8;
            stream1.Read(dat, 0, dat.Length);
            PntrNumber = BitConverter.ToInt32(dat, 0);
            stream1.Read(dat, 0, dat.Length);
            int saizu = BitConverter.ToInt32(dat, 0);

            stream1.Position = 0x10;
            Filename = new string[PntrNumber];
            for (int k = 0; k < PntrNumber; k++)
            {
                byte[] name = new byte[100];
                stream1.Read(name, 0, name.Length);
                Filename[k] = Encoding.GetEncoding("EUC-JP").GetString(name).Split('\0')[0];
                stream1.Position += 1;
            }
            stream1.Close();

            for (int i = 0; i < PntrNumber; ++i)
            {
                ListViewItem item = new ListViewItem(new string[] { 
                   i.ToString(), 
                   Filename[i]});
                listView1.Items.AddRange(new ListViewItem[] { item });
            }
        }
    }
}
