using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Versioning;
using Raven.Client.Extensions;
using Raven.Client.Linq;
using Raven.Client.Util;
using Raven.Client.Document.SessionOperations;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using System.Linq.Expressions;
using ZCMS.Core.Business.Content;

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

        public ZCMSPage GetCmsPage(string pageID)
        {
            return _session.Load<ZCMSPage>(pageID);
        }

        public List<ZCMSPage> SearchPages(string query, PageStatus status)
        {
            if (status==PageStatus.Published||status==PageStatus.Draft)
                return _session.Query<ZCMSPage>().Where(p => p.Status==status).Take(25).ToList();
            if(status==PageStatus.Any && String.IsNullOrEmpty(query))
                return _session.Query<ZCMSPage>().Take(25).ToList();
            return _session.Advanced.LuceneQuery<ZCMSPage, PageIndexer>().Search("Body", query).ToList();

        }

        public List<ZCMSPage> GetRecentPages(DateTime ?fromDate, int ?numItems)
        {             
            if (fromDate.HasValue && numItems.HasValue)
                return _session.Query<ZCMSPage>().Where(p => p.Created >= fromDate.Value).Take(numItems.Value).ToList();
            else if(fromDate.HasValue)
            {                
                return _session.Query<ZCMSPage>().Where(p => p.Created >= fromDate.Value).ToList();
            }
            else if (numItems.HasValue)
            {
                return _session.Query<ZCMSPage>().Take(numItems.Value).ToList();
            }
            else
            {
                throw new Exception("Either fromDate or numItems must have a value!");
            }
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
            return metadatas.ToList();
        }

        public void RegisterPageType(IZCMSPageType pageTypeInstance)
        {
            IZCMSPageType pt = _session.Load<IZCMSPageType>(pageTypeInstance.FriendlyPageTypeName);//.Where(p => p.PageTypeName == pageTypeInstance.PageTypeName).FirstOrDefault();
            if (pt == null)
            {
                _session.Store(pageTypeInstance, pageTypeInstance.FriendlyPageTypeName);
                _session.SaveChanges();
            }
        }

        public void CreateCmsPage(ZCMSPage page)
        {
            _session.Store(page, page.PageID.ToString());
        }

        public void DeleteCmsPage(ZCMSPage page)
        {
            ZCMSPage pageToDelete = _session.Load<ZCMSPage>(page.PageID);
            if (pageToDelete != null)
            {
                _session.Delete<ZCMSPage>(pageToDelete);
            }
        }



        public List<IZCMSPageType> GetPageTypes()
        {
            List<IZCMSPageType> pageTypes = _session.Query<IZCMSPageType>().Where(pt => pt.PageTypeName != String.Empty).ToList();
            return pageTypes;
        }

        public List<ZCMSMenuItem> GetMenu(string id)
        {
            var item = _session.Load<ZCMSMenu>("Menu/" + id).MenuItems;
            return item;
        }

        public List<ZCMSMenu> GetMainMenus()
        {
            return _session.Query<ZCMSMenu>().ToList();
        }
    }
}