using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Namco_file_project
{
  public class Form1 : Form
  {
    private Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string>()
    {
      {
        '0',
        "0000"
      },
      {
        '1',
        "0001"
      },
      {
        '2',
        "0010"
      },
      {
        '3',
        "0011"
      },
      {
        '4',
        "0100"
      },
      {
        '5',
        "0101"
      },
      {
        '6',
        "0110"
      },
      {
        '7',
        "0111"
      },
      {
        '8',
        "1000"
      },
      {
        '9',
        "1001"
      },
      {
        'a',
        "1010"
      },
      {
        'b',
        "1011"
      },
      {
        'c',
        "1100"
      },
      {
        'd',
        "1101"
      },
      {
        'e',
        "1110"
      },
      {
        'f',
        "1111"
      }
    };
    private int PointerNumber;
    private int[] pointers;
    private string fullpath;
    private string[] flag;
    private string[] padding;
    private IContainer components = (IContainer) null;
    private Button button1;
    private Button button2;
    private ToolTip toolTip1;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private Button button3;

    public Form1() => this.InitializeComponent();

    private void button1_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.Title = "Open Namco.bdi";
      openFileDialog1.Filter = "bdi Files|*.bdi||";
      if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
        return;
      BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(openFileDialog1.FileName));
      binaryReader.BaseStream.Position = 6L;
      OpenFileDialog openFileDialog2;
      switch (binaryReader.ReadInt32())
      {
        case 2475:
          this.toolStripStatusLabel1.Text = "File Status:";
          this.toolStripStatusLabel2.Text = "Loaded!";
          this.toolStripStatusLabel2.ToolTipText = "File Name: " + Path.GetFileName(openFileDialog1.FileName) + "\nFile Location: " + Path.GetDirectoryName(openFileDialog1.FileName);
          this.toolStripStatusLabel2.ForeColor = Color.Chartreuse;
          this.toolStripStatusLabel1.Visible = true;
          this.toolStripStatusLabel2.Visible = true;
          binaryReader.BaseStream.Position = 65540L;
          this.PointerNumber = binaryReader.ReadInt32();
          FileStream fileStream1 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
          this.pointers = new int[this.PointerNumber + 1];
          this.flag = new string[this.PointerNumber];
          this.padding = new string[this.PointerNumber];
          for (int index = 0; index < this.PointerNumber; ++index)
          {
            byte[] buffer = new byte[4];
            fileStream1.Position = (long) (65556 + index * 8);
            fileStream1.Read(buffer, 0, buffer.Length);
            int int32 = BitConverter.ToInt32(buffer, 0);
            string source1 = int32.ToString("X");
            if (source1.Count<char>() < 8)
              source1 = new string('0', 8 - source1.Count<char>()) + int32.ToString("X");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in source1)
              stringBuilder.Append(this.hexCharacterToBinary[char.ToLower(c)]);
            string source2 = stringBuilder.ToString();
            this.flag[index] = new string(source2.Take<char>(1).ToArray<char>());
            string str = new string(source2.Skip<char>(1).Take<char>(20).ToArray<char>());
            this.padding[index] = new string(source2.Skip<char>(21).Take<char>(11).ToArray<char>());
            this.pointers[index] = Convert.ToInt32(str, 2) * 2048;
          }
          this.pointers[this.pointers.Length - 1] = (int) fileStream1.Length;
          this.toolStripStatusLabel2.Visible = false;
          this.toolStripStatusLabel1.Text = "Now Pick a Folder...";
          FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
          folderBrowserDialog1.Description = "Pick where to extract the archive's content...";
          folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(openFileDialog1.FileName);
          if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
            break;
          this.toolStripStatusLabel2.Text = "Creating 'Unpacked' Directory in chosen folder...";
          Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + "\\Unpacked");
          StreamWriter streamWriter1 = new StreamWriter(folderBrowserDialog1.SelectedPath + "\\Unpacked\\Do_Not_Touch_Me.txt", false, Encoding.Unicode);
          for (int index = 0; index < this.PointerNumber; ++index)
            streamWriter1.WriteLine((index + 1).ToString() + "＾" + this.flag[index] + "＾" + this.padding[index]);
          streamWriter1.Close();
          this.toolStripStatusLabel2.Text = "Extracting Header...";
          byte[] buffer1 = new byte[184320];
          fileStream1.Position = 0L;
          fileStream1.Read(buffer1, 0, buffer1.Length);
          FileStream fileStream2 = new FileStream(folderBrowserDialog1.SelectedPath + "\\Unpacked\\header", FileMode.Create);
          fileStream2.Write(buffer1, 0, buffer1.Length);
          fileStream2.Close();
          this.toolStripStatusLabel2.Text = "Header Extracted!";
          int num1 = MessageBox.Show("Would you like to add extentions to known headers ?\nWARNING:\nYou cannot repack files with extentions !\nMake sure you have all files extracted somewhere before extracting with Extentions !", "Extentions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes ? 0 : 1;
          string str1 = "";
          for (int index = 0; index < this.PointerNumber; ++index)
          {
            this.toolStripStatusLabel2.Text = "Extracting " + (index + 1).ToString() + "/" + this.PointerNumber.ToString();
            fileStream1.Position = (long) this.pointers[index];
            byte[] buffer2 = new byte[this.pointers[index + 1] - this.pointers[index]];
            fileStream1.Read(buffer2, 0, buffer2.Length);
            fileStream1.Position = (long) this.pointers[index];
            if (num1 == 1)
            {
              byte[] numArray = new byte[4];
              fileStream1.Read(numArray, 0, numArray.Length);
              switch (Encoding.ASCII.GetString(numArray))
              {
                case "EZBI":
                  str1 = ".arc";
                  break;
                case "PSMF":
                  str1 = ".pmf";
                  break;
              }
            }
            FileStream fileStream3 = new FileStream(folderBrowserDialog1.SelectedPath + "\\Unpacked\\" + (index + 1).ToString() + str1, FileMode.Create);
            fileStream3.Write(buffer2, 0, buffer2.Length);
            fileStream3.Close();
          }
          this.toolStripStatusLabel2.Text = "Extraction of all " + this.PointerNumber.ToString() + " files is complete! Enjoy :)";
          fileStream1.Close();
          openFileDialog1.Dispose();
          openFileDialog2 = (OpenFileDialog) null;
          break;
        case 5947:
          this.toolStripStatusLabel1.Text = "File Status:";
          this.toolStripStatusLabel2.Text = "Loaded!";
          this.toolStripStatusLabel2.ToolTipText = "File Name: " + Path.GetFileName(openFileDialog1.FileName) + "\nFile Location: " + Path.GetDirectoryName(openFileDialog1.FileName);
          this.toolStripStatusLabel2.ForeColor = Color.Chartreuse;
          this.toolStripStatusLabel1.Visible = true;
          this.toolStripStatusLabel2.Visible = true;
          binaryReader.BaseStream.Position = 32772L;
          this.PointerNumber = binaryReader.ReadInt32();
          FileStream fileStream4 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
          this.pointers = new int[this.PointerNumber + 1];
          this.flag = new string[this.PointerNumber];
          this.padding = new string[this.PointerNumber];
          for (int index = 0; index < this.PointerNumber; ++index)
          {
            byte[] buffer3 = new byte[4];
            fileStream4.Position = (long) (32788 + index * 8);
            fileStream4.Read(buffer3, 0, buffer3.Length);
            string str2 = BitConverter.ToInt32(buffer3, 0).ToString("X8");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str2)
              stringBuilder.Append(this.hexCharacterToBinary[char.ToLower(c)]);
            string source = stringBuilder.ToString();
            this.flag[index] = new string(source.Take<char>(1).ToArray<char>());
            string str3 = new string(source.Skip<char>(1).Take<char>(20).ToArray<char>());
            this.padding[index] = new string(source.Skip<char>(21).Take<char>(11).ToArray<char>());
            this.pointers[index] = Convert.ToInt32(str3, 2) * 2048;
          }
          this.pointers[this.pointers.Length - 1] = (int) fileStream4.Length;
          this.toolStripStatusLabel2.Visible = false;
          this.toolStripStatusLabel1.Text = "Now Pick a Folder...";
          FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog();
          folderBrowserDialog2.Description = "Pick where to extract the archive's content...";
          folderBrowserDialog2.SelectedPath = Path.GetDirectoryName(openFileDialog1.FileName);
          if (folderBrowserDialog2.ShowDialog() == DialogResult.Cancel)
            break;
          this.toolStripStatusLabel2.Text = "Creating 'Unpacked' Directory in chosen folder...";
          Directory.CreateDirectory(folderBrowserDialog2.SelectedPath + "\\Unpacked");
          StreamWriter streamWriter2 = new StreamWriter(folderBrowserDialog2.SelectedPath + "\\Unpacked\\Do_Not_Touch_Me.txt", false, Encoding.Unicode);
          for (int index = 0; index < this.PointerNumber; ++index)
            streamWriter2.WriteLine((index + 1).ToString() + "＾" + this.flag[index] + "＾" + this.padding[index]);
          streamWriter2.Close();
          this.toolStripStatusLabel2.Text = "Extracting Header...";
          byte[] buffer4 = new byte[184320];
          fileStream4.Position = 0L;
          fileStream4.Read(buffer4, 0, buffer4.Length);
          FileStream fileStream5 = new FileStream(folderBrowserDialog2.SelectedPath + "\\Unpacked\\header", FileMode.Create);
          fileStream5.Write(buffer4, 0, buffer4.Length);
          fileStream5.Close();
          this.toolStripStatusLabel2.Text = "Header Extracted!";
          int num2 = MessageBox.Show("Would you like to add extentions to known headers ?\nWARNING:\nYou cannot repack files with extentions !\nMake sure you have all files extracted somewhere before extracting with Extentions !", "Extentions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes ? 0 : 1;
          string str4 = "";
          for (int index = 0; index < this.PointerNumber; ++index)
          {
            this.toolStripStatusLabel2.Text = "Extracting " + (index + 1).ToString() + "/" + this.PointerNumber.ToString();
            fileStream4.Position = (long) this.pointers[index];
            byte[] buffer5 = new byte[this.pointers[index + 1] - this.pointers[index]];
            fileStream4.Read(buffer5, 0, buffer5.Length);
            fileStream4.Position = (long) this.pointers[index];
            if (num2 == 1)
            {
              byte[] numArray = new byte[4];
              fileStream4.Read(numArray, 0, numArray.Length);
              switch (Encoding.ASCII.GetString(numArray))
              {
                case "EZBI":
                  str4 = ".arc";
                  break;
                case "PSMF":
                  str4 = ".pmf";
                  break;
              }
            }
            FileStream fileStream6 = new FileStream(folderBrowserDialog2.SelectedPath + "\\Unpacked\\" + (index + 1).ToString() + str4, FileMode.Create);
            fileStream6.Write(buffer5, 0, buffer5.Length);
            fileStream6.Close();
          }
          this.toolStripStatusLabel2.Text = "Extraction of all " + this.PointerNumber.ToString() + " files is complete! Enjoy :)";
          fileStream4.Close();
          openFileDialog1.Dispose();
          openFileDialog2 = (OpenFileDialog) null;
          break;
        default:
          int num3 = (int) MessageBox.Show("The file you are trying to open is either Invalid or Corrupt!", "Error: Invalid File!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          break;
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Are you repacking for:\nTales of Heroes(YES)\nTales of the World 3(NO)", "Pick the proper Game", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
      {
        this.PointerNumber = 14738;
        this.flag = new string[this.PointerNumber];
        this.padding = new string[this.PointerNumber];
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.Description = "Pick the desired folder to repack...";
        if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
          return;
        StreamReader streamReader = new StreamReader(folderBrowserDialog.SelectedPath + "\\Do_Not_Touch_Me.txt", Encoding.Unicode);
        while (streamReader.Peek() >= 0)
        {
          string[] strArray = streamReader.ReadLine().Split('＾');
          if (strArray.Length == 3)
          {
            int num = int.Parse(strArray[0].ToString());
            this.flag[num - 1] = strArray[1];
            this.padding[num - 1] = strArray[2];
          }
        }
        FileStream fileStream1 = new FileStream(folderBrowserDialog.SelectedPath + "\\header", FileMode.Open, FileAccess.Read);
        byte[] buffer1 = new byte[fileStream1.Length];
        fileStream1.Read(buffer1, 0, buffer1.Length);
        fileStream1.Close();
        FileStream fileStream2 = new FileStream(folderBrowserDialog.SelectedPath + "\\New_Namco.bdi", FileMode.Create);
        fileStream2.Write(buffer1, 0, buffer1.Length);
        this.toolStripStatusLabel1.Visible = true;
        this.toolStripStatusLabel1.Text = "Repacking in progress...";
        for (int index = 0; index < this.PointerNumber; ++index)
        {
          FileStream fileStream3 = new FileStream(folderBrowserDialog.SelectedPath + "\\" + (index + 1).ToString(), FileMode.Open, FileAccess.Read);
          byte[] buffer2 = new byte[fileStream3.Length];
          fileStream3.Read(buffer2, 0, buffer2.Length);
          long length = fileStream3.Length;
          fileStream3.Close();
          fileStream2.Write(buffer2, 0, buffer2.Length);
          int num1 = 2048;
          long num2 = (fileStream2.Position - length) / (long) num1;
          string str = num2.ToString("X");
          StringBuilder stringBuilder = new StringBuilder();
          foreach (char c in str)
            stringBuilder.Append(this.hexCharacterToBinary[char.ToLower(c)]);
          string source = stringBuilder.ToString();
          if (source.Count<char>() < 20)
            source = new string('0', 20 - source.Count<char>()) + stringBuilder.ToString();
          int int32 = Convert.ToInt32(this.flag[index] + source + this.padding[index], 2);
          fileStream2.Position = (long) (65556 + index * 8);
          byte[] bytes = BitConverter.GetBytes(int32);
          fileStream2.Write(bytes, 0, 4);
          fileStream2.Position = num2 * (long) num1 + length;
        }
        fileStream2.Close();
        this.toolStripStatusLabel1.Text = "Repack Complete!";
      }
      else
      {
        this.PointerNumber = 7344;
        this.flag = new string[this.PointerNumber];
        this.padding = new string[this.PointerNumber];
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.Description = "Pick the desired folder to repack...";
        if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
          return;
        StreamReader streamReader = new StreamReader(folderBrowserDialog.SelectedPath + "\\Do_Not_Touch_Me.txt", Encoding.Unicode);
        while (streamReader.Peek() >= 0)
        {
          string[] strArray = streamReader.ReadLine().Split('＾');
          if (strArray.Length == 3)
          {
            int num = int.Parse(strArray[0].ToString());
            this.flag[num - 1] = strArray[1];
            this.padding[num - 1] = strArray[2];
          }
        }
        FileStream fileStream4 = new FileStream(folderBrowserDialog.SelectedPath + "\\header", FileMode.Open, FileAccess.Read);
        byte[] buffer3 = new byte[fileStream4.Length];
        fileStream4.Read(buffer3, 0, buffer3.Length);
        fileStream4.Close();
        FileStream fileStream5 = new FileStream(folderBrowserDialog.SelectedPath + "\\New_Namco.bdi", FileMode.Create);
        fileStream5.Write(buffer3, 0, buffer3.Length);
        this.toolStripStatusLabel1.Visible = true;
        this.toolStripStatusLabel1.Text = "Repacking in progress...";
        for (int index = 0; index < this.PointerNumber; ++index)
        {
          FileStream fileStream6 = new FileStream(folderBrowserDialog.SelectedPath + "\\" + (index + 1).ToString(), FileMode.Open, FileAccess.Read);
          byte[] buffer4 = new byte[fileStream6.Length];
          fileStream6.Read(buffer4, 0, buffer4.Length);
          long length = fileStream6.Length;
          fileStream6.Close();
          fileStream5.Write(buffer4, 0, buffer4.Length);
          int num3 = 2048;
          long num4 = (fileStream5.Position - length) / (long) num3;
          string str = num4.ToString("X");
          StringBuilder stringBuilder = new StringBuilder();
          foreach (char c in str)
            stringBuilder.Append(this.hexCharacterToBinary[char.ToLower(c)]);
          string source = stringBuilder.ToString();
          if (source.Count<char>() < 20)
            source = new string('0', 20 - source.Count<char>()) + stringBuilder.ToString();
          int int32 = Convert.ToInt32(this.flag[index] + source + this.padding[index], 2);
          fileStream5.Position = (long) (32788 + index * 8);
          byte[] bytes = BitConverter.GetBytes(int32);
          fileStream5.Write(bytes, 0, 4);
          fileStream5.Position = num4 * (long) num3 + length;
        }
        fileStream5.Close();
        this.toolStripStatusLabel1.Text = "Repack Complete!";
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      int num = (int) MessageBox.Show("Namco.bdi Extractor/rePacker 0.4 Alpha\nby Omarrrio\nDO NOT DISTRIBUTE THIS SHIT !!!!!\n\nCopyright©2013-ZOA Soft", "About this tool...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
      this.button1 = new Button();
      this.button2 = new Button();
      this.toolTip1 = new ToolTip(this.components);
      this.statusStrip1 = new StatusStrip();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new ToolStripStatusLabel();
      this.button3 = new Button();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.button1.Location = new Point(12, 12);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Unpack";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.Location = new Point(93, 12);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 1;
      this.button2.Text = "Rebuild";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.statusStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripStatusLabel1,
        (ToolStripItem) this.toolStripStatusLabel2
      });
      this.statusStrip1.Location = new Point(0, 48);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(380, 22);
      this.statusStrip1.SizingGrip = false;
      this.statusStrip1.Stretch = false;
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new Size(38, 17);
      this.toolStripStatusLabel1.Text = "TSSL1";
      this.toolStripStatusLabel1.Visible = false;
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new Size(38, 17);
      this.toolStripStatusLabel2.Text = "TSSL1";
      this.toolStripStatusLabel2.Visible = false;
      this.button3.Location = new Point(293, 12);
      this.button3.Name = "button3";
      this.button3.Size = new Size(75, 23);
      this.button3.TabIndex = 4;
      this.button3.Text = "About";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new EventHandler(this.button3_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(380, 70);
      this.Controls.Add((Control) this.button3);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = nameof (Form1);
      this.Text = "Namco.bdi File Unpacker/Rebuilder 0.4 Alpha";
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
