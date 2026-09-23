using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using HexPower = guestFromHeresy.Powers.HexPower;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Hex() : TemporaryArtSkill(1, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override string PortraitName => nameof(Hex);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DisintegrationPower>(10m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<DisintegrationPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { } target) return;
        await PowerCmd.Apply<HexPower>(choiceContext, target, DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["DisintegrationPower"].UpgradeValueBy(3m);
}
