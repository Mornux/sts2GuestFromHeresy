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
public sealed class DimBarrage() : TemporaryArtAttack(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(DimBarrage);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CalculationBaseVar(0m), new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => PileType.Exhaust.GetPile(card.Owner).Cards.Count(c => c is ShadowMissile))];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>(IsUpgraded)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        if (CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead) return;
        var selected = await CardSelectCmd.FromHand(context, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card.IsTransformable, this);
        foreach (var card in selected)
        {
            var missile = CardScope!.CreateCard<ShadowMissile>(Owner);
            if (IsUpgraded) CardCmd.Upgrade(missile, CardPreviewStyle.None);
            await CardCmd.Transform(card, missile, CardPreviewStyle.None);
        }
    }
    protected override void OnUpgrade() { }
}
