using guestFromHeresy.Cards;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace guestFromHeresy.Relics;

[RegisterRelic(typeof(RelicPool))]
public sealed class MoonDustNodule : TemporaryIconRelic
{
    private int _count;
    public override RelicRarity Rarity => RelicRarity.Common;
    public override bool ShowCounter => true;
    public override int DisplayAmount => _count;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<DexterityPower>()];
    [SavedProperty]
    public int CardsPlayed
    {
        get => _count;
        set
        {
            AssertMutable();
            _count = Math.Clamp(value, 0, 2);
            InvokeDisplayAmountChanged();
        }
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card.Owner != Owner || !play.Card.Tags.Contains(CardTags.MoonPhase)) return;
        bool trigger = CardsPlayed == 2;
        CardsPlayed = trigger ? 0 : CardsPlayed + 1;
        if (!trigger || Owner.Creature.IsDead) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(context, Owner.Creature, 1m, Owner.Creature, null);
        await PowerCmd.Apply<DexterityPower>(context, Owner.Creature, 1m, Owner.Creature, null);
    }
}
