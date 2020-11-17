using System;

namespace C_
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Geben Sie eine Deizamzahl ein, welche sie umwandeln wollen");
            string s = Console.ReadLine();
            bool b = false;
            int i_Zahl = 0;
            while(!b) 
            {
                b = int.TryParse(s, out i_Zahl);
            }
            Umwandeln(i_Zahl);
        }
        static void Umwandeln(double i_Zahl) 
        {
            string s_End = "";
            while(!(s_End == "e" || s_End == "E")) 
            {
                Console.WriteLine("Geben Sie E zum Beenden ein!");
                s_End = Console.ReadLine();
                int i_Basis = 0;
                while(i_Basis < 1 || i_Basis > 16) 
                {
                    Console.WriteLine("Geben Sie die Basis ein, auf welche Sie umwandeln wollen");
                    string s = Console.ReadLine();
                    int.TryParse(s, out i_Basis);
                }
            }
        }
    }
}
