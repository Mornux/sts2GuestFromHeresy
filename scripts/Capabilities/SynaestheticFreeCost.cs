using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Models.Capabilities;

namespace guestFromHeresy.Capabilities;

[RegisterModelCapability]
public sealed class SynaestheticFreeCost : ModelCapability<CardModel>, IModelCapabilityHookListener,
    ICardDescriptionContributor, ICardHoverTipContributor, ICardGlowContributor
{
    public IEnumerable<CardDescriptionFragment> GetDescriptionFragments(CardDescriptionContext context) =>
        [new(new LocString("cards", "GUEST_FROM_HERESY_FREE_COST_AFFIX.description"), CardDescriptionFragmentPlacement.AfterBase)];
    public IEnumerable<IHoverTip> GetHoverTips(CardModel card) => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    public bool ShouldGlowGold(CardModel card) => Synaesthesia.IsActive(card);
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        bool active = card == Owner && Synaesthesia.IsActive(card);
        modifiedCost = active ? 0m : originalCost;
        return active;
    }
}
