using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        string savefilename = "";
        string openfilename = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void open_forth_script_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear(); 
            string text = "";

            try
            {
                my_OFD.ShowDialog();
                openfilename = my_OFD.FileName;

                using (TextReader my_TR1 = new StreamReader(openfilename))
                {
                    while ((text = my_TR1.ReadLine()) != null)
                        richTextBox1.AppendText(text + "\r\n");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void create_nvram_button_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            
            String text = String.Empty;

            try
            {
                my_SFD.FileName = "NVRAM_" + Path.GetFileName(openfilename);
                my_SFD.ShowDialog();
                savefilename = my_SFD.FileName;
                // File.Create(savefilename).Close();
                int lcl_int;
                int lcl_int2;

                    // create a writer and open the file
                    using (TextWriter my_TW = new StreamWriter(savefilename))
                    {

                        my_TW.WriteLine("0x10000 userOpen");
                        my_TW.WriteLine(": put start: userWrite ;");

                        foreach(string str in richTextBox1.Lines)
                        {
                            text = str;
                            text = text.Trim();
                            lcl_int = text.IndexOf("//");
                            if (lcl_int != 0) // -1 is not there and 0 means a whole line comment
                            {
                                if (lcl_int != -1)
                                {
                                    text = text.Substring(0, lcl_int);
                                }

                                while (((lcl_int = text.IndexOf("( ")) >= 0) && ((lcl_int2 = text.IndexOf(" )")) >= 0))
                                {
                                    text = text.Remove(lcl_int, lcl_int2 - lcl_int + 2);
                                }

                                while (text.Length > 60)
                                {
                                    int index = text.LastIndexOf(' ', 60);

                                    string text2 = text.Substring(index, text.Length - index);
                                    text = text.Substring(0, index);
                                    output_to_file(text, my_TW);
                                    text = text2;
                                }

                                int lcl = text.IndexOf("OK");
                                text = (lcl > 0)
                                    ? text.Remove(lcl, "OK".Length)
                                    : text;

                                output_to_file(text, my_TW);
                            }
                        }

                        my_TW.WriteLine("userClose");
                    }

                using (TextReader my_TR2 = new StreamReader(savefilename))
                {
                    while ((text = my_TR2.ReadLine()) != null)
                        richTextBox2.AppendText(text + "\r\n");
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        private static void output_to_file(string text, TextWriter my_TW)
        {
            if ((text.Contains("forget") != true) && (text != ""))
            {
                StringBuilder sb = new StringBuilder(text);

                sb.Replace('\t', ' ');

                text = sb.ToString().Trim();

                if (text != "")
                {
                    my_TW.WriteLine("put " + text);
                }
            }
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.AllowDrop = true;
            richTextBox1.DragEnter += new DragEventHandler(richTextBox1_DragEnter);
            richTextBox1.DragDrop += new DragEventHandler(richTextBox1_DragDrop);
        }

        void richTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void richTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = e.Data.GetData(DataFormats.FileDrop) as string[];
            openfilename = filenames[0];
            string filetype = openfilename.Substring(openfilename.LastIndexOf(@"\") + 1);
            if (filetype.IndexOf(".txt") != -1)
            {
                richTextBox1.LoadFile(openfilename, RichTextBoxStreamType.PlainText);
            }
            else if (filetype.IndexOf(".4th") != -1)
            {
                richTextBox1.LoadFile(openfilename, RichTextBoxStreamType.PlainText);
            }
            else
            {
                MessageBox.Show("Sorry, I cannot support this file type");
            }

        }
    }
}