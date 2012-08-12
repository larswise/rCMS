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
using Raven.Client.Document;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using System.Linq.Expressions;

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

            ZCMSPage page = _session.Load<ZCMSPage>(pageID);
            
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
        }

        public void CreateCmsPage(ZCMSPage page)
        {

            _session.Store(page, page.PageID.ToString());
            
            _session.SaveChanges();
        }

        public void DeleteCmsPage(ZCMSPage page)
        {
            ZCMSPage pageToDelete = _session.Query<ZCMSPage>().Where(p => p.PageID == page.PageID).FirstOrDefault();
            //ZCMSPage pageToDelete = _session.Load<ZCMSPage>(page.PageID);
            if (pageToDelete != null)
            {
                _session.Delete<ZCMSPage>(pageToDelete);
                _session.SaveChanges();
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

        public List<string> GetAllFileTypes()
        {
            var items = _session.Query<ZCMSFileDocument>().
                Select(x => x.Extension).Distinct();

            return items.ToList();

        }

        public List<ZCMSFileDocument> GetN_MostRecentAttachments(int n)
        {
            return _session.Query<ZCMSFileDocument>().OrderBy(o => o.Created).Take(n).ToList();
        }

        public void StoreAttachment(ZCMSFileDocument fileDocument, MemoryStream dataStream)
        {            
            _session.Store(fileDocument);
            _session.Advanced.GetMetadataFor(fileDocument)["Raven-Cascade-Delete-Documents"] = RavenJArray.FromObject(new[] { fileDocument.FileKey });
            _documentStore.DatabaseCommands.PutAttachment(fileDocument.FileKey, null, dataStream, null);

            _session.SaveChanges(); //base controller does this!
        }

        public List<ZCMSFileDocument> AttachToPage(List<string> attachments, string pageId)
        {
            try
            {
                ZCMSPage cmspage = _session.Load<ZCMSPage>(pageId);

                if (cmspage != null && cmspage.Properties.Where(p => p is ImageListProperty).Any())
                {
                    int count = ((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Count;
                    if (count >= 6)
                        return new List<ZCMSFileDocument>();

                    foreach(string attach in attachments) 
                    {
                        ((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Add(attach);
                    }
                }
                _session.SaveChanges();
                var items = _session.Query<ZCMSFileDocument>().ToList().Where(fd => attachments.Any(a => a == fd.FileKey)).ToList();
                return items;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public string DetachFromPage(string key, string pageId)
        {
            try
            {
                ZCMSPage cmspage = _session.Load<ZCMSPage>(pageId);

                if (cmspage != null && cmspage.Properties.Where(p => p is ImageListProperty).Any())
                {

                    if (((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Contains(key))
                    {
                        ((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Remove(key);
                    }
                    
                }
                _session.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message + " - " + ex.StackTrace);
                return ex.Message;
            }
        }

        public MemoryStream RetrieveAttachment(string key)
        {
            var attachment = _documentStore.DatabaseCommands.GetAttachment(key);
            return (MemoryStream)attachment.Data();
        }

        public List<string> RetrieveMultipleAttachments(List<string> keys)
        {            
            return _session.Query<ZCMSFileDocument>().ToList().Where(z => keys.Any(s => s == z.FileKey)).Select(k => "/Backend/GetCurrentImage?key=" + k.FileKey).ToList();
        }

        public List<ZCMSFileDocument> QueryAttachment(List<string> extensionValue, string filterFreeText)
        {
            if (extensionValue.Count == 1 && extensionValue.First() == "*")
                if (String.IsNullOrEmpty(filterFreeText))
                    return _session.Query<ZCMSFileDocument>().ToList();
                else
                    return _session.Query<ZCMSFileDocument>().ToList().Where(z => z.FileName.Contains(filterFreeText)).ToList();
            else
            {

                Func<ZCMSFileDocument, bool> predicate;
                if (!String.IsNullOrEmpty(filterFreeText))
                    predicate = (x => x.FileName.Contains(filterFreeText));
                else
                    predicate = (x => x.FileName.Length > 0);

                return _session.Query<ZCMSFileDocument>().ToList().
                    Where(z => extensionValue.Any(s => s == z.Extension) && predicate.Invoke(z)).ToList();
            }
        }


        public void RemoveAttachment(string key)
        {
            _documentStore.DatabaseCommands.DeleteAttachment(key, null);
        }
    }
}