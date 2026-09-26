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
public sealed class TidalForm() : TemporaryArtPower(3, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [new HoverTip(new MegaCrit.Sts2.Core.Localization.LocString("card_keywords", "MOON_PHASE.title"), new MegaCrit.Sts2.Core.Localization.LocString("card_keywords", "MOON_PHASE.description")), HoverTipFactory.Static(StaticHoverTip.ReplayStatic)];

    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<TidalFormPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
