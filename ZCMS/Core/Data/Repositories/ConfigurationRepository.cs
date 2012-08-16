using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using Raven.Client.Extensions;
using ZCMS.Core.Business;

namespace ZCMS.Core.Data.Repositories
{
    public class ConfigurationRepository
    {

        private DocumentStore _documentStore;
        private IDocumentSession _session;

        public ConfigurationRepository(DocumentStore store, IDocumentSession sess)
        {
            _documentStore = store;
            _session = sess;
        }

        public bool InitialSetup()
        {
            try
            {
                var item = _session.Load<dynamic>("Raven/Versioning/DefaultConfiguration");
                if (item == null)
                {
                    return true;
                }
                return false;
            }
            catch { return true;  }
        }

        public void EnsureDbExists(bool ignorefail)
        {
            _documentStore.DatabaseCommands.EnsureDatabaseExists(ConfigurationManager.AppSettings["RavenDBDefaultDb"].ToString(),ignorefail);
        }



        public void WireUpVersioning()
        {
            _session.Store(new
            {
                Exclude = false,
                Id = "Raven/Versioning/DefaultConfiguration",
                MaxRevisions = 5
            });            

            _session.Store(new
            {
                Exclude = true,
                Id = "Raven/Versioning/IZCMSPageType",
                MaxRevisions = 1
            });

            _session.Store(new
            {
                Exclude = true,
                Id = "Raven/Versioning/ZCMSMenus",
                MaxRevisions = 1
            });

            _session.Store(new
            {
                Exclude = true,
                Id = "Raven/Versioning/ZCMSFileDocument",
                MaxRevisions = 1
            });
            _session.SaveChanges();
        }

        public void SetUpMenus()
        {
            try
            {

                System.Threading.Thread.Sleep(10000);
                List<IZCMSPageType> pageTypes = _session.Query<IZCMSPageType>().Where(pt => pt.PageTypeName != String.Empty).ToList();

                
                if (pageTypes == null || pageTypes.Count == 0)
                    throw new Exception("No pagetypes!");
                _session.Store(new ZCMSMenu()
                {
                    MenuItems = new List<ZCMSMenuItem>()
                    {
                        new ZCMSMenuItem() { ItemName = CMS_i18n.BackendResources.MenuMyFiles, ItemAction = "FileManager" }, 
                        new ZCMSMenuItem() { ItemName = CMS_i18n.BackendResources.MenuUploadFiles, ItemAction = "FileManager" }
                    }
                }, "Menu/FilesMenu");
               
                _session.Store(new ZCMSMenu()
                {
                    MenuItems = pageTypes.Select(p => new ZCMSMenuItem()
                        {
                            ItemName = p.FriendlyPageTypeName,
                            ItemAction = "PageEditor"
                        }).ToList()
                    
                }, "Menu/PublishMenu");

                _session.Store(new ZCMSMenu()
                {
                    MenuItems = new List<ZCMSMenuItem>()
                {
                    new ZCMSMenuItem() { ItemName = CMS_i18n.BackendResources.MenuConfigPageTypes, ItemAction = "ConfigurePageTypes" }
                }
                }, "Menu/ConfigureMenu");

                _session.Store(new ZCMSMenu()
                {
                    MenuItems = new List<ZCMSMenuItem>()
                {
                    new ZCMSMenuItem() { ItemName = CMS_i18n.BackendResources.MenuSecurityUsers, ItemAction = "ManageSecurity" },
                    new ZCMSMenuItem() { ItemName = CMS_i18n.BackendResources.MenuSecurityRoles, ItemAction = "ManageSecurity" }
                }
                }, "Menu/SecurityMenu");
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "    -    " + e.StackTrace);
            }
        }
    }
}