using Diva2.Core.Main.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Content
{
    public interface IPageService
    {
        IEnumerable<Page> GetAll();

        IEnumerable<Page> GetVisibleForMenu();

        Page GetByType(PageType type);

        Page GetById(int id);


        void Update(Page p);

    }
}
