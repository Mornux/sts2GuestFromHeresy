using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using guestFromHeresy.Keywords;
using guestFromHeresy.Mechanics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class NightBlindness() : TemporaryArtPower(1, CardRarity.Uncommon)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [new HoverTip(
        new MegaCrit.Sts2.Core.Localization.LocString("card_keywords", "MOON_PHASE.title"),
        new MegaCrit.Sts2.Core.Localization.LocString("card_keywords", "MOON_PHASE.description"))];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<NightBlindnessPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
