using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using guestFromHeresy.Cards;
using guestFromHeresy.Keywords;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Keywords;

AssemblyLoadContext.Default.Resolving += (context, name) =>
{
    string root = "F:/SteamLibrary/steamapps/common/Slay the Spire 2/";
    foreach (string dir in new[] { "data_sts2_windows_x86_64", "mods/STS2-RitsuLib/compat/0.111.0", "mods/STS2-RitsuLib/shared" })
    {
        string path = Path.Combine(root, dir, name.Name + ".dll");
        if (File.Exists(path)) return context.LoadFromAssemblyPath(path);
    }
    return null;
};
Run();

[MethodImpl(MethodImplOptions.NoInlining)]
static void Run()
{
    TestMode.IsOn = true;
    SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(SealedArtifact));
    SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(VoidForgingPower));
    int checks = 0;
    void Check(bool value, string message) { if (!value) throw new Exception(message); checks++; }
    Type[] parts = [typeof(InverseLevel), typeof(NegativeEnergyShield), typeof(BloomingCore), typeof(ArtisansHammer), typeof(TidalMarrow), typeof(DisintegrationProbe), typeof(RitualHilt), typeof(EnchantedMagnet)];
    for (int i = 0; i < parts.Length; i++)
    {
        var part = (ArtifactPartCard)RuntimeHelpers.GetUninitializedObject(parts[i]);
        Check(part.PartCode == i, "stable part hex mapping");
        Check(part.MaxUpgradeLevel == 0 && part.IsPart, "parts cannot upgrade");
    }
    var empty = (SealedArtifact)RuntimeHelpers.GetUninitializedObject(typeof(SealedArtifact));
    Check(empty.MaxUpgradeLevel == 1 && !empty.IsPart, "artifact can upgrade and is not a part");
    Check(!empty.EffectCodes.Any(), "blank template has no effects");
    Check(!empty.CanBeGeneratedInCombat && !empty.CanBeGeneratedByModifiers, "artifact excluded from normal generation");
    for (int a = 0; a < 8; a++) for (int b = 0; b < 8; b++) for (int c = 0; c < 8; c++)
    {
        var card = (SealedArtifact)RuntimeHelpers.GetUninitializedObject(typeof(SealedArtifact));
        card.Blueprint = 0x1000 | (a << 8) | (b << 4) | c;
        Check(card.EffectCodes.SequenceEqual(new[] {a,b,c}), "ordered recipe round trip");
        Check(card.TargetType == (new[] {a,b,c}.Any(x => x is 6 or 7) ? TargetType.AnyEnemy : new[] {a,b,c}.Contains(5) ? TargetType.AllEnemies : TargetType.Self), "composite targeting");
        var restored = (SealedArtifact)RuntimeHelpers.GetUninitializedObject(typeof(SealedArtifact));
        SavedProperties.FromInternal(card, null)!.FillInternal(restored);
        Check(restored.Blueprint == card.Blueprint && restored.EffectCodes.SequenceEqual(card.EffectCodes), "native saved properties preserve recipe");
        restored.Blueprint = 0x1777;
        Check(card.Blueprint == (0x1000 | (a << 8) | (b << 4) | c), "recipes do not share mutable state");
    }
    var forge = (VoidForgingPower)RuntimeHelpers.GetUninitializedObject(typeof(VoidForgingPower));
    forge.PendingCount = 2; forge.PendingRecipe = 0x01;
    var restoredForge = (VoidForgingPower)RuntimeHelpers.GetUninitializedObject(typeof(VoidForgingPower));
    SavedProperties.FromInternal(forge, null)!.FillInternal(restoredForge);
    Check(restoredForge.PendingCount == 2 && restoredForge.PendingRecipe == 1, "pending leading-zero recipe persists");
    Console.WriteLine($"{checks} checks passed across all 512 three-part recipes.");
}
