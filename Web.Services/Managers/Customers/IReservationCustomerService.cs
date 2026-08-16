using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using System.Collections.Generic;
using System.Linq;

namespace Diva2.Services.Managers.Customers;

/// <summary>
/// Reservation-specific customer queries and group assignments.
/// These operations are intentionally separate from the reusable user service.
/// </summary>
public interface IReservationCustomerService
{
    IQueryable<User8> GetCustomers(bool includeCredits = false);
    IQueryable<User8> GetCustomers(IEnumerable<int> ids, bool includeCredits = false);
    IList<User8> GetAllByGroup(int groupId, bool includeCredits = false);
    IList<User8GroupUser> GetUsersGroup(int userId);
    void AddUserGroup(User8GroupUser relation);
    bool RemoveUserGroup(User8GroupUser relation);
}
