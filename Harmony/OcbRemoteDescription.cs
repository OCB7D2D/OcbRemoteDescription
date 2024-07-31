using HarmonyLib;
using System.Reflection;

public class OcbRemoteDescription : IModApi
{

    public static ulong ResultsValid = 300;
    public static ulong ResultsRefresh = 50;
    public static ulong RefreshDelay = 15;

    public void InitMod(Mod mod)
    {
        Log.Out("OCB Harmony Patch: " + GetType().ToString());
        Harmony harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        // Make sure to reset all data when a new game is started
        ModEvents.GameStartDone.RegisterHandler(NetPkgCustomInfo.ResetInfo);
    }


    [HarmonyPatch(typeof(Block))]
    [HarmonyPatch("GetCustomDescription")]
    public class Block_GetCustomDescription
    {

        static ulong LastAsk = 0;

        public static bool Prefix(
            Vector3i _blockPos,
            BlockValue _bv,
            ref string __result)
        {
            if (_bv.type == BlockValue.Air.type) return true;
            if (!_bv.Block.Properties.GetBool("RemoteDescription")) return true;
            if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                __result = string.Empty; // Reset the result
                var wait = ResultsRefresh; // Default wait time
                // Check if cached information is still valid
                bool refresh = NetPkgCustomInfo.LastType != _bv.type ||
                    NetPkgCustomInfo.LastPosition != _blockPos;
                // Faster refresh at new position
                if (refresh) wait = RefreshDelay;
                // Data still valid?
                if (!refresh)
                {
                    // Check if data is still somewhat good to be trusted
                    if (NetPkgCustomInfo.LastTick + ResultsValid > GameTimer.Instance.ticks)
                    {
                        __result = NetPkgCustomInfo.LastText;
                    }
                    // Refresh if data is going to be out of sync soonish
                    refresh = NetPkgCustomInfo.LastTick + wait < GameTimer.Instance.ticks;
                }
                if (refresh && (LastAsk + wait < GameTimer.Instance.ticks))
                {
                    LastAsk = GameTimer.Instance.ticks;
                    // Try to lazy load the information from the server
                    // ToDo: use a specific package type for requests!?
                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(
                        NetPackageManager.GetPackage<NetPkgCustomInfo>()
                            .ToServer(_blockPos));
                }
                return false;
            }
            return true;
        }
    }
}
