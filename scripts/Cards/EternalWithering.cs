using MegaCrit.Sts2.Core.HoverTips;
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
[RegisterDustyTomeCard(typeof(Characters.Character))]
public sealed class EternalWithering() : TemporaryArtPower(2, CardRarity.Ancient)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>()];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 3m)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<EternalWitheringPower>(context, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(1);
}
