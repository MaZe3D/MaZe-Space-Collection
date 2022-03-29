namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                _program.Echo("Loading doors...");
                var allDoors = new List<IMyDoor>();
                _program.GridTerminalSystem.GetBlocksOfType(allDoors, block => block.CustomName.StartsWith(AIRLOCK_PREFIX));
                _program.Echo($"Found {allDoors.Count} potential airlock-doors.");
                allDoors.ForEach(LoadDoor);

                CleaupInvalidAirlocks();
                _program.Echo("Done.");
            }


            /// <summary>
            /// Manages all airlocks.
            /// </summary>
            public void Manage()
            {
                _program.Echo($"=== Airlock Manager v1.0 ===");
                foreach (var pair in _airlocks.OrderBy(p => p.Key))
                {
                    _program.Echo($"AirLock No. {pair.Key}: {pair.Value.Status}");
                    pair.Value.Manager();

                }
                _program.Echo("");
                _program.Echo("=== Stistics ===");
                _program.Echo($"Stript Runtime: {_program.Runtime.LastRunTimeMs}ms");
            }


            private void LoadDoor(IMyDoor door)
            {
                var parameter = door.CustomName.Split(ARG_SEPERATORS);
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
                foreach (var pair in _airlocks)
                {
                    if (!pair.Value.IsValid())
                    {
                        _airlocks.Remove(pair.Key);
                    }
                }
            }

        }
    }
}
