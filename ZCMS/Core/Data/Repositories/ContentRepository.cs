using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Versioning;
using Raven.Json.Linq;
using ZCMS.Core.Business;

namespace ZCMS.Core.Data.Repositories
{
    public class ContentRepository
    {
        private DocumentStore _documentStore;
        private IDocumentSession _session;

        public ContentRepository(DocumentStore store, IDocumentSession sess)
        {
            _documentStore = store;
            _session = sess;
        }

        public void SaveCmsPage(ZCMSPage page)
        {
            _session.SaveChanges();
        }

        public ZCMSPage GetCmsPage(string pageID)
        {
            Int32 pgid;

            if (!Int32.TryParse(pageID, out pgid))
            {
                return _session.Load<ZCMSPage>(pageID);
            }

            ZCMSPage page = _session.Query<ZCMSPage>().Where(p => p.PageID == Int32.Parse(pageID)).FirstOrDefault();
            _session.SaveChanges();
            return page;
        }

        public List<ZCMSMetaData> GetPastRevisions(string pageID)
        {
            List<ZCMSMetaData> metadatas = new List<ZCMSMetaData>();
            ZCMSPage[] revisions = _session.Advanced.GetRevisionsFor<ZCMSPage>(pageID, 0, 5);

            foreach (var item in revisions)
            {
                ZCMSMetaData mData = new ZCMSMetaData();
                mData.MetaDatas = new List<ZCMSMetaDataItem>();

                RavenJObject obj = _session.Advanced.GetMetadataFor<ZCMSPage>(item);
                mData.MetaDatas.Add(new ZCMSMetaDataItem() { MetaKey = "Status", MetaValue = obj["Raven-Document-Revision-Status"].ToString() });
                mData.MetaDatas.Add(new ZCMSMetaDataItem() { MetaKey = "Last-Modified", MetaValue = obj["Last-Modified"].ToString() });
                mData.MetaDatas.Add(new ZCMSMetaDataItem() { MetaKey = "Id", MetaValue = obj["@id"].ToString().Replace("/", ".") });

                metadatas.Add(mData);
            }
            _session.SaveChanges();
            return metadatas.ToList();
        }



        public void RegisterPageType(IZCMSPageType pageTypeInstance)
        {
            IZCMSPageType pt = _session.Query<IZCMSPageType>().Where(p => p.PageTypeName == pageTypeInstance.PageTypeName).FirstOrDefault();
            if (pt == null)
            {
                _session.Store(pageTypeInstance);
                _session.SaveChanges();
            }
            _session.SaveChanges();
        }

        public void CreateCmsPage(ZCMSPage page)
        {

            _session.Store(page, page.PageID.ToString());
            
            _session.SaveChanges();
        }

        public List<IZCMSPageType> GetPageTypes()
        {
            List<IZCMSPageType> pageTypes = _session.Query<IZCMSPageType>().Where(pt => pt.PageTypeName != String.Empty).ToList();
            _session.SaveChanges();
            return pageTypes;
        }

        public List<ZCMSMenuItem> GetMenu(string id)
        {
            var item = _session.Load<ZCMSMenu>("Menu/" + id).MenuItems;
            return item;
        }

        public void StoreAttachment(string pageID, string fileName, Stream dataStream)
        {
            string key;
            try
            {
                string[] filenamearr = fileName.Split('.');
                key = Guid.NewGuid().ToString() + "." + filenamearr[filenamearr.Length - 1];
            }
            catch
            {
                key = Guid.NewGuid().ToString();
            }

            _documentStore.DatabaseCommands.PutAttachment(key, null, dataStream, new RavenJObject { { "PageId", pageID } });
            ZCMSPage cmspage = GetCmsPage(pageID);
            var property = cmspage.Properties.Where(p => p is ImageListProperty).FirstOrDefault();
            if (property != null)
            {
                try
                {
                    ((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Add(key);
                }
                catch
                {
                }
                _session.SaveChanges();
            }
        }

        public MemoryStream TransStorageBt(string key)
        {
            var attachment = _documentStore.DatabaseCommands.GetAttachment(key);
            return (MemoryStream)attachment.Data();
        }

        public void RemoveAttachment(string key)
        {
            _documentStore.DatabaseCommands.DeleteAttachment(key, null);
        }
    }
}