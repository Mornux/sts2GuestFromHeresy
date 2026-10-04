using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class DespairShield() : TemporaryArtSkill(0, CardRarity.Uncommon)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Multiplier", 4m)];
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<DisintegrationPower>(context, Owner.Creature, 10m, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(DynamicVars["Multiplier"].BaseValue * Owner.Creature.GetPowerAmount<DisintegrationPower>(), ValueProp.Move), play);
    }
    protected override void OnUpgrade() => DynamicVars["Multiplier"].UpgradeValueBy(2m);
}
