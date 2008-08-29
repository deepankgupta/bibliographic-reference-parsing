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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the input File :";
            // 
            // inputFileButton
            // 
            this.inputFileButton.Location = new System.Drawing.Point(179, 148);
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
            this.finishButton.Location = new System.Drawing.Point(179, 213);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 4;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // fileModeRadio
            // 
            this.fileModeRadio.AutoSize = true;
            this.fileModeRadio.Location = new System.Drawing.Point(24, 84);
            this.fileModeRadio.Name = "fileModeRadio";
            this.fileModeRadio.Size = new System.Drawing.Size(103, 17);
            this.fileModeRadio.TabIndex = 5;
            this.fileModeRadio.TabStop = true;
            this.fileModeRadio.Text = "Single File Mode";
            this.fileModeRadio.UseVisualStyleBackColor = true;
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
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 259);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.batchModeRadio);
            this.Controls.Add(this.fileModeRadio);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.inputFileButton);
            this.Controls.Add(this.label1);
            this.Name = "InputForm";
            this.Text = "InputForm";
            this.Load += new System.EventHandler(this.InputForm_Load);
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
    }
}