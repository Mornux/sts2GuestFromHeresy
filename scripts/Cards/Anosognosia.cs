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
public sealed class Anosognosia() : TemporaryArtSkill(1, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DisintegrationPower>(4m), new CardsVar(1)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override bool ShouldGlowGoldInternal => IsMutable && CombatState != null && CombatState.HittableEnemies.Any(enemy => enemy.GetPowerAmount<DisintegrationPower>() > 0);
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Target is not { } target) return;
        bool hadDisintegration = target.GetPowerAmount<DisintegrationPower>() > 0;
        await PowerCmd.Apply<DisintegrationPower>(context, target, DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
        if (hadDisintegration) await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
    }
    protected override void OnUpgrade() { DynamicVars["DisintegrationPower"].UpgradeValueBy(2m); DynamicVars.Cards.UpgradeValueBy(1); }
}
