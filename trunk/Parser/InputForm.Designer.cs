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


namespace Parser
{
    partial class InputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.inputFileButton = new System.Windows.Forms.Button();
            this.inputFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.finishButton = new System.Windows.Forms.Button();
            this.inputFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.fileModeRadio = new System.Windows.Forms.RadioButton();
            this.batchModeRadio = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.noFilesDone = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.filenameLabel = new System.Windows.Forms.Label();
            this.slashLabel = new System.Windows.Forms.Label();
            this.totalFiles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the input File :";
            // 
            // inputFileButton
            // 
            this.inputFileButton.Location = new System.Drawing.Point(179, 153);
            this.inputFileButton.Name = "inputFileButton";
            this.inputFileButton.Size = new System.Drawing.Size(75, 23);
            this.inputFileButton.TabIndex = 1;
            this.inputFileButton.Text = "Browse ...";
            this.inputFileButton.UseVisualStyleBackColor = true;
            this.inputFileButton.Click += new System.EventHandler(this.inputFileButton_Click);
            // 
            // inputFileDialog
            // 
            this.inputFileDialog.FileName = "openFileDialog1";
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(179, 344);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 4;
            this.finishButton.Text = "Submit";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // fileModeRadio
            // 
            this.fileModeRadio.AutoSize = true;
            this.fileModeRadio.Checked = true;
            this.fileModeRadio.Location = new System.Drawing.Point(24, 84);
            this.fileModeRadio.Name = "fileModeRadio";
            this.fileModeRadio.Size = new System.Drawing.Size(103, 17);
            this.fileModeRadio.TabIndex = 5;
            this.fileModeRadio.TabStop = true;
            this.fileModeRadio.Text = "Single File Mode";
            this.fileModeRadio.UseVisualStyleBackColor = true;
            this.fileModeRadio.CheckedChanged += new System.EventHandler(this.fileModeRadio_CheckedChanged);
            // 
            // batchModeRadio
            // 
            this.batchModeRadio.AutoSize = true;
            this.batchModeRadio.Location = new System.Drawing.Point(179, 84);
            this.batchModeRadio.Name = "batchModeRadio";
            this.batchModeRadio.Size = new System.Drawing.Size(83, 17);
            this.batchModeRadio.TabIndex = 6;
            this.batchModeRadio.TabStop = true;
            this.batchModeRadio.Text = "Batch Mode";
            this.batchModeRadio.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Select the mode";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(24, 301);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(230, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // noFilesDone
            // 
            this.noFilesDone.AutoSize = true;
            this.noFilesDone.Location = new System.Drawing.Point(21, 223);
            this.noFilesDone.Name = "noFilesDone";
            this.noFilesDone.Size = new System.Drawing.Size(13, 13);
            this.noFilesDone.TabIndex = 9;
            this.noFilesDone.Text = "0";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(24, 239);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(230, 23);
            this.progressBar2.TabIndex = 10;
            // 
            // filenameLabel
            // 
            this.filenameLabel.AutoSize = true;
            this.filenameLabel.Location = new System.Drawing.Point(21, 285);
            this.filenameLabel.Name = "filenameLabel";
            this.filenameLabel.Size = new System.Drawing.Size(35, 13);
            this.filenameLabel.TabIndex = 11;
            this.filenameLabel.Text = "label4";
            // 
            // slashLabel
            // 
            this.slashLabel.AutoSize = true;
            this.slashLabel.Location = new System.Drawing.Point(44, 223);
            this.slashLabel.Name = "slashLabel";
            this.slashLabel.Size = new System.Drawing.Size(12, 13);
            this.slashLabel.TabIndex = 12;
            this.slashLabel.Text = "/";
            // 
            // totalFiles
            // 
            this.totalFiles.AutoSize = true;
            this.totalFiles.Location = new System.Drawing.Point(62, 223);
            this.totalFiles.Name = "totalFiles";
            this.totalFiles.Size = new System.Drawing.Size(13, 13);
            this.totalFiles.TabIndex = 13;
            this.totalFiles.Text = "1";
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 379);
            this.Controls.Add(this.totalFiles);
            this.Controls.Add(this.slashLabel);
            this.Controls.Add(this.filenameLabel);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.noFilesDone);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.batchModeRadio);
            this.Controls.Add(this.fileModeRadio);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.inputFileButton);
            this.Controls.Add(this.label1);
            this.Name = "InputForm";
            this.Text = "InputForm";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.Activated += new System.EventHandler(this.InputForm_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button inputFileButton;
        private System.Windows.Forms.OpenFileDialog inputFileDialog;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.FolderBrowserDialog inputFolderDialog;
        private System.Windows.Forms.RadioButton fileModeRadio;
        private System.Windows.Forms.RadioButton batchModeRadio;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label noFilesDone;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label filenameLabel;
        private System.Windows.Forms.Label slashLabel;
        private System.Windows.Forms.Label totalFiles;
    }
}