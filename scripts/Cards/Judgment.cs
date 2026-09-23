using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Judgment() : TemporaryArtSkill(1, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override string PortraitName => nameof(Judgment);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        EnergyCost.AddThisCombat(1);
        if (cardPlay.Target is not { } target) return;
        if (target.GetPower<JudgmentPower>() is { } judgment) await judgment.Advance();
        else await PowerCmd.Apply<JudgmentPower>(choiceContext, target, 13m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
