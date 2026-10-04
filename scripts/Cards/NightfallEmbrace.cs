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
public sealed class NightfallEmbrace() : SynaestheticAttack(1, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool active = Synaesthesia.IsActive(this);
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        if (!active || CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead) return;
        for (int i = 0; i < (IsUpgraded ? 2 : 1); i++)
        {
            var missile = CardScope!.CreateCard<ShadowMissile>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(missile, PileType.Hand, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
