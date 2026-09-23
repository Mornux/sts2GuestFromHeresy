using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Keywords;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public sealed class PsychicTurbulence() : TemporaryArtAttack(1, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move), new DynamicVar("AffixDamage", 7m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await Attack(DynamicVars.Damage.BaseValue, play).Execute(context);
        if (CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead) return;
        var selected = await CardSelectCmd.FromHand(context, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        foreach (var card in selected)
            STS2RitsuLib.Models.Capabilities.ModelCapabilityExtensions.GetOrCreateCapability<guestFromHeresy.Capabilities.SynaestheticDamage>(card).AddDamage(DynamicVars["AffixDamage"].BaseValue);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["AffixDamage"].UpgradeValueBy(2m); }
}
