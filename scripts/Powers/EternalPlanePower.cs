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
public sealed class EternalPlanePower : TemporaryIconPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Progress", 0m)];
    public static decimal AppliedDebuffLayers(PowerModel power, decimal delta)
    {
        if (power.Type == PowerType.Debuff) return Math.Max(0m, delta);
        if (!power.AllowNegative) return 0m;
        return Math.Max(0m, Math.Max(0m, -power.Amount) - Math.Max(0m, -(power.Amount - delta)));
    }
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? source)
    {
        if (applier != Owner || Owner.IsDead) return;
        var progress = DynamicVars["Progress"];
        progress.BaseValue += AppliedDebuffLayers(power, amount);
        int procs = (int)(progress.BaseValue / 42m);
        progress.BaseValue %= 42m;
        if (procs > 0) await PowerCmd.Apply<IntangiblePower>(context, Owner, procs * Amount, Owner, source);
    }
}
