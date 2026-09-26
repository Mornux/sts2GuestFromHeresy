using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using guestFromHeresy.Keywords;
using guestFromHeresy.Mechanics;
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
public sealed class SyzygyConfusion() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTags.MoonPhase];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip, HoverTipFactory.FromCard<Crescent>(), HoverTipFactory.FromCard<FullMoon>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain), HoverTipFactory.Static(StaticHoverTip.Transform)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var originals = PileType.Hand.GetPile(Owner).Cards.Where(card => card is Crescent or FullMoon).ToArray();
        foreach (var original in originals)
        {
            if (original.Pile?.Type != PileType.Hand || !original.IsTransformable || CombatManager.Instance.IsOverOrEnding) continue;
            CardModel replacement = original is Crescent ? CardScope!.CreateCard<FullMoon>(Owner) : CardScope!.CreateCard<Crescent>(Owner);
            if (original.IsUpgraded) CardCmd.Upgrade(replacement, CardPreviewStyle.None);
            replacement.GiveSingleTurnRetain();
            await CardCmd.Transform(original, replacement, CardPreviewStyle.None);
        }
    }
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
