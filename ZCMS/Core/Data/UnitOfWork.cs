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

        public UnitOfWork(DocumentStore documentStore, IDocumentSession session)
        {
            _documentStore = documentStore;
            _session = session;

            _configRepository = new ConfigurationRepository(_documentStore, _session);
            _authRepository = new AuthRepository(_documentStore, _session);
            _contentRepository = new ContentRepository(_documentStore, _session);
        }

        public ConfigurationRepository ConfigRepository
        {
            get
            {
                return _configRepository;
            }
        }

        public AuthRepository AuthenticationRepository
        {
            get
            {
                return _authRepository;
            }
        }

        public ContentRepository CmsContentRepository
        {
            get
            {
                return _contentRepository;
            }
        }

        public void Dispose()
        {
            _session.Dispose();
            _documentStore.Dispose();
        }
    }
}