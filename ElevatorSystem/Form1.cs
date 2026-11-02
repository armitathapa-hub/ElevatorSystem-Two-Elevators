#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ElevatorSystem
{
    public partial class Form1 : Form, IElevatorObserver
    {
        // Controls whether MySQL database logging is used.
        private bool useDatabase = true;

        // The two elevators and the dispatcher that manages outside requests.
        private Elevator elevatorA = new Elevator("Elevator A");
        private Elevator elevatorB = new Elevator("Elevator B");
        private ElevatorDispatcher dispatcher = new ElevatorDispatcher();

        // One timer is used to update both elevators.
        private System.Windows.Forms.Timer animTimer;
        private delegate void TickAction();                     // Delegate used to store the methods that run on each timer tick.
        private TickAction tickActions;

        // Stores and manages the operation log.
        private DataTable logTable;
        private int localLogId = 0;
        private BackgroundWorker worker;
        private List<LogEntry> pendingLogs = new List<LogEntry>();
        private List<LogEntry> lastBatch = new List<LogEntry>();
        private Color onlineColor;

        public Form1()
        {
            InitializeComponent();
            HideOldSingleElevatorPanels();
            ArrangeLayout();

            try { picLogo.Image = ImageHelper.LoadImageCopy("icon_elevator.png"); }
            catch (Exception) { }                               // The system can still run if the logo cannot be loaded.

            // The form and dispatcher observe changes from both elevators.
            elevatorA.Attach(this);
            elevatorB.Attach(this);
            dispatcher.AddElevator(elevatorA);
            dispatcher.AddElevator(elevatorB);

            // Connects each visual panel to its elevator.
            panelA.Attach(elevatorA, "A");
            panelB.Attach(elevatorB, "B");
            panelA.StatusMessage += Say;                        // Receives status messages from Elevator A's panel.
            panelB.StatusMessage += Say;
            panelA.BringToFront();                              // Keeps the elevator panels visible above older controls.
            panelB.BringToFront();

            SetupLogTable();
            SetupWorker();
            onlineColor = lblOnline.ForeColor;

            // Adds all methods that need to run during each timer tick.
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 20;
            animTimer.Tick += AnimTimer_Tick;
            tickActions += elevatorA.Tick;                      // Updates the movement of Elevator A.
            tickActions += elevatorB.Tick;                      // Updates the movement of Elevator B.
            tickActions += panelA.RefreshView;                  // Refreshes the display for Elevator A.
            tickActions += panelB.RefreshView;                  // Refreshes the display for Elevator B.
            tickActions += RefreshLabels;                       // Updates the clock and floor status.

            btnRequest1.Click += btnRequest1_Click;
            btnRequest2.Click += btnRequest2_Click;
            btnViewLogs.Click += btnViewLogs_Click;
            Load += Form1_Load;

            panelA.RefreshView();
            panelB.RefreshView();
            RefreshLabels();
            animTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogEvent(1, 1, "Elevator system initialized: 2 elevators online.");
            if (ElevatorPanel.ImageError != "")
                LogEvent(1, 1, "Image problem: " + ElevatorPanel.ImageError);
        }

        // Hides the old single-elevator panels if they are still present.
        private void HideOldSingleElevatorPanels()
        {
            foreach (string name in new string[] { "pnlShaft", "pnlControl" })
                foreach (Control c in Controls.Find(name, true))
                    c.Visible = false;
        }

        // Arranges the main interface to provide space for both elevators.
        private void ArrangeLayout()
        {
            ClientSize = new Size(2800, 1700);
            MinimumSize = new Size(2400, 1500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(242, 246, 250);

            // Positions the header and system status information.
            pnlHeader.Dock = DockStyle.None;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Size = new Size(2800, 200);
            lblClock.Location = new Point(2260, 45);
            lblOnline.Location = new Point(2340, 115);

            // Positions the two elevator panels.
            panelA.Location = new Point(20, 230);
            panelA.Size = new Size(760, 1150);
            panelB.Location = new Point(800, 230);
            panelB.Size = new Size(760, 1150);

            // Positions the floor status section.
            pnlFloorStatus.Location = new Point(1580, 230);
            pnlFloorStatus.Size = new Size(1200, 430);
            lblStatusTitle.Location = new Point(30, 20);
            pnlCard2.Location = new Point(30, 90);
            pnlCard2.Size = new Size(1140, 130);
            pnlCard1.Location = new Point(30, 250);
            pnlCard1.Size = new Size(1140, 130);
            lblDot2.Location = new Point(25, 30);
            lblDot1.Location = new Point(25, 30);
            lblCard2Name.Location = new Point(100, 20);
            lblCard1Name.Location = new Point(100, 20);
            lblCard2Sub.Location = new Point(100, 70);
            lblCard1Sub.Location = new Point(100, 70);
            lblCard2Badge.Location = new Point(900, 35);
            lblCard1Badge.Location = new Point(900, 35);
            lblCard2Badge.Size = new Size(190, 60);
            lblCard1Badge.Size = new Size(190, 60);

            // Positions the operation log section.
            pnlLog.Location = new Point(1580, 690);
            pnlLog.Size = new Size(1200, 690);
            lblLogTitle.Location = new Point(30, 25);
            btnViewLogs.Location = new Point(930, 20);
            btnViewLogs.Size = new Size(220, 65);
            dgvLog.Location = new Point(30, 110);
            dgvLog.Size = new Size(1140, 530);

            // Positions the outside floor request controls.
            pnlOutside.Location = new Point(20, 1410);
            pnlOutside.Size = new Size(1540, 180);
            label2.Location = new Point(30, 20);
            btnRequest1.Location = new Point(30, 80);
            btnRequest1.Size = new Size(720, 75);
            btnRequest2.Location = new Point(790, 80);
            btnRequest2.Size = new Size(720, 75);

            // Positions the safety information section.
            pnlSafety.Location = new Point(1580, 1410);
            pnlSafety.Size = new Size(580, 180);
            label1.Location = new Point(20, 15);
            label3.Location = new Point(20, 55);
            label3.Size = new Size(540, 110);

            // Positions the emergency information section.
            pnlEmergencyNotice.Location = new Point(2180, 1410);
            pnlEmergencyNotice.Size = new Size(600, 180);
            label4.Location = new Point(20, 15);
            label5.Location = new Point(20, 55);
            label5.Size = new Size(560, 110);

            // Positions the footer and status message.
            pnlFooter.Dock = DockStyle.None;
            pnlFooter.Location = new Point(0, 1610);
            pnlFooter.Size = new Size(2800, 90);
            lblFooterLeft.Location = new Point(30, 30);
            lblMessage.Location = new Point(600, 20);
            lblMessage.Size = new Size(1500, 50);
            lblFooterRight.Location = new Point(2380, 30);
            lblFooterRight.Size = new Size(360, 30);
        }


        // Runs every 20 milliseconds to update the elevator system.
        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (tickActions != null) tickActions();             // Runs all methods currently stored in the delegate.
        }

        private void RefreshLabels()
        {
            lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            UpdateCard(pnlCard2, lblDot2, lblCard2Sub, lblCard2Badge, 2);
            UpdateCard(pnlCard1, lblDot1, lblCard1Sub, lblCard1Badge, 1);
        }

        // Updates a floor card to show whether an elevator is currently there.
        private void UpdateCard(Panel card, Label dot, Label sub, Label badge, int floor)
        {
            string here = "";
            if (Math.Abs(elevatorA.Position - floor) < 0.05) here = "A";
            if (Math.Abs(elevatorB.Position - floor) < 0.05) here = (here == "") ? "B" : "A & B";

            bool present = here != "";
            card.BackColor = present ? Color.FromArgb(232, 246, 236) : Color.White;
            dot.ForeColor = present ? Color.FromArgb(30, 150, 80) : Color.SlateGray;
            sub.Text = present ? "Elevator " + here + " here" : "No elevator here";
            badge.Text = present ? "Active" : "Inactive";
            badge.BackColor = present ? Color.FromArgb(30, 150, 80) : Color.SlateGray;
        }


        // The dispatcher selects an elevator when a request is made from outside.
        private void btnRequest1_Click(object sender, EventArgs e) { RequestOutside(1); }
        private void btnRequest2_Click(object sender, EventArgs e) { RequestOutside(2); }

        private void RequestOutside(int floor)
        {
            Elevator chosen = dispatcher.AssignHallCall(floor);
            if (chosen == null)
                Say("No elevator can take the Floor " + floor + " request now - it is waiting.");
            else
                Say("Outside request for Floor " + floor + " given to " + chosen.Name + ".");
        }


        // Receives elevator changes through the observer and adds them to the log.
        public void ElevatorChanged(Elevator elevator, int fromFloor, int toFloor, string description)
        {
            LogEvent(fromFloor, toFloor, description);
        }


        // Handles status messages and operation logging.
        private void Say(string message)                        // Displays the latest message in the footer.
        {
            lblMessage.Text = DateTime.Now.ToString("HH:mm:ss") + "    " + message;
        }

        private void LogEvent(int fromFloor, int toFloor, string description)
        {
            Say(description);
            LogEntry entry = new LogEntry(fromFloor, toFloor, description);

            AddLocalRow(entry);                                 // Shows the event locally even if the database is unavailable.

            if (useDatabase)
            {
                pendingLogs.Add(entry);
                StartWorker();                                  // Saves database records in the background.
            }
        }

        private void AddLocalRow(LogEntry entry)
        {
            localLogId++;
            DataRow row = logTable.NewRow();
            row["Log ID"] = localLogId;
            row["Date & Time"] = entry.Time;
            row["From Floor"] = entry.FromFloor;
            row["To Floor"] = entry.ToFloor;
            row["Event Description"] = entry.Description;
            logTable.Rows.InsertAt(row, 0);                     // Displays the newest event at the top.
            if (logTable.Rows.Count > 100) logTable.Rows.RemoveAt(100);
        }

        private void btnViewLogs_Click(object sender, EventArgs e)
        {
            if (useDatabase) StartWorker();                     // Refreshes the latest records from MySQL in the background.

            // Opens a larger window to make the operation logs easier to view.
            Form logForm = new Form();
            logForm.Text = "Operation Logs Viewer";
            logForm.Size = new Size(1600, 500);
            logForm.StartPosition = FormStartPosition.CenterParent;

            DataGridView popupGrid = new DataGridView();
            popupGrid.Dock = DockStyle.Fill;
            popupGrid.DataSource = logTable;
            popupGrid.ReadOnly = true;
            popupGrid.AllowUserToAddRows = false;
            popupGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            logForm.Controls.Add(popupGrid);
            logForm.ShowDialog(this);
        }

        private void SetupLogTable()
        {
            logTable = new DataTable();
            logTable.Columns.Add("Log ID", typeof(int));
            logTable.Columns.Add("Date & Time", typeof(DateTime));
            logTable.Columns.Add("From Floor", typeof(int));
            logTable.Columns.Add("To Floor", typeof(int));
            logTable.Columns.Add("Event Description", typeof(string));
            ShowLogTable(logTable);
        }

        private void ShowLogTable(DataTable table)
        {
            dgvLog.DataSource = table;

            dgvLog.ReadOnly = true;
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.RowHeadersVisible = false;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.BorderStyle = BorderStyle.None;
            dgvLog.BackgroundColor = Color.White;
            dgvLog.GridColor = Color.FromArgb(220, 228, 238);
            dgvLog.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvLog.EnableHeadersVisualStyles = false;
            dgvLog.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 64, 115);
            dgvLog.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLog.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(31, 64, 115);
            dgvLog.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvLog.ColumnHeadersHeight = 34;
            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 250);
            dgvLog.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 240);
            dgvLog.DefaultCellStyle.SelectionForeColor = Color.Black;

            SetColumn("Log ID", 12);
            SetColumn("Date & Time", 28);
            SetColumn("From Floor", 14);
            SetColumn("To Floor", 14);
            SetColumn("Event Description", 42);
            if (dgvLog.Columns.Contains("Date & Time"))
                dgvLog.Columns["Date & Time"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        }

        private void SetColumn(string name, float weight)
        {
            if (dgvLog.Columns.Contains(name)) dgvLog.Columns[name].FillWeight = weight;
        }


        // Uses a BackgroundWorker so database operations do not freeze the interface.
        private void SetupWorker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void StartWorker()
        {
            if (worker.IsBusy) return;                          // Keeps new log entries waiting until the worker is available.
            lastBatch = new List<LogEntry>(pendingLogs);
            pendingLogs.Clear();
            worker.RunWorkerAsync(lastBatch);
        }

        // Runs the database work on the background thread without changing UI controls.
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<LogEntry> batch = (List<LogEntry>)e.Argument;
            try
            {
                if (batch.Count > 0) DbHelper.SaveLogs(batch);
                e.Result = DbHelper.LoadLogs();
            }
            catch (Exception ex)
            {
                e.Result = ex;                                   // Passes the error back so it can be handled safely on the UI thread.
            }
        }

        // Handles the database result after the background work is finished.
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Exception error = e.Error;
            if (error == null && e.Result is Exception) error = (Exception)e.Result;

            if (error != null)
            {
                lblOnline.Text = "● DATABASE OFFLINE";           // Keeps the database problem visible in the interface.
                lblOnline.ForeColor = Color.Orange;
                Say("Database problem: " + error.Message);
                pendingLogs.InsertRange(0, lastBatch);           // Keeps unsaved entries so they can be tried again later.
                if (pendingLogs.Count > 500) pendingLogs.RemoveRange(0, pendingLogs.Count - 500);
                return;                                          // Local logs remain available even when MySQL is offline.
            }

            lblOnline.Text = "● SYSTEM ONLINE";
            lblOnline.ForeColor = onlineColor;
            logTable = (DataTable)e.Result;                      // Displays the latest records loaded from MySQL.
            ShowLogTable(logTable);
            if (pendingLogs.Count > 0) StartWorker();
        }

        private void dgvLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kept because the DataGridView click event is connected to this method in the Designer.
        }
    }
}