using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
namespace guestFromHeresy.Cards;
[RegisterCard(typeof(CardPool))]
public sealed class MoralDebt() : TemporaryArtPower(1, CardRarity.Rare)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3), new CardsVar(3)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [EnergyHoverTip, HoverTipFactory.FromPower<GazedPower>(), HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<MoralDebtPower>(context, Owner.Creature, 5m, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
