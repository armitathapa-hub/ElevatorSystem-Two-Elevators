#nullable disable
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace ElevatorSystem
{
    // Finds and loads the elevator images from the Images folder.
    public static class ImageHelper
    {
        public static string FindImagePath(string fileName)
        {
            DirectoryInfo dir =
                new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null)
            {
                string candidate =
                    Path.Combine(
                        dir.FullName,
                        "Images",
                        fileName
                    );

                if (File.Exists(candidate))
                {
                    return candidate;
                }

                dir = dir.Parent;
            }

            return "";
        }

        public static Image LoadImageCopy(string fileName)
        {
            string path = FindImagePath(fileName);

            if (path == "")
            {
                throw new FileNotFoundException(
                    "Image not found: " + fileName
                );
            }

            using (Image original = Image.FromFile(path))
            {
                // Creates a copy so the original image file can be released.
                return new Bitmap(original);
            }
        }
    }


    // Represents the visual panel for one elevator.
    // The same panel is used for both Elevator A and Elevator B.
    public class ElevatorPanel : UserControl
    {
        // Images are shared because both elevators use the same graphics.
        private static Image[] doorFrames;
        private static Image shaftImage;
        private static bool imagesLoaded = false;

        public static string ImageError = "";

        // The elevator model represented by this panel.
        public Elevator Model;

        // Sends elevator messages to the status area in Form1.
        public event Action<string> StatusMessage;

        // Controls used in the elevator panel.
        private PictureBox picShaft;
        private PictureBox picCar;

        private Label lblName;
        private Label lblTitle;
        private Label lblDisplay;
        private Label lblFloorNow;
        private Label lblDirection;
        private Label lblDoorState;
        private Label lblState;

        private Button btnFloor1;
        private Button btnFloor2;
        private Button btnOpen;
        private Button btnClose;
        private Button btnEmergency;


        // Sets up the elevator panel.
        public ElevatorPanel()
        {
            LoadImagesOnce();
            BuildScreen();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (lblName == null || lblTitle == null)
                return;

            if (Name == "panelA")
            {
                lblName.Text = "ELEVATOR A";
                lblTitle.Text = "CONTROL PANEL - A";
            }
            else if (Name == "panelB")
            {
                lblName.Text = "ELEVATOR B";
                lblTitle.Text = "CONTROL PANEL - B";
            }
        }


        // Connects this panel with its elevator model.
        public void Attach(
    Elevator model,
    string letter
)
        {
            Model = model;

            lblName.Text =
                "ELEVATOR " + letter;

            lblTitle.Text =
                "CONTROL PANEL - " + letter;
        }


        // Loads the shaft and door images once for both elevators.
        private static void LoadImagesOnce()
        {
            if (imagesLoaded)
            {
                return;
            }

            imagesLoaded = true;

            try
            {
                Image[] frames =
                    new Image[4];

                for (int i = 0; i < 4; i++)
                {
                    frames[i] =
                        ImageHelper.LoadImageCopy(
                            "car_door_" + i + ".png"
                        );
                }

                doorFrames = frames;

                shaftImage =
                    ImageHelper.LoadImageCopy(
                        "shaft_bg.png"
                    );

                ImageError = "";
            }
            catch (Exception ex)
            {
                doorFrames = null;
                shaftImage = null;

                ImageError = ex.Message;
            }
        }


        // Builds the elevator shaft and control panel interface.
        private void BuildScreen()
        {
            SuspendLayout();

            // Keeps the panel size consistent with Form1.
            Size = new Size(760, 1150);

            MinimumSize =
                new Size(760, 1150);

            MaximumSize =
                new Size(760, 1150);

            BackColor = Color.White;

            // Controls use fixed positions inside this panel.
            AutoScaleMode = AutoScaleMode.None;


            // Left side of the panel containing the elevator shaft.
            Panel pnlShaft =
                new Panel();

            pnlShaft.Location =
                new Point(0, 0);

            pnlShaft.Size =
                new Size(350, 1150);

            pnlShaft.BackColor =
                Color.FromArgb(
                    220,
                    230,
                    240
                );

            Controls.Add(pnlShaft);


            // Elevator name.
            lblName =
                MakeLabel(
                    pnlShaft,
                    "ELEVATOR",
                    10,
                    8,
                    330,
                    48,
                    15f,
                    true
                );

            lblName.TextAlign =
                ContentAlignment.MiddleCenter;

            lblName.ForeColor =
                Color.FromArgb(
                    31,
                    64,
                    115
                );


            // Floor 2 label.
            Label floor2Label =
                MakeLabel(
                    pnlShaft,
                    "FLOOR 2",
                    10,
                    65,
                    330,
                    40,
                    10f,
                    true
                );

            floor2Label.TextAlign =
                ContentAlignment.MiddleCenter;


            // Displays the elevator shaft image.
            picShaft =
                new PictureBox();

            picShaft.Location =
                new Point(15, 110);

            picShaft.Size =
                new Size(320, 900);

            picShaft.SizeMode =
                PictureBoxSizeMode.StretchImage;

            picShaft.BorderStyle =
                BorderStyle.FixedSingle;

            picShaft.BackColor =
                Color.FromArgb(
                    190,
                    205,
                    220
                );

            if (shaftImage != null)
            {
                picShaft.Image =
                    shaftImage;
            }

            pnlShaft.Controls.Add(
                picShaft
            );


            // Displays the moving elevator car inside the shaft.
            picCar =
                new PictureBox();

            picCar.SizeMode =
                PictureBoxSizeMode.StretchImage;

            picCar.BackColor =
                Color.Transparent;

            // Places the elevator car inside the shaft so it can move within it.
            picShaft.Controls.Add(
                picCar
            );

            picCar.BringToFront();


            // Floor 1 label.
            Label floor1Label =
                MakeLabel(
                    pnlShaft,
                    "FLOOR 1",
                    10,
                    1060,
                    330,
                    45,
                    10f,
                    true
                );

            floor1Label.TextAlign =
                ContentAlignment.MiddleCenter;


            // Right side containing the elevator controls and status.
            Panel pnlControl =
                new Panel();

            pnlControl.Location =
                new Point(370, 0);

            pnlControl.Size =
                new Size(390, 1150);

            pnlControl.BackColor =
                Color.White;

            Controls.Add(
                pnlControl
            );


            // Shows which elevator control panel is being used.
            lblTitle =
                MakeLabel(
                    pnlControl,
                    "CONTROL PANEL",
                    5,
                    8,
                    380,
                    55,
                    13f,
                    true
                );

            lblTitle.TextAlign =
                ContentAlignment.MiddleCenter;

            lblTitle.ForeColor =
                Color.FromArgb(
                    25,
                    25,
                    25
                );


            // Floor selection buttons.
            btnFloor2 =
                MakeButton(
                    pnlControl,
                    "FLOOR 2",
                    5,
                    85,
                    180,
                    75,
                    Color.FromArgb(
                        46,
                        125,
                        50
                    )
                );

            btnFloor1 =
                MakeButton(
                    pnlControl,
                    "FLOOR 1",
                    200,
                    85,
                    180,
                    75,
                    Color.FromArgb(
                        46,
                        125,
                        50
                    )
                );

            btnFloor2.Tag = 2;
            btnFloor1.Tag = 1;

            btnFloor1.Click +=
                FloorButton_Click;

            btnFloor2.Click +=
                FloorButton_Click;


            // Door control buttons.
            btnOpen =
                MakeButton(
                    pnlControl,
                    "DOOR OPEN",
                    5,
                    180,
                    180,
                    75,
                    Color.FromArgb(
                        30,
                        136,
                        229
                    )
                );

            btnClose =
                MakeButton(
                    pnlControl,
                    "DOOR CLOSE",
                    200,
                    180,
                    180,
                    75,
                    Color.FromArgb(
                        30,
                        136,
                        229
                    )
                );

            btnOpen.Click +=
                btnOpen_Click;

            btnClose.Click +=
                btnClose_Click;


            // Emergency stop button.
            btnEmergency =
                MakeButton(
                    pnlControl,
                    "EMERGENCY STOP",
                    5,
                    275,
                    375,
                    80,
                    Color.FromArgb(
                        198,
                        40,
                        40
                    )
                );

            btnEmergency.Font =
                new Font(
                    "Segoe UI",
                    9.5f,
                    FontStyle.Bold
                );

            btnEmergency.Click +=
                btnEmergency_Click;


            // Digital display showing the current floor.
            lblDisplay =
                MakeLabel(
                    pnlControl,
                    "1",
                    5,
                    385,
                    375,
                    120,
                    34f,
                    true
                );

            lblDisplay.Font =
                new Font(
                    "Consolas",
                    32f,
                    FontStyle.Bold
                );

            lblDisplay.BackColor =
                Color.FromArgb(
                    10,
                    25,
                    35
                );

            lblDisplay.ForeColor =
                Color.LimeGreen;

            lblDisplay.TextAlign =
                ContentAlignment.MiddleCenter;


            // Shows the current elevator information.
            lblFloorNow =
                MakeLabel(
                    pnlControl,
                    "Current Floor: Floor 1",
                    10,
                    545,
                    370,
                    50,
                    12f,
                    false
                );

            lblFloorNow.TextAlign =
                ContentAlignment.MiddleLeft;

            lblDirection =
                MakeLabel(
                    pnlControl,
                    "Direction: IDLE",
                    10,
                    610,
                    370,
                    50,
                    12f,
                    false
                );

            lblDirection.TextAlign =
                ContentAlignment.MiddleLeft;

            lblDoorState =
                MakeLabel(
                    pnlControl,
                    "Door State: CLOSED",
                    10,
                    675,
                    370,
                    50,
                    12f,
                    false
                );

            lblDoorState.TextAlign =
                ContentAlignment.MiddleLeft;

            lblState =
                MakeLabel(
                    pnlControl,
                    "State: Idle",
                    10,
                    740,
                    370,
                    50,
                    12f,
                    false
                );

            lblState.TextAlign =
                ContentAlignment.MiddleLeft;

            // Gives a short instruction at the bottom of the panel.
            Label lblHint =
                MakeLabel(
                    pnlControl,
                    "Select a floor or door action",
                    10,
                    825,
                    370,
                    45,
                    10f,
                    false
                );

            lblHint.ForeColor =
                Color.DimGray;

            lblHint.TextAlign =
                ContentAlignment.MiddleCenter;

            ResumeLayout(false);
        }


        // Creates labels with the same style and positioning setup.
        private Label MakeLabel(
            Control parent,
            string text,
            int x,
            int y,
            int w,
            int h,
            float size,
            bool bold
        )
        {
            Label lbl =
                new Label();

            lbl.Text =
                text;

            lbl.Location =
                new Point(x, y);

            lbl.Size =
                new Size(w, h);

            lbl.AutoSize =
                false;

            lbl.BackColor =
                Color.Transparent;

            lbl.Font =
                new Font(
                    "Segoe UI",
                    size,
                    bold
                        ? FontStyle.Bold
                        : FontStyle.Regular
                );

            parent.Controls.Add(
                lbl
            );

            return lbl;
        }


        // Creates buttons with the same style and positioning setup.
        private Button MakeButton(
            Control parent,
            string text,
            int x,
            int y,
            int w,
            int h,
            Color back
        )
        {
            Button btn =
                new Button();

            btn.Text =
                text;

            btn.Location =
                new Point(x, y);

            btn.Size =
                new Size(w, h);

            btn.FlatStyle =
                FlatStyle.Flat;

            btn.FlatAppearance.BorderSize =
                0;

            btn.BackColor =
                back;

            btn.ForeColor =
                Color.White;

            btn.Font =
                new Font(
                    "Segoe UI",
                    9f,
                    FontStyle.Bold
                );

            btn.Cursor =
                Cursors.Hand;

            btn.TextAlign =
                ContentAlignment.MiddleCenter;

            btn.UseVisualStyleBackColor =
                false;

            parent.Controls.Add(
                btn
            );

            return btn;
        }


        // Sends a message to the main form when there is something to display.
        private void Say(string text)
        {
            if (
                text != ""
                && StatusMessage != null
            )
            {
                StatusMessage(text);
            }
        }


        // Handles a floor request made from inside the elevator.
        private void FloorButton_Click(
            object sender,
            EventArgs e
        )
        {
            int floor =
                Convert.ToInt32(
                    ((Button)sender).Tag
                );

            Say(
                Model.RequestCar(floor)
            );
        }


        // Handles the door open button.
        private void btnOpen_Click(
            object sender,
            EventArgs e
        )
        {
            Say(
                Model.OpenDoors()
            );
        }


        // Handles the door close button.
        private void btnClose_Click(
            object sender,
            EventArgs e
        )
        {
            Say(
                Model.CloseDoors()
            );
        }


        // Handles emergency stop and reset.
        private void btnEmergency_Click(
            object sender,
            EventArgs e
        )
        {
            if (Model.IsEmergency)
            {
                Say(
                    Model.ResetEmergency()
                );
            }
            else
            {
                Model.EmergencyStop();
            }
        }


        // Updates the elevator graphics and status while the system is running.
        // Form1 calls this regularly using the timer.
        public void RefreshView()
        {
            if (Model == null)
            {
                return;
            }


            // Updates the size and position of the elevator car.
            int w =
                picShaft.ClientSize.Width;

            int h =
                picShaft.ClientSize.Height;

            // Keeps the car size proportional to the shaft image.
            picCar.Width =
                w * 200 / 410;

            picCar.Height =
                h * 190 / 504;

            picCar.Left =
                w * 105 / 410;

            // Position at Floor 2.
            double top2 =
                30.0 / 504 * h;

            // Position at Floor 1.
            double top1 =
                290.0 / 504 * h;

            picCar.Top =
                (int)Math.Round(
                    top2
                    +
                    (Elevator.TopFloor - Model.Position)
                    *
                    (top1 - top2)
                );


            // Updates the door image to show the opening and closing animation.
            if (doorFrames != null)
            {
                int frame =
                    (int)Math.Round(
                        Model.DoorPosition * 3
                    );

                // Keeps the frame number within the available four images.
                if (frame < 0)
                {
                    frame = 0;
                }

                if (frame > 3)
                {
                    frame = 3;
                }

                picCar.Image =
                    doorFrames[frame];
            }
            else
            {
                // Uses simple colours if the door images cannot be loaded.
                picCar.Image = null;

                picCar.BackColor =
                    Model.DoorPosition > 0
                        ? Color.SeaGreen
                        : Color.SteelBlue;
            }


            // Updates the elevator direction.
            string arrow = "";

            if (Model.IsEmergency)
            {
                lblDirection.Text =
                    "Direction: STOPPED";
            }
            else if (Model.State is MovingUpState)
            {
                arrow = "▲ ";

                lblDirection.Text =
                    "Direction: UP";
            }
            else if (Model.State is MovingDownState)
            {
                arrow = "▼ ";

                lblDirection.Text =
                    "Direction: DOWN";
            }
            else
            {
                lblDirection.Text =
                    "Direction: IDLE";
            }


            // Updates the floor display and current state.
            lblDisplay.Text =
                arrow + Model.CurrentFloor;

            lblFloorNow.Text =
                "Current Floor: Floor "
                + Model.CurrentFloor;

            lblState.Text =
                "State: "
                + Model.State.Name;

            lblState.ForeColor =
                Model.IsEmergency
                    ? Color.Firebrick
                    : Color.Black;


            // Updates the displayed door state.
            if (Model.Doors == DoorPhase.Closed)
            {
                lblDoorState.Text =
                    "Door State: CLOSED";
            }
            else if (Model.Doors == DoorPhase.Open)
            {
                lblDoorState.Text =
                    "Door State: OPEN";
            }
            else if (Model.Doors == DoorPhase.Opening)
            {
                lblDoorState.Text =
                    "Door State: OPENING";
            }
            else
            {
                lblDoorState.Text =
                    "Door State: CLOSING";
            }


            // Changes the emergency button text depending on the current state.
            btnEmergency.Text =
                Model.IsEmergency
                    ? "RESET EMERGENCY"
                    : "EMERGENCY STOP";
        }
    }
}