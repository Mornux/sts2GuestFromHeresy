using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class MassEnergyConversion() : TemporaryArtPower(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(MassEnergyConversion);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GazedPower>(2m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [EnergyHoverTip, HoverTipFactory.FromPower<GazedPower>()];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<MassEnergyConversionPower>(context, Owner.Creature, DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["GazedPower"].UpgradeValueBy(1m);
}
