using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class MillenniumRegret() : TemporaryArtSkill(2, CardRarity.Rare)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<guestFromHeresy.Powers.GazedPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GazedPower>(3m), new DynamicVar("MaxHp", 20m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GazedPower>(context, Owner.Creature, DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
        ProjectionManager.ProjectionGainMaxHp(Owner, DynamicVars["MaxHp"].IntValue);
    }
    protected override void OnUpgrade() { DynamicVars["GazedPower"].UpgradeValueBy(2m); DynamicVars["MaxHp"].UpgradeValueBy(5m); }
}
