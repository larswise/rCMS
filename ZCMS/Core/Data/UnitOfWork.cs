using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Raven.Bundles.Authentication;
using Raven.Bundles.Versioning;
using Raven.Client.Versioning;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using ZCMS.Core.Business;
using Microsoft.CSharp.RuntimeBinder;
using Raven.Bundles.Authorization.Model;
using ZCMS.Core.Data.Repositories;

namespace ZCMS.Core.Data
{
    public class UnitOfWork : IDisposable
    {
        private DocumentStore _documentStore;
        private IDocumentSession _session;
        private ConfigurationRepository _configRepository;
        private AuthRepository _authRepository;
        private ContentRepository _contentRepository;

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

        public void OpenSession()
        {
            _session = _documentStore.OpenSession();
        }

        public void SaveAllChanges()
        {
            if (_session != null)
                _session.SaveChanges();
        }

        public void CloseSession()
        {
            _session.Dispose();
            _authRepository = null;
            _configRepository = null;
            _contentRepository = null;
        }

        public void Dispose()
        {
            _session.Dispose();
            _documentStore.Dispose();
        }
    }
}