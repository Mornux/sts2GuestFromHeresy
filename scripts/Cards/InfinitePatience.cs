using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class InfinitePatience() : TemporaryArtSkill(0, CardRarity.Common)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CalculationBaseVar(0m), new CalculationExtraVar(1m),
         new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) =>
             card.IsMutable && card.CombatState != null ? TemporaryArtAttack.DebuffStacks(card.Owner.Creature) : 0)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(null), ValueProp.Move, play);
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
