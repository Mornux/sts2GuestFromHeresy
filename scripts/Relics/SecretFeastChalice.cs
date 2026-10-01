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
public sealed class SecretFeastChalice : TemporaryIconRelic
{
    private CardModel? _firstCard;
    private CardPlay? _firstPlay;
    private Creature? _target;
    private bool _firstFinished;
    private bool _gazed;
    private bool _spent;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => true;
    public override int DisplayAmount => _spent ? 0 : 1;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>()];

    private void ResetCombatState()
    {
        _firstCard = null;
        _firstPlay = null;
        _target = null;
        _firstFinished = _gazed = _spent = false;
    }
    public override Task BeforeCombatStart()
    {
        ResetCombatState();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    public override Task BeforeCardPlayed(CardPlay play)
    {
        if (play.Card.Owner == Owner && _firstCard == null && !_spent)
        {
            // A detached snapshot survives transformations without adding a card to any pile.
            _firstCard = (CardModel)play.Card.MutableClone();
            _firstPlay = play;
            _target = play.Target;
        }
        return Task.CompletedTask;
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play != _firstPlay) return;
        _firstFinished = true;
        await TryReplay(context);
    }
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not GazedPower || power.Owner != Owner.Creature || amount <= 0 || power.Amount <= 0 || _gazed) return;
        _gazed = true;
        await TryReplay(context);
    }
    private async Task TryReplay(PlayerChoiceContext context)
    {
        if (_spent || !_gazed || !_firstFinished || _firstCard == null || Owner.Creature.IsDead) return;
        _spent = true;
        InvokeDisplayAmountChanged();
        Flash();
        var dupe = _firstCard.CreateDupe(Owner);
        var target = _target is { IsAlive: true } ? _target : dupe.TargetType == TargetType.AnyPlayer ? Owner.Creature : null;
        await CardCmd.AutoPlay(context, dupe, target, skipXCapture: true);
    }
    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        ResetCombatState();
    }
}
