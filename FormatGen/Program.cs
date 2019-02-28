using System;
using System.IO;
using LibUtil;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatGen
{
    class Program
    {
        static StreamReader fileIn;
        static string fileName, filerpt, file;
        const string DOCTYPE = "CATECHISM";
        static string[] titleArray = new[] { "Ten Commandments","First Commandment",
            "Second Commandment","Third Commandment","Fourth Commandment","Fifth Commandment","Sixth Commandment",
        "Seventh Commandment","Eighth Commandment","Ninth Commandment", "Tenth Commandment", "Lord's Prayer", "Baptism",
            "Communion","Shortcoming of Man","Repentance","Wages of Sin","Church","Sacraments","Prayer","Adultery","Faith","Glorification","Sanctification};
        const string NEWFIELD = "~";
        static StreamWriter fileOut,file2;
        //   public enum questionVars { Who, Did, Wherein,What ,Are,How,Where,Which, Towhom };
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Type File Name Here:");
            file = Console.ReadLine();
            fileName = file + ".txt";
            Console.WriteLine("Opening Files");
            filerpt = "KCH_Part3.txt";
            OpenFiles();
            FormatText();
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
                fileOut = File.CreateText(filerpt); file2 = File.CreateText(file+"_DBIN.txt");
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
        private static void FormatText()
        {
            string[] words,pieces;
            string question = ""; string answer = "",textBlock="",sentence="";
            int chCt = 1,ch2=0;
            int questionID = 0; string title = "Baptist Catechism"; int docID = 10; string proofs = "";
            file2.WriteLine(String.Format("{0},{1}", title, docID));
            fileOut.WriteLine(title); char ch ='N';
            string chTitle = ""; string tags = "";
            textBlock = fileIn.ReadToEnd();
            words = textBlock.Split('~');
            foreach (string word in words)
            {
              
                sentence = ""; tags = "";
                pieces = word.Split('|');
                try
                {
                    questionID = Int32.Parse(pieces[1].Replace(".", ""));
                }
                catch (FormatException ex)
                {
                    questionID = Int32.Parse(pieces[1].Split('.')[0]);
                }
                catch (IndexOutOfRangeException ey)
                {
                    continue;
                }
                if (questionID < 95)
                    continue;
                    question = pieces[2];
                answer = pieces[4].Split('|')[0];
                try
                {
                    if (pieces.Length < 6 | pieces[5] == "" | pieces[5] == null)
                    {
                        proofs = "No Proofs Here";
                    }

                    else
                    { proofs = GetBetween(pieces[5], "(", ")"); }
                }
                catch (IndexOutOfRangeException ex)
                {
                    proofs=pieces[4].Split('|')[1];
                }
                Console.WriteLine("{0}", question);
                answer = answer.Trim('\t');
                Console.WriteLine(answer);
               
                if (proofs != "")
                    proofs = proofs.Replace(")", "").Replace(";","|").Trim(' ','\t');
                Console.WriteLine(proofs);
                ch = 'N';
               while (ch.ToString().ToUpper() != "Y")
                {
                    ch2 = 0;
                    Console.WriteLine("Title Array Display");
                    foreach (string titles in titleArray)
                    { Console.WriteLine("{0}. {1}", ch2, titles); ch2++; }
                    Console.WriteLine("Enter Chapter Title Value Here:");
                    try
                    {
                        chCt = Int32.Parse(Console.ReadLine());
                    }
                    catch (FormatException ex)
                    {
                            Console.WriteLine("Enter Chapter Title Value Here:");
                            chCt = Int32.Parse(Console.ReadLine());
                        
                       
                    }

                    //catch (FormatException e2)
                    //{
                    //    Console.WriteLine("Enter Chapter Title Value Here:");
                    //    chCt = Int32.Parse(Console.ReadLine());
                    //}
                    try
                    {
                        if (chCt < 19)
                            chTitle = titleArray[chCt];
                        else
                            chTitle = titleArray[chCt - 1];
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        Console.WriteLine("Enter Chapter Title Value Here:");
                        chCt = Int32.Parse(Console.ReadLine());
                    }
                    Console.WriteLine("Enter Relevant Tags here");
                    chCt = Int32.Parse(Console.ReadLine());
                    if (chCt < 19)
                        tags = titleArray[chCt];
                    else
                        tags = titleArray[chCt - 1];
                    
                    sentence = String.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}{1}{6}{1}{7}{1}{8}", DOCTYPE, NEWFIELD, questionID, question, answer, chTitle, proofs, title, tags);
                    Console.WriteLine(sentence);
                    Console.WriteLine("Does the above text look right to you?");
                    ch = Console.ReadKey().KeyChar;
                }
                    WriteFile(sentence, fileOut, file2);
                
                    //DOCTYPE+NEWFIELD+questionID+NEWFIELD+question+NEWFIELD+answer+NEWFIELD+chTitle+NEWFIELD+proofs
            }
            

        }
        private static void WriteFile(string text,params StreamWriter[] files)
        {
            foreach(StreamWriter file in files)
            {
                file.WriteLine(text);
            }
            Console.WriteLine();
        }
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start) + strEnd.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }

}
