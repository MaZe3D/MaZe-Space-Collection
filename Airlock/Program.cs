using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        AirLock[] airLockList = new AirLock[10];
        DateTime time = DateTime.UtcNow;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            for (int i = 0; i < airLockList.Length; i++)
            {
                airLockList[i] = new AirLock();
            }
            initAirLocks();
            for (int i = 0; i < airLockList.Length; i++)
            {
                if (!airLockList[i].Valid)
                {
                    airLockList[i] = null;
                }
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            time = DateTime.UtcNow;
            Echo($"=== Airlock Manager v1.0 ===");
            for (int i = 0; i < airLockList.Length; i++)
            {
                if (airLockList[i] != null)
                {
                    Echo($"AirLock No. {i}: {airLockList[i].Status}");
                    airLockList[i].Manager();
                }
            }
            Echo("");
            Echo("=== Stistics ===");
            Echo($"Stript Runtime: {(DateTime.UtcNow - time).TotalSeconds}ms");
        }

        void initAirLocks()
        {
            List<IMyTerminalBlock> allDoors = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(allDoors);

            foreach (IMyDoor door in allDoors)
            {
                string[] parameter;
                int doorNumber;
                int airLockNumber;
                double delay;
                if (door.CustomName.StartsWith("AirLock_"))
                {
                    parameter = door.CustomName.Split('_', ' ');
                    if (parameter.Length >= 3)
                    {
                        doorNumber = Convert.ToInt32(parameter[2]);
                        airLockNumber = Convert.ToInt32(parameter[1]);
                        switch (doorNumber)
                        {
                            default:

                            case 1:
                                Echo("Case 1");
                                airLockList[airLockNumber].door1 = door;
                                break;
                            case 2:
                                airLockList[airLockNumber].door2 = door;
                                Echo("Case 2");
                                break;
                        }

                        if (parameter.Length >= 4)
                        {
                            try
                            {
                                delay = Convert.ToDouble(parameter[3]);
                                airLockList[airLockNumber].delayInMiliseconds = delay;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
        }
    }

    public class AirLock
    {
        public double delayInMiliseconds = 4000;
        public AirLockStatus Status = AirLockStatus.Idle;
        public IMyDoor door1;
        public IMyDoor door2;
        int doorOpened = 0;

        DateTime t1 = DateTime.Now;

        public AirLock()
        {

        }

        public void Manager()
        {
            if (!Valid)
                return;

            int ALSreturn = ALS();

            switch (Status)
            {
                case AirLockStatus.Idle:
                    if ((doorOpened = ALSreturn) != 0)
                    {
                        Status = AirLockStatus.In;
                        t1 = DateTime.Now;
                    }
                    break;
                case AirLockStatus.In:
                    if ((DateTime.Now - t1).TotalMilliseconds >= delayInMiliseconds)
                    {
                        Status = AirLockStatus.closeDoor;
                    }
                    break;
                case AirLockStatus.closeDoor:
                    closeDoors();
                    if (ALSreturn == 0)
                    {
                        Status = AirLockStatus.Out;
                        t1 = DateTime.Now;
                    }
                    break;
                case AirLockStatus.Out:
                    if (doorOpened == 1)
                        door2.OpenDoor();
                    else if (doorOpened == 2)
                        door1.OpenDoor();

                    if ((DateTime.Now - t1).TotalMilliseconds >= delayInMiliseconds)
                    {
                        Status = AirLockStatus.closeDoor2;
                    }
                    break;
                case AirLockStatus.closeDoor2:
                    closeDoors();
                    if (ALSreturn == 0)
                    {
                        Status = AirLockStatus.Idle;
                    }
                    break;

                default:
                    break;
            }
        }

        public int ALS()
        {
            if (door1.Status == DoorStatus.Opening || door1.Status == DoorStatus.Open || door1.Status == DoorStatus.Closing)
            {
                door2.Enabled = false;
                return 1;
            }
            else if (door1.Status == DoorStatus.Closed)
            {
                door2.Enabled = true;
            }

            if (door2.Status == DoorStatus.Opening || door2.Status == DoorStatus.Open || door2.Status == DoorStatus.Closing)
            {
                door1.Enabled = false;
                return 2;
            }
            else if (door2.Status == DoorStatus.Closed)
            {
                door1.Enabled = true;
            }

            return 0;
        }

        public bool Valid
        {
            get
            {
                if ((door1 != null) && (door2 != null))
                    return true;
                else return false;
            }
        }

        public void closeDoors()
        {
            if (Valid)
            {
                door1.CloseDoor();
                door2.CloseDoor();
            }
        }
    }
    public enum AirLockStatus
    {
        Init = 0,
        Idle = 1,
        In = 2,
        closeDoor = 3,
        Out = 4,
        closeDoor2 = 5,

    }
}