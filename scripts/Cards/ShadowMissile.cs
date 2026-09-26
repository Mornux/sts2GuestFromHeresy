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
public sealed class ShadowMissile() : SynaestheticAttack(0, CardRarity.Token)
{
    protected override string PortraitName => nameof(ShadowMissile);
    internal bool RuneTargetsAttacker { get; set; }
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override TargetType TargetType => !RuneTargetsAttacker && Synaesthesia.IsActive(this) ? TargetType.AllEnemies : TargetType.AnyEnemy;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2m, ValueProp.Move), new PowerVar<DisintegrationPower>(2m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool active = !RuneTargetsAttacker && Synaesthesia.IsActive(this);
        var targets = active ? CombatState!.HittableEnemies.ToArray() : new[] { play.Target! };
        await Attack(DynamicVars.Damage.BaseValue, play, active).Execute(context);
        await PowerCmd.Apply<DisintegrationPower>(context, targets.Where(t => t.IsAlive), DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
        int vulnerable = Owner.Creature.GetPowerAmount<TransdimensionalWoundPower>();
        if (vulnerable > 0)
            await PowerCmd.Apply<VulnerablePower>(context, targets.Where(t => t.IsAlive), vulnerable, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars["DisintegrationPower"].UpgradeValueBy(1m); }
}
