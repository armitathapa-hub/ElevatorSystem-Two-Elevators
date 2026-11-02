#nullable disable
using System;

namespace ElevatorSystem
{
    // Defines the different movement states used by the elevator.
    // The elevator's behaviour changes depending on its current state.
    public interface IElevatorState
    {
        string Name { get; }
        void Move(Elevator e);          // Runs on each timer tick when the elevator doors are closed.
    }

    public class IdleState : IElevatorState
    {
        public string Name { get { return "Idle"; } }

        public void Move(Elevator e)
        {
            if (!e.HasStops) return;                          // Stays idle when there are no floor requests.

            int up = e.NearestStopAbove();
            int down = e.NearestStopBelow();

            if (up == -1 && down == -1)                       // Handles a request for the floor where the elevator already is.
            {
                e.Arrive(e.CurrentFloor);
                return;
            }
            if (down == -1 || (up != -1 && up - e.Position <= e.Position - down))
                e.StartMoving(new MovingUpState());
            else
                e.StartMoving(new MovingDownState());
        }
    }

    public class MovingUpState : IElevatorState
    {
        public string Name { get { return "MovingUp"; } }

        public void Move(Elevator e)
        {
            int next = e.NearestStopAbove();
            if (next == -1)                                   // Changes direction or becomes idle when there are no stops above.
            {
                if (e.NearestStopBelow() != -1) e.SetState(new MovingDownState());
                else e.SetState(new IdleState());
                return;
            }

            e.Position = e.Position + Elevator.Speed;         // Moves the elevator slightly upward on each timer tick.

            if (e.Position >= next - Elevator.Eps)            // Handles the elevator reaching the next requested floor.
            {
                e.Arrive(next);
                if (e.NearestStopAbove() == -1)
                {
                    if (e.NearestStopBelow() != -1) e.SetState(new MovingDownState());
                    else e.SetState(new IdleState());
                }
            }
        }
    }

    public class MovingDownState : IElevatorState
    {
        public string Name { get { return "MovingDown"; } }

        public void Move(Elevator e)
        {
            int next = e.NearestStopBelow();
            if (next == -1)
            {
                if (e.NearestStopAbove() != -1) e.SetState(new MovingUpState());
                else e.SetState(new IdleState());
                return;
            }

            e.Position = e.Position - Elevator.Speed;         // Moves the elevator slightly downward on each timer tick.

            if (e.Position <= next + Elevator.Eps)
            {
                e.Arrive(next);
                if (e.NearestStopBelow() == -1)
                {
                    if (e.NearestStopAbove() != -1) e.SetState(new MovingUpState());
                    else e.SetState(new IdleState());
                }
            }
        }
    }

    public class EmergencyState : IElevatorState
    {
        public string Name { get { return "Emergency"; } }
        public void Move(Elevator e) { /* Keeps the elevator stopped until the emergency is reset. */ }
    }
}