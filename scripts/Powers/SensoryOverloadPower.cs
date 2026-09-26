using guestFromHeresy.Mechanics;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
namespace guestFromHeresy.Powers;
[RegisterPower]
public sealed class SensoryOverloadPower : TemporaryIconPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Progress", 0m)];
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card.Owner.Creature != Owner || !Owner.IsAlive || !SensoryCardHistory.IsZeroCost(play)) return;
        var progress = DynamicVars["Progress"];
        progress.BaseValue++;
        if (progress.BaseValue < 3) return;
        progress.BaseValue -= 3;
        await PowerCmd.Apply<GazedPower>(context, Owner, Amount, Owner, null);
    }
}
