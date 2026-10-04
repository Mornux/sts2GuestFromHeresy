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
public sealed class LunarEclipse() : TemporaryArtAttack(1, CardRarity.Common)
{
    protected override string PortraitName => nameof(LunarEclipse);
    protected override HashSet<CardTag> CanonicalTags => [CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip, HoverTipFactory.FromCard<Crescent>(), HoverTipFactory.FromCard<FullMoon>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        if (CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead) return;
        var selected = await CardSelectCmd.FromCombatPile(context, PileType.Draw.GetPile(Owner), Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card is Crescent or FullMoon);
        foreach (var card in selected)
        {
            card.ExhaustOnNextPlay = true;
            await CardCmd.AutoPlay(context, card, null);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
