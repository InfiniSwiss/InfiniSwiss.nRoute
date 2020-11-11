using Microsoft.Win32;
using nRoute.Components.Composition;
using nRoute.ViewServices.Contracts;
using System.IO;
using System.Linq;

namespace nRoute.ViewServices.Services
{
    [MapViewService(typeof(IOpenFileViewService), OpenFileViewService.SERVICE_NAME,
         InitializationMode = InitializationMode.OnDemand, Lifetime = ViewServiceLifetime.PerInstance)]
    public class OpenFileViewService
         : IOpenFileViewService
    {
        private const string SERVICE_NAME = "nRouteOpenFileViewService";
        private const string DEFAULT_FILES_FILTER = "All Files (*.*)|*.*";
        private const int DEFAULT_FILTER_INDEX = 1;

        #region IOpenFileViewService Members

        public OpenFileResponse OpenFile(bool multiselect)
        {
            return OpenFile(DEFAULT_FILES_FILTER, DEFAULT_FILTER_INDEX, multiselect);
        }

        public OpenFileResponse OpenFile(string filter, int filterIndex, bool multiselect)
        {
            var _openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = filterIndex,
                Multiselect = multiselect
            };

            // note this is blocking
            var _result = _openFileDialog.ShowDialog();

            return !_result.GetValueOrDefault(false) ? new OpenFileResponse(true) :
                new OpenFileResponse(
                    new FileInfo(_openFileDialog.FileName),
                    from f in _openFileDialog.FileNames
                    select new FileInfo(f));

        }

        #endregion

    }
}