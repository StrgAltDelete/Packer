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
            someOtherClass.Algorithm();
        }
    }

    class Decoder
    {
        string magicNumber;
        string originalfilename;
        int contentStartPos;
        string filename;
        string filenameOut;
        byte seperator;
        byte currentByte;
        byte multiplier;
        byte letter;
        FileStream fs_read;
        FileStream fs_write;
        BinaryReader br;
        BinaryWriter bw;

        public Decoder()    //Konstruktor
        {
            magicNumber = " ";
            originalfilename = " ";
            contentStartPos = 0;
            filename = "frames.j.fun";
            filenameOut = "Will get set later";
            currentByte = 0;
            multiplier = 0;
            letter = 0;

            fs_read = new FileStream(filename, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs_read);
            fs_write = new FileStream(filenameOut, FileMode.Create, FileAccess.Write);
            bw = new BinaryWriter(fs_write);
        }

        public void Algorithm()
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
                        letter = br.ReadByte();
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


            seperator = br.ReadByte(); //Trennzeichen ist immer an der 4. Stelle

            char tmpname = ' ';
            while (br.ReadByte() != '[')
            {
                fs_read.Position--;
                tmpname = (char)br.ReadByte();
                originalfilename += tmpname;
            }

            contentStartPos = (int)fs_read.Position;

            filenameOut = originalfilename;


            bw.Flush();
            fs_read.Flush();
            fs_write.Flush();
            fs_write.Close();
            fs_read.Close();
            bw.Close();
            br.Close();
        }
    }
}
