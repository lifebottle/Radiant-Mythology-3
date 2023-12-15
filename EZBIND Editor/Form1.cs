using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EZBIND_Editor
{
  public class Form1 : Form
  {
    private IContainer components = (IContainer) null;
    private MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem2;
    private MenuItem menuItem3;
    private MenuItem menuItem4;
    private MenuItem menuItem5;
    private MenuItem menuItem6;
    private MenuItem menuItem7;
    private MenuItem menuItem8;
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private ColumnHeader columnHeader4;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem extractToolStripMenuItem;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private OpenFileDialog openFileDialog1;
    private SaveFileDialog saveFileDialog1;
    private FolderBrowserDialog folderBrowserDialog1;
    private MenuItem menuItem9;
    private MenuItem menuItem10;
    private string fullPath;
    private string[] Filename;
    private int[] nameindex;
    private int[] pointer;
    private int[] size;
    private int PntrNumber;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.mainMenu1 = new MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.menuItem7 = new MenuItem();
      this.menuItem8 = new MenuItem();
      this.listView1 = new ListView();
      this.contextMenuStrip1 = new ContextMenuStrip(this.components);
      this.extractToolStripMenuItem = new ToolStripMenuItem();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.columnHeader3 = new ColumnHeader();
      this.columnHeader4 = new ColumnHeader();
      this.statusStrip1 = new StatusStrip();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.openFileDialog1 = new OpenFileDialog();
      this.saveFileDialog1 = new SaveFileDialog();
      this.folderBrowserDialog1 = new FolderBrowserDialog();
      this.menuItem9 = new MenuItem();
      this.menuItem10 = new MenuItem();
      this.contextMenuStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem1,
        this.menuItem5,
        this.menuItem8
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem2,
        this.menuItem3,
        this.menuItem4
      });
      this.menuItem1.Text = "File";
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "Open";
      this.menuItem2.Click += new EventHandler(this.menuItem2_Click);
      this.menuItem3.Index = 1;
      this.menuItem3.Text = "-";
      this.menuItem4.Index = 2;
      this.menuItem4.Text = "Exit";
      this.menuItem5.Index = 1;
      this.menuItem5.MenuItems.AddRange(new MenuItem[4]
      {
        this.menuItem6,
        this.menuItem7,
        this.menuItem9,
        this.menuItem10
      });
      this.menuItem5.Text = "Edit";
      this.menuItem6.Index = 0;
      this.menuItem6.Text = "Export All Files";
      this.menuItem6.Click += new EventHandler(this.menuItem6_Click);
      this.menuItem7.Index = 1;
      this.menuItem7.Text = "Import All Files";
      this.menuItem7.Click += new EventHandler(this.menuItem7_Click);
      this.menuItem8.Index = 2;
      this.menuItem8.Text = "About";
      this.listView1.Columns.AddRange(new ColumnHeader[4]
      {
        this.columnHeader1,
        this.columnHeader2,
        this.columnHeader3,
        this.columnHeader4
      });
      this.listView1.FullRowSelect = true;
      this.listView1.GridLines = true;
      this.listView1.Location = new Point(12, 12);
      this.listView1.Name = "listView1";
      this.listView1.Size = new Size(356, 364);
      this.listView1.TabIndex = 0;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.contextMenuStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.extractToolStripMenuItem
      });
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new Size(110, 26);
      this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
      this.extractToolStripMenuItem.Size = new Size(152, 22);
      this.extractToolStripMenuItem.Text = "Extract";
      this.extractToolStripMenuItem.Click += new EventHandler(this.extractToolStripMenuItem_Click);
      this.columnHeader1.Text = "ID";
      this.columnHeader1.Width = 34;
      this.columnHeader2.Text = "Pointer";
      this.columnHeader3.Text = "File Name";
      this.columnHeader3.Width = 190;
      this.columnHeader4.Text = "Length";
      this.statusStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripStatusLabel1
      });
      this.statusStrip1.Location = new Point(0, 385);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(380, 22);
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new Size(38, 17);
      this.toolStripStatusLabel1.Text = "TSSL1";
      this.toolStripStatusLabel1.Visible = false;
      this.openFileDialog1.FileName = "openFileDialog1";
      this.menuItem9.Index = 2;
      this.menuItem9.Text = "-";
      this.menuItem10.Index = 3;
      this.menuItem10.Text = "Batch Export (Folder Export)";
      this.menuItem10.Click += new EventHandler(this.menuItem10_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(380, 407);
      this.Controls.Add((Control) this.listView1);
      this.Controls.Add((Control) this.statusStrip1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Menu = this.mainMenu1;
      this.Name = nameof (Form1);
      this.Text = "EZBIND File Extractor/Repacker 0.1 Alpha";
      this.contextMenuStrip1.ResumeLayout(false);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public Form1() => this.InitializeComponent();

    private void menuItem2_Click(object sender, EventArgs e)
    {
      this.listView1.Items.Clear();
      this.openFileDialog1.Filter = "EZBIND|*.ezb||";
      this.openFileDialog1.InitialDirectory = Application.ExecutablePath;
      if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      this.fullPath = Path.GetDirectoryName(this.openFileDialog1.FileName);
      this.toolStripStatusLabel1.Visible = true;
      this.toolStripStatusLabel1.Text = this.openFileDialog1.FileName;
      this.Openezb(this.openFileDialog1.FileName);
      for (int index = 0; index < this.PntrNumber; ++index)
        this.listView1.Items.AddRange(new ListViewItem[1]
        {
          new ListViewItem(new string[4]
          {
            index.ToString(),
            this.pointer[index].ToString("X"),
            this.Filename[index],
            this.size[index].ToString("X")
          })
        });
    }

    private void Openezb(string FileName)
    {
      FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
      byte[] buffer1 = new byte[4];
      fileStream.Position = 8L;
      fileStream.Read(buffer1, 0, buffer1.Length);
      this.PntrNumber = BitConverter.ToInt32(buffer1, 0);
      this.Filename = new string[this.PntrNumber];
      this.pointer = new int[this.PntrNumber];
      this.size = new int[this.PntrNumber];
      this.nameindex = new int[this.PntrNumber];
      for (int index = 0; index < this.PntrNumber; ++index)
      {
        fileStream.Position = (long) (16 + index * 16);
        byte[] buffer2 = new byte[4];
        fileStream.Read(buffer2, 0, buffer2.Length);
        this.nameindex[index] = BitConverter.ToInt32(buffer2, 0);
        fileStream.Read(buffer2, 0, buffer2.Length);
        this.size[index] = BitConverter.ToInt32(buffer2, 0);
        fileStream.Read(buffer2, 0, buffer2.Length);
        this.pointer[index] = BitConverter.ToInt32(buffer2, 0);
      }
      for (int index = 0; index < this.PntrNumber; ++index)
      {
        fileStream.Position = (long) this.nameindex[index];
        byte[] numArray = new byte[20];
        fileStream.Read(numArray, 0, numArray.Length);
        this.Filename[index] = Encoding.GetEncoding("GBK").GetString(numArray).Split(new char[1])[0];
      }
      fileStream.Close();
    }

    private void extractToolStripMenuItem_Click(object sender, EventArgs e)
    {
      int selectedIndex = this.listView1.SelectedIndices[0];
      this.saveFileDialog1.Filter = "*.*|*.*||";
      this.saveFileDialog1.InitialDirectory = this.fullPath;
      this.saveFileDialog1.FileName = this.Filename[selectedIndex];
      if (this.saveFileDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      this.Export(selectedIndex, this.saveFileDialog1.FileName, this.openFileDialog1.FileName);
      this.toolStripStatusLabel1.Text = "Export Complete!";
    }

    private void Export(int i, string FileName, string fileName)
    {
      FileStream fileStream1 = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      fileStream1.Position = (long) this.pointer[i];
      byte[] buffer = new byte[this.size[i]];
      fileStream1.Read(buffer, 0, buffer.Length);
      FileStream fileStream2 = new FileStream(FileName, FileMode.Create);
      fileStream2.Write(buffer, 0, buffer.Length);
      fileStream2.Close();
      fileStream1.Close();
    }

    private void menuItem6_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = this.fullPath;
      this.folderBrowserDialog1.Description = "Pick where to extract the files...";
      if (this.folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      for (int i = 0; i < this.listView1.Items.Count; ++i)
        this.Export(i, this.folderBrowserDialog1.SelectedPath + "\\" + this.Filename[i], this.openFileDialog1.FileName);
      this.toolStripStatusLabel1.Text = "Export Complete!";
    }

    private void menuItem7_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = this.fullPath;
      if (this.folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      FileStream fileStream1 = new FileStream(this.openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
      byte[] buffer1 = new byte[this.pointer[0]];
      fileStream1.Read(buffer1, 0, buffer1.Length);
      fileStream1.Close();
      FileStream fileStream2 = new FileStream(this.openFileDialog1.FileName + "_new", FileMode.Create);
      fileStream2.Write(buffer1, 0, buffer1.Length);
      for (int index = 0; index < this.PntrNumber; ++index)
      {
        FileStream fileStream3 = new FileStream(this.folderBrowserDialog1.SelectedPath + "\\" + this.Filename[index], FileMode.Open, FileAccess.Read);
        byte[] buffer2 = new byte[fileStream3.Length];
        fileStream3.Read(buffer2, 0, buffer2.Length);
        long length = fileStream3.Length;
        fileStream3.Close();
        fileStream2.Write(buffer2, 0, buffer2.Length);
        long num = fileStream2.Position - length;
        fileStream2.Position = (long) (20 + index * 16);
        byte[] bytes1 = BitConverter.GetBytes(length);
        fileStream2.Write(bytes1, 0, 4);
        byte[] bytes2 = BitConverter.GetBytes(num);
        fileStream2.Write(bytes2, 0, 4);
        fileStream2.Position = num + length;
      }
      fileStream2.Close();
      this.toolStripStatusLabel1.Text = "Import Complete!";
    }

    private void menuItem10_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = this.fullPath;
      if (this.folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      DirectoryInfo directoryInfo = new DirectoryInfo(this.folderBrowserDialog1.SelectedPath);
      Directory.CreateDirectory(this.folderBrowserDialog1.SelectedPath + "\\Exported");
      foreach (FileInfo file in directoryInfo.GetFiles())
      {
        if (file.Extension.ToLower() == ".ezb")
        {
          this.Openezb(file.FullName);
          Application.DoEvents();
          for (int i = 0; i < this.PntrNumber; ++i)
          {
            Directory.CreateDirectory(this.folderBrowserDialog1.SelectedPath + "\\Exported\\" + file.Name);
            this.Export(i, this.folderBrowserDialog1.SelectedPath + "\\Exported\\" + file.Name + "\\" + this.Filename[i], file.FullName);
          }
        }
      }
      this.toolStripStatusLabel1.Text = "Batch Extraction Complete!";
    }
  }
}
