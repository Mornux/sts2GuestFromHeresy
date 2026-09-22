using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.addons.mega_text;

namespace guestFromHeresy.Projection;

public partial class ProjectionHealthBar : Control
{
    private static readonly Color ProjectionGray = new(89f / 255f, 89f / 255f, 89f / 255f);
    private static readonly Color GazedGreen = new(6f / 255f, 123f / 255f, 139f / 255f);
    public NCreature CreatureNode { get; set; } = null!;
    private NHealthBar _bar = null!;
    private NHealthBar _originalBar = null!;
    private NCreatureStateDisplay _display = null!;
    private Creature _displayCreature = null!;
    private MegaLabel _hpLabel = null!;
    private Control _fill = null!;
    private Control _originalFill = null!;
    private CanvasItem? _visuals;
    private Color _originalColor;
    private bool _wasGazed;
    private int _lastHp = -1;
    private int _lastMaxHp = -1;
    private float _lastWidth = -1;

    //Reuse the native health bar scene, textures, font and damage animation.
    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        ProcessPriority = 100;
        _display = CreatureNode.GetNode<NCreatureStateDisplay>("%HealthBar");
        _originalBar = _display.GetNode<NHealthBar>("%HealthBar");
        _bar = GD.Load<PackedScene>("res://scenes/combat/health_bar.tscn").Instantiate<NHealthBar>();
        _bar.Name = "ProjectionNativeHealthBar";
        AddChild(_bar);
        _bar.MouseFilter = MouseFilterEnum.Ignore;
        _bar.Modulate = Colors.White;
        _hpLabel = _bar.GetNode<MegaLabel>("%HpLabel");
        _fill = _bar.GetNode<Control>("%HpForeground");
        _originalFill = _originalBar.GetNode<Control>("%HpForeground");

        var player = CreatureNode.Entity.Player!;
        var state = ProjectionManager.Get(player);
        //This UI-only creature is never added to combat and cannot receive gameplay actions.
        _displayCreature = new Creature(player, state.CurrentHp, state.MaxHp)
        {
            HpDisplay = player.Creature.HpDisplay
        };
        _bar.SetCreature(_displayCreature);
        _bar.GetNode<Control>("%BlockContainer").Hide();
        _bar.HpBarContainer.MouseFilter = MouseFilterEnum.Pass;
        _bar.HpBarContainer.TooltipText = new LocString("powers",
            "GUEST_FROM_HERESY_POWER_GAZED_POWER.projection").GetFormattedText();
        _visuals = CreatureNode.Visuals;
    }

    //Keep both native bars together at the character's feet, above the existing power row.
    public override void _Process(double delta)
    {
        var player = CreatureNode.Entity.Player!;
        _bar.Visible = player.Creature.CombatState != null && _display.IsVisibleInTree();
        if (!_bar.Visible) return;

        var state = ProjectionManager.Get(player);
        bool gazed = ProjectionManager.IsGazed(player);
        if (gazed != _wasGazed) _originalBar.RefreshValues();
        bool changed = state.CurrentHp != _lastHp || state.MaxHp != _lastMaxHp;
        if (changed)
        {
            _displayCreature.SetMaxHpInternal(state.MaxHp);
            _displayCreature.SetCurrentHpInternal(state.CurrentHp);
        }

        float width = _originalBar.HpBarContainer.Size.X;
        if (!Mathf.IsEqualApprox(width, _lastWidth))
        {
            _bar.UpdateWidthRelativeToReferenceValue(state.MaxHp, width);
            _lastWidth = width;
            changed = true;
        }
        var sourceRect = _originalBar.HpBarContainer.GetGlobalRect();
        _bar.HpBarContainer.GlobalPosition = sourceRect.Position - new Vector2(0, sourceRect.Size.Y + 6);
        if (changed)
        {
            _bar.RefreshValues();
            //A broken projection is not a dead player; retain a numeric zero instead of DEAD.
            _hpLabel.SetTextAutoSize($"{state.CurrentHp}/{state.MaxHp}");
            _lastHp = state.CurrentHp;
            _lastMaxHp = state.MaxHp;
        }
        _fill.SelfModulate = gazed ? GazedGreen : ProjectionGray;
        if (gazed) _originalFill.SelfModulate = ProjectionGray;

        if (_visuals != null && gazed != _wasGazed)
        {
            if (gazed)
            {
                _originalColor = _visuals.Modulate;
                _visuals.Modulate = _originalColor * new Color(0.48f, 0.9f, 1f, 0.65f);
            }
            else _visuals.Modulate = _originalColor;
        }
        _wasGazed = gazed;
    }

    //Restore native colors; the child health bar is freed with this controller.
    public override void _ExitTree()
    {
        if (GodotObject.IsInstanceValid(_originalBar)) _originalBar.RefreshValues();
        if (_wasGazed && GodotObject.IsInstanceValid(_visuals)) _visuals!.Modulate = _originalColor;
    }
}
