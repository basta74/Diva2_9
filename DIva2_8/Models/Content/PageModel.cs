using Diva2.Core.Main.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Content
{


    public class PageModel
    {
        public int Id { get; set; }

        public PageType Type { get; set; }

        [Required]
        [Display(Name = "Url")]
        public string UrlName { get; set; }

        [Required]
        [Display(Name = "Pořadí")]
        [Range(1, 9999)]
        public int Order { get; set; }

        public bool Active { get; set; }

        [Required]
        [Display(Name = "Titulek")]
        public string Title { get; set; }

        
        public string Content { get; set; }

        public PageModel()
        {

        }

        public PageModel(Page p)
        {
            Id = p.Id;
            CopyFromDb(p);
        }

        public void CopyToDb(Page db) {

            db.Active = Active;
            db.UrlName = UrlName;
            db.Order = Order;
            db.Title = Title;
            db.Content = Content;

        }

        public void CopyFromDb(Page db)
        {

            Active = db.Active;
            UrlName = db.UrlName;
            Order = db.Order;
            Title = db.Title;
            Content = db.Content;

        }

    }
}
