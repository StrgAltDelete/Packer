using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Decoder_Alpha
{
    class Program
    {
        static void Main(string[] args)     //Ctrl K + Ctrl D
        {
            //Decoder someOtherClass = new Decoder();
            //someOtherClass.someMethod();
        }
    }

    class Decoder
    {
        string filename;
        string fileCache;
        char seperator;
        char multiplier;
        char tmpCache;
        int tmpResult;
        FileStream fs;
        BinaryReader br;

        public Decoder()    //Konstruktor
        {
            filename = "Test.txt";
            fileCache = " ";
            seperator = '³';
            multiplier = ' ';
            tmpCache = ' ';
            tmpResult = 0;
            fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
        }

        public void someMethod()
        {
            Decoder Const = new Decoder();
            Const.fs.Position = 0;
            while (Const.fs.Position < Const.fs.Length)
            {
                Const.fileCache += (char)br.ReadByte();
            }

            Console.WriteLine(fileCache);

            for (int i = 0; i < fileCache.Length; i++)
            {
                if (i == seperator)
                {
                    Const.multiplier = fileCache[i + 1];
                    Const.tmpCache = fileCache[i + 2];
                    Const.tmpResult = (int)multiplier * (int)tmpCache;
                    Console.WriteLine(tmpResult);
                }
            }

            Console.ReadKey();
        }
    }
}
