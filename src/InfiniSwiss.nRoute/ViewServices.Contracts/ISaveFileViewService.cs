using nRoute.Components.Composition;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface ISaveFileViewService
    {
        SaveFileResponse SaveFile(string defaultExt);

        SaveFileResponse SaveFile(string filter, int filterIndex, string defaultExt);
    }
}