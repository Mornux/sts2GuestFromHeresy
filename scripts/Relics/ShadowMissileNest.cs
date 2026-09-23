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
public sealed class ShadowMissileNest : TemporaryIconRelic
{
    private bool _triggered;
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>()];
    public override Task BeforeCombatStart() { _triggered = false; return Task.CompletedTask; }
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext context, ICombatState combat)
    {
        if (player != Owner || _triggered) return;
        _triggered = true;
        Flash();
        for (int i = 0; i < 3; i++)
            await CardPileCmd.AddGeneratedCardToCombat(combat.CreateCard<ShadowMissile>(Owner), PileType.Hand, Owner);
    }
}
