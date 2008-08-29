using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Parser
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        private void inputFileButton_Click(object sender, EventArgs e)
        {
            if (fileModeRadio.Checked)
            {
                DialogResult result = inputFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Common.inputFilePath = inputFileDialog.FileName;
                    Common.outputFilePath = Path.GetDirectoryName(Common.inputFilePath) +
                        @"\" + Path.GetFileNameWithoutExtension(Common.inputFilePath) + @"_" + @"output.txt";
                }
            }
            else
            {
                DialogResult result = inputFolderDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Common.inputFilePath = inputFolderDialog.SelectedPath;
                }
            }           
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (fileModeRadio.Checked)
            {
                Common.Init();
                Program.Start();                
            }
            else
            {
                BatchTestClass.Start();
            }
            this.Close();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {

        }        
    }
}