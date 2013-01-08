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
using Raven.Client.Authorization;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using System.Linq.Expressions;
using ZCMS.Core.Business.Content;
using Raven.Bundles.Authorization.Model;


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

        #region Public CMS Methods
        public ZCMSContent<ZCMSPage> GetCmsPage(string pageID)
        {
            var content = new ZCMSContent<ZCMSPage>(HttpContext.Current.User.Identity.IsAuthenticated) { Instance = _session.Load<ZCMSPage>(pageID), };
            if(content.Instance!=null)
                PopulatePageMetadata(ref content);
            return content;
        }

        public ZCMSContent<ZCMSPage> GetCmsPageBySlug(string slug)
        {
            ZCMSContent<ZCMSPage> p;
            var page = _session.Advanced.LuceneQuery<ZCMSPage, PageIndexer>().Search("Slug", slug).FirstOrDefault();

            if (((page.StartPublish > DateTime.Now) || (page.EndPublish!=DateTime.MinValue && page.EndPublish < DateTime.Now)) || page.Status == PageStatus.Draft)
            {
                p = new ZCMSContent<ZCMSPage>(HttpContext.Current.User.Identity.IsAuthenticated);
                p.PushMetadata("Reasons", CMS_i18n.BackendResources.PageNotPublished);
                return p;
            }

            var ops = _session.Advanced.IsOperationAllowedOnDocument
                ("Authorization/Users/" + (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous"),
                "RetrievePage",
                page.PageID.ToString());

            if (!ops.IsAllowed)
            {
                p = new ZCMSContent<ZCMSPage>(HttpContext.Current.User.Identity.IsAuthenticated);
                p.PushMetadata("Reasons", String.Join(",", ops.Reasons.ToArray()));
                p.PushMetadata("RedirectUrl", slug);                
            }
            else
            {
                p = new ZCMSContent<ZCMSPage>(page);

                var items = _session.Load<SocialService>("Facebook", "Twitter");
                if (items.Where(i => i.ServiceName == "Facebook").Any())
                    p.FacebookApiKey = items.Where(i => i.Activated && i.ServiceName == "Facebook").First().Key;

                if (items.Where(i => i.ServiceName == "Twitter").Any())
                    p.TwitterConsumerKey = items.Where(i => i.Activated && i.ServiceName == "Twitter").First().Key;

                PopulatePageMetadata(ref p);
            }
            
            return p;
        }

        public List<ZCMSPage> SearchPages(string query, PageStatus status)
        {
            if (status == PageStatus.Published || status == PageStatus.Draft)
                return _session.Query<ZCMSPage>().Where(p => p.Status == status).Take(25).ToList();
            if (status == PageStatus.Any && String.IsNullOrEmpty(query))
                return _session.Query<ZCMSPage>().Take(25).ToList();
            return _session.Advanced.LuceneQuery<ZCMSPage, PageIndexer>().Search("Body", query).ToList();

        }

        public List<ZCMSPage> GetRecentPages(DateTime? fromDate, int? numItems)
        {
            if (fromDate.HasValue && numItems.HasValue)
                return _session.Query<ZCMSPage>().Where(p => p.Created >= fromDate.Value).Take(numItems.Value).ToList();
            else if (fromDate.HasValue)
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

        public void CreateCmsPage(ZCMSPage page, string permission)
        {
            _session.SecureFor("Authorization/Users/" + HttpContext.Current.User.Identity.Name, "SaveAPage");
            _session.Store(page, page.PageID.ToString());
            SetPermissionsForPage(page, permission);
            _session.SaveChanges();
        }

        public void SetPermissionsForPage(ZCMSPage page, string permission)
        {
            if (permission.Equals("Elevated"))
            {                
                _session.SetAuthorizationFor(page, new Raven.Bundles.Authorization.Model.DocumentAuthorization()
                {
                    Tags = { permission },
                    Permissions =
                    {
                        new DocumentPermission() { Allow = true, Operation = "RetrievePage", Role = "Authorization/Roles/Users" },
                        new DocumentPermission() { Allow = true, Operation = "RetrievePage", Role = "Authorization/Roles/Administrators" },
                        new DocumentPermission() { Allow = true, Operation = "EditPage", Role = "Authorization/Roles/Administrators" }
                    }
                });
            }
            else if (permission.Equals("None"))
            {
                _session.Advanced.GetMetadataFor<ZCMSPage>(page).Remove("Raven-Document-Authorization");               
            }
            else if (permission.Equals("Admin"))
            {
                _session.SetAuthorizationFor(page, new Raven.Bundles.Authorization.Model.DocumentAuthorization()
                {
                    Tags = { permission },
                    Permissions = 
                    {
                        new DocumentPermission() { Allow = true, Operation = "RetrievePage", Role = "Authorization/Roles/Administrators" },
                        new DocumentPermission() { Allow = true, Operation = "EditPage", Role = "Authorization/Roles/Administrators" }
                    }
                });
            }
        }

        public void DeleteCmsPage(ZCMSPage page)
        {
            //ZCMSPage pageToDelete = _session.Load<ZCMSPage>(page.PageID);
            var pageToDelete = _session.Query<ZCMSPage>().Where(p => p.PageID == page.PageID).FirstOrDefault();
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

        #endregion

        #region Public methods settings

        public List<ZCMSMenuItem> GetMenu(string id)
        {
            var item = _session.Load<ZCMSMenu>("Menu/" + id).MenuItems;
            return item;
        }

        public List<ZCMSMenu> GetMainMenus()
        {
            return _session.Query<ZCMSMenu>().ToList();
        }

        public List<SocialService> GetSocialServiceConfigs()
        {
            var items = _session.Query<SocialService>().ToList();
            return items;
        }

        public int GetCount<T>(Func<T, bool> func)
        {
            var stats = new RavenQueryStatistics();
            var results = _session.Query<T>().Statistics(out stats).ToArray();
            var inner = results.Where(p => func.Invoke(p)).ToArray();
            
            return inner.Count();
        }

        public void SaveSocialConfigs(ZCMSSocial social)
        {
            SocialService sc;
            if ((sc = _session.Load<SocialService>("Facebook")) != null)
            {
                sc.Key = social.Facebook.Key;
                sc.Secret = social.Facebook.Secret;
                sc.Activated = social.Facebook.Activated;
            }
            else
            {
                _session.Store(social.Facebook, social.Facebook.ServiceName);
            }

            if ((sc = _session.Load<SocialService>("Twitter")) != null)
            {
                sc.Key = social.Twitter.Key;
                sc.Secret = social.Twitter.Secret;
                sc.PrivateToken = social.Twitter.PrivateToken;
                sc.Activated = social.Twitter.Activated;
            }
            else
            {
                _session.Store(social.Twitter, social.Twitter.ServiceName);
            }

            if ((sc = _session.Load<SocialService>("Disqus")) != null)
            {
                sc.JsIntegration = social.Disqus.JsIntegration;
                sc.Activated = social.Disqus.Activated;
            }
            else
            {
                _session.Store(social.Disqus, social.Disqus.ServiceName);
            }

        }

        public ZCMSTopics GetTopics()
        {
            var topics = _session.Query<ZCMSTopics>().FirstOrDefault();
            if (topics == null)
            {
                _session.Store(new ZCMSTopics());
                _session.SaveChanges();
                topics = _session.Query<ZCMSTopics>().FirstOrDefault();
            }

            topics.Topics.ForEach(a => a.TotalCount = GetCount<ZCMSPage>(p => p.TopicId.HasValue && p.TopicId.Value == a.TopicId.Value));
            return topics;
        }

        public void SaveTopics(ZCMSTopics topics)
        {
            ZCMSTopics savedtopics;
            if ((savedtopics = _session.Query<ZCMSTopics>().FirstOrDefault()) != null)
            {
                savedtopics = topics;
            }
            else
            {
                _session.Store(topics);
            }

        }

        public ZCMSSiteDescription GetSiteDescription()
        {
            return _session.Query<ZCMSSiteDescription>().SingleOrDefault();
        }

        public void SaveSiteDescription(ZCMSSiteDescription description)
        {
            ZCMSSiteDescription desc;
            if ((desc = _session.Query<ZCMSSiteDescription>().SingleOrDefault()) != null)
            {
                desc.SiteDescription = description.SiteDescription;
                desc.SiteLogo = description.SiteLogo;
                desc.SiteName = description.SiteName;
            }
            else
            {
                _session.Store(description);
            }
        }

        #endregion

        #region Private Methods

        private void PopulatePageMetadata(ref ZCMSContent<ZCMSPage> content)
        {
            foreach (var item in _session.Advanced.GetMetadataFor<ZCMSPage>(content.Instance))
            {
                if (item.Key == "Raven-Document-Authorization")
                {
                    try
                    {
                        RavenJValue value = (RavenJValue)item.Value.Values().Values().First();
                        content.PushMetadata(item.Key, value.ToString());
                    }
                    catch(Exception e) { System.Diagnostics.Debug.Write(e.Message); }

                }
                else
                    content.PushMetadata(item.Key.ToString(), item.Value.ToString());
            }
        }

        #endregion
    }
}