using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public class Rule8 : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IniCategory Category { get; set; }

        /// <summary>
        /// define public by user
        /// </summary>
        public bool PublicAble { get; set; }
    }


    public enum IniCategory {Basta, Rules, Board, Users, Setting, Content, Video}


    public enum IniItemsBasta { all_able }

    public enum IniItemsRules { prava_edit, prava_view, role_view, role_edit, role_change }

    public enum IniItemsBoard { spin_view, spin_view_all_lesson,   lektor_view, spin_close, spin_del_after }

    public enum IniItemsSetting { spinstat_view, spin_edit, platba_del_do5, platba_del_po5, sms_view }

    public enum IniItemsUsers { users_delete, users_register , users_role_edit , users_role_view , users_view_all_email, users_category_edit, users_category_change
    , heslo_edit  , pob_for_user    }

    public enum IniItemsContent { content_manage, page_view, page_edit, page_insert }

    public enum IniItemsVideo { video_manage, video_edit, video_insert }
}
