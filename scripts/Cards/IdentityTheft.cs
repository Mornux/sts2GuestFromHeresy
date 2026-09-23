using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class IdentityTheft() : TemporaryArtPower(1, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Target?.Player is not { } player) return;
        var power = await PowerCmd.Apply<IdentityTheftPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        power?.SetCharacter(player.Character);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
