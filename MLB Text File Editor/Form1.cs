using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MLB_Text_File_Editor
{
  public class Form1 : Form
  {
    private int MasterPointers;
    private int[] Pointernumber;
    private int[] Masters;
    private int[] Pointers;
    private string[] Strings;
    private string file;
    private string path;
    private string[] ASTR;
    private int curr_pointer;
    private int writendx = 0;
    private string folder;
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
    private MenuItem menuItem9;
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private GroupBox groupBox1;
    private TextBox textBox1;
    private GroupBox groupBox2;
    private TextBox textBox2;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ColumnHeader columnHeader4;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private ToolTip toolTip1;

    public Form1() => this.InitializeComponent();

    private void menuItem2_Click(object sender, EventArgs e)
    {
      this.listView1.Items.Clear();
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Open MLB file...";
      openFileDialog.Filter = "MLB Files|*.mlb||";
      if (openFileDialog.ShowDialog() == DialogResult.Cancel)
        return;
      this.file = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
      this.folder = Path.GetDirectoryName(openFileDialog.FileName);
      this.path = openFileDialog.FileName;
      this.toolStripStatusLabel1.Text = this.path;
      this.toolStripStatusLabel1.Visible = true;
      this.menuItem3.Enabled = true;
      FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
      BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(openFileDialog.FileName));
      binaryReader.BaseStream.Position = 3L;
      this.MasterPointers = (int) binaryReader.ReadByte();
      this.Masters = new int[this.MasterPointers + 1];
      for (int index = 0; index < this.MasterPointers; ++index)
      {
        fileStream.Position = (long) (4 + index * 4);
        byte[] buffer = new byte[4];
        fileStream.Read(buffer, 0, buffer.Length);
        this.Masters[index] = BitConverter.ToInt32(buffer, 0);
      }
      this.Masters[this.Masters.Length - 1] = (int) fileStream.Length;
      this.Pointernumber = new int[this.MasterPointers];
      for (int index = 0; index < this.MasterPointers; ++index)
      {
        fileStream.Position = (long) this.Masters[index];
        byte[] buffer = new byte[4];
        fileStream.Read(buffer, 0, buffer.Length);
        this.Pointernumber[index] = BitConverter.ToInt32(buffer, 0);
      }
      for (int index1 = 0; index1 < this.MasterPointers; ++index1)
      {
        this.Pointers = new int[this.Pointernumber[index1] + 1];
        this.Strings = new string[this.Pointernumber[index1]];
        int num = this.Masters[index1] + 4;
        for (int index2 = 0; index2 < this.Pointernumber[index1]; ++index2)
        {
          fileStream.Position = (long) (num + index2 * 4);
          byte[] buffer = new byte[4];
          fileStream.Read(buffer, 0, buffer.Length);
          this.Pointers[index2] = BitConverter.ToInt32(buffer, 0);
        }
        this.Pointers[this.Pointers.Length - 1] = this.Masters[index1 + 1];
        for (int index3 = 0; index3 < this.Pointers.Length - 1; ++index3)
        {
          fileStream.Position = (long) this.Pointers[index3];
          byte[] numArray = new byte[this.Pointers[index3 + 1] >= this.Pointers[index3] ? this.Pointers[index3 + 1] - this.Pointers[index3] : 300];
          fileStream.Read(numArray, 0, numArray.Length);
          this.Strings[index3] = Encoding.GetEncoding("EUC-JP").GetString(numArray).Split(new char[1])[0];
          this.listView1.Items.AddRange(new ListViewItem[1]
          {
            new ListViewItem(new string[4]
            {
              index3.ToString(),
              this.Pointers[index3].ToString("X2"),
              this.Strings[index3].Replace("\n", "<NL>"),
              this.Strings[index3].Replace("\n", "<NL>")
            })
          });
        }
      }
      this.toolStripStatusLabel1.Text = "File accessed:";
      this.toolStripStatusLabel2.ForeColor = Color.DarkRed;
      this.toolStripStatusLabel2.Text = Path.GetFileName(this.path);
      this.toolStripStatusLabel2.ToolTipText = "Click to open Directory";
      this.toolStripStatusLabel2.Visible = true;
      fileStream.Dispose();
      fileStream.Close();
      binaryReader.Dispose();
      binaryReader.Close();
      openFileDialog.Dispose();
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.listView1.SelectedItems.Count <= 0)
        return;
      this.textBox1.Text = new string(this.listView1.SelectedItems[0].SubItems[3].Text.Skip<char>(1).ToArray<char>());
    }

    private void textBox2_TextChanged(object sender, EventArgs e) => this.listView1.SelectedItems[0].SubItems[2].Text = "@" + this.textBox2.Text;

    private void menuItem7_Click(object sender, EventArgs e)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
      saveFileDialog.Title = "Save Text file";
      saveFileDialog.FileName = this.file;
      if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
        return;
      StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.GetEncoding("EUC-JP"));
      for (int index = 0; index < this.listView1.Items.Count; ++index)
        streamWriter.WriteLine((index + 1).ToString() + "＾" + this.listView1.Items[index].SubItems[3].Text.Replace("\n", "<NL>"));
      streamWriter.Close();
    }

    private void menuItem8_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Text Files (*.txt)|*.txt";
      openFileDialog.Title = "Open Text file";
      if (openFileDialog.ShowDialog() == DialogResult.Cancel)
        return;
      StreamReader streamReader = new StreamReader(openFileDialog.FileName, Encoding.GetEncoding("EUC-JP"));
      while (streamReader.Peek() >= 0)
      {
        string[] strArray = streamReader.ReadLine().Split('＾');
        if (strArray.Length == 2)
          this.listView1.Items[int.Parse(strArray[0].ToString()) - 1].SubItems[2].Text = strArray[1];
      }
    }

    private void menuItem3_Click(object sender, EventArgs e)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(this.path);
      this.folder = Path.GetDirectoryName(this.path);
      BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Create(this.folder + "\\" + withoutExtension + "_new.mlb"));
      Encoding encoding = Encoding.GetEncoding("EUC-JP");
      binaryWriter.Write(encoding.GetBytes("MLT"));
      binaryWriter.Write(this.MasterPointers);
      for (int index1 = 0; index1 < this.MasterPointers; ++index1)
      {
        this.curr_pointer = this.Masters[index1] + this.Pointernumber[index1] * 4 + 4;
        binaryWriter.BaseStream.Position = (long) (this.MasterPointers * index1 + 4);
        binaryWriter.Write(this.Masters[index1]);
        binaryWriter.BaseStream.Position = (long) this.Masters[index1];
        binaryWriter.Write(this.Pointernumber[index1]);
        for (int index2 = 0; index2 < this.Pointernumber[index1]; ++index2)
        {
          binaryWriter.BaseStream.Position = (long) (this.Masters[index1] + 4 + index2 * 4);
          binaryWriter.Write(this.curr_pointer);
          binaryWriter.BaseStream.Position = (long) this.curr_pointer;
          binaryWriter.Write(encoding.GetBytes(this.listView1.Items[this.writendx + index2].SubItems[2].Text.Replace("<NL>", "\n") + (object) char.MinValue));
          this.curr_pointer += encoding.GetByteCount(this.listView1.Items[this.writendx + index2].SubItems[2].Text.Replace("<NL>", "\n")) + 1;
        }
        this.writendx += this.Pointernumber[index1];
        this.Masters[index1 + 1] = this.curr_pointer;
      }
      binaryWriter.Write((int) ushort.MaxValue);
      this.toolStripStatusLabel1.Text = "New file created:";
      this.toolStripStatusLabel2.ForeColor = Color.ForestGreen;
      this.toolStripStatusLabel2.Text = withoutExtension + "_new.mlb";
      this.toolStripStatusLabel2.ToolTipText = "Click to open Directory";
      this.toolStripStatusLabel2.Visible = true;
      binaryWriter.Flush();
      binaryWriter.Close();
    }

    private void menuItem5_Click(object sender, EventArgs e) => this.Close();

    private void menuItem9_Click(object sender, EventArgs e)
    {
      int num = (int) MessageBox.Show("By Omarrrio 2013 blablablabla just do me a fucking favor\nand don't redestribute this plz, ok ? thank you :)", "bout dis sheit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
    }

    private void toolStripStatusLabel2_Click(object sender, EventArgs e)
    {
      if (this.folder == null)
        return;
      Process.Start(this.folder);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.mainMenu1 = new MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.menuItem7 = new MenuItem();
      this.menuItem8 = new MenuItem();
      this.menuItem9 = new MenuItem();
      this.listView1 = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.columnHeader3 = new ColumnHeader();
      this.columnHeader4 = new ColumnHeader();
      this.groupBox1 = new GroupBox();
      this.textBox1 = new TextBox();
      this.groupBox2 = new GroupBox();
      this.textBox2 = new TextBox();
      this.statusStrip1 = new StatusStrip();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new ToolStripStatusLabel();
      this.toolTip1 = new ToolTip(this.components);
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem1,
        this.menuItem6,
        this.menuItem9
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[4]
      {
        this.menuItem2,
        this.menuItem3,
        this.menuItem4,
        this.menuItem5
      });
      this.menuItem1.Text = "File";
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "Open";
      this.menuItem2.Click += new EventHandler(this.menuItem2_Click);
      this.menuItem3.Enabled = false;
      this.menuItem3.Index = 1;
      this.menuItem3.Text = "Save";
      this.menuItem3.Click += new EventHandler(this.menuItem3_Click);
      this.menuItem4.Index = 2;
      this.menuItem4.Text = "-";
      this.menuItem5.Index = 3;
      this.menuItem5.Text = "Exit";
      this.menuItem5.Click += new EventHandler(this.menuItem5_Click);
      this.menuItem6.Index = 1;
      this.menuItem6.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem7,
        this.menuItem8
      });
      this.menuItem6.Text = "Edit";
      this.menuItem7.Index = 0;
      this.menuItem7.Text = "Export text File";
      this.menuItem7.Click += new EventHandler(this.menuItem7_Click);
      this.menuItem8.Index = 1;
      this.menuItem8.Text = "Import text File";
      this.menuItem8.Click += new EventHandler(this.menuItem8_Click);
      this.menuItem9.Index = 2;
      this.menuItem9.Text = "About";
      this.menuItem9.Click += new EventHandler(this.menuItem9_Click);
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
      this.listView1.Size = new Size(504, 286);
      this.listView1.TabIndex = 0;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader1.Text = "ID";
      this.columnHeader1.Width = 35;
      this.columnHeader2.Text = "Pointer";
      this.columnHeader3.Text = "New String";
      this.columnHeader3.Width = 184;
      this.columnHeader4.Text = "Old String";
      this.columnHeader4.Width = 216;
      this.groupBox1.Controls.Add((Control) this.textBox1);
      this.groupBox1.Location = new Point(522, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(314, 140);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Japanese Text";
      this.textBox1.BackColor = SystemColors.Window;
      this.textBox1.Location = new Point(6, 19);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new Size(302, 115);
      this.textBox1.TabIndex = 0;
      this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
      this.groupBox2.Controls.Add((Control) this.textBox2);
      this.groupBox2.Location = new Point(522, 158);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(314, 140);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "English Text (it auto-updates the listbox)";
      this.textBox2.Location = new Point(6, 19);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(302, 115);
      this.textBox2.TabIndex = 0;
      this.textBox2.TextChanged += new EventHandler(this.textBox2_TextChanged);
      this.statusStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripStatusLabel1,
        (ToolStripItem) this.toolStripStatusLabel2
      });
      this.statusStrip1.Location = new Point(0, 308);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.ShowItemToolTips = true;
      this.statusStrip1.Size = new Size(848, 22);
      this.statusStrip1.SizingGrip = false;
      this.statusStrip1.Stretch = false;
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new Size(118, 17);
      this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Visible = false;
      this.toolStripStatusLabel2.AutoToolTip = true;
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new Size(118, 17);
      this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Visible = false;
      this.toolStripStatusLabel2.Click += new EventHandler(this.toolStripStatusLabel2_Click);
      this.toolTip1.ToolTipIcon = ToolTipIcon.Info;
      this.toolTip1.ToolTipTitle = "Quick Tip";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(848, 330);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Controls.Add((Control) this.listView1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.Menu = this.mainMenu1;
      this.Name = nameof (Form1);
      this.Text = "MLB Text Editor";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
