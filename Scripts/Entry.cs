using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace guestFromHeresy.Scripts;

// Required attribute for registering the mod. The string must match the initialization function name.
[ModInitializer(nameof(Init))]
public class Entry
{
    // Initialization function.
    public static void Init()
    {
        // Used for patching, which modifies the game's code.
        // The argument can be any unique identifier, as long as it does not conflict with another mod.
        var harmony = new Harmony("sts2.morn.guestFromHeresy");
        harmony.PatchAll();
        // Allows .tscn files to load custom scripts.
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        Log.Info("Mod initialized!");
    }
}
