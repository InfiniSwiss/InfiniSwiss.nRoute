using System.Collections.Generic;
using System.IO;

namespace nRoute.ViewServices.Contracts
{
    public class OpenFileResponse
    {
        private readonly bool _cancelled;
        private readonly FileInfo _file;
        private readonly IEnumerable<FileInfo> _files;

        public OpenFileResponse(bool cancelled)
        {
            _cancelled = cancelled;
        }

        public OpenFileResponse(FileInfo file, IEnumerable<FileInfo> files)
        {
            _file = file;
            _files = files;
        }

        #region Properties

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public FileInfo File
        {
            get { return _file; }
        }

        public IEnumerable<FileInfo> Files
        {
            get { return _files; }
        }

        #endregion

    }
}
