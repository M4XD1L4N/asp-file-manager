namespace FileCipherClient
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openFileDialog = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            checkBoxPnl = new Panel();
            accessTokenTB = new TextBox();
            fileInfo = new Label();
            filePath = new TextBox();
            openPathBtn = new Button();
            startBtn = new Button();
            infoLbl = new Label();
            mainPB = new ProgressBar();
            uploadCB = new CheckBox();
            mainPanel = new Panel();
            downloadCB = new CheckBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            checkBoxPnl.SuspendLayout();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // checkBoxPnl
            // 
            checkBoxPnl.Controls.Add(accessTokenTB);
            checkBoxPnl.Controls.Add(fileInfo);
            checkBoxPnl.Controls.Add(filePath);
            checkBoxPnl.Controls.Add(openPathBtn);
            checkBoxPnl.Controls.Add(startBtn);
            checkBoxPnl.Controls.Add(infoLbl);
            checkBoxPnl.Location = new Point(35, 23);
            checkBoxPnl.Name = "checkBoxPnl";
            checkBoxPnl.Size = new Size(807, 369);
            checkBoxPnl.TabIndex = 1;
            // 
            // accessTokenTB
            // 
            accessTokenTB.Location = new Point(13, 311);
            accessTokenTB.Name = "accessTokenTB";
            accessTokenTB.Size = new Size(791, 27);
            accessTokenTB.TabIndex = 7;
            accessTokenTB.TextChanged += accessTokenTB_TextChanged;
            // 
            // fileInfo
            // 
            fileInfo.AutoSize = true;
            fileInfo.Location = new Point(96, 105);
            fileInfo.Name = "fileInfo";
            fileInfo.Size = new Size(0, 20);
            fileInfo.TabIndex = 6;
            // 
            // filePath
            // 
            filePath.Location = new Point(96, 58);
            filePath.Name = "filePath";
            filePath.Size = new Size(299, 27);
            filePath.TabIndex = 5;
            filePath.Text = "C:\\";
            // 
            // openPathBtn
            // 
            openPathBtn.Location = new Point(421, 56);
            openPathBtn.Name = "openPathBtn";
            openPathBtn.Size = new Size(94, 29);
            openPathBtn.TabIndex = 4;
            openPathBtn.Text = "Open";
            openPathBtn.UseVisualStyleBackColor = true;
            openPathBtn.Click += openPathBtn_Click;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(301, 265);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(94, 29);
            startBtn.TabIndex = 3;
            startBtn.Text = "start";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_ClickAsync;
            // 
            // infoLbl
            // 
            infoLbl.AutoSize = true;
            infoLbl.Location = new Point(13, 9);
            infoLbl.Name = "infoLbl";
            infoLbl.Size = new Size(0, 20);
            infoLbl.TabIndex = 1;
            // 
            // mainPB
            // 
            mainPB.Location = new Point(12, 517);
            mainPB.Name = "mainPB";
            mainPB.Size = new Size(1274, 29);
            mainPB.TabIndex = 2;
            // 
            // uploadCB
            // 
            uploadCB.AutoSize = true;
            uploadCB.Location = new Point(24, 19);
            uploadCB.Name = "uploadCB";
            uploadCB.Size = new Size(78, 24);
            uploadCB.TabIndex = 3;
            uploadCB.Text = "upload";
            uploadCB.UseVisualStyleBackColor = true;
            uploadCB.CheckedChanged += uploadCB_CheckedChanged;
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(downloadCB);
            mainPanel.Controls.Add(uploadCB);
            mainPanel.Location = new Point(889, 23);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(250, 125);
            mainPanel.TabIndex = 4;
            // 
            // downloadCB
            // 
            downloadCB.AutoSize = true;
            downloadCB.Checked = true;
            downloadCB.CheckState = CheckState.Checked;
            downloadCB.Location = new Point(23, 55);
            downloadCB.Name = "downloadCB";
            downloadCB.Size = new Size(98, 24);
            downloadCB.TabIndex = 4;
            downloadCB.Text = "download";
            downloadCB.UseVisualStyleBackColor = true;
            downloadCB.CheckedChanged += downloadCB_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1315, 569);
            Controls.Add(mainPanel);
            Controls.Add(mainPB);
            Controls.Add(checkBoxPnl);
            Name = "MainForm";
            Text = "FileCipherClient";
            checkBoxPnl.ResumeLayout(false);
            checkBoxPnl.PerformLayout();
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog1;
        private Panel checkBoxPnl;
        private Button startBtn;
        private Label infoLbl;
        private ProgressBar mainPB;
        private CheckBox uploadCB;
        private Panel mainPanel;
        private CheckBox downloadCB;
        private Label fileInfo;
        private TextBox filePath;
        private Button openPathBtn;
        private TextBox accessTokenTB;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}