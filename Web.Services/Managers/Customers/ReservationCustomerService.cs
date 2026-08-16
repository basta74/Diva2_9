using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using Diva2.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Diva2.Services.Managers.Customers;

public sealed class ReservationCustomerService : IReservationCustomerService
{
    private readonly IRepository<User8> users;
    private readonly IRepository<User8GroupUser> userGroups;

    public ReservationCustomerService(
        IRepository<User8> users,
        IRepository<User8GroupUser> userGroups)
    {
        this.users = users;
        this.userGroups = userGroups;
    }

    public IQueryable<User8> GetCustomers(bool includeCredits = false)
    {
        IQueryable<User8> query = users.TableUntracked;

        if (includeCredits)
        {
            query = query
                .Include(user => user.Kredity)
                .Include(user => user.KredityCas);
        }

        return query
            .OrderBy(user => user.Prijmeni)
            .ThenBy(user => user.Jmeno);
    }

    public IQueryable<User8> GetCustomers(IEnumerable<int> ids, bool includeCredits = false)
    {
        IQueryable<User8> query = users.TableUntracked
            .Where(user => ids.Contains(user.Id));

        if (includeCredits)
        {
            query = query
                .Include(user => user.Kredity)
                .Include(user => user.KredityCas);
        }

        return query
            .OrderBy(user => user.Prijmeni)
            .ThenBy(user => user.Jmeno);
    }

    public IList<User8> GetAllByGroup(int groupId, bool includeCredits = false)
    {
        IQueryable<User8> query = users.TableUntracked;

        if (includeCredits)
        {
            query = query
                .Include(user => user.Kredity)
                .Include(user => user.KredityCas);
        }

        return (from user in query
                join relation in userGroups.TableUntracked on user.Id equals relation.UserId
                where relation.GroupId == groupId && !user.Deleted
                orderby user.Prijmeni, user.Jmeno
                select user).ToList();
    }

    public IList<User8GroupUser> GetUsersGroup(int userId)
    {
        return userGroups.TableUntracked
            .Where(relation => relation.UserId == userId)
            .ToList();
    }

    public void AddUserGroup(User8GroupUser relation)
    {
        userGroups.Insert(relation);
    }

    public bool RemoveUserGroup(User8GroupUser relation)
    {
        User8GroupUser? existing = userGroups.Table
            .FirstOrDefault(item =>
                item.UserId == relation.UserId &&
                item.GroupId == relation.GroupId);

        if (existing is null)
        {
            return false;
        }

        userGroups.Delete(existing);
        return true;
    }
}
