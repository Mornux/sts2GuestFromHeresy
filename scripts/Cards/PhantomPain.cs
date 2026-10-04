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
public sealed class PhantomPain() : TemporaryArtAttack(1, CardRarity.Common)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        Attack(DynamicVars.Damage.BaseValue, play, hits: 1 + DisintegrationTriggers.TotalTimesTriggered(CombatState)).Execute(context);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
