namespace Umbraco.VS.NewProject.Wizard
{
    partial class UserInputForm
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreateProj = new System.Windows.Forms.Button();
            this.rdoSQLCE = new System.Windows.Forms.RadioButton();
            this.rdoCustomDB = new System.Windows.Forms.RadioButton();
            this.pnlCustomDB = new System.Windows.Forms.Panel();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDBConnection = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoWebForms = new System.Windows.Forms.RadioButton();
            this.rdoMVC = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Log = new System.Windows.Forms.Label();
            this.pnlCustomDB.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreateProj
            // 
            this.btnCreateProj.Location = new System.Drawing.Point(143, 265);
            this.btnCreateProj.Name = "btnCreateProj";
            this.btnCreateProj.Size = new System.Drawing.Size(114, 23);
            this.btnCreateProj.TabIndex = 0;
            this.btnCreateProj.Text = "Do Magic Voodoo";
            this.btnCreateProj.UseVisualStyleBackColor = true;
            this.btnCreateProj.Click += new System.EventHandler(this.btnCreateProj_Click);
            // 
            // rdoSQLCE
            // 
            this.rdoSQLCE.AutoSize = true;
            this.rdoSQLCE.Checked = true;
            this.rdoSQLCE.Location = new System.Drawing.Point(6, 19);
            this.rdoSQLCE.Name = "rdoSQLCE";
            this.rdoSQLCE.Size = new System.Drawing.Size(282, 17);
            this.rdoSQLCE.TabIndex = 10;
            this.rdoSQLCE.TabStop = true;
            this.rdoSQLCE.Text = "SQL CE - An embedded file database (Recommended)";
            this.rdoSQLCE.UseVisualStyleBackColor = true;
            // 
            // rdoCustomDB
            // 
            this.rdoCustomDB.AutoSize = true;
            this.rdoCustomDB.Location = new System.Drawing.Point(6, 42);
            this.rdoCustomDB.Name = "rdoCustomDB";
            this.rdoCustomDB.Size = new System.Drawing.Size(165, 17);
            this.rdoCustomDB.TabIndex = 11;
            this.rdoCustomDB.Text = "Custom DB Connection String";
            this.rdoCustomDB.UseVisualStyleBackColor = true;
            this.rdoCustomDB.CheckedChanged += new System.EventHandler(this.rdoCustomDB_CheckedChanged);
            // 
            // pnlCustomDB
            // 
            this.pnlCustomDB.Controls.Add(this.btnTestConnection);
            this.pnlCustomDB.Controls.Add(this.label1);
            this.pnlCustomDB.Controls.Add(this.txtDBConnection);
            this.pnlCustomDB.Location = new System.Drawing.Point(143, 165);
            this.pnlCustomDB.Name = "pnlCustomDB";
            this.pnlCustomDB.Size = new System.Drawing.Size(370, 94);
            this.pnlCustomDB.TabIndex = 12;
            this.pnlCustomDB.Visible = false;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(256, 56);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(111, 23);
            this.btnTestConnection.TabIndex = 11;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Please enter your Database Connection:";
            // 
            // txtDBConnection
            // 
            this.txtDBConnection.Location = new System.Drawing.Point(6, 25);
            this.txtDBConnection.Name = "txtDBConnection";
            this.txtDBConnection.Size = new System.Drawing.Size(361, 20);
            this.txtDBConnection.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoWebForms);
            this.groupBox1.Controls.Add(this.rdoMVC);
            this.groupBox1.Location = new System.Drawing.Point(143, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 72);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Please choose your template rendering mode:";
            // 
            // rdoWebForms
            // 
            this.rdoWebForms.AutoSize = true;
            this.rdoWebForms.Location = new System.Drawing.Point(7, 44);
            this.rdoWebForms.Name = "rdoWebForms";
            this.rdoWebForms.Size = new System.Drawing.Size(79, 17);
            this.rdoWebForms.TabIndex = 1;
            this.rdoWebForms.Text = "Web Forms";
            this.rdoWebForms.UseVisualStyleBackColor = true;
            // 
            // rdoMVC
            // 
            this.rdoMVC.AutoSize = true;
            this.rdoMVC.Checked = true;
            this.rdoMVC.Location = new System.Drawing.Point(7, 20);
            this.rdoMVC.Name = "rdoMVC";
            this.rdoMVC.Size = new System.Drawing.Size(129, 17);
            this.rdoMVC.TabIndex = 0;
            this.rdoMVC.TabStop = true;
            this.rdoMVC.Text = "MVC (Recommended)";
            this.rdoMVC.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoSQLCE);
            this.groupBox2.Controls.Add(this.rdoCustomDB);
            this.groupBox2.Location = new System.Drawing.Point(143, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(367, 69);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Please choose your DB type:";
            // 
            // Log
            // 
            this.Log.AutoSize = true;
            this.Log.Location = new System.Drawing.Point(520, 13);
            this.Log.Name = "Log";
            this.Log.Size = new System.Drawing.Size(0, 13);
            this.Log.TabIndex = 15;
            // 
            // UserInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 305);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlCustomDB);
            this.Controls.Add(this.btnCreateProj);
            this.MaximizeBox = false;
            this.Name = "UserInputForm";
            this.Text = "Umbraco Project - Wizard";
            this.pnlCustomDB.ResumeLayout(false);
            this.pnlCustomDB.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        private System.Windows.Forms.Button btnCreateProj;
        private System.Windows.Forms.RadioButton rdoSQLCE;
        private System.Windows.Forms.RadioButton rdoCustomDB;
        private System.Windows.Forms.Panel pnlCustomDB;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDBConnection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdoWebForms;
        private System.Windows.Forms.RadioButton rdoMVC;
        private System.Windows.Forms.Label Log;
    }
}