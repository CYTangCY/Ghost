using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    /// <summary>
    /// What the Final Chapter's route board needs from the presenter: cards dragged in from the
    /// palette, cards moved around or thrown away once they are on the board, and wires drawn between
    /// their dots.
    ///
    /// Kept separate from <see cref="IAct6PipelineInteractionHost"/> because dropping a chip into a
    /// named slot and laying out a graph are different gestures, and one interface covering both would
    /// have to lie about half its methods.
    /// </summary>
    public interface IFinalChapterWireHost
    {
        // ---------------------------------------------------------------- cards

        /// <summary>A palette card was released; place it if the pointer is over the board.</summary>
        void TryPlaceStepAtPointer(string stepId, PointerEventData eventData);

        /// <summary>A card on the board is being dragged. Follows the pointer without a re-render.</summary>
        void MoveStepToPointer(string stepId, RectTransform card, PointerEventData eventData);

        /// <summary>The drag ended. Over the bin, the card comes off the board.</summary>
        void CompleteStepDrag(string stepId, RectTransform card, PointerEventData eventData);

        // ---------------------------------------------------------------- wires

        void BeginWireDrag(FinalChapterRoutePortView port, PointerEventData eventData);

        void UpdateWireDrag(PointerEventData eventData);

        void EndWireDrag();

        /// <summary>Always called source-first, whichever end the player started dragging from.</summary>
        void CompleteWireDrop(FinalChapterRoutePortView outPort, FinalChapterRoutePortView inPort);

        /// <summary>Clicking a port that already has a wire takes it back down.</summary>
        void ClearWireFrom(FinalChapterRoutePortView outPort);
    }
}
