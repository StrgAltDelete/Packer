﻿using System;
using System.IO;
using System.Windows;
using System.Threading;

namespace PackerWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Microsoft.Win32.OpenFileDialog fileDialog;
        string s_filePath;
        string s_fileName;
        public MainWindow()
        {
            InitializeComponent();
            btn_encode.IsEnabled = false;   //Deaktiviert weil sonst null als Pfad übergeben werden könnte
            btn_decode.IsEnabled = false;
            btn_encode.Visibility = Visibility.Hidden;
            btn_decode.Visibility = Visibility.Hidden;
        }

        private void btn_files_Click(object sender, RoutedEventArgs e)
        {
            txt_messages.Text = null;
            fileDialog = new Microsoft.Win32.OpenFileDialog();
            bool b_exist = (bool)fileDialog.ShowDialog();
            if (b_exist == true)
            {
                btn_encode.IsEnabled = true;
                btn_decode.IsEnabled = true;
                btn_encode.Visibility = Visibility.Visible;
                btn_decode.Visibility = Visibility.Visible;
                s_filePath = fileDialog.FileName;
                s_fileName = "";    //Sonst stackt sich der Name wenn man mehrere Dateikonvertierungen macht
                for (int i = fileDialog.FileName.Length - 1; i > 0; i--)
                {
                    if(fileDialog.FileName[i] == '\\')
                    {
                        break;
                    }
                    s_fileName += fileDialog.FileName[i];
                }
                char[] ac_fileName = s_fileName.ToCharArray();
                Array.Reverse(ac_fileName);
                s_fileName = new string(ac_fileName);
                txt_name.Text = "Ausgewählte Datei:\n" + s_fileName;
            }
        }
        private void btn_encode_Click(object sender, RoutedEventArgs e)
        {
            txt_messages.Text = null;
            string s_file = s_fileName;
            for (int i = s_file.Length - 1; i > 0; i--)
            {
                if(s_file[i] == '.')
                {
                    s_file = s_file.Remove(i);  //Name ohne Dateiendung
                    break;
                }
            }
            if (s_fileName.Length > 8)  //Überprüft ob der Name gekürtzt werden muss 
            {
                s_file = s_fileName.Remove(8);
                s_file += '~';
            }
            Encoder encoder = new Encoder(s_filePath, s_file);
            if (encoder.Exist() == true)    //Wenn die .fun-Zieldatei noch nicht exestiert wird eine erstellt
            {
                encoder.Encoding();
                txt_messages.Foreground = System.Windows.Media.Brushes.DarkGreen;
                txt_messages.Text = "Datei erfolgreich komprimiert";
            }
            else
            {
                txt_messages.Foreground = System.Windows.Media.Brushes.DarkRed;
                txt_messages.Text = "Datei existiert bereits";
            }
        }

        private void btn_decode_Click(object sender, RoutedEventArgs e)
        {
            txt_messages.Text = null;
            Decoder decoder = new Decoder(s_filePath);
            decoder.CallMethods(ref txt_messages);
        }
    }
    public class Encoder
    {
        string s_prefix = "MOIN";                                                               
        string s_Path;  //Dateiname mit Pfad
        string s_name;  //Dateiname (gekürtzt)
        string s_ending;    //Dateiendung
        string s_fun;  //Dateinname für.fun
        byte b_sign;    //Trennzeichen
        byte[] ab_signs = new byte[256];    //Array zur Ermittlung des besten Trennzeichens
        FileStream fs_Write;                                                                    
        FileStream fs_Read;                                                                     
        BinaryReader br;
        BinaryWriter bw;

        public Encoder(string s_originalName, string s_name)                   
        {
            this.s_Path = s_originalName;
            this.s_name = s_name;
            Name(s_name);
        }
        private void Name(string s_name)                                                    
        {
            s_name += ".fun";   //Gekürtzer Name + neue Dateiendung
            this.s_fun = s_name;
        }
        public bool Exist()
        {
            if (!File.Exists(s_fun))   //Überprüft ob die neue .fun Datei bereits exestiert
                return true;                                                                    
            else
                return false;                                                                   
        }
        public void Encoding()  //Ruft alle notwendigen Methoden zum encoden auf
        {
            this.b_sign = FindSign();
            Prefix();
            Algorithm();
            Done();
        }
        private void Prefix()   //Ruft alle notwendigen Methoden zum setzen des Headers auf
        {
            MagicNumber();
            Seperator();
            Name();
        }
        private void MagicNumber()                                                          
        {
            fs_Write = new FileStream(s_fun, FileMode.Create);
            bw = new BinaryWriter(fs_Write);
            foreach (byte b in s_prefix)    //Setzt unsere MagicLine
                bw.Write(b);
        }
        private void Name() //Schreibt den Dateinamen in den Header
        {
            for (int i = s_Path.Length - 1; i > 0; i--)
            {
                if(s_Path[i] == '.')
                {
                    for (int j = i; j < s_Path.Length; j++)
                    {
                        s_ending += s_Path[j];
                    }
                    if(s_ending.Length > 5) //Dateiendung wird auf 4 Zeichen gekürzt, 5 weil der Punkt dazuzählt
                    {
                        this.s_ending = s_ending.Remove(5);
                        s_ending += '~';
                    }
                    break;
                }
            }
            foreach (byte b in s_name)
                bw.Write(b);
            foreach (byte b in s_ending)
                bw.Write(b);
            bw.Write((byte)'[');    //Identikator für das Ende des Namens der originellen Datei
        }
        private void Seperator()    //Schreibt das Trennzeichen in den Header
        {
            bw.Write(b_sign);
        }
        private void Done() //Schliesst und leer alle offenen Streams
        {
            bw.Flush();
            bw.Close();
            br.Close();
            fs_Write.Close();
            fs_Read.Close();
        }
        private byte FindSign() //Sucht das beste Trennzeichen für die Datei
        {
            fs_Read = new FileStream(s_Path, FileMode.Open);
            br = new BinaryReader(fs_Read);
            while (fs_Read.Position < fs_Read.Length)
            {
                ab_signs[br.ReadByte()]++;
            }
            byte b_bestPos = 0;
            byte b_bestValue = 255;
            for (byte b = 1; b < ab_signs.Length; b++)  //Weil Byte 0 (also NULL) nicht funtioniert
            {
                if (ab_signs[b] == 0)   //wenn das Zeichen nicht von der Datei genutzt wird wird es gewählt
                    return b;
                else if (ab_signs[b] < b_bestValue) //Sucht das Zeichen, welches am wenigsten genutzt wird 
                {
                    b_bestValue = ab_signs[b];
                    b_bestPos = b;
                }
                if (b == 255)   //Da es sonst bei 0 wieder zu zählen anfängt
                {
                    break;
                }
            }
            return ab_signs[b_bestPos];
        }
        private void Algorithm()
        {
            byte b_sameSigns = 1;   //Startet bei 1, da der Ausgangspunkt nicht mitgezählt wird
            long l_length = fs_Read.Length;
            for (long l = 0; l < l_length; l++)
            {
                fs_Read.Position = l;
                if (l != l_length - 1)  //Wenn es nicht das letzte Zeichen ist
                {
                    if ((b_sameSigns >= 4 && br.ReadByte() != br.ReadByte()) || b_sameSigns == 255) //Wenn das nächste Byte nicht mehr das selbe ist
                    {
                        fs_Read.Position = l;   //Muss durch die Bedingung der if zurückgestzt werden
                        bw.Write(b_sign);
                        bw.Write(b_sameSigns);
                        bw.Write(br.ReadByte());
                        b_sameSigns = 1;
                        continue;   //Damit es nicht in die folgende if-Anweisung geht
                    }
                    fs_Read.Position = l;
                    if (br.ReadByte() == b_sign)    //Wenn es das Trennzeichen ist
                    {
                        bw.Write(b_sign);
                        bw.Write(b_sameSigns);
                        bw.Write(b_sign);
                        continue;
                    }
                    fs_Read.Position = l;
                    if (br.ReadByte() == br.ReadByte())
                    {
                        b_sameSigns++;
                    }
                    else if (b_sameSigns > 1)   //Falls es mehrere Zeichen einer Art sind, aber nicht genug zum packen
                    {
                        fs_Read.Position = l;
                        byte b = br.ReadByte();
                        for (byte i2 = b_sameSigns; i2 > 0; i2--)
                        {
                            bw.Write(b);
                        }
                        b_sameSigns = 1;
                    }
                    else   //Falls es nur 1 Zeichen ist
                    {
                        fs_Read.Position = l;
                        bw.Write(br.ReadByte());
                        b_sameSigns = 1;
                    }
                }
                else if (b_sameSigns >= 4)  //Falls es die letzte Stelle ist
                {
                    bw.Write(b_sign);
                    bw.Write(b_sameSigns);
                    bw.Write(br.ReadByte());
                }
                else
                {
                    byte b = br.ReadByte();
                    for (byte b_num = b_sameSigns; b_num > 0; b_num--)
                    {
                        bw.Write(b);
                    }
                }
            }
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
        bool b_PrefixValid;
        FileStream fs_read;
        FileStream fs_write;
        BinaryReader br;
        BinaryWriter bw;

        public Decoder(string filename)    //Konstruktor
        {
            this.filename = filename;
        }

        public void CallMethods(ref System.Windows.Controls.TextBlock txt_messages)
        {
            GetPrefix();
            if(File.Exists(filenameOut)) // Popup FileAlreadyExists
            {
                txt_messages.Foreground = System.Windows.Media.Brushes.DarkRed;
                txt_messages.Text = "Datei existiert bereits";
            }
            if(b_PrefixValid == true)
            {
                Algorithm(ref txt_messages);
            }
            else //To do: Popup
            {
                txt_messages.Foreground = System.Windows.Media.Brushes.DarkRed;
                txt_messages.Text = "Keine gültige Datei";
            }
        }

        public void Algorithm(ref System.Windows.Controls.TextBlock txt_messages)
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
            txt_messages.Foreground = System.Windows.Media.Brushes.DarkGreen;
            txt_messages.Text = "Datei erfolgreich dekomprimiert";
            fs_read.Flush();
            bw.Flush();
            fs_read.Close();
            bw.Close();
            br.Close();
        }

        public void GetPrefix()         //Methode ermittelt die magicNumber, seperator, ending und die contentStartPos Variablen
        {
            fs_read = new FileStream(filename, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs_read);
            for (int i = 0; i < 4; i++)  //Die ersten vier Zeichen der Datei werden ausgelesen und als Magic Number gespeichert
            {
                magicNumber += (char)br.ReadByte();
            }

            seperator = br.ReadByte(); //Trennzeichen ist immer an der 4. Stelle

            string s_ending = "";
            for (int i = filename.Length - 3; i < filename.Length; i++)
            {
                s_ending += filename[i];
            }

            if (magicNumber == "MOIN" && s_ending == "fun")   //Es wird überprüft, ob alle Elemente im Prefix passen
            {
                b_PrefixValid = true;
            }
            else
            {
                b_PrefixValid = false;
            }

            if (b_PrefixValid == true)
            {
                byte tmpname = 0;
                while (br.ReadByte() != '[')    //Bis das Zeichen erkannt wird, welches das Ende des Headers markiert, wird der Dateiname mit Endung abgespeichert
                {
                    fs_read.Position--;
                    tmpname = br.ReadByte();
                    originalfilename += (char)tmpname;  //Der Originaldateiname wird gespeichert
                }
                contentStartPos = (int)fs_read.Position;
            }

            filenameOut = originalfilename; //Filename wird auf das Original gesetzt

            fs_read.Flush();
            fs_read.Close();
            br.Close();
        }
    }
}
