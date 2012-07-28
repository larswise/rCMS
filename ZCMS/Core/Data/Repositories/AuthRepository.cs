using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Bundles.Authorization.Model;
using Raven.Bundles.Authentication;
using Raven.Bundles.Versioning;
using Raven.Client.Versioning;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;
using Microsoft.CSharp.RuntimeBinder;


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
        
        // not finished!
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
                        User = "larswise",
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
                                    Operation = "*"
                                }
                            }
                });
                s += "1";
                _session.SaveChanges();
                s += "a";
                _session.Store(new AuthorizationRole
                {
                    Id = String.Format("Authorization/Roles/{0}", "Users"),
                    Permissions = {
                                new OperationPermission
                                {
                                    Allow = true,
                                    Operation = "*"
                                }
                            }
                }); s += "2";
                _session.SaveChanges();
                s += "3";
                _session.Store(new AuthorizationUser
                {
                    Id = String.Format("Authorization/Users/{0}", "larswise"),
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
                _session.SaveChanges();

                _session.Store(new AuthenticationUser
                {
                    Name = "lars@sunsteam.com",
                    Id = String.Format("Raven/Users/larswise"),
                    AllowedDatabases = new[] { "*" }
                }.SetPassword("sunwolf43"));

                _session.SaveChanges();

                s += "4";
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(s + "  --- " + e.Message + e.StackTrace);
            }
        }

        
        public bool AuthorizeCurrentUser()
        {
            try
            {
                var user = _session.Load<dynamic>(String.Format("Raven/Users/{0}", HttpContext.Current.User.Identity.Name));
                return _session.Load<dynamic>(user.Id.Replace("Raven", "Authorization"))
                    .Roles.Contains("Authorization/Roles/Administrators");
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                AuthenticationUser user = _session.Load<AuthenticationUser>(String.Format("Raven/Users/{0}", username));
                if(user!=null)
                {
                    return user.ValidatePassword(password);
                }
                else 
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }

           

 
}