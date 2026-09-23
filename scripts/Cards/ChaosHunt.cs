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
public sealed class ChaosHunt() : SynaestheticAttack(3, CardRarity.Uncommon)
{
    public override TargetType TargetType => Synaesthesia.IsActive(this) ? TargetType.AllEnemies : TargetType.AnyEnemy;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GazedPower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var attack = await Attack(DynamicVars.Damage.BaseValue, play, Synaesthesia.IsActive(this)).Execute(context);
        int kills = attack.Results.SelectMany(r => r).Where(r => r.WasTargetKilled).Select(r => r.Receiver).Distinct().Count();
        if (kills > 0) await PowerCmd.Apply<GazedPower>(context, Owner.Creature, kills * 3m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(12m);
}
