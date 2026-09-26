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
public sealed class Tinnitus() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new PowerVar<DisintegrationPower>(2m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>(), HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override bool ShouldGlowGoldInternal => IsMutable && CombatState != null && SensoryCardHistory.PlayedMissileThisTurn(this);
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, 1m, Owner);
        if (!SensoryCardHistory.PlayedMissileThisTurn(this)) return;
        var target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies.ToArray());
        if (target != null) await PowerCmd.Apply<DisintegrationPower>(context, target, DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["DisintegrationPower"].UpgradeValueBy(1m);
}
