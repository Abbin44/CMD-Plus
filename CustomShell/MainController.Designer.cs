
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
            this.outputBox = new System.Windows.Forms.RichTextBox();
            this.inputBox = new System.Windows.Forms.RichTextBox();
            this.wandTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // outputBox
            // 
            this.outputBox.BackColor = System.Drawing.Color.Black;
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.outputBox.HideSelection = false;
            this.outputBox.Location = new System.Drawing.Point(0, 0);
            this.outputBox.Name = "outputBox";
            this.outputBox.ReadOnly = true;
            this.outputBox.Size = new System.Drawing.Size(928, 460);
            this.outputBox.TabIndex = 0;
            this.outputBox.TabStop = false;
            this.outputBox.Text = "";
            // 
            // inputBox
            // 
            this.inputBox.BackColor = System.Drawing.Color.Black;
            this.inputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputBox.ForeColor = System.Drawing.Color.Red;
            this.inputBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.inputBox.Location = new System.Drawing.Point(0, 434);
            this.inputBox.Multiline = false;
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(928, 26);
            this.inputBox.TabIndex = 1;
            this.inputBox.TabStop = false;
            this.inputBox.Text = "";
            this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyDown);
            // 
            // wandTextBox
            // 
            this.wandTextBox.BackColor = System.Drawing.Color.Black;
            this.wandTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wandTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wandTextBox.ForeColor = System.Drawing.Color.Aqua;
            this.wandTextBox.HideSelection = false;
            this.wandTextBox.Location = new System.Drawing.Point(0, 0);
            this.wandTextBox.Name = "wandTextBox";
            this.wandTextBox.Size = new System.Drawing.Size(928, 434);
            this.wandTextBox.TabIndex = 2;
            this.wandTextBox.TabStop = false;
            this.wandTextBox.Text = "";
            this.wandTextBox.Visible = false;
            this.wandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.wandTextBox_KeyDown);
            // 
            // MainController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(928, 460);
            this.Controls.Add(this.wandTextBox);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.outputBox);
            this.KeyPreview = true;
            this.Name = "MainController";
            this.Text = "CMD++";
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.RichTextBox outputBox;
        public System.Windows.Forms.RichTextBox inputBox;
        public System.Windows.Forms.RichTextBox wandTextBox;
    }
}

