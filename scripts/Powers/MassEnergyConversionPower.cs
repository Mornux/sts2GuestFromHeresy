using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class MassEnergyConversionPower : TemporaryIconPower
{
    public override string CustomIconPath => "res://guestFromHeresy/images/projection/Gazed_temp.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Progress", 0m)];
    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Owner || !Owner.IsAlive || amount <= 0) return;
        var progress = DynamicVars["Progress"];
        progress.BaseValue += amount;
        while (progress.BaseValue >= 5m && Owner.IsAlive)
        {
            progress.BaseValue -= 5m;
            await PowerCmd.Apply<GazedPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, card);
        }
    }
}
