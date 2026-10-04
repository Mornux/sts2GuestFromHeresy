using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class FateTakeover() : TemporaryArtSkill(3, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(FateTakeover);

    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(21m, ValueProp.Move), new PowerVar<DisintegrationPower>(3m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<DisintegrationPower>(choiceContext, Owner.Creature,
            DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
    }

    public override async Task AfterAutoPrePlayPhaseEnteredEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Pile?.Type != PileType.Exhaust) return;
        await CardCmd.AutoPlay(choiceContext, this, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}
