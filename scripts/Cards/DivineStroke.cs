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
using guestFromHeresy.Projection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class DivineStroke() : TemporaryArtSkill(0, CardRarity.Common)
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
        if (drawAllowed && handCount < 2 && energy > 0)
        {
            await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
            return;
        }
        decimal extraBlock = decimal.Truncate(Hook.ModifyBlock(CombatState!, Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, this, play, out _));
        decimal[] bodyHpLoss = new decimal[2];
        // Compare dealing HP loss (0) with gaining block (1), without changing combat state.
        for (int option = 0; option < 2; option++)
        {
            int projectionHp = ProjectionManager.IsGazed(Owner) ? ProjectionManager.Get(Owner).CurrentHp : 0;
            decimal block = Owner.Creature.Block + (option == 1 ? Math.Max(0m, extraBlock) : 0m);
            foreach (Creature enemy in CombatState!.HittableEnemies)
            {
                if (enemy.Monster == null) continue;
                if (option == 0)
                {
                    decimal loss = Hook.ModifyDamage(Owner.RunState, CombatState, enemy, Owner.Creature, DynamicVars.HpLoss.BaseValue,
                        ValueProp.Unpowered | ValueProp.Unblockable, this, play, ModifyDamageHookType.All, CardPreviewMode.None, out _);
                    if (enemy.CurrentHp <= loss && Hook.ShouldDie(Owner.RunState, CombatState, enemy, out _)) continue;
                }
                foreach (var intent in enemy.Monster.NextMove.Intents.OfType<AttackIntent>())
                {
                    if (intent.DamageCalc == null) continue;
                    decimal damage = Math.Max(0m, decimal.Floor(Hook.ModifyDamage(Owner.RunState, CombatState,
                        Owner.Creature, enemy, intent.DamageCalc(), ValueProp.Move, null, null,
                        ModifyDamageHookType.All, CardPreviewMode.None, out _)));
                    for (int hit = 0; hit < Math.Max(1, intent.Repeats); hit++)
                    {
                        decimal unblocked = Math.Max(0m, damage - block);
                        block = Math.Max(0m, block - damage);
                        if (projectionHp > 0)
                        {
                            // Projection destruction absorbs the breaking hit's overflow.
                            projectionHp -= (int)Math.Min(projectionHp, unblocked);
                        }
                        else bodyHpLoss[option] += unblocked;
                    }
                }
            }
        }
        if (bodyHpLoss[0] > bodyHpLoss[1])
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        else
            await CreatureCmd.Damage(context, CombatState!.HittableEnemies.ToArray(), DynamicVars.HpLoss.BaseValue,
                ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature, this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.HpLoss.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
