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
public sealed class Surrender() : TemporaryArtSkill(0, CardRarity.Common)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GazedPower>(2m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>(), HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GazedPower>(context, Owner.Creature, DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<DisintegrationPower>(context, Owner.Creature, 2m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["GazedPower"].UpgradeValueBy(1m);
}
