using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class NightRaid() : TemporaryArtAttack(1, CardRarity.Common)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<guestFromHeresy.Powers.GazedPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move), new PowerVar<GazedPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        await PowerCmd.Apply<GazedPower>(context, Owner.Creature, DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["GazedPower"].UpgradeValueBy(1m); }
}
