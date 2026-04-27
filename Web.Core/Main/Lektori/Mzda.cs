using Diva2.Core;
using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Core.Main.Lektori
{
    public class Mzda : BaseEntity
    {
        public int PokladnaId { get; set; }
        public int Mesic { get; set; }
        public int Den { get; set; }
        public int Hodina { get; set; }
        public int Zakaznik { get; set; }
        public int Pro100 { get; set; }
        public int Zak01 { get; set; }
        public int Zak02 { get; set; }
        public int Zak03 { get; set; }
        public int Zak04 { get; set; }
        public int Zak05 { get; set; }
        public int Zak06 { get; set; }
        public int Zak07 { get; set; }
        public int Zak08 { get; set; }
        public int Zak09 { get; set; }
        public int Zak10 { get; set; }
        public int Zak11 { get; set; }
        public int Zak12 { get; set; }
        public int Zak13 { get; set; }
        public int Zak14 { get; set; }
        public int Zak15 { get; set; }
        public int Zak16 { get; set; }
        public int Zak17 { get; set; }
        public int Zak18 { get; set; }
        public int Zak19 { get; set; }
        public int Zak20 { get; set; }

        public static int GetByPocet(int[] arr, int pocet) {

            int ret = 0;
            if (arr.Count() < pocet) {
                pocet = arr.Count() - 1;
            }
            for (int i = pocet; i > 0; i--)
            {
                try
                {
                    if (arr[i] > 0)
                    {
                        ret = arr[i];
                        break;
                    }
                }
                catch (Exception ex) { 
                }
            }
            return ret;
        }
    }
}
