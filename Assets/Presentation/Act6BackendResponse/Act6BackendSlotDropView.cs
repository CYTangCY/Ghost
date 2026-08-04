using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6BackendResponse
{
    public sealed class Act6BackendSlotDropView : MonoBehaviour,
        IDropHandler,
        IPointerClickHandler
    {
        private IAct6BackendInteractionHost host;
        private string roleId;

        public void Configure(
            string targetRoleId,
            IAct6BackendInteractionHost interactionHost)
        {
            roleId = targetRoleId ?? string.Empty;
            host = interactionHost;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (host == null || eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var card = eventData.pointerDrag.GetComponent<Act6BackendCardDragView>();
            if (card != null)
            {
                host.DropCardOnRole(card.CardId, roleId);
                eventData.Use();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null || eventData.used)
            {
                return;
            }

            host?.HandleRoleSocketClick(roleId);
            eventData.Use();
        }
    }
}
