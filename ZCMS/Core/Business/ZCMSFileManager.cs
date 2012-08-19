using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Business
{
    public class ZCMSFileManager
    {
        private List<ZCMSFileDocument> _fileDocuments;
        private DateTime _createdGreaterThan;
        private List<string> _allExtensions;
        private List<string> _showExtensions;

        public ZCMSFileManager(List<string> allExt, List<string> showExt, DateTime createdGreaterThan)
        {
            _allExtensions = allExt;
            _showExtensions = showExt;
            _createdGreaterThan = createdGreaterThan;
        }

        public List<ZCMSFileDocument> FileDocuments
        {
            get
            {
                return _fileDocuments;
            }
            set
            {
                _fileDocuments = value;
            }
        }

        public List<string> ShowExtensions
        {
            get
            {
                return _showExtensions;
            }
            set
            {
                _showExtensions = value;
            }
        }

        public List<string> AllExtensions
        {
            get
            {
                return _allExtensions;
            }
            set
            {
                _allExtensions = value;
            }
        }

    }
}