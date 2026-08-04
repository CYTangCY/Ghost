using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act3DialogGraph
{
    public interface IDialogGraphWireInteractionHost
    {
        void BeginWireDrag(Act3DialogGraphOutputPortView outputPort, PointerEventData eventData);

        void UpdateWireDrag(PointerEventData eventData);

        void EndWireDrag(Act3DialogGraphOutputPortView outputPort);

        void CompleteWireDrop(
            Act3DialogGraphOutputPortView outputPort,
            Act3DialogGraphInputPortView inputPort);

        /// <summary>A wire started from the receiving end and dragged back towards a source.</summary>
        void BeginReverseWireDrag(Act3DialogGraphInputPortView inputPort, PointerEventData eventData);

        void EndReverseWireDrag(Act3DialogGraphInputPortView inputPort);
    }
}