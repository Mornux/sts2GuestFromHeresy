using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
[RegisterCharacterStarterCard(typeof(Characters.Character), 1)]
public sealed class InnerFear : ModCardTemplate
{
    public InnerFear() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true) { }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/InnerFear.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/TempImage.png");

    public override TargetType TargetType => Synaesthesia.IsActive(this) ? TargetType.AllEnemies : TargetType.AnyEnemy;
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new PowerVar<VulnerablePower>(1m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool allEnemies = Synaesthesia.IsActive(this);
        var targets = allEnemies ? CombatState!.HittableEnemies.ToArray()
            : new[] { cardPlay.Target ?? throw new InvalidOperationException("Inner Fear requires an enemy target.") };
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay);
        if (allEnemies) attack.TargetingAllOpponents(CombatState!);
        else attack.Targeting(targets[0]);
        await attack.WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, targets.Where(target => target.IsAlive),
            DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}
