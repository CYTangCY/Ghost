namespace Ghost.Presentation.Act6BackendResponse
{
    public interface IAct6BackendInteractionHost
    {
        void SelectCard(string cardId);

        void DropCardOnRole(string cardId, string roleId);

        void HandleRoleSocketClick(string roleId);
    }
}
