#nullable disable
using System;
using System.Collections.Generic;

namespace ElevatorSystem
{
    public enum DoorPhase { Closed, Opening, Open, Closing }

    // Allows other parts of the system to listen for elevator changes.
    // Form1 and the dispatcher use this to receive elevator events.
    public interface IElevatorObserver
    {
        void ElevatorChanged(Elevator elevator, int fromFloor, int toFloor, string description);
    }


    // Contains the main logic and current information for one elevator.
    // It works with the Observer and State patterns used in the system.
    public class Elevator
    {
        public const int BottomFloor = 1;
        public const int TopFloor = 2;
        public const double Speed = 0.012;       // Distance the elevator moves during each timer tick.
        public const double DoorSpeed = 0.05;    // Controls how quickly the doors open and close.
        public const int HoldTicks = 100;        // Keeps the doors open for about two seconds.
        public const double Eps = 0.0001;        // Small value used when comparing decimal positions.

        public string Name;
        public double Position = 1.0;            // 1.0 is Floor 1, 2.0 is Floor 2, and values between them mean the elevator is moving.
        public double DoorPosition = 0.0;
        public DoorPhase Doors = DoorPhase.Closed;
        public IElevatorState State = new IdleState();
        public int DepartureFloor = 1;

        private List<int> carStops = new List<int>();    // Stores floor requests made from inside this elevator.
        private List<int> hallStops = new List<int>();   // Stores outside requests assigned by the dispatcher.
        private List<IElevatorObserver> observers = new List<IElevatorObserver>();
        private int holdCounter = 0;

        public Elevator(string name)
        {
            Name = name;
        }


        // Provides basic information about the current elevator.
        public int CurrentFloor { get { return (int)Math.Round(Position); } }
        public bool AtFloor { get { return Math.Abs(Position - Math.Round(Position)) < Eps; } }
        public bool IsEmergency { get { return State is EmergencyState; } }
        public bool HasStops { get { return carStops.Count > 0 || hallStops.Count > 0; } }
        public int StopCount { get { return AllStops().Count; } }
        public bool HasHallStop(int floor) { return hallStops.Contains(floor); }

        public List<int> AllStops()
        {
            List<int> all = new List<int>(carStops);
            foreach (int f in hallStops)
                if (!all.Contains(f)) all.Add(f);
            return all;
        }


        // Finds the nearest requested floor above the current position.
        public int NearestStopAbove()             // Returns -1 when there is no stop above.
        {
            int best = -1;
            foreach (int f in AllStops())
                if (f > Position + Eps && (best == -1 || f < best)) best = f;
            return best;
        }

        // Finds the nearest requested floor below the current position.
        public int NearestStopBelow()
        {
            int best = -1;
            foreach (int f in AllStops())
                if (f < Position - Eps && (best == -1 || f > best)) best = f;
            return best;
        }


        // Adds an observer so it can receive elevator events.
        public void Attach(IElevatorObserver observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
        }

        // Sends an elevator event to all attached observers.
        private void Notify(int fromFloor, int toFloor, string description)
        {
            foreach (IElevatorObserver o in new List<IElevatorObserver>(observers))
                o.ElevatorChanged(this, fromFloor, toFloor, description);
        }


        // Handles floor requests made inside the elevator or from outside.
        public string RequestCar(int floor) { return AddStop(carStops, floor, "Panel"); }
        public string AssignHall(int floor) { return AddStop(hallStops, floor, "Outside"); }

        private string AddStop(List<int> list, int floor, string source)
        {
            if (floor < BottomFloor || floor > TopFloor) return "Invalid floor.";
            if (IsEmergency) return Name + " is in emergency stop - press RESET EMERGENCY first.";

            if (State is IdleState && AtFloor && CurrentFloor == floor)      // Opens the doors if the elevator is already at the requested floor.
            {
                Notify(floor, floor, Name + ": " + source + " request for Floor " + floor + " (already here)");
                return OpenDoors();
            }

            if (!carStops.Contains(floor) && !hallStops.Contains(floor))
            {
                list.Add(floor);
                Notify(CurrentFloor, floor, Name + ": " + source + " request for Floor " + floor);
            }

            return "";
        }


        // Removes and returns outside requests if this elevator becomes unavailable.
        public List<int> TakeHallStops()
        {
            List<int> copy = new List<int>(hallStops);
            hallStops.Clear();
            return copy;
        }


        // Controls the opening and closing of the elevator doors.
        public string OpenDoors()
        {
            if (IsEmergency) return Name + " is in emergency stop - press RESET EMERGENCY first.";
            if (!(State is IdleState) || !AtFloor) return "Cannot open door while " + Name + " is moving!";
            if (Doors == DoorPhase.Closed || Doors == DoorPhase.Closing) Doors = DoorPhase.Opening;
            holdCounter = HoldTicks;
            return "";
        }

