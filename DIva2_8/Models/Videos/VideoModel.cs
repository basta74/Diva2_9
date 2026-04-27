using Diva2.Core.Main.Videa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Videos
{
    public class VideoModel
    {
        public VideoModel()
        {
        }

        public VideoModel(Video vi)
        {
            CopyFromDb(vi);
        }

        // 
        public int UserId { get; set; }

        public int Id { get; set; }


        [Required]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Url videa")]
        public string Url { get; set; }

        [Display(Name = "Url obrázku")]
        public string Image { get; set; }

        [Display(Name = "Viditelné na webu")]
        public bool Visible { get; set; }

        [Required]
        public string Desc { get; set; }

        [Range(0, 9999)]
        public int Kredity { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public void CopyFromDb(Video vi)
        {
            Id = vi.Id;
            Name = vi.Name;
            Url = vi.Url;
            Image = vi.Image;
            Desc = vi.Desc;
            Kredity = vi.Kredity;
            Visible = vi.Visible;
            DateFrom = vi.DateFrom;
            DateTo = vi.DateTo;
        }

        public void CopyToDb(Video vi)
        {
            vi.Name = Name;
            vi.Url = Url;
            vi.Image = Image;
            vi.Desc = Desc;
            vi.Kredity = Kredity;
            vi.Visible = Visible;
            vi.DateFrom = DateFrom;
            vi.DateTo = DateTo;
        }
    }
}
