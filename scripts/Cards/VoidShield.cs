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
public sealed class VoidShield() : TemporaryArtSkill(1, CardRarity.Common)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<guestFromHeresy.Powers.GazedPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxHp", 6m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GazedPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        ProjectionManager.ProjectionGainMaxHp(Owner, DynamicVars["MaxHp"].IntValue);
    }
    protected override void OnUpgrade() => DynamicVars["MaxHp"].UpgradeValueBy(2m);
}
