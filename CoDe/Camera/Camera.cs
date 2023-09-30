namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    using System;

    internal partial class Program
    {
        /// <summary>
        /// Class modeling an airlock.
        /// </summary>
        public class Camera
        {
            IMyCameraBlock _camera;
            public Camera(IMyCameraBlock camera)
            {
                _camera = camera;
            }

            //public IMyCameraBlock CameraBlock { get { return _camera; } }
            public string Name { get { return _camera.CustomName; } }
            public bool EnableRaycast
            {
                get { return _camera.EnableRaycast; }
                set
                {
                    if (_camera.Enabled)
                    {
                        _camera.EnableRaycast = value;
                    }
                    else
                    {
                        _camera.EnableRaycast = false;
                        throw new ConditionNotMetException("Cannot set EnableRaycast on disabled camera!");
                    }
                }
            }

            public bool CanScan(double distance) => _camera.CanScan(distance);
            public double AvailableScanRange => _camera.AvailableScanRange;
        }
    }
}
