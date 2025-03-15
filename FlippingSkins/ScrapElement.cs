using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippingSkins
{
    internal abstract class ScrapElement
    {
        protected const float FEESKINSMONKEY = 0.84f;
        protected const float FEECSGOSTEAM = 0.87f;
        public string Name { get; set; }

        public ScrapElement(string name) 
        {
            Name = name;
        }
        public abstract void Description();
    }

    internal class ScrapRust : ScrapElement
    {
        public float PriceRustSkinsMonkey { get; set; }
        public float PriceRustSteam { get; set; }
        public double ProcentOfPrice {  get; set; }

        public ScrapRust(string name, float priceRustSkinsMonkey) : base (name)
        {
            PriceRustSkinsMonkey = priceRustSkinsMonkey;
        }

        private float PriceAfterFee() => (float)Math.Round(PriceRustSkinsMonkey * FEESKINSMONKEY, 2);

        private double ProfitOfBuy() => Math.Round(PriceAfterFee() - PriceRustSteam, 2);

        public void SetProcent()
        {
            ProcentOfPrice = Math.Round(PriceAfterFee() * 100 / PriceRustSteam, 2);
        }

        public override void Description()
        {
            Console.WriteLine($"Name:                      {Name}");
            Console.WriteLine($"Sell item on SkinsMonkey:  {PriceAfterFee()}$");
            Console.WriteLine($"Buy order Steam:           {PriceRustSteam}$");
            Console.WriteLine($"Difference:                {ProfitOfBuy()}$");
            Console.WriteLine($"% of price SkinsMonkey:    {ProcentOfPrice}%");
            Console.WriteLine("\n");
        }
    }

    internal class ScrapCSGO : ScrapElement
    {
        public float PriceCSGOSkinsMonkey { get; set; }
        public float PriceCSGOSkinsSteam { get; set; }
        public double ProcentOfPrice { get; set; }

        public ScrapCSGO(string name, float priceCSGOSkinsMonkey) : base(name)
        {
            PriceCSGOSkinsMonkey = priceCSGOSkinsMonkey;
        }


        private float PriceAfterFee() => (float)Math.Round(PriceCSGOSkinsSteam * FEECSGOSTEAM, 2);

        private double ProfitOfBuy() => Math.Round(PriceAfterFee() - PriceCSGOSkinsMonkey, 2);

        public void SetProcent()
        {
            ProcentOfPrice = Math.Round(PriceAfterFee() * 100 / PriceCSGOSkinsMonkey, 2);
        }

        public override void Description()
        {
            Console.WriteLine($"Name:                {Name}");
            Console.WriteLine($"Sell item on Steam:  {PriceAfterFee()}$");
            Console.WriteLine($"Buy on SkinsMonkey:  {PriceCSGOSkinsMonkey}$");
            Console.WriteLine($"Difference:          {ProfitOfBuy()}$");
            Console.WriteLine($"% of price Steam:    {ProcentOfPrice}%");
            Console.WriteLine("\n");
        }
    }

}
