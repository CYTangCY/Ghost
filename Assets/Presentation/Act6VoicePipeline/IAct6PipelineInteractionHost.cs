namespace Ghost.Presentation.Act6VoicePipeline
{
    public interface IAct6PipelineInteractionHost
    {
        void SelectComponent(string componentId);

        void DropComponentOnMainSlot(string componentId, int slotIndex);

        void DropComponentOnBackendSlot(string componentId);

        void PlaceSelectedOnMainSlot(int slotIndex);

        void PlaceSelectedOnBackendSlot();
    }
}
