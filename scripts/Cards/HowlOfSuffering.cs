using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class HowlOfSuffering : ModCardTemplate
{
    public HowlOfSuffering() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/HowlOfSuffering.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/HowlOfSuffering.png");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Snapshot before Gazed and draw triggers change the player's powers.
        int stacks = Owner.Creature.Powers.Sum(power => power.Type == PowerType.Debuff
            ? Math.Max(0, power.Amount)
            : power.AllowNegative ? Math.Max(0, -power.Amount) : 0);
        if (stacks == 0) return;
        await PowerCmd.Apply<GazedPower>(choiceContext, Owner.Creature, stacks, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, stacks * DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
