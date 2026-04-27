using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Users
{
    public interface IRuleService
    {
        Rule8 GetById(int id);
        List<Rule8> GetAll();
        void Insert(Rule8 o);


        Role8 GetRoleById(int id);
        List<Role8> GetRoleAll();


        List<int> GetRulesByRole(int id);

        RoleRule8 GetRoleRule(int roleId, int ruleId);

        void AddRuleToRole(RoleRule8 o);

        void RemoveRuleFromRole(RoleRule8 o);

        List<UserRoles8> GetUserRoles(int id);
        void AddRoleToUser(UserRoles8 userRoles8);
        void RemoveRoleFromUser(UserRoles8 rr);
    }
}
