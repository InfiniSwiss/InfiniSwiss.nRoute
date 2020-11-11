using nRoute.Components.Composition;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface IOpenFileViewService
    {
        OpenFileResponse OpenFile(bool multiselect);

        OpenFileResponse OpenFile(string filter, int filterIndex, bool multiselect);
    }
}
