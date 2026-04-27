using Diva2.Core.Main.Users;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Diva2.Services.Managers.Users
{
    public class RuleService : IRuleService
    {


        private readonly IRepository<RoleRule8> repRoleRule;
        private readonly IRepository<Rule8> repRule;
        private readonly IRepository<UserRoles8> repUserRole;
        private readonly IRepository<Role8> repRole;

        protected static CacheHelper cache;
        public RuleService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
            IRepository<RoleRule8> repRoleRule, IRepository<Rule8> repRule, IRepository<Role8> repRole, IRepository<UserRoles8> repUserRole)
        {
            cache = new CacheHelper(memoryCache, dbContext.SubDomain);

            this.repRoleRule = repRoleRule;
            this.repRule = repRule;
            this.repUserRole = repUserRole;
            this.repRole = repRole;
        }

        #region Prava
        public Rule8 GetById(int id)
        {
            return repRule.Table.Where(d => d.Id == id).FirstOrDefault();
        }


        public List<Rule8> GetAll()
        {
            string cacheItemKey = $"Rules";
            var list = cache.GetDataSub<List<Rule8>>(cacheItemKey);

            if (list == null)
            {
                list = GetInisFromDb();
                //list = repRule.Table.OrderBy(d => d.Name).ToList();
                cache.SetDataSub(cacheItemKey, list);
            }
            return list;
        }


        public List<Rule8> GetInisFromDb()
        {

            List<Rule8> defaults = GetDefaults();
            List<string> keys = defaults.Select(d => d.Name).ToList();

            var listDb = repRule.Table.ToList();
            List<Rule8> insert = new List<Rule8>();
            List<string> keysDb = listDb.Select(d => d.Name).ToList();

            foreach (var ini in listDb)
            {
                if (keys.Contains(ini.Name))
                {
                    keys.Remove(ini.Name);
                    keysDb.Remove(ini.Name);
                }
            }

            if (keys.Count() > 0)
            {
                foreach (var key in keys)
                {
                    var Ini = defaults.Where(d => d.Name == key).FirstOrDefault();
                    if (Ini != null)
                    {
                        insert.Add(Ini);
                        listDb.Add(Ini);
                    }
                }
            }

            if (insert.Count > 0)
            {
                foreach (var rule in insert)
                {
                    Insert(rule);
                }
            }

            return listDb;

        }

        public List<Rule8> GetDefaults()
        {
            List<Rule8> list = new List<Rule8>();

         
            foreach (var suit in (IniItemsBasta[])Enum.GetValues(typeof(IniItemsBasta)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Basta, PublicAble = false });

            }
            foreach (var suit in (IniItemsRules[])Enum.GetValues(typeof(IniItemsRules)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Rules, PublicAble = true });
            }

            foreach (var suit in (IniItemsBoard[])Enum.GetValues(typeof(IniItemsBoard)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Board, PublicAble = true });
            }


            foreach (var suit in (IniItemsSetting[])Enum.GetValues(typeof(IniItemsSetting)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Setting, PublicAble = true });
            }

            foreach (var suit in (IniItemsUsers[])Enum.GetValues(typeof(IniItemsUsers)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Users, PublicAble = true });
            }

            foreach (var suit in (IniItemsContent[])Enum.GetValues(typeof(IniItemsContent)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Content, PublicAble = true });
            }

            foreach (var suit in (IniItemsVideo[])Enum.GetValues(typeof(IniItemsVideo)))
            {
                list.Add(new Rule8() { Name = suit.ToString(), Category = IniCategory.Video, PublicAble = true });
            }

            return list;
        }


        public void Insert(Rule8 o)
        {
            repRule.Insert(o);
        }
        #endregion

        #region Role
        public Role8 GetRoleById(int id)
        {
            return repRole.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public List<Role8> GetRoleAll()
        {
            return repRole.Table.OrderBy(d => d.Name).ToList();
        } 
        #endregion
               

        #region Vazba Pravo - Role
        public List<int> GetRulesByRole(int id)
        {
            return repRoleRule.Table.Where(d => d.RoleId == id).Select(d => d.PravoId).ToList();
        }

        public RoleRule8 GetRoleRule(int roleId, int ruleId)
        {
            return repRoleRule.Table.Where(d => d.RoleId == roleId && d.PravoId == ruleId).FirstOrDefault();
        }

        public void AddRuleToRole(RoleRule8 o)
        {
            repRoleRule.Insert(o);
        }

        public void RemoveRuleFromRole(RoleRule8 o)
        {
            repRoleRule.Delete(o);
        }

        public List<UserRoles8> GetUserRoles(int id)
        {
            return repUserRole.TableUntracked.Where(d => d.UserId == id).ToList();
        }


        #endregion

        #region Vazba User - Role

        public void AddRoleToUser(UserRoles8 o)
        {
            repUserRole.Insert(o);
        }

        public void RemoveRoleFromUser(UserRoles8 o)
        {
            repUserRole.Delete(o);
        } 
        #endregion

    }
}
