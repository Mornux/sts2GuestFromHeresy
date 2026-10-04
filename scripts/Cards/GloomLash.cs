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
public sealed class GloomLash() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(GloomLash);

    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int reduction = Synaesthesia.IsActive(this) ? 2 : 1;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (Owner.Creature.IsDead) return;
        var leftmost = PileType.Hand.GetPile(Owner).Cards.FirstOrDefault();
        if (leftmost != null && !leftmost.EnergyCost.CostsX) leftmost.EnergyCost.AddThisCombat(-reduction);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Retain);
    }
}
