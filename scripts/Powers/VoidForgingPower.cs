using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class VoidForgingPower : TemporaryIconPower
{
    private List<ArtifactPartCard> _pendingParts = [];
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int PendingRecipe { get; set; }
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int PendingCount { get; set; }
    public override LocString Description
    {
        get
        {
            var description = base.Description;
            description.Add("Count", PendingCount);
            description.Add("Parts", string.Join("、", Enumerable.Range(0, PendingCount).Select(i =>
            {
                int code = (PendingRecipe >> ((PendingCount - i - 1) * 4)) & 15;
                return new LocString("cards", "GUEST_FROM_HERESY_CARD_" + ArtifactPartCard.PartEntries[code] + ".title").GetFormattedText();
            })));
            return description;
        }
    }
    public async Task Record(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext context, ArtifactPartCard part)
    {
        _pendingParts.Add(part);
        PendingRecipe = (PendingRecipe << 4) | part.PartCode;
        PendingCount++;
        SetAmount(PendingCount);
        if (PendingCount < 3) return;
        var artifact = part.CardScope!.CreateCard<SealedArtifact>(Owner.Player!);
        artifact.SetBlueprint(PendingRecipe);
        var components = _pendingParts.Distinct().ToArray();
        // Reset before adding the card: generated-card hooks may play more components.
        PendingRecipe = 0;
        PendingCount = 0;
        _pendingParts.Clear();
        SetAmount(0);
        foreach (var component in components)
            if (component.Pile != null && component.Pile.Type != PileType.Exhaust)
                await CardCmd.Exhaust(context, component);
        await CardPileCmd.AddGeneratedCardToCombat(artifact, PileType.Hand, Owner.Player!);
    }
    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _pendingParts = [];
    }
}
