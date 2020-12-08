using System;
using System.IO;

namespace Zipper
{
    class Program
    {
        static void Main(string[] args)
        {
            string s_file = "";
            string s_originalName = "";
            while (true)                                                                        //Schleife zur Eingabe des Dateinamens und der Überprüfung, ob diese exestiert
            {
                Console.WriteLine("Geben Sie den Dateinamen mit Dateiendung ein");
                s_originalName = Console.ReadLine();
                if (File.Exists(s_originalName))                                                //Überprüft ob die Datei exestiert und bricht die Schleife ab wenn ja
                {
                    s_file = s_originalName;
                    if (s_originalName.Length > 8)                                              //Überprüft ob der Name gekürtzt werden muss
                        s_file = s_originalName.Remove(8);
                    break;
                }
            }
            Encoder encoder = new Encoder(s_originalName, s_file);
            if (encoder.Exist() == true)                                                        //Wenn die .fun-Zieldatei noch nicht exestiert wird eine erstellt
            {
                encoder.Encoding();
            }
        }
    }
    public class Encoder
    {
        string s_prefix = "MOIN";                                                               //Dateipräfix zur Identifikation als unsere Datei (MagicNumber)
        string s_file;                                                                          //Dateiname
        string s_name;                                                                          //Dateinname für.fun
        byte b_sign;                                                                            //Trennzeichen
        byte[] ab_signs = new byte[256];
        FileStream fs_Write;                                                                    //Stream zum Setzen der Datei
        FileStream fs_Read;                                                                     //Stream zum Lesen der Datei
        BinaryReader br;
        BinaryWriter bw;

