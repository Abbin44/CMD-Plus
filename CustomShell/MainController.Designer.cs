
namespace CustomShell
{
    partial class MainController
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainController));
            this.outputBox = new System.Windows.Forms.RichTextBox();
            this.inputBox = new System.Windows.Forms.RichTextBox();
            this.wandTextBox = new System.Windows.Forms.RichTextBox();
            this.sshTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // outputBox
            // 
            this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.outputBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(119)))), ((int)(((byte)(68)))));
            this.outputBox.HideSelection = false;
            this.outputBox.Location = new System.Drawing.Point(0, 0);
            this.outputBox.Name = "outputBox";
            this.outputBox.ReadOnly = true;
            this.outputBox.Size = new System.Drawing.Size(1038, 497);
            this.outputBox.TabIndex = 0;
            this.outputBox.TabStop = false;
            this.outputBox.Text = "";
            // 
            // inputBox
            // 
            this.inputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.inputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(119)))), ((int)(((byte)(68)))));
            this.inputBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.inputBox.Location = new System.Drawing.Point(0, 496);
            this.inputBox.Multiline = false;
            this.inputBox.Name = "inputBox";
            this.inputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            this.inputBox.Size = new System.Drawing.Size(1038, 26);
            this.inputBox.TabIndex = 1;
            this.inputBox.TabStop = false;
            this.inputBox.Text = "";
            this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyDown);
            this.inputBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyUp);
            // 
            // wandTextBox
            // 
            this.wandTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.wandTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wandTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wandTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(119)))), ((int)(((byte)(68)))));
            this.wandTextBox.HideSelection = false;
            this.wandTextBox.Location = new System.Drawing.Point(0, 0);
            this.wandTextBox.Name = "wandTextBox";
            this.wandTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.wandTextBox.Size = new System.Drawing.Size(1038, 496);
            this.wandTextBox.TabIndex = 2;
            this.wandTextBox.TabStop = false;
            this.wandTextBox.Text = "";
            this.wandTextBox.Visible = false;
            this.wandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.wandTextBox_KeyDown);
            // 
            // sshTextBox
            // 
            this.sshTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.sshTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sshTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sshTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(119)))), ((int)(((byte)(68)))));
            this.sshTextBox.HideSelection = false;
            this.sshTextBox.Location = new System.Drawing.Point(0, 0);
            this.sshTextBox.Name = "sshTextBox";
            this.sshTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.sshTextBox.Size = new System.Drawing.Size(1038, 496);
            this.sshTextBox.TabIndex = 3;
            this.sshTextBox.TabStop = false;
            this.sshTextBox.Text = "";
            this.sshTextBox.Visible = false;
            // 
            // MainController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(1038, 522);
            this.Controls.Add(this.sshTextBox);
            this.Controls.Add(this.wandTextBox);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.outputBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainController";
            this.Text = "CMD++";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainController_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.RichTextBox outputBox;
        public System.Windows.Forms.RichTextBox inputBox;
        public System.Windows.Forms.RichTextBox wandTextBox;
        public System.Windows.Forms.RichTextBox sshTextBox;
    }
}

