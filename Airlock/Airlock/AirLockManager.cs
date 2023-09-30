namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal partial class Program
    {
        /// <summary>
        /// Class managing the airlock functionality.
        /// </summary>
        public class AirLockManager
        {
            private const string AIRLOCK_PREFIX = "AirLock_";
            private static readonly char[] ARG_SEPERATORS = { '_', ' ' };


            private readonly MyGridProgram _program;
            private readonly Dictionary<string, AirLock> _airlocks;

            private List<IMyTextPanel> _displayPanels;

            


            public AirLockManager(MyGridProgram program)
            {
                _program = program;
                _airlocks = new Dictionary<string, AirLock>();

            }

            /// <summary>
            /// Scans grid for airlocks.
            /// </summary>
            public void InitAirLocks()
            {
                Logger.AttachLogger(new FunctionalLogListener((string msg) => { _program.Echo(msg); return true; }));
                Logger.Log("Loading doors...");
                var allDoors = new List<IMyDoor>();
                _program.GridTerminalSystem.GetBlocksOfType(allDoors, block => block.CustomName.StartsWith(AIRLOCK_PREFIX));
                Logger.Log($"Found {allDoors.Count} airlock-doors.");
                allDoors.ForEach(d => LoadDoor(new SimpleDoor(d)));

                var groups = new List<IMyBlockGroup>();
                _program.GridTerminalSystem.GetBlockGroups(groups, group => group.Name.StartsWith(AIRLOCK_PREFIX));
                Logger.Log($"Found {groups.Count} airlock-groups.");
                groups.ForEach(d => LoadDoor(new GroupDoor(d)));

                var displays = new List<IMyTextPanel>();
                _program.GridTerminalSystem.GetBlocksOfType(displays, display => display.CustomName.StartsWith(AIRLOCK_PREFIX));
                Logger.Log($"Found {groups.Count} airlock-displays.");
                _displayPanels = displays;

                CleaupInvalidAirlocks();
                _program.Echo("Done.");
            }


            /// <summary>
            /// Manages all airlocks.
            /// </summary>
            public void Manage()
            {
                Logger.Log($"=== Airlock Manager v1.0 ===");
                foreach (var pair in _airlocks.OrderBy(p => p.Key))
                {
                    Logger.Log($"AirLock \"{pair.Key}\": {pair.Value.Status}");
                    pair.Value.Manager();
                }
                Logger.Log("");
                Logger.Log("=== Statistics ===");
                Logger.Log($"Script Runtime: {_program.Runtime.LastRunTimeMs}ms");
            }


            private void LoadDoor(IDoor door)
            {
                var parameter = door.DisplayName.Split(ARG_SEPERATORS);
                if (parameter.Length < 3) return;

                string airLockId = parameter[1];
                int doorNumber;
                if (!int.TryParse(parameter[2], out doorNumber)) return;

                AirLock airlock;
                if (_airlocks.ContainsKey(airLockId))
                {
                    airlock = _airlocks[airLockId];
                }
                else
                {
                    airlock = new AirLock();
                    _airlocks.Add(airLockId, airlock);
                }

                airlock.SetDoor(doorNumber, door);

                if (parameter.Length >= 4)
                {
                    try
                    {
                        airlock.DelayInMiliseconds = Double.Parse(parameter[3]);
                    } catch (FormatException)
                    {
                        // ignore exception and continue
                    }
                }

                _program.Echo($"Added Door #{doorNumber} to lock: {airLockId}");
            }
            
            private void CleaupInvalidAirlocks()
            {
                foreach (var pair in _airlocks.Where(p => !p.Value.IsValid()).ToList())
                {
                    _airlocks.Remove(pair.Key);
                }
            }
        }
    }
}