        public Encoder(string s_originalName, string s_name)                   /*Konstruktor*/
        {
            this.s_file = s_originalName;
            this.b_sign = Sign();
            Name(s_name);
        }
        private void Name(string s_name)                                                    /*Name der Datei speichern*/
        {
            s_name += ".fun";                                                                   //Gekürtzer Name + neue Dateiendung
            this.s_name = s_name;
        }
        public bool Exist()                                                                 /*Überprüft ob die .fun Datei bereits exestiert und gibt false zurück wenn sie bereits exestiert und true wenn nicht*/
        {
            if (!File.Exists(s_name))                                                           //Überprüft ob die neue .fun Datei bereits exestiert
                return true;                                                                    //Gibt true zurück, da mit dem encoden fortgefahren werden kann
            else
                return false;                                                                   //Gibt false zurück, da die Datei überschrieben werden würde
        }
        public void Encoding()                                                              /*Ruft alle notwendigen Methoden zum encoden auf*/
        {
            Prefix();
            Algorythm();
            Done();
        }
        private void Prefix()                                                               /*Ruft alle notwendigen Methoden zum setzen des Headers auf*/
        {
            MagicNumber();
            Seperator();
            Name();
        }
        private void MagicNumber()                                                          /*Identikator, dass es unsere .fun-Datei ist*/
        {
            fs_Write = new FileStream(s_name, FileMode.Create);
            bw = new BinaryWriter(fs_Write);
            foreach (byte b in s_prefix)                                                        //Setzt unsere MagicLine
                bw.Write(b);
        }
        private void Name()                                                               /*Schreibt den evtl gekürtzten Dateinamen in den Header*/
        {
            foreach (byte b in s_name)                                                      //geht durch den Namen der .fun Datei
            {
                bool b_dot = false;
                if (b == '.')                                                               //Wenn es . erreicht wechselt es zur originellen Datei
                {
                    b_dot = true;
                    bool b_dot2 = false;
                    foreach (byte b2 in s_file)                                             //Geht durch die originelle Datei
                    {
                        if (b2 == '.')                                                      //Wenn es . erreicht wird die Dateiendung in die .fun-Datei geschrieben
                            b_dot2 = true;
                        if (b_dot2 == true)
                            bw.Write(b2);
                    }
                }
                if (b_dot == true)
                    break;
                bw.Write(b);
            }
            bw.Write((byte)'[');                                                                      //Identikator für das Ende des Namens der originellen Datei (Festgelegt)
        }
        private void Seperator()                                                            /*Schreibt das Trennzeichen in den Header*/
        {
            bw.Write(b_sign);
        }
        private void Done()                                                                 /*Schliesst und leer alle offenen Streams*/
        {
            bw.Flush();
            bw.Close();
            br.Close();
            fs_Write.Close();
            fs_Read.Close();
        }
        private byte Sign()
        {
            fs_Read = new FileStream(s_file, FileMode.Open);
            br = new BinaryReader(fs_Read);
            while (fs_Read.Position < fs_Read.Length)
            {
                ab_signs[br.ReadByte()]++;
            }
            byte b_bestPos = 0;
            byte b_bestValue = 255;
            for (byte b = 1; b < ab_signs.Length; b++)                                          //Weil Byte 0 (also NULL) nicht funtioniert
            {
                if (ab_signs[b] == 0)
                    return b;
                else if (ab_signs[b] < b_bestValue)
                    b_bestValue = ab_signs[b];
                b_bestPos = b;
                if (b == 255)
                {
                    break;
                }
            }
            return ab_signs[b_bestPos];
        }
        private void Algorythm()                                                            /*Der endgültige Encoder-Algorythmus*/
        {
            byte b_sameSigns = 1;                                                               //Zählt die Anzahl an gleichen Zeichen, startet bei 1, da der Ausgangspunkt nicht mitgezählt wird
            long l_length = fs_Read.Length;
            for (long l = 0; l < l_length; l++)                                           //Durch die Datei lesen
            {
                fs_Read.Position = l;
                if (l != l_length - 1)                                                    //Wenn es nicht das letzte Zeichen ist
                {
                    if ((b_sameSigns >= 4 && br.ReadByte() != br.ReadByte()) || b_sameSigns == 255)                     //Wenn das nächste Byte nicht mehr genauso wie das aktuelle ist und sich unser packing lohnt
                    {
                        fs_Read.Position = l;                                                   //Muss durch die Bedingung der if zurückgestzt werden
                        bw.Write(b_sign);
                        bw.Write(b_sameSigns);
                        bw.Write(br.ReadByte());
                        b_sameSigns = 1;
                        continue;                                                               //Damit es nicht nochmal des Zeichen von der else darunter schreibt
                    }
                    fs_Read.Position = l;                                                       //Muss durch die Bedingung der if zurückgestzt werden
                    if (br.ReadByte() == b_sign)
                    {
                        bw.Write(b_sign);
                        bw.Write(b_sameSigns);
                        bw.Write(b_sign);
                        continue;                                                               //Damit es nicht nochmal des Zeichen von der else darunter schreibt
                    }
                    fs_Read.Position = l;
                    if (br.ReadByte() == br.ReadByte())                                         //Wenn das darauf folgende Byte das selbe ist
                    {
                        b_sameSigns++;
                    }
                    else if (b_sameSigns > 1)                                                   //Falls es mehrere Zeichen einer Art sind, aber nicht genug zum packen
                    {
                        fs_Read.Position = l;
                        byte b = br.ReadByte();
                        for (byte i2 = b_sameSigns; i2 > 0; i2--)
                        {
                            bw.Write(b);
                        }
                        b_sameSigns = 1;
                    }
                    else                                                                        //Falls es nur 1 Zeichen ist
                    {
                        fs_Read.Position = l;                                                   //Muss durch die Bedingung der if davor zurückgestzt werden
                        bw.Write(br.ReadByte());
                        b_sameSigns = 1;
                    }
                }
                else if (b_sameSigns >= 4)                                                      //Falls es die letzte Stelle ist und schon Zeichen davor waren
                {
                    bw.Write(b_sign);
                    bw.Write(b_sameSigns);
                    bw.Write(br.ReadByte());
                }
                else
                {
                    bw.Write(br.ReadByte());                                                    //Falls es die letzte Stelle ist und es ein einzelnes Zeichen ist
                }
            }
        }
    }
}
