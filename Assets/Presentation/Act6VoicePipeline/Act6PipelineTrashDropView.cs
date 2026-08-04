using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6PipelineTrashDropView : MonoBehaviour,
        IDropHandler,
        IPointerClickHandler
    {
        private FinalChapterConversationPresenter presenter;

        public void Configure(FinalChapterConversationPresenter conversationPresenter)
        {
            presenter = conversationPresenter;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var card = eventData.pointerDrag.GetComponentInParent<Act6PipelinePartDragView>();
            if (card == null)
            {
                return;
            }

            Act6PipelinePartDragView.ClearActivePreviews();
            presenter?.RemoveOption(card.ComponentId);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            presenter?.RemoveSelectedOption();
        }
    }
}
