using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class BigChomp() : TemporaryArtPower(2, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(BigChomp);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var skills = PileType.Hand.GetPile(Owner).Cards.Where(c => c.Type == CardType.Skill).ToArray();
        if (skills.Length == 0) return;
        var chosen = IsUpgraded
            ? (await CardSelectCmd.FromHand(context, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), c => c.Type == CardType.Skill, this)).FirstOrDefault()
            : Owner.RunState.Rng.CombatCardSelection.NextItem(skills);
        if (chosen == null) return;
        await CardCmd.Exhaust(context, chosen);
        var power = await PowerCmd.Apply<BigChompPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        power?.SetMeal(chosen);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Retain);
    }
}
