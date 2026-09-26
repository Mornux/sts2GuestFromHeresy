using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class WitheringSickness() : TemporaryArtSkill(0, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(), HoverTipFactory.FromCard<MegaCrit.Sts2.Core.Models.Cards.Slimed>(), HoverTipFactory.Static(StaticHoverTip.Transform)];

    private bool _transformPending;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(13m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Target != null) await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(context, play.Target, DynamicVars["DisintegrationPower"].BaseValue, Owner.Creature, this);
        _transformPending = play.PlayIndex == play.PlayCount - 1 && !IsDupe;
    }
    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || !_transformPending || oldPileType != PileType.Play || Pile == null) return;
        _transformPending = false;
        if (IsTransformable && !CombatManager.Instance.IsEnding)
            await CardCmd.Transform(this, CardScope!.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Slimed>(Owner), CardPreviewStyle.None);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["DisintegrationPower"].UpgradeValueBy(3m);
        AddKeyword(CardKeyword.Innate);
    }
}
