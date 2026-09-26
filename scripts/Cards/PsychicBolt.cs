using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class PsychicBolt() : TemporaryArtPower(2, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<guestFromHeresy.Powers.GazedPower>(), HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<PsychicBoltPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
