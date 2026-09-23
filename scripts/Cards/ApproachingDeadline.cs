using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ApproachingDeadline() : TemporaryArtSkill(1, CardRarity.Rare)
{
    protected override string PortraitName => nameof(ApproachingDeadline);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        var deadline = Owner.Creature.GetPower<DeadlinePower>();
        if (deadline != null) deadline.Reset();
        else await PowerCmd.Apply<DeadlinePower>(choiceContext, Owner.Creature, 13m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Energy.UpgradeValueBy(2m);
}
