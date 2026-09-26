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
public sealed class SelfAlienation() : SynaestheticAttack(1, CardRarity.Common)
{
    private bool _playing;
    private bool _doubleDamage;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? source) =>
        source == this && props.IsPoweredAttack() && (_playing ? _doubleDamage : Synaesthesia.IsActive(this)) ? 2m : 1m;
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        _doubleDamage = Synaesthesia.IsActive(this);
        _playing = true;
        try
        {
            if (_doubleDamage) await ProjectionManager.LoseHp(context, Owner, 3m);
            await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        }
        finally { _playing = false; _doubleDamage = false; }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
