using System;
using System.IO;

namespace Decoder_Alpha
{
    class Program
    {
        static void Main(string[] args)     //Ctrl K + Ctrl D
        {
            Decoder someOtherClass = new Decoder();
            someOtherClass.GetHeaderInfo();
            someOtherClass.Alorithm();

            Console.ReadKey();
        }
    }

    class Decoder
    {
        string magicNumber;
        string originalfilename;
        int contentStartPos;
        string filename;
        string filenameOut;
        char seperator;
        byte currentByte;
        long currentPos;
        byte multiplier;
        char letter;
        FileStream fs_read;
        FileStream fs_write;
        BinaryReader br;
        BinaryWriter bw;

        public Decoder()    //Konstruktor
        {
            magicNumber = " ";
            originalfilename = " ";
            contentStartPos = 0;
            filename = "T6.txt.fun";
            filenameOut = "HiOut.fun";
            currentByte = 0;
            currentPos = 0;
            multiplier = 0;
            letter = ' ';

            fs_read = new FileStream(filename, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs_read);
            fs_write = new FileStream(filenameOut, FileMode.Create, FileAccess.Write);
            bw = new BinaryWriter(fs_write);
        }

        public void Alorithm()
        {
            fs_read = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fs_write = new FileStream(filenameOut, FileMode.Create, FileAccess.Write);
            br = new BinaryReader(fs_read);
            bw = new BinaryWriter(fs_write);

            fs_read.Position = contentStartPos;
            while (fs_read.Position < fs_read.Length)
            {
                currentByte = br.ReadByte();
                if (currentByte == seperator && fs_read.Position != fs_read.Length)
                {
                    multiplier = br.ReadByte();
                    letter = (char)br.ReadByte();
                    for (int i = 0; i < multiplier; i++)
                    {
                        bw.Write(letter);
                    }
                }
                else
                {
                    bw.Write(currentByte);
                }
            }

            fs_read.Flush();
            bw.Flush();
            fs_read.Close();
            bw.Close();
            br.Close();
        }

        public void GetHeaderInfo()         //Methode ermittelt die magicNumber, seperator, ending und die contentStartPos Variablen
        {
            char tmpmagic = ' ';
            for (int i = 0; i < 4; i++)  //get Magic Number
            {
                tmpmagic = (char)br.ReadByte();
                magicNumber += tmpmagic;
            }


            seperator = (char)br.ReadByte(); //Trennzeichen ist immer an der 4. Stelle

            char tmpname = ' ';
            while (br.ReadByte() != '[')
            {
                fs_read.Position--;
                tmpname = (char)br.ReadByte();
                originalfilename += tmpname;
                contentStartPos = (int)fs_read.Position;
            }

            filenameOut = originalfilename;


            bw.Flush();
            fs_read.Flush();
            fs_write.Flush();
            fs_write.Close();
            fs_read.Close();
            bw.Close();
            br.Close();


            //bw.Write(magicNumber + seperator + ending);                   //Schreibt die Magic Number, Seperator und die Ending Var in eine .txt Datei
            Console.Write(magicNumber + "\r\n" + seperator + "\r\n" + originalfilename);
        }
    }
}
