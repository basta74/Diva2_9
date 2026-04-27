using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public interface ILekceService
    {

        BranchData GetWeaksApi(int pobId);

        Rozvrhy GetWeaksFuture(int pobId, DateTime? dt);

        Rozvrhy2 GetWeaksFuture2(int pobId, DateTime? dt);

        Rozvrhy GetWeaksFromMonday(int pobId, DateTime dt);

        IEnumerable<Lekce> GetWeakByDay(int pobId, DateTime dt);
        Lekce GetById(int id);
      
        void Insert(Lekce obj);
        void Update(Lekce obj);
        void Delete(Lekce obj);

        List<int> GetLektoryMesic(int rok, int mes);

        List<Lekce> GetMzdyMesic(int rok, int mes);
        List<Lekce> GetMzdyMesic(int id, int year, int month);
        List<Lekce> GetByDay(int pobId, DateTime currentDate);
        Lekce GetBy(int rok, int tyden, int den, int minutaKey);

        IList<Lekce> GetNotClossed(int id);
        Rozvrhy3 GetWeaksFuture3(int id);
    }
}
