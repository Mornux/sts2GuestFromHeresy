using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Models.Capabilities;

namespace guestFromHeresy.Capabilities;

[RegisterModelCapability]
public sealed class SynaestheticDamage : ModelCapability<CardModel>, IModelCapabilityHookListener,
    ICardDescriptionContributor, ICardHoverTipContributor, ICardGlowContributor
{
    private HashSet<CardPlay> _activePlays = [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("TurbulenceDamage", 0m)];
    public void AddDamage(decimal damage) => Modify(_ => DynamicVars["TurbulenceDamage"].BaseValue += damage);
    public IEnumerable<CardDescriptionFragment> GetDescriptionFragments(CardDescriptionContext context) =>
        [new(new LocString("cards", "GUEST_FROM_HERESY_TURBULENCE_AFFIX.description"), CardDescriptionFragmentPlacement.AfterBase)];
    public IEnumerable<IHoverTip> GetHoverTips(CardModel card) => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    public bool ShouldGlowGold(CardModel card) => Synaesthesia.IsActive(card);

    public override Task BeforeCardPlayed(CardPlay play)
    {
        if (play.Card == Owner && Synaesthesia.IsActive(Owner)) _activePlays.Add(play);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card != Owner || !_activePlays.Remove(play) || Owner.Owner.Creature.IsDead) return;
        var combat = Owner.Owner.Creature.CombatState;
        if (combat == null || combat.HittableEnemies.Count == 0) return;
        await DamageCmd.Attack(DynamicVars["TurbulenceDamage"].BaseValue).FromCard(Owner)
            .TargetingRandomOpponents(combat).WithHitFx("vfx/vfx_attack_slash").Execute(context);
    }

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _activePlays = [];
    }
}
