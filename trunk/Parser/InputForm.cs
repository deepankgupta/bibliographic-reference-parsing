//    This file is part of bibliographic-reference-parsing. 
//    Bibliographic-Reference-Parsing is free software; you can redistribute it
//    and/or modify it under the terms of the GNU General Public License as 
//    published by the Free Software Foundation; either version 3 of the License,
//    or (at your option) any later version.

//    Bibliographic-Reference-Parsing is distributed in the hope that it will be 
//    useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//    Author : Deepank Gupta  (deepankgupta AT gmail DOT com)
//    Date   : 18/08/08


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
            filenameLabel.Text = Common.inputFilePath;
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (finishButton.Text == "Submit")
            {
                if (!fileModeRadio.Checked)
                {
                    totalFiles.Text = Directory.GetFiles(Common.inputFilePath).Length.ToString();
                    noFilesDone.Text = "0";
                }
                backgroundWorker1.RunWorkerAsync();
            }
            else
                this.Close();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {            
        }

        private void InputForm_Activated(object sender, EventArgs e)
        {
            this.Update();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (fileModeRadio.Checked)
            {
                Common.Init();
                Program.Stage1();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.ReportProgress(25);
                Program.Stage2();                
                backgroundWorker1.ReportProgress(50);
                Program.Stage3();
                backgroundWorker1.ReportProgress(70);
                Program.Stage4();
                backgroundWorker1.ReportProgress(100);
            }
            else
            {
                BatchStart();
            }            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if(e.ProgressPercentage == 0)
                filenameLabel.Text = Common.inputFilePath;
            else if (e.ProgressPercentage == 100)
            {
                int temp = Convert.ToInt32(noFilesDone.Text);
                temp = temp + 1;
                noFilesDone.Text = temp.ToString();
                int total = Convert.ToInt32(totalFiles.Text);
                double progress = (float)temp / total * 100.0;
                progressBar2.Value = (int)progress;                
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            finishButton.Text = "Close";
        }

        internal void BatchStart()
        {
            string directory = Common.inputFilePath;
            if (!Directory.Exists(directory))
                return;
            string[] files = Directory.GetFiles(directory);            
            backgroundWorker1.WorkerReportsProgress = true;
            for (int i = 0; i < files.Length; i++)
            {
                Common.inputFilePath = files[i];
                backgroundWorker1.ReportProgress(0);
                Common.Init();                
                Program.Stage1();                
                backgroundWorker1.ReportProgress(25);
                Program.Stage2();
                backgroundWorker1.ReportProgress(50);
                Program.Stage3();
                backgroundWorker1.ReportProgress(70);
                Program.Stage4();
                backgroundWorker1.ReportProgress(100);
            }
        }

        private void fileModeRadio_CheckedChanged(object sender, EventArgs e)
        {
            noFilesDone.Text = "0";
            totalFiles.Text = "1";                
        }
    }
}