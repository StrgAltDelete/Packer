using System.Net;
using System;
using System.IO;

namespace Zipper
{
    class Program
    {
        static void Main(string[] args)
        {
            string s_file = "test.txt";
            char c_sign = '³';
            //while(true) //Schleife zur Eingabe des Dateinamens und der Überprüfung, ob diese exestiert
            //{
                /*Console.WriteLine("Geben Sie den Dateinamen ein");
                s = Console.ReadLine();
                if(File.Exists(s)) //Überprüft ob die Datei exestiert und bricht die Schleife ab wenn ja
                {
                    break;
                }*/
            //}
            string s_name = "";
            foreach(char c in s_file)
            {
              if(c == '.')
                break;
              s_name += c;
            }
            s_name += ".fun";
            if(!File.Exists(s_name))
            {
              File.Create(s_name);
            }
            else
            {
              Console.WriteLine("Datei bereits vorhanden");
            }
            FileStream fs = new FileStream(s_file, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            BinaryWriter bw = new BinaryWriter(fs);
            string s_bytes = "";
            while(fs.Position < fs.Length)
            {
              s_bytes += (char)br.ReadByte();
            }
            char[] ac_ASCII = s_bytes.ToCharArray();
            foreach(char c in ac_ASCII)
            {
              Console.Write(c);
            }
            bw.Flush();
            bw.Close();
            br.Close();
            fs.Close();
        }
    }
}
