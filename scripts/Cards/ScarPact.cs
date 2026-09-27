using guestFromHeresy.Pools;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ScarPact() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("ProjectionHpCost", 3m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<ShadowMissile>(Owner), PileType.Hand, Owner);
        int cost = DynamicVars["ProjectionHpCost"].IntValue;
        if (ProjectionManager.Get(Owner).CurrentHp < cost) return;
        int paid = await ProjectionManager.LoseHp(context, Owner, cost);
        if (paid == cost)
            await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<ShadowMissile>(Owner), PileType.Hand, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["ProjectionHpCost"].UpgradeValueBy(-1m);
}
