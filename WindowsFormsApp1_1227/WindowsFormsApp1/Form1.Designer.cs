namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.startRecording = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.joystickList = new System.Windows.Forms.ListBox();
            this.recorded1 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.stopMacro = new System.Windows.Forms.Button();
            this.runMacro = new System.Windows.Forms.Button();
            this.macro2 = new System.Windows.Forms.TextBox();
            this.macro1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.recorded2 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // startRecording
            // 
            this.startRecording.Location = new System.Drawing.Point(39, 294);
            this.startRecording.Name = "startRecording";
            this.startRecording.Size = new System.Drawing.Size(230, 97);
            this.startRecording.TabIndex = 0;
            this.startRecording.Text = "Start Recording";
            this.startRecording.UseVisualStyleBackColor = true;
            this.startRecording.Click += new System.EventHandler(this.startRecording_Click);
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(725, 289);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(230, 97);
            this.Back.TabIndex = 2;
            this.Back.Text = "Back";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.back_Click);
            // 
            // joystickList
            // 
            this.joystickList.FormattingEnabled = true;
            this.joystickList.ItemHeight = 21;
            this.joystickList.Location = new System.Drawing.Point(39, 35);
            this.joystickList.Name = "joystickList";
            this.joystickList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.joystickList.Size = new System.Drawing.Size(421, 235);
            this.joystickList.TabIndex = 3;
            // 
            // recorded1
            // 
            this.recorded1.Location = new System.Drawing.Point(555, 35);
            this.recorded1.Multiline = true;
            this.recorded1.Name = "recorded1";
            this.recorded1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.recorded1.Size = new System.Drawing.Size(411, 356);
            this.recorded1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1650, 930);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.recorded2);
            this.tabPage1.Controls.Add(this.stopMacro);
            this.tabPage1.Controls.Add(this.runMacro);
            this.tabPage1.Controls.Add(this.macro2);
            this.tabPage1.Controls.Add(this.macro1);
            this.tabPage1.Controls.Add(this.joystickList);
            this.tabPage1.Controls.Add(this.recorded1);
            this.tabPage1.Controls.Add(this.startRecording);
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1642, 895);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Macro / Recording";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // stopMacro
            // 
            this.stopMacro.Location = new System.Drawing.Point(1078, 671);
            this.stopMacro.Name = "stopMacro";
            this.stopMacro.Size = new System.Drawing.Size(242, 100);
            this.stopMacro.TabIndex = 8;
            this.stopMacro.Text = "Stop Macro";
            this.stopMacro.UseVisualStyleBackColor = true;
            this.stopMacro.Click += new System.EventHandler(this.stopMacro_Click);
            // 
            // runMacro
            // 
            this.runMacro.Location = new System.Drawing.Point(1078, 537);
            this.runMacro.Name = "runMacro";
            this.runMacro.Size = new System.Drawing.Size(242, 100);
            this.runMacro.TabIndex = 7;
            this.runMacro.Text = "Run Macro";
            this.runMacro.UseVisualStyleBackColor = true;
            this.runMacro.Click += new System.EventHandler(this.runMacro_Click);
            // 
            // macro2
            // 
            this.macro2.Location = new System.Drawing.Point(555, 537);
            this.macro2.Multiline = true;
            this.macro2.Name = "macro2";
            this.macro2.Size = new System.Drawing.Size(482, 323);
            this.macro2.TabIndex = 6;
            // 
            // macro1
            // 
            this.macro1.Location = new System.Drawing.Point(39, 537);
            this.macro1.Multiline = true;
            this.macro1.Name = "macro1";
            this.macro1.Size = new System.Drawing.Size(482, 323);
            this.macro1.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.Back);
            this.tabPage2.Location = new System.Drawing.Point(4, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1379, 895);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "vJoy Direct Control";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // recorded2
            // 
            this.recorded2.Location = new System.Drawing.Point(995, 35);
            this.recorded2.Multiline = true;
            this.recorded2.Name = "recorded2";
            this.recorded2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.recorded2.Size = new System.Drawing.Size(411, 356);
            this.recorded2.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1855, 954);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startRecording;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.ListBox joystickList;
        private System.Windows.Forms.TextBox recorded1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button stopMacro;
        private System.Windows.Forms.Button runMacro;
        private System.Windows.Forms.TextBox macro2;
        private System.Windows.Forms.TextBox macro1;
        private System.Windows.Forms.TextBox recorded2;
    }
}

