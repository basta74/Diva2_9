using Diva2.Core.Main.Videa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Videos
{
    public class UserVideoModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int VideoId { get; set; }

        public Video Video { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public int Kredity { get; set; }

        public bool Zaplaceno { get; set; }

        public DateTime ZaplacenoDt { get; set; }

        public bool Aktivovano { get; set; }

        public DateTime AktivovanoDt { get; set; }

        public bool Zobrazeno { get; set; }

        public DateTime ZobrazenoDt { get; set; }

        public UserVideoModel(UserVideo vi)
        {
            Id = vi.Id;
            UserId = vi.UserId;
            VideoId = vi.VideoId;
            Video = vi.Video;
            Name = vi.Nazev;
            Url = vi.Url;
            Image = vi.Image;

            Kredity = vi.Kredity;
            Zaplaceno = vi.Zaplaceno;
            ZaplacenoDt = vi.ZaplacenoDt;

            Aktivovano = vi.Aktivovano;
            AktivovanoDt = vi.AktivovanoDt;

            Zobrazeno = vi.Zobrazeno;
            ZobrazenoDt = vi.ZobrazenoDt;

        }

        public UserVideoModel()
        {
        }
    }
}
