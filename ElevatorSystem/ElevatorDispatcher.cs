#nullable disable
using System;
using System.Collections.Generic;

namespace ElevatorSystem
{
    // Assigns each outside request to the elevator with the shortest estimated response time.
    // It also observes both elevators so outside requests can be reassigned during an emergency.
    public class ElevatorDispatcher : IElevatorObserver
    {
        private List<Elevator> elevators = new List<Elevator>();
        private List<int> pendingCalls = new List<int>();      // Stores outside requests that are waiting for an available elevator.

        public void AddElevator(Elevator e)
        {
            elevators.Add(e);
            e.Attach(this);                                    // Allows the dispatcher to receive events from this elevator.
        }

        // Selects an elevator for the outside request, or returns null if none is available.
        public Elevator AssignHallCall(int floor)
        {
            // Returns the elevator if this outside request has already been assigned.
            foreach (Elevator e in elevators)
            {
                if (!e.IsEmergency && e.HasHallStop(floor))
                    return e;
            }

            Elevator best = null;
            double bestCost = double.MaxValue;

            foreach (Elevator e in elevators)
            {
                // Skips an elevator while it is in emergency mode.
                if (e.IsEmergency)
                    continue;

                double cost = e.EstimateCost(floor);

                if (cost < bestCost - 0.0001)
                {
                    best = e;
                    bestCost = cost;
                }
                else if (best != null &&
                         Math.Abs(cost - bestCost) < 0.0001 &&
                         e.StopCount < best.StopCount)
                {
                    best = e;
                }
            }

            // Keeps the request waiting if both elevators are unavailable.
            if (best == null)
            {
                if (!pendingCalls.Contains(floor))
                    pendingCalls.Add(floor);

                return null;
            }

            pendingCalls.Remove(floor);

            best.AssignHall(floor);

            return best;
        }


        // Receives elevator events and handles requests that need to be reassigned.
        public void ElevatorChanged(Elevator e, int fromFloor, int toFloor, string description)
        {
            if (e.IsEmergency)
            {
                foreach (int floor in e.TakeHallStops())       // Reassigns its outside requests to another available elevator.
                    AssignHallCall(floor);
            }
            else if (pendingCalls.Count > 0)
            {
                List<int> waiting = new List<int>(pendingCalls);
                pendingCalls.Clear();
                foreach (int floor in waiting)
                    AssignHallCall(floor);
            }
        }
    }
}