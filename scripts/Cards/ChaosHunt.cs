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
public sealed class ChaosHunt() : SynaestheticAttack(3, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private HashSet<CardPlay> _repeatPlays = [];
    protected override string PortraitName => nameof(ChaosHunt);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(24m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        bool active = Synaesthesia.IsActive(this);
        var attack = await Attack(DynamicVars.Damage.BaseValue, play, all: true).Execute(context);
        if (active && Owner.Creature.IsAlive && attack.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
        {
            await PowerCmd.Apply<GazedPower>(context, Owner.Creature, 3m, Owner.Creature, this);
            _repeatPlays.Add(play);
        }
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card == this && _repeatPlays.Remove(play) && Owner.Creature.IsAlive
            && CombatState!.HittableEnemies.Any() && !CombatManager.Instance.IsOverOrEnding)
            await CardCmd.AutoPlay(context, this, null);
    }
    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _repeatPlays = [];
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
