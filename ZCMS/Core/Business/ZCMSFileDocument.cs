using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public class ZCMSFileDocument
    {
        private List<ZCMSMetaDataItem> _metadata;
        private string _fileKey;
        private DateTime _created;
        private string _fileName;
        private string _extension;
        private string _description;

        public ZCMSFileDocument(string fileName, string description)
        {
            _fileName = fileName;
            _created = DateTime.Now;
            string key;
            string ext = string.Empty;
            try
            {
                string[] filenamearr = fileName.Split('.');
                ext = filenamearr[filenamearr.Length - 1];

                key = Guid.NewGuid().ToString() + "." + ext;
            }
            catch
            {
                key = Guid.NewGuid().ToString();
            }
            _fileKey = key;
            _description = description;
            _extension = ext;

        }

        public string FileName 
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }


        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public List<ZCMSMetaDataItem> MetaData
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        public string FileKey
        {
            get
            {
                return _fileKey;
            }
            set
            {
                _fileKey = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }
    }
}