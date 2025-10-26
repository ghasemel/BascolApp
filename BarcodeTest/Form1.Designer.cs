namespace BascolApp
{
    partial class Form1
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
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReadData = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(311, 215);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open4";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(299, 120);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "COM4";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(311, 255);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(75, 23);
            this.btnReadData.TabIndex = 4;
            this.btnReadData.Text = "Read Data";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Baud rate:";
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Location = new System.Drawing.Point(299, 146);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.Size = new System.Drawing.Size(100, 20);
            this.txtBaudRate.TabIndex = 5;
            this.txtBaudRate.Text = "9600";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBaudRate);
            this.Controls.Add(this.btnReadData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btnOpen);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBaudRate;
    }
}

