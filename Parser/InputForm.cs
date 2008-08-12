using System;
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
            DialogResult result = inputFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Common.inputFilePath = inputFileDialog.FileName;
            }
        }

        private void outputFileButton_Click(object sender, EventArgs e)
        {
            DialogResult result = outputFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Common.outputFilePath = outputFileDialog.FileName;
            }
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            Common.Init();
            Program.Start();
            this.Close();
        }        
    }
}
