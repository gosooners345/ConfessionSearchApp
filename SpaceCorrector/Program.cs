using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUtil;

namespace SpaceCorrector
{
    class Program
    {
        static StreamWriter fileOut;
        static StreamReader fileIn;
        static string fileName = "",file,filerpt;

        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            Console.WriteLine("Type File Name Here:");
            file = Console.ReadLine();
            fileName = file + ".txt";
            Console.WriteLine("Opening Files");
            filerpt = file+"_new.txt";
            OpenFiles(); 
            FormatText();
            CloseFiles();
            Console.WriteLine("Formatting Finished");
        }
        static void FormatText()
        {
            string ignore = fileIn.ReadLine();
            fileOut.WriteLine(ignore);
            string sentence = "",wrapper="~";
            string[] words;
            while(!fileIn.EndOfStream)
            {
                try
                {
                    fileOut.WriteLine(FixSpaces(fileIn.ReadLine()));//.Replace("\r", "").Replace("\t"," ").Replace("\n",""));
                }
                catch(NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                  
                    CloseFiles();
                    
                }
              //  words = fileIn.ReadLine().Split('~');
              ////  foreach (string word in words)
              //  { sentence = words[0] + wrapper + words[1] + wrapper + words[2].Replace("   ", " ") + wrapper + words[3].Replace("   ", " ")
              //          + wrapper + words[4] + wrapper + words[5].Replace("   ", " ") 
              //          + words[6] + wrapper + words[7]; }
              //  fileOut.WriteLine(sentence);
            }
          
        }

       static string FixSpaces(string fileIn)
        {
            string newString="";
            if(fileIn.Contains("\r")||fileIn.Contains(" ")||fileIn.Contains("\n")||fileIn.Contains("  ")||fileIn.Contains("\t"))
            { Console.WriteLine("It has it");
                newString = fileIn.Replace("    ", " ").Replace("     "," ").Replace("   "," ").Replace("  "," "); }

            else {
                Console.WriteLine("it doesnt");
                newString = fileIn;
            }
            return newString;
        }

        static void OpenFiles()
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
        private static void CloseFiles()
        {
            fileIn.Close(); fileOut.Close();
        }
    }
}
