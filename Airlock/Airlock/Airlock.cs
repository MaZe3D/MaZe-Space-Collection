namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    using SpaceEngineers.Game.ModAPI.Ingame;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal partial class Program
    {
        /// <summary>
        /// Class modeling an airlock.
        /// </summary>
        public class AirLock
        {
            private const int DEFAULT_DELAY = 4000;

            public double DelayInMiliseconds { get; set; } = DEFAULT_DELAY;
            public AirLockStatus Status { get; set; } = AirLockStatus.IDLE;


            public IDoor Door1 { get; private set; }
            public IDoor Door2 { get; private set; }
            public List<IMyAirVent> AirVents { get; private set; } = new List<IMyAirVent>();
            internal int doorOpened = 0;
            internal DateTime t1 = DateTime.Now;


            /// <summary>
            /// Manages this airlock.
            /// </summary>
            public void Manager()
            {

                int ALSreturn = ALS();
                
                switch (Status)
                {
                    case AirLockStatus.IDLE:
                        if ((doorOpened = ALSreturn) != 0)
                        {
                            Status = AirLockStatus.IN;
                            t1 = DateTime.Now;
                        }
                        break;
                    case AirLockStatus.IN:
                        if ((DateTime.Now - t1).TotalMilliseconds >= DelayInMiliseconds)
                        {
                            Status = AirLockStatus.CLOSE_DOOR;
                        }
                        break;
                    case AirLockStatus.CLOSE_DOOR:
                        CloseDoors();
                        if (ALSreturn == 0)
                        {
                            Status = AirLockStatus.DEPRESSURIZE;
                            AirVents.ForEach(v => v.Depressurize = true);
                            AirVents.ForEach(v => v.Enabled= true);
                            t1 = DateTime.Now;
                        }
                        break;
                    case AirLockStatus.DEPRESSURIZE:
                        if (AirVents.All(v => v.GetOxygenLevel() == 0))
                        {
                            Status = AirLockStatus.OUT;
                            AirVents.ForEach(v => v.Enabled = false);
                        }

                        break;
                    case AirLockStatus.OUT:
                        if (doorOpened == 1)
                            Door2.OpenDoor();
                        else if (doorOpened == 2)
                            Door1.OpenDoor();

                        if ((DateTime.Now - t1).TotalMilliseconds >= DelayInMiliseconds)
                        {
                            Status = AirLockStatus.CLOSE_DOOR_2;
                        }
                        break;
                    case AirLockStatus.CLOSE_DOOR_2:
                        CloseDoors();
                        if (ALSreturn == 0)
                        {
                            Status = AirLockStatus.IDLE;
                        }
                        break;

                    default:
                        break;
                }
            }

            /// <summary>
            /// The air lock state.
            /// </summary>
            /// <returns>The <see cref="int"/>.</returns>
            public int ALS()
            {
                if (Door1.Status == DoorStatus.Opening || Door1.Status == DoorStatus.Open || Door1.Status == DoorStatus.Closing)
                {
                    Door2.Enabled = false;
                    return 1;
                }
                else if (Door1.Status == DoorStatus.Closed)
                {
                    Door2.Enabled = true;
                }

                if (Door2.Status == DoorStatus.Opening || Door2.Status == DoorStatus.Open || Door2.Status == DoorStatus.Closing)
                {
                    Door1.Enabled = false;
                    return 2;
                }
                else if (Door2.Status == DoorStatus.Closed)
                {
                    Door1.Enabled = true;
                }

                return 0;
            }



            /// <summary>
            /// Closes all doors.
            /// </summary>
            public void CloseDoors()
            {
                Door1.CloseDoor();
                Door2.CloseDoor();
            }

            /// <summary>
            /// The IsValid.
            /// </summary>
            /// <returns>The <see cref="bool"/>.</returns>
            public bool IsValid()
            {
                return Door1 != null && Door2 != null;
            }

            public void SetDoor(int idx, IDoor door)
            {
                switch(idx)
                {
                    case 1:
                        Door1 = door;
                        break;
                    case 2:
                        Door2 = door;
                        break;
                }
            }

            /// <summary>
            /// Adds an airvent to the lock.
            /// </summary>
            /// <param name="vent">vent to add</param>
            public void AddVent(IMyAirVent vent)
            {
                AirVents.Add(vent);
            }

            /// <summary>
            /// Defines the AirLockStatus.
            /// </summary>
            public enum AirLockStatus
            {
                INIT,
                IDLE,
                IN,
                CLOSE_DOOR,
                DEPRESSURIZE,
                OUT,
                CLOSE_DOOR_2

            }
        }
    }
}
