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
public sealed class SilentFollowing() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GazedPower>(1m), new CardsVar(2)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>(), HoverTipFactory.FromCard<Assimilation>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GazedPower>(context, Owner.Creature, DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        for (int i = 0; i < 2; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<Assimilation>(Owner), PileType.Draw, Owner, CardPilePosition.Random);
    }
    protected override void OnUpgrade() => DynamicVars["GazedPower"].UpgradeValueBy(1m);
}
