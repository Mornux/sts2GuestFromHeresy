using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace guestFromHeresy.Scripts;

// Required attribute for registering the mod. The string must match the initialization function name.
[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "guestFromHeresy";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    // Initialization function.
    public static void Init()
    {
        // Used for patching, which modifies the game's code.
        // The argument can be any unique identifier, as long as it does not conflict with another mod.
        var harmony = new Harmony("sts2.morn.guestFromHeresy");
        harmony.PatchAll();

        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        Keywords.Synaesthesia.RegisterOutline();
        Logger.Info("Mod initialized!");
    }
}
