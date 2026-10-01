using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class ImpostorPower : TemporaryIconPower
{
    private NCreatureVisuals? _disguise;
    private Node2D? _originalBody;
    private CharacterModel? _previous;

    public override Task AfterApplied(Creature? applier, CardModel? source)
    {
        ChangeAppearance();
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (player == Owner.Player && Owner.IsAlive) ChangeAppearance();
        return Task.CompletedTask;
    }

    private void RestoreAppearance()
    {
        if (GodotObject.IsInstanceValid(_disguise))
        {
            _disguise!.Hide();
            _disguise.QueueFree();
        }
        _disguise = null;
        if (GodotObject.IsInstanceValid(_originalBody)) _originalBody!.Show();
        _originalBody = null;
    }

    private void ChangeAppearance()
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (node == null) return;
        // Cosmetic RNG must not consume the player's gameplay RNG stream.
        var candidates = ModelDb.AllCharacters
            .Where(m => m.GetType().Assembly == typeof(CharacterModel).Assembly
                && m != Owner.Player?.Character && m != _previous).ToArray();
        if (candidates.Length == 0) return;
        var model = candidates[System.Random.Shared.Next(candidates.Length)];
        NCreatureVisuals? next = null;
        try
        {
            next = model.CreateVisuals();
            // Keep native visuals/animation references intact for health bars, targeting and mods.
            node.Visuals.AddChild(next);
            if (next.SpineBody != null) model.GenerateAnimator(next.SpineBody, Owner);
            RestoreAppearance();
            _originalBody = node.Visuals.GetCurrentBody();
            _originalBody.Hide();
            _disguise = next;
            _previous = model;
        }
        catch (Exception error)
        {
            if (GodotObject.IsInstanceValid(next)) next!.QueueFree();
            GD.PushWarning($"Impostor appearance unavailable: {model.Id}: {error.Message}");
        }
    }

    public override Task AfterRemoved(Creature owner)
    {
        RestoreAppearance();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        RestoreAppearance();
        return Task.CompletedTask;
    }
}
