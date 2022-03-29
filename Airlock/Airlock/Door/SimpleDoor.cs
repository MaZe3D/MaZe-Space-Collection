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
    partial class Program
    {
        public class SimpleDoor : IDoor
        {
            private IMyDoor _door;

            public SimpleDoor(IMyDoor door)
            {
                _door = door;
            }

            public DoorStatus Status
            {
                get
                {
                    return _door.Status;
                }
            }

            public bool Enabled
            {
                get
                {
                    return _door.Enabled;
                }

                set
                {
                    _door.Enabled = value;
                }
            }

            public string DisplayName
            {
                get
                {
                    return _door.CustomName;
                }
            }

            public void CloseDoor()
            {
                _door.CloseDoor();
            }

            public void OpenDoor()
            {
                _door.OpenDoor();
            }

        }
    }
}
