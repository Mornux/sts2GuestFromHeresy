using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class FaithShaking() : TemporaryArtSkill(0, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<guestFromHeresy.Powers.GazedPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("GazedCount", 3m)];
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        int count = Math.Max(0, DynamicVars["GazedCount"].IntValue);
        string prefix = "GUEST_FROM_HERESY_CARD_FAITH_SHAKING.";
        string sequence = count == 0
            ? new LocString("cards", prefix + "noGaze").GetFormattedText()
            : string.Join(new LocString("cards", prefix + "separator").GetFormattedText(),
                Enumerable.Repeat(new LocString("cards", prefix + "oneGaze").GetFormattedText(), count))
                + new LocString("cards", prefix + "sequenceEnd").GetFormattedText();
        description.Add("GazeSequence", sequence);
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int count = DynamicVars["GazedCount"].IntValue;
        DynamicVars["GazedCount"].BaseValue = Math.Max(0, count - 1);
        if (count > 0 && Owner.Creature.IsAlive)
            await PowerCmd.Apply<GazedPower>(context, Owner.Creature, count, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["GazedCount"].UpgradeValueBy(1m);
}
