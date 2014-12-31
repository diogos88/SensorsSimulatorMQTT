namespace diogos88.MQTT.SensorsSimulator
{
   partial class SettingsForm
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
         this.TopicTextBox = new System.Windows.Forms.TextBox();
         this.PasswordTextBox = new System.Windows.Forms.TextBox();
         this.UsernameTextBox = new System.Windows.Forms.TextBox();
         this.PortTextBox = new diogos88.MQTT.SensorsSimulator.NumericTextbox();
         this.BrokerURLTextBox = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.OKbutton = new System.Windows.Forms.Button();
         this.CancelBtn = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // TopicTextBox
         // 
         this.TopicTextBox.Location = new System.Drawing.Point(82, 116);
         this.TopicTextBox.Name = "TopicTextBox";
         this.TopicTextBox.Size = new System.Drawing.Size(192, 20);
         this.TopicTextBox.TabIndex = 9;
         // 
         // PasswordTextBox
         // 
         this.PasswordTextBox.Location = new System.Drawing.Point(82, 90);
         this.PasswordTextBox.Name = "PasswordTextBox";
         this.PasswordTextBox.PasswordChar = '*';
         this.PasswordTextBox.Size = new System.Drawing.Size(192, 20);
         this.PasswordTextBox.TabIndex = 8;
         // 
         // UsernameTextBox
         // 
         this.UsernameTextBox.Location = new System.Drawing.Point(82, 64);
         this.UsernameTextBox.Name = "UsernameTextBox";
         this.UsernameTextBox.Size = new System.Drawing.Size(192, 20);
         this.UsernameTextBox.TabIndex = 7;
         // 
         // PortTextBox
         // 
         this.PortTextBox.EnableDecimal = false;
         this.PortTextBox.Location = new System.Drawing.Point(82, 38);
         this.PortTextBox.MaxLength = 5;
         this.PortTextBox.Name = "PortTextBox";
         this.PortTextBox.Size = new System.Drawing.Size(192, 20);
         this.PortTextBox.TabIndex = 6;
         // 
         // BrokerURLTextBox
         // 
         this.BrokerURLTextBox.Location = new System.Drawing.Point(82, 12);
         this.BrokerURLTextBox.Name = "BrokerURLTextBox";
         this.BrokerURLTextBox.Size = new System.Drawing.Size(192, 20);
         this.BrokerURLTextBox.TabIndex = 5;
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(13, 116);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(34, 13);
         this.label5.TabIndex = 4;
         this.label5.Text = "Topic";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(13, 92);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(53, 13);
         this.label4.TabIndex = 3;
         this.label4.Text = "Password";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(13, 67);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(55, 13);
         this.label3.TabIndex = 2;
         this.label3.Text = "Username";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(13, 41);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(26, 13);
         this.label2.TabIndex = 1;
         this.label2.Text = "Port";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(13, 16);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(51, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Broker IP";
         // 
         // OKbutton
         // 
         this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.OKbutton.Location = new System.Drawing.Point(119, 142);
         this.OKbutton.Name = "OKbutton";
         this.OKbutton.Size = new System.Drawing.Size(75, 23);
         this.OKbutton.TabIndex = 10;
         this.OKbutton.Text = "OK";
         this.OKbutton.UseVisualStyleBackColor = true;
         // 
         // CancelBtn
         // 
         this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.CancelBtn.Location = new System.Drawing.Point(200, 142);
         this.CancelBtn.Name = "CancelBtn";
         this.CancelBtn.Size = new System.Drawing.Size(75, 23);
         this.CancelBtn.TabIndex = 11;
         this.CancelBtn.Text = "Cancel";
         this.CancelBtn.UseVisualStyleBackColor = true;
         // 
         // SettingsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(287, 178);
         this.Controls.Add(this.CancelBtn);
         this.Controls.Add(this.OKbutton);
         this.Controls.Add(this.BrokerURLTextBox);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.TopicTextBox);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.PasswordTextBox);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.UsernameTextBox);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.PortTextBox);
         this.Controls.Add(this.label5);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "SettingsForm";
         this.Text = "Settings";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
         this.Load += new System.EventHandler(this.SettingsForm_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox TopicTextBox;
      private System.Windows.Forms.TextBox PasswordTextBox;
      private System.Windows.Forms.TextBox UsernameTextBox;
      private NumericTextbox PortTextBox;
      private System.Windows.Forms.TextBox BrokerURLTextBox;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button OKbutton;
      private System.Windows.Forms.Button CancelBtn;
   }
}