using Microsoft.Win32;
using nRoute.Components.Composition;
using nRoute.ViewServices.Contracts;

namespace nRoute.ViewServices.Services
{
    [MapViewService(typeof(ISaveFileViewService), SaveFileViewService.SERVICE_NAME,
         InitializationMode = InitializationMode.OnDemand, Lifetime = ViewServiceLifetime.PerInstance)]
    public class SaveFileViewService
         : ISaveFileViewService
    {
        private const string SERVICE_NAME = "nRouteSaveFileViewService";
        private const string DEFAULT_FILES_FILTER = "All Files (*.*)|*.*";
        private const int DEFAULT_FILTER_INDEX = 1;

        #region ISaveFileViewService Members

        public SaveFileResponse SaveFile(string defaultExt)
        {
            return SaveFile(DEFAULT_FILES_FILTER, DEFAULT_FILTER_INDEX, defaultExt);
        }

        public SaveFileResponse SaveFile(string filter, int filterIndex, string defaultExt)
        {
            var _saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                FilterIndex = filterIndex,
                DefaultExt = defaultExt
            };

            // note the show dialog call is blocking
            var _result = _saveFileDialog.ShowDialog();
            return !_result.GetValueOrDefault(false) ?
                new SaveFileResponse(true) :
                new SaveFileResponse(_saveFileDialog.SafeFileName, _saveFileDialog.OpenFile);
        }

        #endregion

    }
}