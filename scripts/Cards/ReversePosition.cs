using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ReversePosition() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(ReversePosition);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Reduction", 50m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal reduction = Synaesthesia.IsActive(this) ? DynamicVars["Reduction"].BaseValue : 0m;
        var existing = Owner.Creature.GetPower<ReversalPower>();
        if (existing != null)
        {
            existing.SetAmount(3);
            existing.SetReduction(reduction);
            return;
        }
        var power = await PowerCmd.Apply<ReversalPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this);
        power?.SetReduction(reduction);
    }

    protected override void OnUpgrade() => DynamicVars["Reduction"].UpgradeValueBy(25m);
}
