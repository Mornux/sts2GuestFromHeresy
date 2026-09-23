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
public sealed class FormlessDivinity() : TemporaryArtPower(3, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [STS2RitsuLib.Keywords.ModKeywordRegistry.CreateHoverTip(guestFromHeresy.Keywords.Synaesthesia.Id)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var power = await PowerCmd.Apply<FormlessDivinityPower>(context, Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
        if (power != null) await power.Choose(context);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
