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
public sealed class AuditoryHallucination() : TemporaryArtSkill(1, CardRarity.Common)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Missiles", 1m), new CardsVar(1)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<ShadowMissile>()];
    protected override bool ShouldGlowGoldInternal => IsMutable && CombatState != null && SensoryCardHistory.PlayedMissileThisTurn(this);
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        for (int i = 0; i < DynamicVars["Missiles"].IntValue; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<ShadowMissile>(Owner), PileType.Hand, Owner);
        if (SensoryCardHistory.PlayedMissileThisTurn(this)) await CardPileCmd.Draw(context, 1m, Owner);
    }
    protected override void OnUpgrade() => DynamicVars["Missiles"].UpgradeValueBy(1m);
}
