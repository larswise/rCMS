using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Data.Repositories
{
    public class FileRepository
    {
        private DocumentStore _documentStore;
        private IDocumentSession _session;

        public FileRepository(DocumentStore store, IDocumentSession sess)
        {
            _documentStore = store;
            _session = sess;
        }

        public void DeleteAttachments(List<string> keys)
        {
            var docs = _session.Load<ZCMSFileDocument>(keys.ToArray());
            foreach (ZCMSFileDocument doc in docs)
            {
                _session.Delete<ZCMSFileDocument>(doc);
            }
            _session.SaveChanges();
        }

        public List<string> GetAllFileTypes()
        {
            return _session.Query<ZCMSFileDocument>().
                Select(x => x.Extension).Distinct().ToList();

        }

        public List<ZCMSFileDocument> GetN_MostRecentAttachments(int n)
        {
            return _session.Query<ZCMSFileDocument>().OrderBy(o => o.Created).Take(n).ToList();
        }

        public ZCMSFileDocument Get(string key)
        {
            return _session.Load<ZCMSFileDocument>(key);
        }

        public void StoreAttachment(ZCMSFileDocument fileDocument, MemoryStream dataStream)
        {
            _session.Store(fileDocument, fileDocument.FileKey);
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

                    foreach (string attach in attachments)
                    {
                        ((List<string>)cmspage.Properties.Where(p => p is ImageListProperty).First().PropertyValue).Add(attach);
                    }
                }
                _session.SaveChanges();
                var items = _session.Load<ZCMSFileDocument>(attachments.ToArray());
                return items.ToList();
            }
            catch (Exception ex)
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

        public Raven.Abstractions.Data.Attachment RetrieveAttachmentItem(string key)
        {
            return _documentStore.DatabaseCommands.GetAttachment(key);
        }

        public void UpdateAttachment(string key, Stream stream)
        {
            _documentStore.DatabaseCommands.PutAttachment(key, null, stream, null);            
        }

        public List<string> RetrieveMultipleAttachments(List<string> keys)
        {
            return _session.Load<ZCMSFileDocument>(keys.ToArray()).Where(i => i != null).Select(k => "/File/GetCurrentImage?key=" + k.FileKey).ToList();
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
                    Where(z => extensionValue.Any(s => s == z.Extension.ToLower()) && predicate.Invoke(z)).ToList();
            }
        }


        public void RemoveAttachment(string key)
        {
            _documentStore.DatabaseCommands.DeleteAttachment(key, null);
        }

    }
}