namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;

    
    internal partial class Program : MyGridProgram
    {
        
        internal readonly AirLockManager manager;
        
        public Program()
        {

            manager = new AirLockManager(this);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            manager.InitAirLocks();
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            manager.Manage();
        }
    }
}
