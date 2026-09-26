using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class RealitySolution() : TemporaryArtAttack(56, CardRarity.Uncommon)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [EnergyHoverTip];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(42m, ValueProp.Move)];
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card != this) { modifiedCost = originalCost; return false; }
        var applied = CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>()
            .Where(entry => entry.Amount > 0).Sum(entry => entry.Amount);
        modifiedCost = Math.Max(0m, originalCost - applied); return true;
    }
    public override Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount,
        MegaCrit.Sts2.Core.Entities.Creatures.Creature? applier, CardModel? source)
    {
        if (amount > 0) InvokeEnergyCostChanged();
        return Task.CompletedTask;
    }
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
    protected override void OnUpgrade() { EnergyCost.UpgradeBy(-14); }
}
