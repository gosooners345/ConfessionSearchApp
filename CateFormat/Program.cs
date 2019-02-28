using System;
using System.Collections.Generic;
using System.Linq;
using LibUtil;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CateFormat
{
    class Program
    {
        static StreamReader fileIn;
        static string fileName, filerpt, file;
        static StreamWriter fileOut;
     //   public enum questionVars { Who, Did, Wherein,What ,Are,How,Where,Which, Towhom };
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Type File Name Here:");
            file = Console.ReadLine();
            fileName = file + ".txt";
            Console.WriteLine("Opening Files");
            filerpt = "KCH_Part2.txt";
            OpenFiles();
            ParseText();
            CloseFiles();
            Console.WriteLine("Finished");


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
        static void ParseText()
        {
            string textBlock = "",part="", sentence = "", trash = "", question = "", answer = "";
            string[] words, pieces;
            int questionID;
            textBlock = fileIn.ReadToEnd().Trim('\n');
            textBlock = textBlock.TrimStart('\n');
            words = textBlock.Split('~');                
                foreach (string word in words)
                {
                sentence = "";
                if (word.Contains('|'))
                {
                    pieces = word.Split('|');
                    foreach (string chunk in pieces)
                        if (!chunk.Contains(')'))
                            sentence += chunk + "|";
                        else if (chunk.Contains(Environment.NewLine) & !chunk.Contains("~"))
                        {

                            sentence += chunk.Trim('\r', '\n', ' ', ' ');
                            sentence = sentence.Replace(Environment.NewLine + Environment.NewLine, " ");
                        }
                     
                        else
                            sentence += chunk + "~\n";
                    sentence = sentence.Replace("\r\n   ", " ");
                    sentence = sentence.Trim(' ');
                    sentence = sentence.TrimEnd('\r', '\n');
                }

                //else
                //{
                //    pieces = word.Trim('\t','\r').Split('|');
                //    foreach (string chunk in pieces)
                //    {
                //        //string part = "";
                        
                //        if(chunk.Contains('Q'))
                //            {
                //            trash += chunk;
                //            part += chunk + "|";
                //        }
                //        if (Int32.TryParse(chunk, out questionID))
                //            { part += chunk+"|";
                //        }
                //        //if (chunk.Contains(""))
                //        //{
                //        //    continue;
                //        //}
                //        //if (chunk.Contains("\n"))
                //        //{


                //        //     if(chunk.Contains("Who")|chunk.Contains("Did")|
                //        //        chunk.Contains("Wherein")|chunk.Contains("What")
                //        //        |chunk.Contains("Are")|chunk.Contains("To whom")|
                //        //        chunk.Contains("Where")|chunk.Contains("Which")
                //        //        | chunk.Contains("How")) {
                //        //       sentence += chunk.Trim(' ', '\n');

                //        //    }
                //        //    if (sentence.Contains("Who") | sentence.Contains("Did") |
                //        //        sentence.Contains("Wherein") | sentence.Contains("What")
                //        //        | sentence.Contains("Are") | sentence.Contains("To whom") |
                //        //        sentence.Contains("Where") | sentence.Contains("Which")
                //        //        | sentence.Contains("How"))
                //        //    {
                //        //        sentence += chunk;

                //        //    }

                //        //    sentence += chunk;
                //        //}

                //        //if (!chunk.Contains("("))
                //        //  part += chunk + "|";
                //        //else
                //        //     part+= chunk + ")";
                //        if (!chunk.Contains(")"))
                //            part += chunk + "|";
                //        else
                //        {
                //            part += chunk + "~";

                //        }


                //    }
                //    if (!part.Contains("~"))
                //        sentence += part;
                //    else
                //        sentence += part+"\n";
                //}

              
                fileOut.WriteLine(sentence);
                //sentence += word + "|";
                Console.WriteLine(sentence);
                Console.WriteLine("completed");
                }
            Console.WriteLine("Program Completed");
                        
        }

    

    }
}
