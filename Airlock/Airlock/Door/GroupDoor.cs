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
        public class GroupDoor : IDoor
        {
            private IMyBlockGroup _group;
            private List<IMyDoor> _doors;

            public GroupDoor(IMyBlockGroup group)
            {
                _group = group;
                _doors = new List<IMyDoor>();
                _group.GetBlocksOfType(_doors);
            }

            public bool Enabled
            {
                get
                {
                    return _doors.GroupBy(d => d.Enabled).OrderByDescending(g => g.Count()).First().First().Enabled;
                }

                set
                {
                    _doors.ForEach(d => d.Enabled = value);
                }
            }

            public string DisplayName
            {
                get
                {
                    return _group.Name;
                }
            }

            DoorStatus IDoor.Status
            {
                get
                {
                    return _doors.GroupBy(d => d.Status).OrderByDescending(g => g.Count()).First().First().Status;
                }
            }

            public void CloseDoor()
            {
                _doors.ForEach(d => d.CloseDoor());
            }

            public void OpenDoor()
            {
                _doors.ForEach(d => d.OpenDoor());
            }
        }
    }
}
