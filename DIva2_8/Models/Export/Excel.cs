namespace Diva2Web.Models.Export
{
    public class Excel
    {
        public string Col_01 { get; set; } = "";
        public string Col_02 { get; set; } = "";
        public string Col_03 { get; set; } = "";

        public string Col_04 { get; set; } = "";
        public string Col_05 { get; set; } = "";
        public string Col_06 { get; set; } = "";

        public string Col_07 { get; set; } = "";
        public string Col_08 { get; set; } = "";
        public string Col_09 { get; set; } = "";


        public Excel() { }

        public Excel(string[] w)
        {
            int count = w.Length;

            Col_01 = w[0];

            if (count > 1) { Col_02 = w[1]; }
            if (count > 2) { Col_03 = w[2]; }
            if (count > 3) { Col_04 = w[3]; }
            if (count > 4) { Col_05 = w[4]; }
            if (count > 5) { Col_06 = w[5]; }
            if (count > 6) { Col_07 = w[6]; }
            if (count > 7) { Col_08 = w[6]; }
            if (count > 8) { Col_09 = w[6]; }
        }
    }
}
