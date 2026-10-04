using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Keywords;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ForgedMoon() : TemporaryArtAttack(0, CardRarity.Common)
{
    protected override string PortraitName => nameof(ForgedMoon);
    protected override HashSet<CardTag> CanonicalTags => [CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [CardTags.MoonPhaseHoverTip, HoverTipFactory.FromPower<GazedPower>(), ModKeywordRegistry.CreateHoverTip(ArtifactPart.Id)];
    protected override bool ShouldGlowGoldInternal => IsMutable && CombatState != null && ProjectionManager.IsGazed(Owner);
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool gazed = ProjectionManager.IsGazed(Owner);
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        if (!gazed || CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead
            || PileType.Hand.GetPile(Owner).Cards.Count >= CardPile.MaxCardsInHand
            || !Hook.ShouldDraw(CombatState!, Owner, false, out _)) return;
        var part = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(c => c.Keywords.Contains(ArtifactPart.Value));
        if (part == null) return;
        await CardPileCmd.Add(part, PileType.Draw, CardPilePosition.Top);
        await CardPileCmd.Draw(context, 1m, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
