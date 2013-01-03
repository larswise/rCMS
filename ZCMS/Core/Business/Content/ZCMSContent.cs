using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSContent<T>
    {
        private T _instance;
        private List<ZCMSMetaDataItem> _metaData;
        private ContentViewStatus _contentViewStatus;

        public ZCMSContent(T instance)
        {
            _instance = instance;
            _metaData = new List<ZCMSMetaDataItem>();
            _contentViewStatus = ContentViewStatus.Authorized;
        }

        public ZCMSContent(bool auth)
        {
            _metaData = new List<ZCMSMetaDataItem>();
            if (!auth)
                _contentViewStatus = ContentViewStatus.NotAuthenticated;
            else
                _contentViewStatus = ContentViewStatus.NotAuthorized;
        }

        public ZCMSContent()
        {
        }        

        public T Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public ContentViewStatus ViewStatus
        {
            get
            {
                return _contentViewStatus;
            }
        }

        public string FacebookApiKey { get; set; }
        public string TwitterConsumerKey { get; set; }

        public ZCMSSocial SocialServices { get; set; }
        public ZCMSSiteDescription SiteDescription { get; set; }

        public void PushMetadata(string key, string value)
        {
            ZCMSMetaDataItem meta = new ZCMSMetaDataItem() { MetaKey = key, MetaValue = value };
            _metaData.Add(meta);                
        }

        public string GetMetadataValue(string key)
        {
            if (_metaData.Any(m => m.MetaKey == key))
                return _metaData.FirstOrDefault(m => m.MetaKey == key).MetaValue;
            else
                return string.Empty;
        }
    }
}