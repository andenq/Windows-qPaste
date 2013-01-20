namespace Windows_qPaste
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.CombineZIPCheckbox = new System.Windows.Forms.CheckBox();
            this.AutostartCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "qPaste - Quickly paste to the cloud";
            this.notifyIcon.BalloonTipTitle = "qPaste";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "qPaste";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "qShare";
            // 
            // CombineZIPCheckbox
            // 
            this.CombineZIPCheckbox.AutoSize = true;
            this.CombineZIPCheckbox.Location = new System.Drawing.Point(17, 53);
            this.CombineZIPCheckbox.Name = "CombineZIPCheckbox";
            this.CombineZIPCheckbox.Size = new System.Drawing.Size(326, 21);
            this.CombineZIPCheckbox.TabIndex = 1;
            this.CombineZIPCheckbox.Text = "Combine to ZIP when multiple files are selected";
            this.CombineZIPCheckbox.UseVisualStyleBackColor = true;
            this.CombineZIPCheckbox.CheckedChanged += new System.EventHandler(this.CombineZIPCheckbox_CheckedChanged);
            // 
            // AutostartCheckbox
            // 
            this.AutostartCheckbox.AutoSize = true;
            this.AutostartCheckbox.Location = new System.Drawing.Point(17, 80);
            this.AutostartCheckbox.Name = "AutostartCheckbox";
            this.AutostartCheckbox.Size = new System.Drawing.Size(148, 21);
            this.AutostartCheckbox.TabIndex = 2;
            this.AutostartCheckbox.Text = "Start with Windows";
            this.AutostartCheckbox.UseVisualStyleBackColor = true;
            this.AutostartCheckbox.CheckedChanged += new System.EventHandler(this.AutostartCheckbox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 255);
            this.Controls.Add(this.AutostartCheckbox);
            this.Controls.Add(this.CombineZIPCheckbox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "qShare";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CombineZIPCheckbox;
        private System.Windows.Forms.CheckBox AutostartCheckbox;
    }
}

