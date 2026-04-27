using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using Diva2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Diva2.Services.Managers.Users
{
    public class User8Service : IUser8Service
    {


        private readonly IRepository<RoleRule8> repRoleRule;
        private readonly IRepository<Rule8> repRule;
        private readonly IRepository<User8> repUsers;
        private readonly IRepository<UserRoles8> repUserRole;
        private readonly IRepository<User8Group> repUserGroup;
        private readonly IRepository<User8GroupUser> repUserGroupUser;

        protected static CacheHelper cache;
        public User8Service(ApplicationDbContext dbContext, IMemoryCache memoryCache,
            IRepository<RoleRule8> repRoleRule, IRepository<Rule8> repRule, IRepository<User8> repUsers, IRepository<UserRoles8> repUserRole,
            IRepository<User8Group> repUserGroup, IRepository<User8GroupUser> repUserGroupU)
        {
            cache = new CacheHelper(memoryCache, dbContext.SubDomain);

            this.repRoleRule = repRoleRule;
            this.repRule = repRule;
            this.repUsers = repUsers;
            this.repUserRole = repUserRole;
            this.repUserGroup = repUserGroup;
            this.repUserGroupUser = repUserGroupU;
        }

        public User8 GetById(int id)
        {

            User8 u = repUsers.Table.Where(d => d.Id == id).First();
            return u;
        }

        public User8 GetByNamePassword(string name, string pass)
        {
            User8 u = null;
            string passIn = Hash(pass).ToLower();
            var rest = repUsers.Table.Where(d => (d.Email == name) && d.PasswordHash == passIn);

            if (rest.Count() == 1)
            {
                u = rest.FirstOrDefault();
            }

            return u;
        }

        public IList<User8> GetForRegister(string email)
        {
            var rest = repUsers.Table.Where(d => d.Email.ToLower() == email.ToLower() || d.UserName.ToLower() == email.ToLower()).ToList();
            return rest;
        }

        public IList<User8> GetByEmail(string email)
        {
            var rest = repUsers.Table.Where(d => d.Deleted == false && d.Email.ToLower() == email.ToLower()).ToList();

            return rest;
        }

        public IList<User8> GetDeletedUsers()
        {

            var rest = repUsers.Table.Where(d => d.Deleted == true).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno).ToList();

            return rest;
        }

        public void Update(User8 user)
        {
            repUsers.Update(user);
        }

        public void Insert(User8 user)
        {
            repUsers.Insert(user);
        }

        public void Delete(User8 user)
        {
            repUsers.Delete(user);
        }

        public static string Hash(string stringToHash)
        {
            using (var sha1 = new SHA1Managed())
            {
                return System.BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToHash))).Replace("-", "");
            }
        }

        public List<string> GetRules(int id)
        {

            List<string> li = (from ru in repUserRole.TableUntracked
                               join rr in repRoleRule.TableUntracked on ru.RoleId equals rr.RoleId
                               join ro in repRule.TableUntracked on rr.PravoId equals ro.Id
                               where ru.UserId == id
                               select ro.Name).Distinct().ToList();
            return li;

        }

        public IQueryable<User8> GetCustomers(bool incl = false)
        {

            if (incl)
            {
                var data = repUsers.TableUntracked.Include(d => d.Kredity).Include(d => d.KredityCas).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno);
                return data;
            }
            else
            {
                var data = repUsers.TableUntracked.OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno);
                return data;
            }


        }

        public IQueryable<User8> GetCustomers(IEnumerable<int> ids, bool incl = false)
        {

            if (incl)
            {
                var data = repUsers.TableUntracked.Where(d => ids.Contains(d.Id)).Include(d => d.Kredity).Include(d => d.KredityCas).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno);
                return data;
            }
            else
            {
                var data = repUsers.TableUntracked.Where(d => ids.Contains(d.Id)).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno);
                return data;
            }


        }

        readonly string cikGetUsers = $"GetUsers";
        public void ClearUsers()
        {
            cache.ClearData(cikGetUsers);
        }

        public IList<User8> GetAll()
        {
            var list = cache.GetData<IList<User8>>(cikGetUsers);
            if (list == null)
            {
                list = repUsers.TableUntracked.ToList();
                cache.SetData<IList<User8>>(cikGetUsers, list);
            }

            return list;
        }

        public IList<User8> GetByRole(int id)
        {
            var data = (from u in repUsers.TableUntracked
                        join r in repUserRole.TableUntracked on u.Id equals r.UserId
                        where r.RoleId == id
                         && u.Deleted == false
                        select u).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno).ToList();

            return data;
        }

        public IList<User8> GetAllByGroup(int id, bool incl = false)
        {
            if (incl != true)
            {
                var data = (from u in repUsers.TableUntracked
                            join r in repUserGroupUser.TableUntracked on u.Id equals r.UserId
                            where r.GroupId == id
                            && u.Deleted == false
                            select u).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno).ToList();
                return data;
            }
            else
            {
                var data = (from u in repUsers.TableUntracked.Include(d => d.Kredity).Include(d => d.KredityCas)
                            join r in repUserGroupUser.TableUntracked on u.Id equals r.UserId
                            where r.GroupId == id
                            && u.Deleted == false
                            select u).OrderBy(d => d.Prijmeni).ThenBy(d => d.Jmeno).ToList();
                return data;
            }

        }

        public IList<User8GroupUser> GetUsersGroup(int id)
        {
            var data = repUserGroupUser.TableUntracked.Where(d => d.UserId == id).ToList();

            return data;
        }

        public void AddUserGroup(User8GroupUser ug)
        {
            repUserGroupUser.Insert(ug);
        }

        public bool RemoveUserGroup(User8GroupUser ug)
        {
            bool ret = false;
            var aa = repUserGroupUser.Table.Where(d => d.UserId == ug.UserId && d.GroupId == ug.GroupId).FirstOrDefault();
            if (aa != null)
            {
                repUserGroupUser.Delete(aa);
                ret = true;
            }
            return ret;
        }

        public IList<User8> GetByIds(IEnumerable<int> id)
        {
            var rest = repUsers.Table.Where(d => id.Contains(d.Id)).ToList();

            return rest;
        }
    }
}
