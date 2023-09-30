namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal partial class Program
    {
        public class CameraManager : IManager
        {
            private const string CAMERA_PREFIX = "CoDe_";
            private List<Camera> _cameras = new List<Camera>();

            private MyGridProgram _program;

            private void LoadCamera(IMyCameraBlock camera)
            {
                _cameras.Add(new Camera(camera));
                Logger.Log($"Added camera \"{camera.CustomName}\" to list");
            }

            public CameraManager(MyGridProgram program)
            {
                _program = program;
            }

            public void Init()
            {
                Logger.AttachLogger(new FunctionalLogListener((string msg) => { _program.Echo(msg); return true; }));

                Logger.Log("Loading cameras...");
                var allCameras = new List<IMyCameraBlock>();
                _program.GridTerminalSystem.GetBlocksOfType(allCameras, block => block.CustomName.StartsWith(CAMERA_PREFIX));
                Logger.Log($"Found {allCameras.Count} cameras.");
                allCameras.ForEach(d => LoadCamera(d));
                EnableCameras();

            }
            public void Manage()
            {
                Logger.Log($"=== CoDe Manager v1.0 ===");
                _cameras.ForEach(d => Logger.Log($"Camera \"{d.Name}\" has range {d.AvailableScanRange}"));
                Logger.Log("");
                Logger.Log("=== Statistics ===");
                Logger.Log($"Script Runtime: {_program.Runtime.LastRunTimeMs}ms");
            }

            private void EnableCameras()
            {
                Logger.Log("Enabling Raycast on cameras...");
                try
                {
                    _cameras.ForEach(d => d.EnableRaycast = true);
                }
                catch (Exception e)
                {
                    Logger.Log($"Exception while enabling raycast on cameras: {e.Message}");
                }
            }
        }
    }
}
