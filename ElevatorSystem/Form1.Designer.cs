namespace ElevatorSystem
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pnlHeader = new Panel();
            picLogo = new PictureBox();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblClock = new Label();
            lblOnline = new Label();
            panelA = new ElevatorPanel();
            panelB = new ElevatorPanel();
            pnlFloorStatus = new Panel();
            lblStatusTitle = new Label();
            pnlCard2 = new Panel();
            lblDot2 = new Label();
            lblCard2Name = new Label();
            lblCard2Sub = new Label();
            lblCard2Badge = new Label();
            pnlCard1 = new Panel();
            lblDot1 = new Label();
            lblCard1Name = new Label();
            lblCard1Sub = new Label();
            lblCard1Badge = new Label();
            pnlLog = new Panel();
            lblLogTitle = new Label();
            btnViewLogs = new Button();
            dgvLog = new DataGridView();
            pnlOutside = new Panel();
            label2 = new Label();
            btnRequest1 = new Button();
            btnRequest2 = new Button();
            pnlSafety = new Panel();
            label1 = new Label();
            label3 = new Label();
            pnlEmergencyNotice = new Panel();
            label4 = new Label();
            label5 = new Label();
            pnlFooter = new Panel();
            lblFooterLeft = new Label();
            lblMessage = new Label();
            lblFooterRight = new Label();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            pnlFloorStatus.SuspendLayout();
            pnlCard2.SuspendLayout();
            pnlCard1.SuspendLayout();
            pnlLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            pnlOutside.SuspendLayout();
            pnlSafety.SuspendLayout();
            pnlEmergencyNotice.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(31, 64, 115);
            pnlHeader.Controls.Add(picLogo);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblClock);
            pnlHeader.Controls.Add(lblOnline);
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(2800, 200);
            pnlHeader.TabIndex = 0;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(20, 45);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(75, 90);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(120, 22);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(636, 72);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Elevator Control System";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.WhiteSmoke;
            lblSubtitle.Location = new Point(123, 112);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(305, 37);
            lblSubtitle.TabIndex = 2;
            lblSubtitle.Text = "Safe • Efficient • Reliable";
            // 
            // lblClock
            // 
            lblClock.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblClock.ForeColor = Color.White;
            lblClock.Location = new Point(2260, 35);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(425, 59);
            lblClock.TabIndex = 3;
            lblClock.Text = "00:00:00";
            lblClock.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOnline
            // 
            lblOnline.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOnline.ForeColor = Color.LightGreen;
            lblOnline.Location = new Point(2340, 112);
            lblOnline.Name = "lblOnline";
            lblOnline.Size = new Size(345, 54);
            lblOnline.TabIndex = 4;
            lblOnline.Text = "● SYSTEM ONLINE";
            lblOnline.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelA
            // 
            panelA.BackColor = Color.White;
            panelA.Location = new Point(20, 230);
            panelA.MaximumSize = new Size(760, 1150);
            panelA.MinimumSize = new Size(760, 1150);
            panelA.Name = "panelA";
            panelA.Size = new Size(760, 1150);
            panelA.TabIndex = 1;
            // 
            // panelB
            // 
            panelB.BackColor = Color.White;
            panelB.Location = new Point(800, 230);
            panelB.MaximumSize = new Size(760, 1150);
            panelB.MinimumSize = new Size(760, 1150);
            panelB.Name = "panelB";
            panelB.Size = new Size(760, 1150);
            panelB.TabIndex = 2;
            // 
            // pnlFloorStatus
            // 
            pnlFloorStatus.BackColor = Color.White;
            pnlFloorStatus.BorderStyle = BorderStyle.FixedSingle;
            pnlFloorStatus.Controls.Add(lblStatusTitle);
            pnlFloorStatus.Controls.Add(pnlCard2);
            pnlFloorStatus.Controls.Add(pnlCard1);
            pnlFloorStatus.Location = new Point(1580, 230);
            pnlFloorStatus.Name = "pnlFloorStatus";
            pnlFloorStatus.Size = new Size(1200, 430);
            pnlFloorStatus.TabIndex = 3;
            // 
            // lblStatusTitle
            // 
            lblStatusTitle.AutoSize = true;
            lblStatusTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblStatusTitle.Location = new Point(30, 25);
            lblStatusTitle.Name = "lblStatusTitle";
            lblStatusTitle.Size = new Size(270, 47);
            lblStatusTitle.TabIndex = 0;
            lblStatusTitle.Text = "FLOOR STATUS";
            // 
            // pnlCard2
            // 
            pnlCard2.BackColor = Color.White;
            pnlCard2.BorderStyle = BorderStyle.FixedSingle;
            pnlCard2.Controls.Add(lblDot2);
            pnlCard2.Controls.Add(lblCard2Name);
            pnlCard2.Controls.Add(lblCard2Sub);
            pnlCard2.Controls.Add(lblCard2Badge);
            pnlCard2.Location = new Point(30, 90);
            pnlCard2.Name = "pnlCard2";
            pnlCard2.Size = new Size(1140, 130);
            pnlCard2.TabIndex = 1;
            // 
            // lblDot2
            // 
            lblDot2.AutoSize = true;
            lblDot2.Font = new Font("Segoe UI", 16F);
            lblDot2.ForeColor = Color.SlateGray;
            lblDot2.Location = new Point(15, 13);
            lblDot2.Name = "lblDot2";
            lblDot2.Size = new Size(51, 59);
            lblDot2.TabIndex = 0;
            lblDot2.Text = "●";
            // 
            // lblCard2Name
            // 
            lblCard2Name.AutoSize = true;
            lblCard2Name.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCard2Name.Location = new Point(60, 7);
            lblCard2Name.Name = "lblCard2Name";
            lblCard2Name.Size = new Size(116, 41);
            lblCard2Name.TabIndex = 1;
            lblCard2Name.Text = "Floor 2";
            // 
            // lblCard2Sub
            // 
            lblCard2Sub.AutoSize = true;
            lblCard2Sub.Font = new Font("Segoe UI", 9F);
            lblCard2Sub.ForeColor = Color.DimGray;
            lblCard2Sub.Location = new Point(60, 52);
            lblCard2Sub.Name = "lblCard2Sub";
            lblCard2Sub.Size = new Size(197, 32);
            lblCard2Sub.TabIndex = 2;
            lblCard2Sub.Text = "Elevator not here";
            // 
            // lblCard2Badge
            // 
            lblCard2Badge.BackColor = Color.SlateGray;
            lblCard2Badge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCard2Badge.ForeColor = Color.White;
            lblCard2Badge.Location = new Point(900, 35);
            lblCard2Badge.Name = "lblCard2Badge";
            lblCard2Badge.Size = new Size(190, 60);
            lblCard2Badge.TabIndex = 3;
            lblCard2Badge.Text = "Inactive";
            lblCard2Badge.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlCard1
            // 
            pnlCard1.BackColor = Color.White;
            pnlCard1.BorderStyle = BorderStyle.FixedSingle;
            pnlCard1.Controls.Add(lblDot1);
            pnlCard1.Controls.Add(lblCard1Name);
            pnlCard1.Controls.Add(lblCard1Sub);
            pnlCard1.Controls.Add(lblCard1Badge);
            pnlCard1.Location = new Point(30, 250);
            pnlCard1.Name = "pnlCard1";
            pnlCard1.Size = new Size(1140, 130);
            pnlCard1.TabIndex = 2;
            // 
            // lblDot1
            // 
            lblDot1.AutoSize = true;
            lblDot1.Font = new Font("Segoe UI", 16F);
            lblDot1.ForeColor = Color.SlateGray;
            lblDot1.Location = new Point(15, 13);
            lblDot1.Name = "lblDot1";
            lblDot1.Size = new Size(51, 59);
            lblDot1.TabIndex = 0;
            lblDot1.Text = "●";
            // 
            // lblCard1Name
            // 
            lblCard1Name.AutoSize = true;
            lblCard1Name.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCard1Name.Location = new Point(60, 7);
            lblCard1Name.Name = "lblCard1Name";
            lblCard1Name.Size = new Size(116, 41);
            lblCard1Name.TabIndex = 1;
            lblCard1Name.Text = "Floor 1";
            // 
            // lblCard1Sub
            // 
            lblCard1Sub.AutoSize = true;
            lblCard1Sub.Font = new Font("Segoe UI", 9F);
            lblCard1Sub.ForeColor = Color.DimGray;
            lblCard1Sub.Location = new Point(60, 52);
            lblCard1Sub.Name = "lblCard1Sub";
            lblCard1Sub.Size = new Size(197, 32);
            lblCard1Sub.TabIndex = 2;
            lblCard1Sub.Text = "Elevator not here";
            // 
            // lblCard1Badge
            // 
            lblCard1Badge.BackColor = Color.SlateGray;
            lblCard1Badge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCard1Badge.ForeColor = Color.White;
            lblCard1Badge.Location = new Point(900, 35);
            lblCard1Badge.Name = "lblCard1Badge";
            lblCard1Badge.Size = new Size(190, 60);
            lblCard1Badge.TabIndex = 3;
            lblCard1Badge.Text = "Inactive";
            lblCard1Badge.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlLog
            // 
            pnlLog.BackColor = Color.White;
            pnlLog.BorderStyle = BorderStyle.FixedSingle;
            pnlLog.Controls.Add(lblLogTitle);
            pnlLog.Controls.Add(btnViewLogs);
            pnlLog.Controls.Add(dgvLog);
            pnlLog.Location = new Point(1580, 690);
            pnlLog.Name = "pnlLog";
            pnlLog.Size = new Size(1200, 690);
            pnlLog.TabIndex = 4;
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblLogTitle.Location = new Point(30, 25);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new Size(300, 47);
            lblLogTitle.TabIndex = 0;
            lblLogTitle.Text = "OPERATION LOG";
            // 
            // btnViewLogs
            // 
            btnViewLogs.BackColor = Color.FromArgb(50, 100, 160);
            btnViewLogs.FlatStyle = FlatStyle.Flat;
            btnViewLogs.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnViewLogs.ForeColor = Color.White;
            btnViewLogs.Location = new Point(930, 20);
            btnViewLogs.Name = "btnViewLogs";
            btnViewLogs.Size = new Size(220, 65);
            btnViewLogs.TabIndex = 1;
            btnViewLogs.Text = "View Logs";
            btnViewLogs.UseVisualStyleBackColor = false;
            // 
            // dgvLog
            // 
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLog.BackgroundColor = Color.White;
            dgvLog.BorderStyle = BorderStyle.Fixed3D;
            dgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLog.Location = new Point(30, 110);
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.RowHeadersWidth = 45;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.Size = new Size(1140, 530);
            dgvLog.TabIndex = 2;
            dgvLog.CellContentClick += dgvLog_CellContentClick;
            // 
            // pnlOutside
            // 
            pnlOutside.BackColor = Color.White;
            pnlOutside.BorderStyle = BorderStyle.FixedSingle;
            pnlOutside.Controls.Add(label2);
            pnlOutside.Controls.Add(btnRequest1);
            pnlOutside.Controls.Add(btnRequest2);
            pnlOutside.Location = new Point(20, 1410);
            pnlOutside.Name = "pnlOutside";
            pnlOutside.Size = new Size(1540, 180);
            pnlOutside.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label2.Location = new Point(30, 20);
            label2.Name = "label2";
            label2.Size = new Size(262, 41);
            label2.TabIndex = 0;
            label2.Text = "Outside Requests";
            // 
            // btnRequest1
            // 
            btnRequest1.BackColor = Color.FromArgb(50, 100, 160);
            btnRequest1.FlatStyle = FlatStyle.Flat;
            btnRequest1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRequest1.ForeColor = Color.White;
            btnRequest1.Location = new Point(30, 80);
            btnRequest1.Name = "btnRequest1";
            btnRequest1.Size = new Size(720, 75);
            btnRequest1.TabIndex = 1;
            btnRequest1.Text = "▼   Request Floor 1";
            btnRequest1.UseVisualStyleBackColor = false;
            // 
            // btnRequest2
            // 
            btnRequest2.BackColor = Color.FromArgb(50, 100, 160);
            btnRequest2.FlatStyle = FlatStyle.Flat;
            btnRequest2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRequest2.ForeColor = Color.White;
            btnRequest2.Location = new Point(790, 80);
            btnRequest2.Name = "btnRequest2";
            btnRequest2.Size = new Size(720, 75);
            btnRequest2.TabIndex = 2;
            btnRequest2.Text = "▲   Request Floor 2";
            btnRequest2.UseVisualStyleBackColor = false;
            // 
            // pnlSafety
            // 
            pnlSafety.BackColor = Color.FromArgb(255, 243, 205);
            pnlSafety.BorderStyle = BorderStyle.FixedSingle;
            pnlSafety.Controls.Add(label1);
            pnlSafety.Controls.Add(label3);
            pnlSafety.Location = new Point(1580, 1410);
            pnlSafety.Name = "pnlSafety";
            pnlSafety.Size = new Size(580, 180);
            pnlSafety.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(160, 100, 0);
            label1.Location = new Point(20, 15);
            label1.Name = "label1";
            label1.Size = new Size(232, 32);
            label1.TabIndex = 0;
            label1.Text = "⚠ SAFETY NOTICE";
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 8F);
            label3.Location = new Point(18, 55);
            label3.Name = "label3";
            label3.Size = new Size(540, 110);
            label3.TabIndex = 1;
            label3.Text = "Do not overload the elevator. Maximum capacity: 6 persons (450 kg). Keep clear of the doors.";
            // 
            // pnlEmergencyNotice
            // 
            pnlEmergencyNotice.BackColor = Color.FromArgb(253, 228, 230);
            pnlEmergencyNotice.BorderStyle = BorderStyle.FixedSingle;
            pnlEmergencyNotice.Controls.Add(label4);
            pnlEmergencyNotice.Controls.Add(label5);
            pnlEmergencyNotice.Location = new Point(2180, 1410);
            pnlEmergencyNotice.Name = "pnlEmergencyNotice";
            pnlEmergencyNotice.Size = new Size(600, 180);
            pnlEmergencyNotice.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(200, 30, 30);
            label4.Location = new Point(20, 15);
            label4.Name = "label4";
            label4.Size = new Size(158, 32);
            label4.TabIndex = 0;
            label4.Text = "EMERGENCY";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 8F);
            label5.Location = new Point(18, 55);
            label5.Name = "label5";
            label5.Size = new Size(560, 110);
            label5.TabIndex = 1;
            label5.Text = "In case of emergency, press the Emergency Stop button or contact building management immediately.";
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(31, 64, 115);
            pnlFooter.Controls.Add(lblFooterLeft);
            pnlFooter.Controls.Add(lblMessage);
            pnlFooter.Controls.Add(lblFooterRight);
            pnlFooter.Location = new Point(0, 1610);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(2800, 90);
            pnlFooter.TabIndex = 8;
            // 
            // lblFooterLeft
            // 
            lblFooterLeft.AutoSize = true;
            lblFooterLeft.ForeColor = Color.White;
            lblFooterLeft.Location = new Point(20, 20);
            lblFooterLeft.Name = "lblFooterLeft";
            lblFooterLeft.Size = new Size(318, 32);
            lblFooterLeft.TabIndex = 0;
            lblFooterLeft.Text = "Elevator Control System v1.0";
            // 
            // lblMessage
            // 
            lblMessage.ForeColor = Color.Gold;
            lblMessage.Location = new Point(600, 20);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(1500, 50);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "Ready.";
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblFooterRight
            // 
            lblFooterRight.ForeColor = Color.White;
            lblFooterRight.Location = new Point(1550, 20);
            lblFooterRight.Name = "lblFooterRight";
            lblFooterRight.Size = new Size(360, 30);
            lblFooterRight.TabIndex = 2;
            lblFooterRight.Text = "Safety First  •  Always";
            lblFooterRight.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(242, 246, 250);
            ClientSize = new Size(2800, 1700);
            Controls.Add(pnlFooter);
            Controls.Add(pnlEmergencyNotice);
            Controls.Add(pnlSafety);
            Controls.Add(pnlOutside);
            Controls.Add(pnlLog);
            Controls.Add(pnlFloorStatus);
            Controls.Add(panelB);
            Controls.Add(panelA);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Elevator Control System";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            pnlFloorStatus.ResumeLayout(false);
            pnlFloorStatus.PerformLayout();
            pnlCard2.ResumeLayout(false);
            pnlCard2.PerformLayout();
            pnlCard1.ResumeLayout(false);
            pnlCard1.PerformLayout();
            pnlLog.ResumeLayout(false);
            pnlLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            pnlOutside.ResumeLayout(false);
            pnlOutside.PerformLayout();
            pnlSafety.ResumeLayout(false);
            pnlSafety.PerformLayout();
            pnlEmergencyNotice.ResumeLayout(false);
            pnlEmergencyNotice.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private PictureBox picLogo;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblClock;
        private Label lblOnline;

        private ElevatorPanel panelA;
        private ElevatorPanel panelB;

        private Panel pnlFloorStatus;
        private Label lblStatusTitle;

        private Panel pnlCard2;
        private Label lblDot2;
        private Label lblCard2Name;
        private Label lblCard2Sub;
        private Label lblCard2Badge;

        private Panel pnlCard1;
        private Label lblDot1;
        private Label lblCard1Name;
        private Label lblCard1Sub;
        private Label lblCard1Badge;

        private Panel pnlLog;
        private Label lblLogTitle;
        private Button btnViewLogs;
        private DataGridView dgvLog;

        private Panel pnlOutside;
        private Label label2;
        private Button btnRequest1;
        private Button btnRequest2;

        private Panel pnlSafety;
        private Label label1;
        private Label label3;

        private Panel pnlEmergencyNotice;
        private Label label4;
        private Label label5;

        private Panel pnlFooter;
        private Label lblFooterLeft;
        private Label lblMessage;
        private Label lblFooterRight;
    }
}