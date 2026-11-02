using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ElevatorSystem
{
    public enum ElevatorStateEnum
    {
        Idle,
        MovingUp,
        MovingDown,
        OpeningDoors,
        DoorsOpen,
        ClosingDoors,
        EmergencyStop
    }

    public class ElevatorContext
    {
        public string Name { get; set; } = "Elevator 1";
        public int CurrentFloor { get; set; } = 1;
        public int TargetFloor { get; set; } = 1;
        public ElevatorStateEnum State { get; set; } = ElevatorStateEnum.Idle;
        public List<int> RequestQueue { get; set; } = new List<int>();

        public float YPosition { get; set; }
        public float DoorOpenRatio { get; set; } = 0.0f; // 0 = fully closed, 1 = fully open
        private int doorHoldTimer = 0;

        public ElevatorContext(float startY)
        {
            YPosition = startY;
        }

        public void Update(float f1Y, float f2Y, Action<int, int, string> logCallback)
        {
            if (State == ElevatorStateEnum.EmergencyStop) return;

            float targetY = (TargetFloor == 1) ? f1Y : f2Y;
            float speed = 2.5f;

            switch (State)
            {
                case ElevatorStateEnum.Idle:
                    if (RequestQueue.Count > 0)
                    {
                        int next = RequestQueue[0];
                        RequestQueue.RemoveAt(0);

                        if (next > CurrentFloor)
                        {
                            TargetFloor = next;
                            State = ElevatorStateEnum.MovingUp;
                            logCallback?.Invoke(CurrentFloor, TargetFloor, $"Elevator moving from Floor {CurrentFloor} to Floor {TargetFloor}");
                        }
                        else if (next < CurrentFloor)
                        {
                            TargetFloor = next;
                            State = ElevatorStateEnum.MovingDown;
                            logCallback?.Invoke(CurrentFloor, TargetFloor, $"Elevator moving from Floor {CurrentFloor} to Floor {TargetFloor}");
                        }
                        else
                        {
                            State = ElevatorStateEnum.OpeningDoors;
                            logCallback?.Invoke(CurrentFloor, CurrentFloor, $"Doors opening at Floor {CurrentFloor}");
                        }
                    }
                    break;

                case ElevatorStateEnum.MovingUp:
                    if (YPosition > targetY)
                    {
                        YPosition -= speed;
                    }
                    else
                    {
                        YPosition = targetY;
                        CurrentFloor = TargetFloor;
                        State = ElevatorStateEnum.OpeningDoors;
                        logCallback?.Invoke(CurrentFloor, CurrentFloor, $"Doors opening at Floor {CurrentFloor}");
                    }
                    break;

                case ElevatorStateEnum.MovingDown:
                    if (YPosition < targetY)
                    {
                        YPosition += speed;
                    }
                    else
                    {
                        YPosition = targetY;
                        CurrentFloor = TargetFloor;
                        State = ElevatorStateEnum.OpeningDoors;
                        logCallback?.Invoke(CurrentFloor, CurrentFloor, $"Doors opening at Floor {CurrentFloor}");
                    }
                    break;

                case ElevatorStateEnum.OpeningDoors:
                    if (DoorOpenRatio < 1.0f)
                    {
                        DoorOpenRatio += 0.05f;
                    }
                    else
                    {
                        DoorOpenRatio = 1.0f;
                        State = ElevatorStateEnum.DoorsOpen;
                        doorHoldTimer = 0;
                    }
                    break;

                case ElevatorStateEnum.DoorsOpen:
                    doorHoldTimer++;
                    if (doorHoldTimer > 50)
                    {
                        State = ElevatorStateEnum.ClosingDoors;
                        logCallback?.Invoke(CurrentFloor, CurrentFloor, $"Doors closing at Floor {CurrentFloor}");
                    }
                    break;

                case ElevatorStateEnum.ClosingDoors:
                    if (DoorOpenRatio > 0.0f)
                    {
                        DoorOpenRatio -= 0.05f;
                    }
                    else
                    {
                        DoorOpenRatio = 0.0f;
                        State = ElevatorStateEnum.Idle;
                    }
                    break;
            }
        }
    }

    public partial class Form1 : Form
    {
        private ElevatorContext elevator;
        private Timer mainTimer;
        private BackgroundWorker dbWorker;
        private string connectionString = "";
        private int logCounter = 1;

        // Visual Coordinates
        private float floor1Y = 280;
        private float floor2Y = 120;

        // Controls
        private DataGridView gridLog;
        private Label lblClock;
        private Panel panelF1Status, panelF2Status;
        private Label lblF1Sub, lblF2Sub, lblF1Badge, lblF2Badge;
        private Label lblDigitalDisplay;

        public Form1()
        {
            InitializeComponent();
            LoadConnectionString();
            SetupDashboardUI();
            InitializeSimulation();
            InitializeBackgroundWorker();
        }

        private void LoadConnectionString()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Config\\dbconfig.txt");
                if (File.Exists(configPath))
                {
                    connectionString = File.ReadAllText(configPath).Trim();
                }
                else
                {
                    connectionString = "Server=192.168.1.2;Port=3306;Database=elevator_db;Uid=elevator_app;Pwd=CHANGE_THIS_PASSWORD;";
                }
            }
            catch
            {
                connectionString = "Server=192.168.1.2;Port=3306;Database=elevator_db;Uid=elevator_app;Pwd=CHANGE_THIS_PASSWORD;";
            }
        }

        private void SetupDashboardUI()
        {
            this.Text = "Elevator Control System";
            this.Width = 1100;
            this.Height = 750;
            this.BackColor = Color.FromArgb(235, 240, 245);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;

            // --- 1. Top Header Bar ---
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(32, 64, 102)
            };
            this.Controls.Add(pnlHeader);

            Label lblTitle = new Label
            {
                Text = "Elevator Control System",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Left = 20,
                Top = 10,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);

            Label lblSubTitle = new Label
            {
                Text = "Safe  •  Efficient  •  Reliable",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 205, 230),
                Left = 22,
                Top = 38,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblSubTitle);

            lblClock = new Label
            {
                Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Top = 20,
                Left = 820,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblClock);

            Label lblOnline = new Label
            {
                Text = "● System Online",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Top = 20,
                Left = 970,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblOnline);

            // --- 2. Center Control Panel (Middle) ---
            Panel pnlControl = new Panel
            {
                Left = 400,
                Top = 80,
                Width = 220,
                Height = 440,
                BackColor = Color.FromArgb(215, 225, 235)
            };
            this.Controls.Add(pnlControl);

            Label lblCtrlTitle = new Label
            {
                Text = "Control Panel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 64, 102),
                Top = 10,
                Left = 10,
                AutoSize = true
            };
            pnlControl.Controls.Add(lblCtrlTitle);

            // Digital Screen inside Control Panel
            Panel pnlDigital = new Panel
            {
                Left = 20,
                Top = 40,
                Width = 180,
                Height = 80,
                BackColor = Color.FromArgb(15, 28, 36)
            };
            pnlControl.Controls.Add(pnlDigital);

            lblDigitalDisplay = new Label
            {
                Text = "▲\n1",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlDigital.Controls.Add(lblDigitalDisplay);

            // Control Buttons: Floor 1 & Floor 2
            Button btnCall1 = CreateStyledButton("▼ 1", 20, 135, 80, 70, Color.FromArgb(38, 140, 85));
            btnCall1.Click += (s, e) => RequestFloor(1);
            pnlControl.Controls.Add(btnCall1);

            Button btnCall2 = CreateStyledButton("▲ 2", 120, 135, 80, 70, Color.FromArgb(38, 140, 85));
            btnCall2.Click += (s, e) => RequestFloor(2);
            pnlControl.Controls.Add(btnCall2);

            // Door Open & Close Buttons
            Button btnDoorOpen = CreateStyledButton("Door Open", 20, 220, 80, 60, Color.FromArgb(60, 80, 100));
            btnDoorOpen.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnDoorOpen.Click += (s, e) => {
                if (elevator.State == ElevatorStateEnum.Idle) elevator.State = ElevatorStateEnum.OpeningDoors;
            };
            pnlControl.Controls.Add(btnDoorOpen);

            Button btnDoorClose = CreateStyledButton("Door Close", 120, 220, 80, 60, Color.FromArgb(60, 80, 100));
            btnDoorClose.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnDoorClose.Click += (s, e) => {
                if (elevator.State == ElevatorStateEnum.DoorsOpen) elevator.State = ElevatorStateEnum.ClosingDoors;
            };
            pnlControl.Controls.Add(btnDoorClose);

            // Emergency Stop Button
            Button btnEmergency = CreateStyledButton("EMERGENCY STOP", 20, 300, 180, 65, Color.FromArgb(215, 45, 45));
            btnEmergency.Click += (s, e) => {
                elevator.State = ElevatorStateEnum.EmergencyStop;
                AddLogEntry(elevator.CurrentFloor, elevator.CurrentFloor, "EMERGENCY STOP TRIGGERED!");
            };
            pnlControl.Controls.Add(btnEmergency);

            // --- 3. Floor Status Cards (Top Right) ---
            Label lblStatusTitle = new Label
            {
                Text = "Floor Status",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 64, 102),
                Left = 640,
                Top = 80,
                AutoSize = true
            };
            this.Controls.Add(lblStatusTitle);

            // Floor 2 Card
            panelF2Status = new Panel { Left = 640, Top = 105, Width = 420, Height = 60, BackColor = Color.White };
            this.Controls.Add(panelF2Status);
            Label lblF2Title = new Label { Text = "Floor 2", Font = new Font("Segoe UI", 11, FontStyle.Bold), Left = 40, Top = 10, AutoSize = true };
            lblF2Sub = new Label { Text = "Elevator not here", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Left = 40, Top = 30, AutoSize = true };
            lblF2Badge = new Label { Text = "Inactive", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.SlateGray, Left = 330, Top = 15, Width = 70, Height = 28, TextAlign = ContentAlignment.MiddleCenter };
            panelF2Status.Controls.AddRange(new Control[] { lblF2Title, lblF2Sub, lblF2Badge });

            // Floor 1 Card
            panelF1Status = new Panel { Left = 640, Top = 175, Width = 420, Height = 60, BackColor = Color.FromArgb(235, 248, 240) };
            this.Controls.Add(panelF1Status);
            Label lblF1Title = new Label { Text = "Floor 1", Font = new Font("Segoe UI", 11, FontStyle.Bold), Left = 40, Top = 10, AutoSize = true };
            lblF1Sub = new Label { Text = "Elevator here", Font = new Font("Segoe UI", 9), ForeColor = Color.DarkGreen, Left = 40, Top = 30, AutoSize = true };
            lblF1Badge = new Label { Text = "Active", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(38, 140, 85), Left = 330, Top = 15, Width = 70, Height = 28, TextAlign = ContentAlignment.MiddleCenter };
            panelF1Status.Controls.AddRange(new Control[] { lblF1Title, lblF1Sub, lblF1Badge });

            // --- 4. Operation Log Table (Middle Right) ---
            Label lblLogTitle = new Label
            {
                Text = "Operation Log",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 64, 102),
                Left = 640,
                Top = 250,
                AutoSize = true
            };
            this.Controls.Add(lblLogTitle);

            gridLog = new DataGridView
            {
                Left = 640,
                Top = 280,
                Width = 420,
                Height = 240,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            gridLog.Columns.Add("LogID", "Log ID");
            gridLog.Columns.Add("DateTime", "Date & Time");
            gridLog.Columns.Add("From", "From");
            gridLog.Columns.Add("To", "To");
            gridLog.Columns.Add("Desc", "Event Description");

            gridLog.Columns[0].Width = 50;
            gridLog.Columns[1].Width = 110;
            gridLog.Columns[2].Width = 40;
            gridLog.Columns[3].Width = 40;
            gridLog.Columns[4].Width = 180;
            this.Controls.Add(gridLog);

            // --- 5. Outside Requests Bottom Panel ---
            Panel pnlOutside = new Panel { Left = 20, Top = 540, Width = 520, Height = 100, BackColor = Color.White };
            this.Controls.Add(pnlOutside);

            Label lblReqTitle = new Label { Text = "Outside Requests", Font = new Font("Segoe UI", 10, FontStyle.Bold), Left = 10, Top = 10, AutoSize = true };
            pnlOutside.Controls.Add(lblReqTitle);

            Button btnReq1 = CreateStyledButton("▼ Request Floor 1", 15, 35, 230, 45, Color.FromArgb(52, 102, 153));
            btnReq1.Click += (s, e) => RequestFloor(1);
            pnlOutside.Controls.Add(btnReq1);

            Button btnReq2 = CreateStyledButton("▲ Request Floor 2", 260, 35, 230, 45, Color.FromArgb(52, 102, 153));
            btnReq2.Click += (s, e) => RequestFloor(2);
            pnlOutside.Controls.Add(btnReq2);

            // --- 6. Safety Notice Box ---
            Panel pnlSafety = new Panel { Left = 560, Top = 540, Width = 500, Height = 100, BackColor = Color.FromArgb(254, 248, 228) };
            this.Controls.Add(pnlSafety);

            Label lblSafetyHeader = new Label { Text = "⚠ SAFETY NOTICE", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DarkGoldenrod, Left = 15, Top = 15, AutoSize = true };
            Label lblSafetyBody = new Label
            {
                Text = "Do not overload the elevator. Maximum capacity: 6 persons (450 kg).\nKeep clear of the doors. Always follow safety instructions.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.DarkSlateGray,
                Left = 15,
                Top = 38,
                AutoSize = true
            };
            pnlSafety.Controls.AddRange(new Control[] { lblSafetyHeader, lblSafetyBody });
        }

        private Button CreateStyledButton(string text, int left, int top, int width, int height, Color bg)
        {
            Button btn = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void InitializeSimulation()
        {
            elevator = new ElevatorContext(floor1Y);

            mainTimer = new Timer { Interval = 25 };
            mainTimer.Tick += MainTimer_Tick;
            mainTimer.Start();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            elevator.Update(floor1Y, floor2Y, (fromF, toF, desc) => {
                AddLogEntry(fromF, toF, desc);
            });

            UpdateUIState();
            this.Invalidate();
        }

        private void RequestFloor(int floor)
        {
            if (elevator.State == ElevatorStateEnum.EmergencyStop) return;

            if (!elevator.RequestQueue.Contains(floor) && elevator.TargetFloor != floor)
            {
                elevator.RequestQueue.Add(floor);
            }
        }

        private void UpdateUIState()
        {
            // Digital Screen update
            string arrow = elevator.State == ElevatorStateEnum.MovingUp ? "▲" : (elevator.State == ElevatorStateEnum.MovingDown ? "▼" : "");
            lblDigitalDisplay.Text = $"{arrow}\n{elevator.CurrentFloor}";

            // Cards status
            if (elevator.CurrentFloor == 1)
            {
                panelF1Status.BackColor = Color.FromArgb(235, 248, 240);
                lblF1Sub.Text = "Elevator here";
                lblF1Sub.ForeColor = Color.DarkGreen;
                lblF1Badge.Text = "Active";
                lblF1Badge.BackColor = Color.FromArgb(38, 140, 85);

                panelF2Status.BackColor = Color.White;
                lblF2Sub.Text = "Elevator not here";
                lblF2Sub.ForeColor = Color.Gray;
                lblF2Badge.Text = "Inactive";
                lblF2Badge.BackColor = Color.SlateGray;
            }
            else
            {
                panelF2Status.BackColor = Color.FromArgb(235, 248, 240);
                lblF2Sub.Text = "Elevator here";
                lblF2Sub.ForeColor = Color.DarkGreen;
                lblF2Badge.Text = "Active";
                lblF2Badge.BackColor = Color.FromArgb(38, 140, 85);

                panelF1Status.BackColor = Color.White;
                lblF1Sub.Text = "Elevator not here";
                lblF1Sub.ForeColor = Color.Gray;
                lblF1Badge.Text = "Inactive";
                lblF1Badge.BackColor = Color.SlateGray;
            }
        }

        private void AddLogEntry(int fromF, int toF, string desc)
        {
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            gridLog.Rows.Insert(0, logCounter++, timeStr, fromF, toF, desc);

            if (!dbWorker.IsBusy)
            {
                var data = new LogData { ElevatorName = elevator.Name, Floor = toF, Status = elevator.State.ToString(), Note = desc };
                dbWorker.RunWorkerAsync(data);
            }
        }

        private void InitializeBackgroundWorker()
        {
            dbWorker = new BackgroundWorker();
            dbWorker.DoWork += DbWorker_DoWork;
        }

        private void DbWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is LogData data)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "INSERT INTO elevator_log (elevator_name, floor_no, status, note, event_time) VALUES (@name, @floor, @status, @note, @time)";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", data.ElevatorName);
                            cmd.Parameters.AddWithValue("@floor", data.Floor);
                            cmd.Parameters.AddWithValue("@status", data.Status);
                            cmd.Parameters.AddWithValue("@note", data.Note);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DB Sync Error: " + ex.Message);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // --- Render 2D Realistic Shaft (Left Panel) ---
            int shaftX = 20;
            int shaftY = 80;
            int shaftW = 360;
            int shaftH = 440;

            // Outer Frame
            g.FillRectangle(Brushes.LightGray, shaftX, shaftY, shaftW, shaftH);
            g.DrawRectangle(new Pen(Color.FromArgb(100, 120, 140), 3), shaftX, shaftY, shaftW, shaftH);

            // Floor Labels
            g.FillRectangle(Brushes.DarkSlateGray, shaftX + 10, shaftY + 20, 80, 25);
            g.DrawString("FLOOR 2", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White, shaftX + 20, shaftY + 24);

            g.FillRectangle(Brushes.DarkSlateGray, shaftX + 10, shaftY + 180, 80, 25);
            g.DrawString("FLOOR 1", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White, shaftX + 20, shaftY + 184);

            // Floor Displays over Elevator Shaft Doors
            g.FillRectangle(Brushes.Black, shaftX + 180, shaftY + 15, 40, 20);
            g.DrawString("▲ 2", new Font("Segoe UI", 8, FontStyle.Bold), Brushes.LimeGreen, shaftX + 188, shaftY + 18);

            g.FillRectangle(Brushes.Black, shaftX + 180, shaftY + 175, 40, 20);
            g.DrawString("▼ 1", new Font("Segoe UI", 8, FontStyle.Bold), Brushes.LimeGreen, shaftX + 188, shaftY + 178);

            // Elevator Car Dimensions
            int carW = 180;
            int carH = 130;
            float carX = shaftX + 110;
            float carY = elevator.YPosition;

            // Car Interior (Light Blue gradient fill)
            g.FillRectangle(Brushes.LightSteelBlue, carX, carY, carW, carH);
            g.DrawRectangle(Pens.SteelBlue, carX, carY, carW, carH);

            // Ceiling Lamp
            g.FillRectangle(Brushes.White, carX + 50, carY + 2, 80, 8);

            // Handrail
            g.DrawLine(new Pen(Color.Silver, 4), carX + 10, carY + 70, carX + carW - 10, carY + 70);

            // Sliding Doors (Double Panels)
            float maxDoorW = carW / 2.0f;
            float currentW = maxDoorW * (1.0f - elevator.DoorOpenRatio);

            // Left Metallic Door
            g.FillRectangle(Brushes.Silver, carX, carY, currentW, carH);
            g.DrawRectangle(Pens.Gray, carX, carY, currentW, carH);

            // Right Metallic Door
            float rightDoorX = carX + carW - currentW;
            g.FillRectangle(Brushes.Silver, rightDoorX, carY, currentW, carH);
            g.DrawRectangle(Pens.Gray, rightDoorX, carY, currentW, carH);
        }
    }

    public class LogData
    {
        public string ElevatorName { get; set; }
        public int Floor { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}