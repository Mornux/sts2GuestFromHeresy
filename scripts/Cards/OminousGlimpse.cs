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

// Removed from the character card pool in v0.4; keep the implementation.
public sealed class OminousGlimpse() : TemporaryArtAttack(1, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move), new CalculationBaseVar(1m), new CalculationExtraVar(1m),
        new CalculatedVar("Hits").WithMultiplier((_, target) => DebuffKinds(target))];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) => await Attack(DynamicVars.Damage.BaseValue, play, hits: 1 + DebuffKinds(play.Target)).Execute(context);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
