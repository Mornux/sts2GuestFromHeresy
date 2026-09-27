using guestFromHeresy.Pools;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
namespace guestFromHeresy.Cards;
[RegisterCard(typeof(CardPool))]
public sealed class Echolalia() : TemporaryArtSkill(2, CardRarity.Uncommon)
{
    private CardModel? _replacement;
    private PlayerChoiceContext? _context;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id), HoverTipFactory.Static(StaticHoverTip.Transform)];
    private CardModel? PreviousCard => !IsMutable || CombatState == null ? null
        : CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(entry => entry.CardPlay.Card.Owner == Owner && entry.HappenedThisTurn(CombatState))?.CardPlay.Card;
    protected override bool IsPlayable => PreviousCard is { } previous && previous is not Echolalia;
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        var previous = PreviousCard;
        var preview = new LocString("cards", previous is null or Echolalia
            ? "GUEST_FROM_HERESY_CARD_ECHOLALIA.noPrevious"
            : "GUEST_FROM_HERESY_CARD_ECHOLALIA.hasPrevious");
        if (previous is not null and not Echolalia) preview.Add("PreviousCard", previous.Title);
        description.Add("TransformPreview", preview.GetFormattedText());
    }
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        bool active = card == this && Synaesthesia.IsActive(this);
        modifiedCost = active ? 0m : originalCost;
        return active;
    }
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (play.PlayIndex == 0)
        {
            var previous = PreviousCard;
            _replacement = previous is null or Echolalia ? null : previous.CreateClone();
            _context = context;
        }
        return Task.CompletedTask;
    }
    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || oldPileType != PileType.Play || _replacement == null || Pile == null) return;
        var replacement = _replacement;
        var context = _context!;
        _replacement = null;
        _context = null;
        if (!IsTransformable || CombatManager.Instance.IsOverOrEnding) return;
        var result = await CardCmd.Transform(this, replacement, CardPreviewStyle.None);
        if (result is { success: true }) await CardCmd.AutoPlay(context, replacement, null);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