        public string CloseDoors()
        {
            if (IsEmergency) return Name + " is in emergency stop - press RESET EMERGENCY first.";
            if (Doors == DoorPhase.Open || Doors == DoorPhase.Opening) Doors = DoorPhase.Closing;
            return "";
        }


        // Stops the elevator immediately when an emergency is activated.
        public void EmergencyStop()
        {
            if (IsEmergency) return;
            carStops.Clear();                          // Cancels requests that were made from inside this elevator.
            State = new EmergencyState();              // Lets the dispatcher know that this elevator is unavailable.
            Notify(CurrentFloor, CurrentFloor, Name + ": EMERGENCY STOP at Floor " + CurrentFloor);
        }

        // Returns the elevator to normal operation after an emergency.
        public string ResetEmergency()
        {
            if (!IsEmergency) return "";
            State = new IdleState();
            if (!AtFloor) carStops.Add(CurrentFloor);  // Sends the elevator to the nearest floor if it stopped between floors.
            if (Doors == DoorPhase.Opening) Doors = DoorPhase.Closing;
            Notify(CurrentFloor, CurrentFloor, Name + ": emergency reset - ready");
            return "";
        }


        // Called by the timer to keep the elevator movement and doors updated.
        public void Tick()
        {
            if (IsEmergency) return;                   // Stops movement and door updates while emergency mode is active.

            if (Doors != DoorPhase.Closed)             // Prevents the elevator from moving while the doors are not closed.
            {
                UpdateDoors();
                return;
            }

            State.Move(this);                          // Lets the current state decide how the elevator should move.
        }


        // Updates the door opening, waiting and closing process.
        private void UpdateDoors()
        {
            if (Doors == DoorPhase.Opening)
            {
                DoorPosition += DoorSpeed;
                if (DoorPosition >= 1)
                {
                    DoorPosition = 1;
                    Doors = DoorPhase.Open;
                    holdCounter = HoldTicks;
                    Notify(CurrentFloor, CurrentFloor, Name + ": doors opened at Floor " + CurrentFloor);
                }
            }
            else if (Doors == DoorPhase.Open)
            {
                holdCounter--;
                if (holdCounter <= 0) Doors = DoorPhase.Closing;      // Closes the doors automatically after the waiting time.
            }
            else if (Doors == DoorPhase.Closing)
            {
                DoorPosition -= DoorSpeed;
                if (DoorPosition <= 0)
                {
                    DoorPosition = 0;
                    Doors = DoorPhase.Closed;
                    Notify(CurrentFloor, CurrentFloor, Name + ": doors closed at Floor " + CurrentFloor);
                }
            }
        }


        // Helper methods used by the elevator state classes.
        public void SetState(IElevatorState newState) { State = newState; }

        public void StartMoving(IElevatorState newState)
        {
            DepartureFloor = CurrentFloor;
            State = newState;
        }

        // Completes the current journey when the elevator reaches a floor.
        public void Arrive(int floor)
        {
            Position = floor;
            carStops.Remove(floor);
            hallStops.Remove(floor);
            Notify(DepartureFloor, floor, Name + " moved from Floor " + DepartureFloor + " to Floor " + floor);
            DepartureFloor = floor;
            Doors = DoorPhase.Opening;                 // Opens the doors automatically when the elevator arrives.
            holdCounter = HoldTicks;
        }


        // Estimates how much travel is needed for the elevator to reach a requested floor.
        // Existing stops are completed in the current direction before the elevator changes direction.
        public double EstimateCost(int target)
        {
            if (IsEmergency) return 9999;

            List<int> stops = AllStops();
            if (!stops.Contains(target)) stops.Add(target);
            stops.Sort();                                              // Arranges the requested floors in order.

            bool goingUp = (State is MovingUpState) ||
                           (!(State is MovingDownState) && target >= Position);

            List<int> route = new List<int>();
            if (goingUp)
            {
                for (int i = 0; i < stops.Count; i++)                  // Adds the floors above the elevator first.
                    if (stops[i] >= Position - Eps) route.Add(stops[i]);
                for (int i = stops.Count - 1; i >= 0; i--)             // Adds the lower floors after the upward journey.
                    if (stops[i] < Position - Eps) route.Add(stops[i]);
            }
            else
            {
                for (int i = stops.Count - 1; i >= 0; i--)             // Adds the floors below the elevator first.
                    if (stops[i] <= Position + Eps) route.Add(stops[i]);
                for (int i = 0; i < stops.Count; i++)                  // Adds the higher floors after the downward journey.
                    if (stops[i] > Position + Eps) route.Add(stops[i]);
            }

            double cost = (Doors != DoorPhase.Closed) ? 0.5 : 0;       // Adds extra time when the doors are still in use.
            double pos = Position;

            foreach (int s in route)
            {
                cost = cost + Math.Abs(s - pos);                       // Adds the distance between each stop.
                pos = s;
                if (s == target) return cost;
                cost = cost + 0.5;                                     // Adds a small delay for stops before the target floor.
            }

            return cost;
        }
    }
}