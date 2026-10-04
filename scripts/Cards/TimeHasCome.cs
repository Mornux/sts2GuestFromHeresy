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
public sealed class TimeHasCome() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override string PortraitName => nameof(TimeHasCome);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new EnergyVar("EmptyEnergy", 2)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [EnergyHoverTip, HoverTipFactory.FromPower<GazedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal energy = Owner.Creature.GetPowerAmount<GazedPower>() > 0
            ? DynamicVars["EmptyEnergy"].BaseValue : DynamicVars.Energy.BaseValue;
        await PlayerCmd.GainEnergy(energy, Owner);
    }

    protected override void OnUpgrade() { }
}
