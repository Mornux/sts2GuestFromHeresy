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
public sealed class BloodMoon() : TemporaryArtAttack(1, CardRarity.Common)
{
    protected override string PortraitName => nameof(BloodMoon);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip];
    protected override HashSet<CardTag> CanonicalTags => [CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new CalculationBaseVar(0m), new CalculationExtraVar(1m),
        new CalculatedVar("Hits").WithMultiplier((card, _) => CombatManager.Instance.History.CardPlaysFinished.Count(e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card is Crescent or FullMoon))];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int hits = DynamicVars.CalculationBase.IntValue + CombatManager.Instance.History.CardPlaysFinished.Count(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card is Crescent or FullMoon);
        if (hits > 0) await Attack(DynamicVars.Damage.BaseValue, play, hits: hits).Execute(context);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars.CalculationBase.UpgradeValueBy(1m); }
}
