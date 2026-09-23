using guestFromHeresy.Cards;
using guestFromHeresy.Capabilities;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class PerpetualDecayPower : TemporaryIconPower
{
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Threshold", 20m), new DynamicVar("Progress", 0m)];
    public void SetThreshold(int threshold) => DynamicVars["Threshold"].BaseValue = threshold;
    public async Task CountTrigger(PlayerChoiceContext context, int layers)
    {
        if (Owner.IsDead) return;
        var progress = DynamicVars["Progress"];
        progress.BaseValue += layers;
        int threshold = DynamicVars["Threshold"].IntValue;
        int procs = (int)(progress.BaseValue / threshold);
        progress.BaseValue %= threshold;
        if (procs > 0) await PowerCmd.Apply<GazedPower>(context, Owner, procs, Owner, null);
    }
}
