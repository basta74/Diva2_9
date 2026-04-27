using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Lektori
{
    public class Lektor : BaseEntity
    {
        public string Nick { get; set; }
        public string Jmeno { get; set; }
        public string Titul { get; set; }
        public int Kredity { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Popis { get; set; }
        public int Poradi { get; set; }
        public bool Platnost { get; set; }
        public bool Viditelnost { get; set; }


        public int UserId { get; set; }
        public int Koeficient { get; set; }

    }
}
