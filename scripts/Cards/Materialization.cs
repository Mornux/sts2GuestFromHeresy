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
public sealed class Materialization() : SynaestheticAttack(2, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CalculationBaseVar(10m), new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => ProjectionManager.Get(card.Owner).CurrentHp)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id), HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool active = Synaesthesia.IsActive(this);
        var attack = await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, play).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        if (active && play.Target!.IsAlive)
            await PowerCmd.Apply<DisintegrationPower>(context, play.Target,
                attack.Results.SelectMany(r => r).Where(r => r.Receiver == play.Target).Sum(r => r.TotalDamage), Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}
