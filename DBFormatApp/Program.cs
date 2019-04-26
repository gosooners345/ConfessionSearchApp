
using System;
using System.IO;
using LibUtil;



namespace DBFormatApp
{
    class DBFormatter
    {
        static StreamReader fileIn;
        static string fileName, filerpt,file;
        static StreamWriter fileOut;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Type File Name Here:");
            file = Console.ReadLine();
            fileName = file+".txt";
            
            Console.WriteLine("Opening Files");
            filerpt =  file+"_DBIN.txt";
            OpenFiles();
            ParseText();
            CloseText();
            Console.WriteLine("Finished");

        }
        private static void OpenFiles()
        {
            try
            {
                fileIn = File.OpenText(fileName);
                Console.WriteLine("{0} was opened", fileName);
            }
            catch
            {
                Console.WriteLine("Error: {0} does not exist\n", fileName);
                ConsoleApp.Exit();
            }
            try
            {
                fileOut = File.CreateText(filerpt);
                Console.WriteLine("{0} was created\n", filerpt);
            }
            catch
            {
                Console.WriteLine("Error: {0} could not be created\n", filerpt);
                ConsoleApp.Exit();
            }

        }
        private static void ParseText()
        {
            string textBlock = "",  word = "", sentence = "", assembly = "",question="",answer="",trash="";
            string[] words, pieces; Char ch;int docDetail, docID, QIDNumber;string text, title, proofs, tags;
            docDetail =688 ;
            pieces = fileIn.ReadLine().Split(',');
            docID = Int32.Parse(pieces[1]);
            //fileOut.WriteLine(pieces[0]);
            while (fileIn.Peek() != -1)
            {
                textBlock = fileIn.ReadLine();
                words = textBlock.Split('~');
                trash = words[0] + words[5];
                text = words[3];
                //trash = words[0] + words[6];
                //text ="Question:|"+ words[2] + "||" +"Answer:|"+ words[3];
                 sentence = (docDetail.ToString() + "~" + words[1] + "~" + words[2] + "~" +words[3]+ "~" + words[4] + "~"+words[6]);
                fileOut.WriteLine(sentence);
                docDetail++;
                #region WCF Formatting
                //assembly = "";
                //for (int i = 0; i < words.Length; i++)
                //{
                //    sentence = "";
                //    //pieces = words[i].Split(' ');
                   
                //    //foreach (string chunks in pieces)
                //    //{
                //    //    word = "";
                //    //    
                //    //    //switch (chunks)
                //    //    //{
                //    //    //case "GEN": word = "|GEN "; sentence += word; break;
                //    //    //case "EXO": word = "|EXO "; sentence += word; break;
                //    //    //case "LEV": word = "|LEV "; sentence += word; break;
                //    //    //case "NUM": word = "|NUMB "; sentence += word; break;
                //    //    //case "DEU": word = "|DEUT "; sentence += word; break;
                //    //    //case "JOS": word = "|JOSHUA "; sentence += word; break;
                //    //    //case "JDG": word = "|JUDGES "; sentence += word; break;
                //    //    //case "RUT": word = "|RUTH "; sentence += word; break;
                //    //    //case "1SA": word = "|1 SAM "; sentence += word; break;
                //    //    //case "2SA": word = "|2 SAM "; sentence += word; break;
                //    //    //case "1CH": word = "|1 CHR "; sentence += word; break;
                //    //    //case "2CH": word = "|2 CHR "; sentence += word; break;
                //    //    //case "1KI": word = "|1 KINGS "; sentence += word; break;
                //    //    //case "2KI": word = "|2 KINGS "; sentence += word; break;
                //    //    //case "NEH": word = "|NEH "; sentence += word; break;
                //    //    //case "EZR": word = "|EZRA "; sentence += word; break;
                //    //    //case "EST": word = "|ESTHER "; sentence += word; break;
                //    //    //case "JOB": word = "|JOB "; sentence += word; break;
                //    //    //case "PSA": word = "|PSALMS "; sentence += word; break;
                //    //    //case "|PSA": word = "|PSALMS "; sentence += word; break;
                //    //    //case "PRO": word = "|PROV "; sentence += word; break;
                //    //    //case "|PRO": word = "|PROV "; sentence += word; break;
                //    //    //case "ECC": word = "|ECC "; sentence += word; break;
                //    //    //case "SON": word = "|SONGS "; sentence += word; break;
                //    //    //case "ISA": word = "|ISA "; sentence += word; break;
                //    //    //case "JER": word = "|JER "; sentence += word; break;
                //    //    //case "LAM": word = "|LAM "; sentence += word; break;
                //    //    //case "EZE": word = "|EZE "; sentence += word; break;
                //    //    //case "DAN": word = "|DANIEL "; sentence += word; break;
                //    //    //case "HOS": word = "|HOSEA "; sentence += word; break;
                //    //    //case "JOE": word = "|JOEL"; sentence += word; break;
                //    //    //case "AMO": word = "|AMOS "; sentence += word; break;
                //    //    //case "OBA": word = "|OBA "; sentence += word; break;
                //    //    //case "JON": word = "|JONAH "; sentence += word; break;
                //    //    //case "MIC": word = "|MICAH "; sentence += word; break;
                //    //    //case "NAH": word = "|NAH "; sentence += word; break;
                //    //    //case "HAB": word = "|HAB "; sentence += word; break;
                //    //    //case "ZEP": word = "|ZEP "; sentence += word; break;
                //    //    //case "HAG": word = "|HAG "; sentence += word; break;
                //    //    //case "ZEC": word = "|ZEC "; sentence += word; break;
                //    //    //case "MAL": word = "|MAL "; sentence += word; break;
                //    //    //case "MAT": word = "|MATT "; sentence += word; break;
                //    //    //case "MAR": word = "|MARK "; sentence += word; break;
                //    //    //case "LUK": word = "|LUKE "; sentence += word; break;
                //    //    //case "|LUK": word = "|LUKE "; sentence += word; break;
                //    //    //case "JOH": word = "|JOHN "; sentence += word; break;
                //    //    //case "|JOH": word = "|JOHN "; sentence += word; break;
                //    //    //case "ACT": word = "|ACTS "; sentence += word; break;
                //    //    //case "ROM": word = "|ROMANS "; sentence += word; break;
                //    //    //case "|ROM": word = "|ROMANS "; sentence += word; break;
                //    //    //case "1CO": word = "|1 COR "; sentence += word; break;
                //    //    //case "|1CO": word = "|1 COR "; sentence += word; break;
                //    //    //case "2CO": word = "|2 COR "; sentence += word; break;
                //    //    //case "|2CO": word = "|2 COR "; sentence += word; break;
                //    //    //case "GAL": word = "|GAL "; sentence += word; break;
                //    //    //case "EPH": word = "|EPH "; sentence += word; break;
                //    //    //case "PHI": word = "|PHIL "; sentence += word; break;
                //    //    //case "COL": word = "|COL "; sentence += word; break;
                //    //    //case "1TH": word = "|1 THESS "; sentence += word; break;
                //    //    //case "|1TH": word = "|1 THESS "; sentence += word; break;
                //    //    //case "2TH": word = "|2 THESS "; sentence += word; break;
                //    //    //case "|2TH": word = "|2 THESS "; sentence += word; break;
                //    //    //case "1TI": word = "|1 TIM "; sentence += word; break;
                //    //    //case "|1TI": word = "|1 TIM "; sentence += word; break;
                //    //    //case "2TI": word = "|2 TIM "; sentence += word; break;
                //    //    //case "|2TI": word = "|2 TIM "; sentence += word; break;
                //    //    //case "TIT": word = "|TITUS "; sentence += word; break;
                //    //    //case "PHM": word = "|PHM "; sentence += word; break;
                //    //    //case "HEB": word = "|HEB "; sentence += word; break;
                //    //    //case "JAM": word = "|JAMES "; sentence += word; break;
                //    //    //case "1PE": word = "|1 PETER "; sentence += word; break;
                //    //    //case "|1PE": word = "|1 PETER "; sentence += word; break;
                //    //    //case "2PE": word = "|2 PETER "; sentence += word; break;
                //    //    //case "|2PE": word = "|2 PETER "; sentence += word; break;
                //    //    //case "1JO": word = "|1 JOHN "; sentence += word; break;
                //    //    //case "|1JO": word = "|1 JOHN "; sentence += word; break;
                //    //    //case "2JO": word = "|2 JOHN"; sentence += word; break;
                //    //    //case "|2JO": word = "|2 JOHN"; sentence += word; break;
                //    //    //case "3JO": word = "|3 JOHN "; sentence += word; break;
                //    //    //case "|3JO": word = "|3 JOHN "; sentence += word; break;
                //    //    //case "JUD": word = "|JUDE "; sentence += word; break;
                //    //    //case "REV": word = "|REVELATION "; sentence += word; break;
                //    //    //case "|REV": word = "|REVELATION "; sentence += word; break;
                //    //    //default: word = chunks + " "; sentence += word; break;
                //    //    // } 
                //    //    
                //    //}
                //    
                //    if (i < words.Length)
                //        sentence += "~";

                //    // fileOut.Write(sentence);
                //    assembly += sentence;


                //}
                #endregion
               // fileOut.WriteLine(assembly);
            }
        }
        private static void CloseText()
        {
            fileIn.Close(); fileOut.Close();
        }

    }
}
