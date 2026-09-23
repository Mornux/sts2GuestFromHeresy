using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class TragicColors() : TemporaryArtSkill(2, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override string PortraitName => nameof(TragicColors);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 5m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<DoomPower>(),
         HoverTipFactory.FromPower<DisintegrationPower>(), HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { } target) return;
        decimal amount = DynamicVars["Stacks"].BaseValue;
        await PowerCmd.Apply<PoisonPower>(choiceContext, target, amount, Owner.Creature, this);
        await PowerCmd.Apply<DoomPower>(choiceContext, target, amount, Owner.Creature, this);
        await PowerCmd.Apply<DisintegrationPower>(choiceContext, target, amount, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, target, amount, Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, target, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(2m);
}
