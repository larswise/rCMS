﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Bundles.Authorization.Model;
using Raven.Bundles.Authentication;
using Raven.Bundles.Versioning;
using Raven.Client.Authorization;
using Raven.Client.Versioning;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System.Configuration;


namespace ZCMS.Core.Data.Repositories
{
    public class AuthRepository
    {

        private DocumentStore _documentStore;
        private IDocumentSession _session;

        public AuthRepository(DocumentStore store, IDocumentSession sess)
        {
            _documentStore = store;
            _session = sess;
        }

        public string GetCurrentUserName()
        {            
            var user = _session.Query<AuthorizationUser>().Where(a => a.Id == String.Format("Authorization/Users/{0}", HttpContext.Current.User.Identity.Name)).FirstOrDefault();
            if (user != null)
                return !String.IsNullOrEmpty(user.Name) ? user.Name : user.Id;
            else
                return CMS_i18n.BackendResources.UnknownPublisher;
        }
        
        public void SetPermissions(string user)
        {
            var perms = new DocumentAuthorization()
            {
                Permissions = new List<DocumentPermission>() 
                {
                    new DocumentPermission() 
                    { 
                        Allow = true,
                        Operation = "*",
                        Role = "Authorization/Roles/Administrators",
                        User = user,
                        Priority = 1                        
                    }
                }
            };
            //_session.SetAuthorizationFor(page, 
        }

        public void CreateDefaultUserAndRoles()
        {
            string s = "start";
            try
            {

                _session.Store(new AuthorizationRole
                {
                    Id = String.Format("Authorization/Roles/{0}", "Administrators"),
                    Permissions = {
                                new OperationPermission
                                {
                                    Allow = true,
                                    Operation = "*",
                                    Tags = { "Elevated" }
                                },
                                new OperationPermission
                                {
                                    Allow = true,
                                    Operation = "SaveAPage",
                                    Tags = { "Elevated" }
                                },
                                new OperationPermission
                                {
                                    Allow = true,
                                    Operation = "RetrieveAPage",
                                    Tags = { "Elevated" }
                                }
                            }
                });
                
                _session.Store(new AuthorizationRole
                {
                    Id = String.Format("Authorization/Roles/{0}", "Users"),
                    Permissions = {
                                new OperationPermission
                                {
                                    Allow = true,
                                    Operation = "RetrieveAPage",
                                    Tags = { "Elevated" }
                                }
                            }
                });

                _session.Store(new AuthorizationUser
                {
                    Id = String.Format("Authorization/Users/{0}", ConfigurationManager.AppSettings["RavenDBAccountUser"].ToString()),
                    Name = "larswise",
                    Roles = new List<string>() { String.Format("Authorization/Roles/{0}", "Administrators") },
                    Permissions = 
                        {
                            new OperationPermission
                            {
                                Allow = true,
                                Operation = "*"
                            }
                        }
                });

                

                _session.Store(new AuthenticationUser
                {
                    Name = "Chief Editor",
                    Id = String.Format("Raven/Users/{0}", ConfigurationManager.AppSettings["RavenDBAccountUser"].ToString()),
                    AllowedDatabases = new[] { "*" }
                }.SetPassword(ConfigurationManager.AppSettings["RavenDBAccountPassword"].ToString()));

                _session.Store(new AuthorizationUser
                {
                    Id = String.Format("Authorization/Users/testuser"),
                    Name = "Testulf Svulst",
                    Roles = new List<string>() { String.Format("Authorization/Roles/Users") },

                });

                _session.Store(new AuthenticationUser
                {
                    Name = "Cheff",
                    Id = String.Format("Raven/Users/testuser"),
                    AllowedDatabases = new[] { "*" }
                }.SetPassword("zlol"));
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(s + "  --- " + e.Message + e.StackTrace);
            }
            finally
            {
                _session.SaveChanges();
            }
        }

        
        public bool AuthorizeCurrentUser()
        {
            try
            {
                var user = _session.Load<dynamic>(String.Format("Raven/Users/{0}", HttpContext.Current.User.Identity.Name));

                return _session.Load<AuthorizationUser>(user.Id.Replace("Raven", "Authorization")).Roles.Contains("Authorization/Roles/Administrators");

            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                var user = _session.Load<AuthenticationUser>(String.Format("Raven/Users/{0}", username));
                if(user!=null)
                {
                    return user.ValidatePassword(password);
                }
                else 
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message + "                ---                  " + e.Message + "                                       ---                         " + e.StackTrace);
                return false;
            }
        }
    }

           

 
}