using FlippingSkins;

internal class Program
{
    private static void Main(string[] args)
    {
        do
        {
            try
            {
                Console.WriteLine("######################################");
                Console.WriteLine("################ MENU ################");
                Console.WriteLine("######################################");

                Console.Write("\n\n1. Start scraping prices\n2. Informations\n3. Exit\n\nNumber: ");
                ConsoleKeyInfo key = Console.ReadKey();

                switch(key.KeyChar)
                {
                    case '1':
                        //scrap
                        break;
                    case '2':
                        // info
                        break;
                    case '3':
                        Environment.Exit(0);
                        break;
                }


            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR!!!");
                Console.ResetColor();
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Click enter to continue");
                Console.ReadKey();
            }
        }while (true);
    }
}