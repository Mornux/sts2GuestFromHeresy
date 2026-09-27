using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class LunarHalo() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip];
    protected override HashSet<CardTag> CanonicalTags => [CardTags.MoonPhase];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (PileType.Hand.GetPile(Owner).Cards.Count >= CardPile.MaxCardsInHand ||
            !MegaCrit.Sts2.Core.Hooks.Hook.ShouldDraw(CombatState!, Owner, false, out _)) return;
        var card = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(card => card.Tags.Contains(CardTags.MoonPhase));
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
        await CardPileCmd.Draw(context, 1m, Owner);
    }
    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}
