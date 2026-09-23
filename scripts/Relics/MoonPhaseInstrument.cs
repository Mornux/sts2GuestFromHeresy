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

namespace guestFromHeresy.Relics;

[RegisterRelic(typeof(RelicPool))]
public sealed class MoonPhaseInstrument : TemporaryIconRelic
{
    private bool _triggered;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromCard<Crescent>(true), HoverTipFactory.FromCard<FullMoon>(true)];
    public override Task BeforeCombatStart() { _triggered = false; return Task.CompletedTask; }
    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext context, Player player)
    {
        if (player != Owner || _triggered) return Task.CompletedTask;
        _triggered = true;
        var cards = PileType.Hand.GetPile(Owner).Cards.Where(c => c is Crescent or FullMoon && c.IsUpgradable).ToArray();
        if (cards.Length > 0) Flash();
        foreach (var card in cards) CardCmd.Upgrade(card, CardPreviewStyle.None);
        return Task.CompletedTask;
    }
}
