using System;
using System.IO;

namespace nRoute.ViewServices.Contracts
{
    public class SaveFileResponse
    {
        private readonly bool _cancelled;
        private readonly string _fileName;
        private readonly Func<Stream> _fileStream;

        public SaveFileResponse(bool cancelled)
        {
            _cancelled = cancelled;
        }

        public SaveFileResponse(string fileName, Func<Stream> fileStream)
        {
            _fileName = fileName;
            _fileStream = fileStream;
        }

        #region Properties

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public Func<Stream> StreamOpener
        {
            get { return _fileStream; }
        }

        #endregion

    }
}
