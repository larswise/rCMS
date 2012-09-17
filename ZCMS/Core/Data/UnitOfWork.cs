using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Raven.Bundles.Authentication;
using Raven.Bundles.Versioning;
using Raven.Client.Versioning;
using Raven.Client.Authorization;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using Microsoft.CSharp.RuntimeBinder;
using Raven.Bundles.Authorization.Model;
using ZCMS.Core.Data.Repositories;
using Raven.Client.Indexes;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Data
{
    public class UnitOfWork : IDisposable
    {
        private DocumentStore _documentStore;
        private IDocumentSession _session;
        private ConfigurationRepository _configRepository;
        private AuthRepository _authRepository;
        private ContentRepository _contentRepository;
        private FileRepository _fileRepository;

        public UnitOfWork(DocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public ConfigurationRepository ConfigRepository
        {
            get
            {
                if (_configRepository != null)
                    return _configRepository;
                else
                    return _configRepository = new ConfigurationRepository(_documentStore, _session);
            }
        }

        public AuthRepository AuthenticationRepository
        {
            get
            {
                if (_authRepository != null)
                    return _authRepository;
                else
                    return _authRepository = new AuthRepository(_documentStore, _session);
            }
        }

        public ContentRepository CmsContentRepository
        {
            get
            {
                if (_contentRepository != null)
                    return _contentRepository;
                else 
                    return _contentRepository = new ContentRepository(_documentStore, _session);
            }
        }

        public FileRepository FileRepository
        {
            get
            {
                if (_fileRepository != null)
                    return _fileRepository;
                else
                    return _fileRepository = new FileRepository(_documentStore, _session);
            }
        }

        public void OpenSession()
        {
            _session = _documentStore.OpenSession();
        }

        public void SaveAllChanges()
        {
            try
            {
                if (_session != null)
                    _session.SaveChanges();
            }
            catch (Raven.Database.Exceptions.OperationVetoedException ove)
            {
                throw ove;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CloseSession()
        {
            _session.Dispose();
            _authRepository = null;
            _configRepository = null;
            _contentRepository = null;
        }

        public void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof(PageIndexer).Assembly, _documentStore);
        }

        public void Dispose()
        {
            _session.Dispose();
            _documentStore.Dispose();
        }

        #region Getdata methods

        public List<MemoryStream> GetAttachments(ZCMSPage page)
        {
            List<MemoryStream> mses = new List<MemoryStream>();
            if (page.Properties.Where(p => p is ImageListProperty).Any())
            {
                var prop = page.Properties.Where(p => p is ImageListProperty).FirstOrDefault();
                List<string> images = (List<string>)prop.PropertyValue;
                foreach (var item in images)
                {
                    mses.Add(FileRepository.RetrieveAttachment(item));
                }
            }
            return mses;
        }

        #endregion
    }
}