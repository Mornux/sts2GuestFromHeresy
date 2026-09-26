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
public sealed class PainMemory() : TemporaryArtSkill(0, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Triggers", 1m), new CardsVar(1)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Target is not { } target) return;
        int lost = 0;
        for (int i = 0; i < DynamicVars["Triggers"].IntValue && target.IsAlive; i++)
            if (target.GetPower<DisintegrationPower>() is { } power) lost += await DisintegrationTriggers.Trigger(context, power);
        if (lost > 0) await CardPileCmd.Draw(context, 1m, Owner);
    }
    protected override void OnUpgrade() => DynamicVars["Triggers"].UpgradeValueBy(1m);
}
