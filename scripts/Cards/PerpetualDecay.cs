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
public sealed class PerpetualDecay() : TemporaryArtPower(1, CardRarity.Uncommon)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(), HoverTipFactory.FromPower<GazedPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Threshold", 20m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var power = await PowerCmd.Apply<PerpetualDecayPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        power?.SetThreshold(DynamicVars["Threshold"].IntValue);
    }
    protected override void OnUpgrade() => DynamicVars["Threshold"].UpgradeValueBy(-5);
}
