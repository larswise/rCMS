using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSFileDocument
    {
        private List<ZCMSMetaDataItem> _metadata;
        private string _fileKey;
        private DateTime _created;
        private string _fileName;
        private string _extension;
        private string _description;
        private string _contentType;
        private FileType _fileType;

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
            _contentType = ((NameValueCollection)ConfigurationManager.GetSection("FileContentTypes"))[_extension.ToLower()].ToString();

        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "FileDocumentContentType")]
        public string ContentType
        {
            get
            {
                return !String.IsNullOrEmpty(_contentType) ? _contentType : ((NameValueCollection)ConfigurationManager.GetSection("FileContentTypes"))[_extension.ToLower()].ToString();
            }
            set
            {
                _contentType = value;
            }
        }

        public FileType FileType
        {
            get
            {                
                if (ConfigurationManager.AppSettings["ImageFileFormats"].Split(',').Where(z => z == this._extension.ToLower()).Any())
                    return FileType.ImageFile;
                else if (ConfigurationManager.AppSettings["DocumentFileFormats"].Split(',').Where(z => z == this._extension.ToLower()).Any())
                    return FileType.DocumentFile;
                else if (ConfigurationManager.AppSettings["VideoFileFormats"].Split(',').Where(z => z == this._extension.ToLower()).Any())
                    return FileType.VideoFile;
                else if (ConfigurationManager.AppSettings["AudioFileFormats"].Split(',').Where(z => z == this._extension.ToLower()).Any())
                    return FileType.AudioFile;
                else
                    return FileType.Unknown;                

            }
            set
            {
                _fileType = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "FileDocumentFileName")]
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
                return _extension.ToLower();
            }
            set
            {
                _extension = value.ToLower();
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