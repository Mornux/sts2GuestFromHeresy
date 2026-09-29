using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Localization;
using guestFromHeresy.Capabilities;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class BigChompPower : TurnStartPower
{
    private CardModel? _meal;
    private CardModel? _mealTemplate;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override LocString Description
    {
        get
        {
            var description = base.Description;
            description.Add("Meal", _mealTemplate?.Title ?? _meal?.Title ?? "");
            description.Add("HasMeal", _mealTemplate != null || _meal != null);
            return description;
        }
    }
    public void SetMeal(CardModel card)
    {
        _meal = card;
        _mealTemplate = (CardModel)card.MutableClone();
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (player != Owner.Player || !GazedAtTurnStart || _meal == null || Owner.IsDead) return;
        // Keep the eaten card exhausted after each replay, even for skills without Exhaust.
        if (_meal.Pile?.Type != PileType.Exhaust)
        {
            // A transforming skill can replace itself after play; preserve what was eaten.
            _meal = _mealTemplate!.CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(_meal, PileType.Exhaust, player);
        }
        _meal.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(context, _meal, null);
    }
}
