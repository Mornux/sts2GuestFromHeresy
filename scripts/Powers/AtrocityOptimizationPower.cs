using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class AtrocityOptimizationPower : ModPowerTemplate
{
    private CardPlay? _boostedPlay;
    private int _consumedAmount;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/AtrocityOptimization_temp.png";
    public override string CustomBigIconPath => CustomIconPath;

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _boostedPlay = null;
        _consumedAmount = 0;
    }

    public override Task BeforeCardPlayed(CardPlay play)
    {
        if (_boostedPlay == null && play.Card.Owner.Creature == Owner && play.Card.Type == CardType.Attack)
        {
            _boostedPlay = play;
            _consumedAmount = Amount;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource?.Type != CardType.Attack || !props.IsPoweredAttack()) return 1m;
        if (_boostedPlay != null) return cardSource == _boostedPlay.Card ? 1m + _consumedAmount / 100m : 1m;
        return cardSource.Pile?.Type == PileType.Play ? 1m : 1m + Amount / 100m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play != _boostedPlay) return;
        int consumed = _consumedAmount;
        _boostedPlay = null;
        _consumedAmount = 0;
        await PowerCmd.ModifyAmount(context, this, -consumed, null, null);
    }
}
