using System;
using System.IO;

namespace Zipper
{
    class Program
    {
        static void Main(string[] args)
        {
            string s_file = "";
            while(true)                  //Schleife zur Eingabe des Dateinamens und der Überprüfung, ob diese exestiert
            {
                Console.WriteLine("Geben Sie den Dateinamen mit Dateiendung ein");
                s_file = Console.ReadLine();
                if(File.Exists(s_file))  //Überprüft ob die Datei exestiert und bricht die Schleife ab wenn ja
                {
                    break;
                }
            }
            Encoder encoder = new Encoder(s_file, '³');
            if(encoder.Exist() == true)
            {
                encoder.Encoding();
            }
        }
    }
    public class Encoder 
    {
        char[] ac_prefix = new char[4] { 'M', 'O', 'I', 'N' };  //Dateipräfix zur Identifikation als unsere Datei (MagicNumber)
        char[] ac_ASCII;
        string s_file;                                          //Dateiname
        string s_name;                                          //Dateinname für.fun
        string s_ending;
        char c_sign;                                            //Trennzeichen
        FileStream fs;
        BinaryReader br;
        BinaryWriter bw;

        public Encoder(string s_file, char c_sign) 
        {
            this.s_file = s_file;
            this.c_sign = c_sign;
            Name();
        }
        private void Name()                                     /*Name der Datei speichern*/
        {
            string s_name = "";
            string s_ending = "";
            bool b_ending = false;
            foreach (char c in s_file)
            {
                if (c == '.')                                   //Wenn . ist, also wenn die Datei endet, wird die Dateiendung extra gespeichert
                    b_ending = true;
                if (b_ending == false)
                    s_name += c;
                else
                    s_ending += c;
            }
            s_name += ".fun";                                   //Gefilterter Name + neue Dateiendung
            this.s_name = s_name;
            this.s_ending = s_ending;
        }
        public bool Exist()                                     /*Überprüft ob die .fun Datei bereits exestiert und gibt false zurück wenn sie bereits exestiert und true wenn nicht*/
        {
            if (!File.Exists(s_name))                           //Überprüft ob die neue .fun Datei bereits exestiert
                return true;                                    //Gibt true zurück, da mit dem encoden fortgefahren werden kann
            else
                return false;                                   //Gibt false zurück, da die Datei überschrieben werden würde
        }
        public void Encoding()                                  /*Ruft alle notwendigen Methoden zum encoden auf*/
        {
            Read();
            Prefix();
            Ending();
            Done();
        }
        private void Read()                                     /*Liest den Inhalt der zu packenden Datei und speichert ihn in einem Char-Array*/
        {
            fs = new FileStream(s_file, FileMode.Open);
            br = new BinaryReader(fs);
            string s_bytes = "";
            while (fs.Position < fs.Length)
            {
                s_bytes += (char)br.ReadByte();
            }
            ac_ASCII = s_bytes.ToCharArray();
            fs.Flush();
            br.Close();
            fs.Close();
        }
        private void Prefix()
        {
            fs = new FileStream(s_name, FileMode.Create);
            br = new BinaryReader(fs);
            bw = new BinaryWriter(fs);
            fs.Position = 0;
            foreach (char c in ac_prefix)
                bw.Write(c);
        }
        private void Ending()
        {
            foreach (char c in s_ending)
                bw.Write(c);
            foreach (char c in ac_ASCII)
                bw.Write(c);

            //debug
            fs.Position = 0;
            while (fs.Position < fs.Length)
            {
                Console.Write((char)br.ReadByte());
            }
            Console.ReadLine();
        }
        private void Done()                                     /*Schliesst und leer alle offenen Streams*/
        {
            bw.Flush();
            bw.Close();
            br.Close();
            fs.Close();
        }
    }
}