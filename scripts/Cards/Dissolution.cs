using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Dissolution() : TemporaryArtAttack(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(Dissolution);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips
    {
        get
        {
            var slime = (MegaCrit.Sts2.Core.Models.CardModel)MegaCrit.Sts2.Core.Models.ModelDb
                .Card<MegaCrit.Sts2.Core.Models.Cards.Slimed>().MutableClone();
            slime.EnergyCost.SetThisCombat(0);
            return [HoverTipFactory.FromCard(slime), HoverTipFactory.Static(StaticHoverTip.Transform)];
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        if (MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsOverOrEnding) return;
        var selected = await CardSelectCmd.FromHand(context, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        foreach (var card in selected)
        {
            var slime = CardScope!.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Slimed>(Owner);
            slime.EnergyCost.SetThisCombat(0);
            await CardCmd.Transform(card, slime, CardPreviewStyle.None);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
