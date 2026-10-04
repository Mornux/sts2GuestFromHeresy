using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class DelusionMadeReal() : TemporaryArtPower(1, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(), HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockAmount", 2m)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<DelusionMadeRealPower>(context, Owner.Creature, DynamicVars["BlockAmount"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["BlockAmount"].UpgradeValueBy(1m);
}
