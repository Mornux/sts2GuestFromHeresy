using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using guestFromHeresy.Mechanics;
using guestFromHeresy.Projection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class DivineStroke() : TemporaryArtSkill(0, CardRarity.Rare)
{
    protected override string PortraitName => nameof(DivineStroke);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move), new CardsVar(2), new HpLossVar(8m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        int energy = Owner.PlayerCombatState!.Energy;
        bool drawAllowed = (PileType.Draw.GetPile(Owner).Cards.Count > 0 || PileType.Discard.GetPile(Owner).Cards.Count > 0)
            && Hook.ShouldDraw(CombatState!, Owner, false, out _);
        if (DivineStrokeForecast.CanKeepDrawing(handCount, energy, drawAllowed))
        {
            await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
            return;
        }
        decimal extraBlock = decimal.Truncate(Hook.ModifyBlock(CombatState!, Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, this, play, out _));
        if (DivineStrokeForecast.Choose(handCount, energy, ExpectedDamage(true, 0), ExpectedDamage(false, extraBlock), drawAllowed) == DivineStrokeOption.Block)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        else
            await CreatureCmd.Damage(context, CombatState!.HittableEnemies.ToArray(), DynamicVars.HpLoss.BaseValue,
                ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature, this);
    }

    private decimal ExpectedDamage(bool damageEnemies, decimal extraBlock)
    {
        int projectionHp = ProjectionManager.IsGazed(Owner) ? ProjectionManager.Get(Owner).CurrentHp : 0;
        var forecast = new DivineStrokeForecast(Owner.Creature.Block + Math.Max(0m, extraBlock), projectionHp);
        foreach (Creature enemy in CombatState!.HittableEnemies)
        {
            if (enemy.Monster == null) continue;
            if (damageEnemies)
            {
                decimal loss = Hook.ModifyDamage(Owner.RunState, CombatState, enemy, Owner.Creature, DynamicVars.HpLoss.BaseValue,
                    ValueProp.Unpowered | ValueProp.Unblockable, this, ModifyDamageHookType.All, CardPreviewMode.None, out _);
                if (enemy.CurrentHp <= loss && Hook.ShouldDie(Owner.RunState, CombatState, enemy, out _)) continue;
            }
            foreach (var intent in enemy.Monster.NextMove.Intents.OfType<AttackIntent>())
            {
                if (intent.DamageCalc == null) continue;
                decimal damage = Math.Max(0m, decimal.Floor(Hook.ModifyDamage(Owner.RunState, CombatState,
                    Owner.Creature, enemy, intent.DamageCalc(), ValueProp.Move, null,
                    ModifyDamageHookType.All, CardPreviewMode.None, out _)));
                for (int hit = 0; hit < Math.Max(1, intent.Repeats); hit++)
                {
                    forecast.ReceiveHit(damage);
                }
            }
        }
        return forecast.BodyHpLoss;
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.HpLoss.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
