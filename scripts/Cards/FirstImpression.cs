using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Keywords;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public sealed class FirstImpression() : SynaestheticAttack(0, CardRarity.Common, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override PileType GetResultPileTypeForCardPlay()
    {
        if (Synaesthesia.IsActive(this)) RemoveKeyword(CardKeyword.Exhaust);
        return base.GetResultPileTypeForCardPlay();
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) => await Attack(DynamicVars.Damage.BaseValue, play, all: true).Execute(context);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
