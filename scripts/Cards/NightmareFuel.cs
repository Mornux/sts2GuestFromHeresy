using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class NightmareFuel() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(NightmareFuel);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [EnergyHoverTip, ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int repeats = Synaesthesia.IsActive(this) ? 2 : 1;
        for (int i = 0; i < repeats && !Owner.Creature.IsDead; i++)
        {
            var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
            foreach (var card in selected)
            {
                await CardCmd.Exhaust(choiceContext, card);
                await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
                await PowerCmd.Apply<NightmareFuelPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade() { DynamicVars.Energy.UpgradeValueBy(1m); RemoveKeyword(CardKeyword.Exhaust); }
}

[RegisterPower]
public sealed class NightmareFuelPower : guestFromHeresy.Powers.TemporaryIconPower
{
    public override string CustomIconPath => "res://guestFromHeresy/images/projection/Gazed_temp.png";
    public override async Task BeforeHandDraw(MegaCrit.Sts2.Core.Entities.Players.Player player, PlayerChoiceContext context, MegaCrit.Sts2.Core.Combat.ICombatState combat)
    {
        if (player != Owner.Player) return;
        int stacks = Amount;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<guestFromHeresy.Powers.GazedPower>(context, Owner, stacks, Owner, null);
    }
}
