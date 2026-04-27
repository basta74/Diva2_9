using Diva2.Core.Main;
using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Pobocky;
using Diva2.Core.Model.Json;
using Diva2Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2Web.Models
{
    public class BaseModel
    {
        public UserModel User { get; set; } = new UserModel();

        public string SubDomain { get; set; }

        public Pobocka Pobocka { get; set; }

        public IList<Lektor> Lektori { get; set; }

        public IEnumerable<Pobocka> Pobocky { get; set; }

        public IList<CasZacatek> Zacatky { get; set; }
    }

    public class BaseItemModel
    {
        public JsonStatus Status { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime CreatedDt { get; set; }

        public int? EditedUserId { get; set; }

        public DateTime? EditedDt { get; set; }

        public int? AcceptedUserId { get; set; }

        public DateTime? AcceptedDt { get; set; }

        /*

        ALTER TABLE `XXX`
	ADD COLUMN `Sloupec 20` DATETIME NOT NULL DEFAULT now() AFTER `email`,
	ADD COLUMN `createdUserId` INT UNSIGNED NOT NULL AFTER `navrhy`,
	ADD COLUMN `editedUserId` INT UNSIGNED NULL DEFAULT NULL AFTER `createdUserId`,
	ADD COLUMN `acceptedUserId` INT UNSIGNED NULL DEFAULT NULL AFTER `editedUserId`,
	ADD COLUMN `createdDt` DATETIME NOT NULL DEFAULT now() AFTER `acceptedUserId`,
	ADD COLUMN `editedDt` DATETIME NULL DEFAULT NULL AFTER `createdDt`,
	ADD COLUMN `acceptedDt` DATETIME NULL DEFAULT NULL AFTER `editedDt`; /**/
    }
}
