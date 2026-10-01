using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class VoidLampOil() : TemporaryArtAttack(1, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => Attack(DynamicVars.Damage.BaseValue, play, true).Execute(context);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
    public override async Task AfterCardExhausted(PlayerChoiceContext context, CardModel card, bool causedByEthereal)
    {
        if (card != this || Owner.Creature.IsDead) return;
        for (int i = 0; i < 2; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CreateClone(), PileType.Draw, Owner, CardPilePosition.Top);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, null)
            .TargetingAllOpponents(CombatState!).Execute(context);
    }
}
