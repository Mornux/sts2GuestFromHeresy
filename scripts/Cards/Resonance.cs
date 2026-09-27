using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Resonance() : TemporaryArtSkill(2, CardRarity.Uncommon)
{
    public override bool GainsBlock => true;
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(13m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id), HoverTipFactory.FromCard<Assimilation>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool active = Synaesthesia.IsActive(this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (active)
            await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<Assimilation>(Owner), PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        AddKeyword(CardKeyword.Retain);
    }
}
