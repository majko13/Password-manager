namespace Password_manager
{
    partial class main
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
            this.loginButton = new System.Windows.Forms.Button();
            this.usernameLoginTextbox = new System.Windows.Forms.TextBox();
            this.passwordLoginTextbox = new System.Windows.Forms.TextBox();
            this.loginGroupbox = new System.Windows.Forms.GroupBox();
            this.forgottenPasswordButton = new System.Windows.Forms.Button();
            this.switchToRegisterButton = new System.Windows.Forms.Button();
            this.LoginPicturebox = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.passwordRegisterTextbox = new System.Windows.Forms.TextBox();
            this.usernameRegisterTextbox = new System.Windows.Forms.TextBox();
            this.rPasswordRegisterTextbox = new System.Windows.Forms.TextBox();
            this.registerGroupbox = new System.Windows.Forms.GroupBox();
            this.CaptchaButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.RegisterPicturebox = new System.Windows.Forms.PictureBox();
            this.switchToLoginButton = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.loginGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoginPicturebox)).BeginInit();
            this.registerGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPicturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.loginButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.loginButton.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(55, 412);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(375, 45);
            this.loginButton.TabIndex = 3;
            this.loginButton.Text = "Log in";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // usernameLoginTextbox
            // 
            this.usernameLoginTextbox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.usernameLoginTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usernameLoginTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.usernameLoginTextbox.Location = new System.Drawing.Point(200, 189);
            this.usernameLoginTextbox.Name = "usernameLoginTextbox";
            this.usernameLoginTextbox.Size = new System.Drawing.Size(232, 23);
            this.usernameLoginTextbox.TabIndex = 1;
            this.usernameLoginTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.usernameLoginTextbox_KeyDown);
            // 
            // passwordLoginTextbox
            // 
            this.passwordLoginTextbox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.passwordLoginTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passwordLoginTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.passwordLoginTextbox.Location = new System.Drawing.Point(200, 288);
            this.passwordLoginTextbox.Name = "passwordLoginTextbox";
            this.passwordLoginTextbox.PasswordChar = '*';
            this.passwordLoginTextbox.Size = new System.Drawing.Size(230, 23);
            this.passwordLoginTextbox.TabIndex = 2;
            this.passwordLoginTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.usernameLoginTextbox_KeyDown);
            // 
            // loginGroupbox
            // 
            this.loginGroupbox.Controls.Add(this.forgottenPasswordButton);
            this.loginGroupbox.Controls.Add(this.switchToRegisterButton);
            this.loginGroupbox.Controls.Add(this.LoginPicturebox);
            this.loginGroupbox.Controls.Add(this.label10);
            this.loginGroupbox.Controls.Add(this.label1);
            this.loginGroupbox.Controls.Add(this.label2);
            this.loginGroupbox.Controls.Add(this.groupBox4);
            this.loginGroupbox.Controls.Add(this.passwordLoginTextbox);
            this.loginGroupbox.Controls.Add(this.groupBox3);
            this.loginGroupbox.Controls.Add(this.usernameLoginTextbox);
            this.loginGroupbox.Controls.Add(this.loginButton);
            this.loginGroupbox.Location = new System.Drawing.Point(-7, -12);
            this.loginGroupbox.Name = "loginGroupbox";
            this.loginGroupbox.Size = new System.Drawing.Size(516, 662);
            this.loginGroupbox.TabIndex = 0;
            this.loginGroupbox.TabStop = false;
            // 
            // forgottenPasswordButton
            // 
            this.forgottenPasswordButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.forgottenPasswordButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.forgottenPasswordButton.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.forgottenPasswordButton.ForeColor = System.Drawing.Color.White;
            this.forgottenPasswordButton.Location = new System.Drawing.Point(55, 511);
            this.forgottenPasswordButton.Name = "forgottenPasswordButton";
            this.forgottenPasswordButton.Size = new System.Drawing.Size(375, 45);
            this.forgottenPasswordButton.TabIndex = 17;
            this.forgottenPasswordButton.Text = "Forgotten password";
            this.forgottenPasswordButton.UseVisualStyleBackColor = false;
            this.forgottenPasswordButton.Click += new System.EventHandler(this.button6_Click);
            // 
            // switchToRegisterButton
            // 
            this.switchToRegisterButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.switchToRegisterButton.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.switchToRegisterButton.ForeColor = System.Drawing.Color.White;
            this.switchToRegisterButton.Location = new System.Drawing.Point(55, 462);
            this.switchToRegisterButton.Margin = new System.Windows.Forms.Padding(2);
            this.switchToRegisterButton.Name = "switchToRegisterButton";
            this.switchToRegisterButton.Size = new System.Drawing.Size(375, 44);
            this.switchToRegisterButton.TabIndex = 4;
            this.switchToRegisterButton.Text = "Register";
            this.switchToRegisterButton.UseVisualStyleBackColor = false;
            this.switchToRegisterButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // LoginPicturebox
            // 
            this.LoginPicturebox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoginPicturebox.Location = new System.Drawing.Point(401, 37);
            this.LoginPicturebox.Name = "LoginPicturebox";
            this.LoginPicturebox.Size = new System.Drawing.Size(100, 50);
            this.LoginPicturebox.TabIndex = 16;
            this.LoginPicturebox.TabStop = false;
            this.LoginPicturebox.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Bauhaus 93", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label10.Location = new System.Drawing.Point(159, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(207, 68);
            this.label10.TabIndex = 14;
            this.label10.Text = "LOG IN";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(52, 184);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 28);
            this.label1.TabIndex = 12;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(52, 283);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 28);
            this.label2.TabIndex = 13;
            this.label2.Text = "Password";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Highlight;
            this.groupBox4.Location = new System.Drawing.Point(55, 314);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(375, 1);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "groupBox4";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Highlight;
            this.groupBox3.Location = new System.Drawing.Point(57, 215);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(375, 1);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.registerButton.Enabled = false;
            this.registerButton.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.Location = new System.Drawing.Point(58, 632);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(375, 45);
            this.registerButton.TabIndex = 5;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = false;
            this.registerButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // passwordRegisterTextbox
            // 
            this.passwordRegisterTextbox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.passwordRegisterTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passwordRegisterTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.passwordRegisterTextbox.Location = new System.Drawing.Point(227, 264);
            this.passwordRegisterTextbox.Name = "passwordRegisterTextbox";
            this.passwordRegisterTextbox.PasswordChar = '*';
            this.passwordRegisterTextbox.Size = new System.Drawing.Size(206, 23);
            this.passwordRegisterTextbox.TabIndex = 2;
            this.passwordRegisterTextbox.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            this.passwordRegisterTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.usernameLoginTextbox_KeyDown);
            // 
            // usernameRegisterTextbox
            // 
            this.usernameRegisterTextbox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.usernameRegisterTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usernameRegisterTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.usernameRegisterTextbox.Location = new System.Drawing.Point(227, 171);
            this.usernameRegisterTextbox.Name = "usernameRegisterTextbox";
            this.usernameRegisterTextbox.Size = new System.Drawing.Size(209, 23);
            this.usernameRegisterTextbox.TabIndex = 1;
            this.usernameRegisterTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.usernameLoginTextbox_KeyDown);
            // 
            // rPasswordRegisterTextbox
            // 
            this.rPasswordRegisterTextbox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rPasswordRegisterTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rPasswordRegisterTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.rPasswordRegisterTextbox.Location = new System.Drawing.Point(263, 351);
            this.rPasswordRegisterTextbox.Name = "rPasswordRegisterTextbox";
            this.rPasswordRegisterTextbox.PasswordChar = '*';
            this.rPasswordRegisterTextbox.Size = new System.Drawing.Size(170, 23);
            this.rPasswordRegisterTextbox.TabIndex = 3;
            this.rPasswordRegisterTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.usernameLoginTextbox_KeyDown);
            // 
            // registerGroupbox
            // 
            this.registerGroupbox.Controls.Add(this.CaptchaButton);
            this.registerGroupbox.Controls.Add(this.label9);
            this.registerGroupbox.Controls.Add(this.label8);
            this.registerGroupbox.Controls.Add(this.label7);
            this.registerGroupbox.Controls.Add(this.label6);
            this.registerGroupbox.Controls.Add(this.RegisterPicturebox);
            this.registerGroupbox.Controls.Add(this.switchToLoginButton);
            this.registerGroupbox.Controls.Add(this.groupBox7);
            this.registerGroupbox.Controls.Add(this.groupBox6);
            this.registerGroupbox.Controls.Add(this.groupBox5);
            this.registerGroupbox.Controls.Add(this.label3);
            this.registerGroupbox.Controls.Add(this.label11);
            this.registerGroupbox.Controls.Add(this.label5);
            this.registerGroupbox.Controls.Add(this.label4);
            this.registerGroupbox.Controls.Add(this.rPasswordRegisterTextbox);
            this.registerGroupbox.Controls.Add(this.usernameRegisterTextbox);
            this.registerGroupbox.Controls.Add(this.passwordRegisterTextbox);
            this.registerGroupbox.Controls.Add(this.registerButton);
            this.registerGroupbox.Location = new System.Drawing.Point(512, -12);
            this.registerGroupbox.Name = "registerGroupbox";
            this.registerGroupbox.Size = new System.Drawing.Size(547, 859);
            this.registerGroupbox.TabIndex = 7;
            this.registerGroupbox.TabStop = false;
            this.registerGroupbox.Visible = false;
            // 
            // CaptchaButton
            // 
            this.CaptchaButton.BackColor = System.Drawing.Color.Red;
            this.CaptchaButton.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.CaptchaButton.ForeColor = System.Drawing.Color.White;
            this.CaptchaButton.Location = new System.Drawing.Point(58, 581);
            this.CaptchaButton.Name = "CaptchaButton";
            this.CaptchaButton.Size = new System.Drawing.Size(375, 45);
            this.CaptchaButton.TabIndex = 4;
            this.CaptchaButton.Text = "CAPTCHA";
            this.CaptchaButton.UseVisualStyleBackColor = false;
            this.CaptchaButton.Click += new System.EventHandler(this.button5_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Bahnschrift", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(59, 517);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(452, 27);
            this.label9.TabIndex = 25;
            this.label9.Text = "password has to include at least 1 uppercase";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Bahnschrift", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(59, 485);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(425, 27);
            this.label8.TabIndex = 24;
            this.label8.Text = "password has to include at least 1 number";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Bahnschrift", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(59, 450);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(469, 27);
            this.label7.TabIndex = 23;
            this.label7.Text = "password has to include at least 1 special char";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Bahnschrift", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(59, 419);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(377, 27);
            this.label6.TabIndex = 22;
            this.label6.Text = "count of chars. has to be 12 and more";
            // 
            // RegisterPicturebox
            // 
            this.RegisterPicturebox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RegisterPicturebox.Location = new System.Drawing.Point(401, 25);
            this.RegisterPicturebox.Name = "RegisterPicturebox";
            this.RegisterPicturebox.Size = new System.Drawing.Size(100, 50);
            this.RegisterPicturebox.TabIndex = 21;
            this.RegisterPicturebox.TabStop = false;
            this.RegisterPicturebox.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // switchToLoginButton
            // 
            this.switchToLoginButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.switchToLoginButton.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.switchToLoginButton.ForeColor = System.Drawing.Color.White;
            this.switchToLoginButton.Location = new System.Drawing.Point(58, 682);
            this.switchToLoginButton.Margin = new System.Windows.Forms.Padding(2);
            this.switchToLoginButton.Name = "switchToLoginButton";
            this.switchToLoginButton.Size = new System.Drawing.Size(375, 45);
            this.switchToLoginButton.TabIndex = 6;
            this.switchToLoginButton.Text = "Log in";
            this.switchToLoginButton.UseVisualStyleBackColor = false;
            this.switchToLoginButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.SystemColors.Highlight;
            this.groupBox7.Location = new System.Drawing.Point(58, 373);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(378, 1);
            this.groupBox7.TabIndex = 20;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "groupBox7";
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.SystemColors.Highlight;
            this.groupBox6.Location = new System.Drawing.Point(58, 195);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(378, 1);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "groupBox6";
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.Highlight;
            this.groupBox5.Location = new System.Drawing.Point(58, 286);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(378, 1);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "groupBox5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(53, 168);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 28);
            this.label3.TabIndex = 14;
            this.label3.Text = "Username";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Bauhaus 93", 36F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label11.Location = new System.Drawing.Point(126, 66);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(280, 68);
            this.label11.TabIndex = 17;
            this.label11.Text = "REGISTER";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(53, 344);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 28);
            this.label5.TabIndex = 16;
            this.label5.Text = "Repeat password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(53, 259);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 28);
            this.label4.TabIndex = 15;
            this.label4.Text = "Password";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1161, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(5, 732);
            this.panel4.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(5, 737);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1161, 5);
            this.panel3.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1161, 5);
            this.panel2.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(5, 742);
            this.panel1.TabIndex = 12;
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1166, 742);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.registerGroupbox);
            this.Controls.Add(this.loginGroupbox);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "main";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.main_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.main_MouseUp);
            this.loginGroupbox.ResumeLayout(false);
            this.loginGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoginPicturebox)).EndInit();
            this.registerGroupbox.ResumeLayout(false);
            this.registerGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPicturebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.TextBox usernameLoginTextbox;
        private System.Windows.Forms.TextBox passwordLoginTextbox;
        private System.Windows.Forms.GroupBox loginGroupbox;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.TextBox passwordRegisterTextbox;
        private System.Windows.Forms.TextBox usernameRegisterTextbox;
        private System.Windows.Forms.TextBox rPasswordRegisterTextbox;
        private System.Windows.Forms.GroupBox registerGroupbox;
        private System.Windows.Forms.Button switchToRegisterButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button switchToLoginButton;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox LoginPicturebox;
        private System.Windows.Forms.PictureBox RegisterPicturebox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button CaptchaButton;
        private System.Windows.Forms.Button forgottenPasswordButton;
    }
}

