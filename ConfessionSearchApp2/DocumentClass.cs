using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGList;
using LibGICloneable;
using LibUtil;
using Android.App;
using System.Diagnostics;
using Android.Content;
using Java.IO;
using Android.OS;
using Android.Util;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using System.Text.RegularExpressions;


namespace ConfessionSearchApp2
{
  public class Document : IComparable<Document>, ICloneable<Document>
  {
    protected int idNumber, matches;
   protected string title, proofs, type,docTitle,tags;
    private Document document;

    public Document()
    {
      this.idNumber = 0; this.title = ""; this.proofs = ""; this.type = ""; this.matches = 0;this.tags = "";
      this.docTitle = "";
      }
    public Document(string titleValue, int idValue, string proofValue, string typeValue)
    {
      this.idNumber = idValue; this.title = titleValue; this.Proofs = proofValue; this.type = typeValue;
    }
    public string Truncate(string value, int maxLength)
    {
      return value?.Substring(0, Math.Min(value.Length, maxLength));
    }
    public Document(Document sourceDocument)
    {
      this.document = sourceDocument;
    }

    public int IDNumber
    {
      get
      {
        return idNumber;
      }
      set
      {
        if (value >= 0)
          idNumber = value;
      }
    }
    public int Matches
    {
      get { return this.matches; }
      set { this.matches = value; }
    }
    public string Tags{ get{ return this.tags; }set { this.tags = value; } }
    
    public String Proofs { get { return this.proofs; } set { this.proofs = Formatter(value); } }
    public String Title { get { return this.title; } set { title = value; } }
    public String DocTitle { get{ return this.docTitle; }set { docTitle = value; } }
    public static Document Parse(String fileName,bool answers, bool proofs)
    {
      string[] words;
      Document document = new Document();
      words = fileName.Split('~');
      document.Type = words[0];
      switch (words[0].ToUpper())
      {
        case "CONFESSION": document = Confession.Parse(fileName,proofs); break;
        case "CATECHISM": document = Catechism.Parse(fileName,answers,proofs); break;
        case "CREED": document = Creed.Parse(fileName); break;
        default:
          ProcessError(String.Format("{0} is an invalid " + "Document type value for the Document " + "class Parse method", words[0]));
          break;
      }
      return document;
    }
   
    public string Type  
    {
      get
      {
        return this.type;
      }
      set
      {
        string toUpperValue = value.ToUpper();
        if (toUpperValue == "CATECHISM" || toUpperValue == "CONFESSION" || toUpperValue == "CREED")
          this.type = value;
        else
          ProcessError(String.Format("{0} can not be assigned to an Type property", value));
      }
    }
    protected static void ProcessError(string message)
    {

      Log.Error("Error", message);
      
    }
    public int CompareTo(Document compareDocument)
    {
      return this.IDNumber.CompareTo(compareDocument.IDNumber);
    }
    public static int CompareTitles(Document document1, Document document2)
    {
      string string1, string2;
      string1 = document1.title + document1.IDNumber.ToString("d3");
      string2 = document2.title + document2.IDNumber.ToString("d3");
      return string1.CompareTo(string2);

    }
    
    public Document Clone()
    {
      Document document = null;
      if (this is Confession)
        document = ((Confession)this).Clone();
      else if (this is Catechism)
        document = ((Catechism)this).Clone();
      return document;
    }
    public void Copy(Document sourceDocument) // Copy method
    {
      this.type = sourceDocument.type;
      this.IDNumber = sourceDocument.IDNumber;
      this.Proofs = sourceDocument.Proofs;
      this.title = sourceDocument.title;
      this.DocTitle = sourceDocument.DocTitle;
    }
    public String Formatter(string stringField)
    {
      int x = 0;
      string formatter = "";
      String[] words;
      words = stringField.Split('|');
      for (int i = 0; i <= words.Length - 1; i++, x++)
      {
        formatter += words[i];
        formatter += "\r\n";

      }

      stringField = formatter;
      return stringField;
    }
    public static int CompareMatches(Document document1, Document document2)
    {
      string string1, string2;
      if (document1.Matches >= document2.Matches)
        return 1;
      if (document1.Matches <= document2.Matches)
        return -1;
        else
        {
        string1 = document1.IDNumber.ToString() + document1.docTitle;
        string2 = document2.IDNumber.ToString() + document2.docTitle;
        return string1.CompareTo(string2);
        }
    }
    public string getBetween(string strSource, string strStart)
    {
      int Start, End;
      if (strSource.Contains(strStart))//&& strSource.Contains(strEnd))
      {
        Start = 0;
        End = strSource.IndexOf(strStart) + strStart.Length;
        strSource = strSource.Substring(Start, End - Start)+"...";
      }

      return strSource;
    }
  }
  public class Catechism : Document
  {
    string question, answer;
    public Catechism() : base() { this.question = ""; this.answer = ""; }
    public Catechism(string questionVal, string answerVal, int idValue, string titleValue, string proofValue, string typeValue) : base(titleValue, idValue, proofValue, typeValue) { this.Question = questionVal; this.Answer = answerVal; }
    public Catechism(Catechism sourceCatechism) : base(sourceCatechism) { this.Copy(sourceCatechism); }
    public void Copy(Catechism catechisms) { this.question = catechisms.question; this.answer = catechisms.answer; }
    public new Catechism Clone() { return new Catechism(this); }
    public string Question { get { return this.question; } set { question = Formatter(value); } }
    public string Answer { get { return this.answer; } set { answer = Formatter(value); } }
    public static new Catechism Parse(string stringValue, bool answers, bool proofs)
    {
      string[] words;
      Catechism catechism = new Catechism();
      words = stringValue.Split('~');
      catechism.Type = words[0];
      catechism.IDNumber = Int32.Parse(words[1]);
      catechism.question = words[2];
      if (answers == true)
        catechism.answer = words[3];
      else
        catechism.answer = "";
      catechism.Title = words[4];
      if (proofs == true)
        catechism.Proofs = words[5];
      else
        catechism.Proofs = "";
      catechism.DocTitle = words[6];
      catechism.Tags = words[7];
      return catechism;
    }

    public string this[string propertyName]
    {
      get
      {
        string returnValue = "";
        switch (propertyName.ToUpper())
        {
          case "IDNUM": returnValue = this.IDNumber.ToString("d2"); break;
          case "TITLE": returnValue = this.Title; break;
          case "QUESTION": returnValue = this.question; break;
          case "ANSWER": returnValue = this.answer; break;
          case "DOCTITLE": returnValue = this.DocTitle; break;
          case "PROOFS": returnValue = this.Proofs; break;
          case "TAGS": returnValue = this.Tags; break;
          default: ProcessError("Property Name not valid"); break;
        }
        return returnValue;
      }
    }
    public string this[int propertyIndex]
    {
      get
      {
        string returnValue = "";
        switch (propertyIndex)
        {
          case 0: returnValue = this.IDNumber.ToString("d2"); break;
          case 1: returnValue = this.DocTitle; break;
          case 2: returnValue = this.Title; break;
          case 3: returnValue = this.Question; break;
          case 4: returnValue = this.Answer; break;
          case 5: returnValue = this.Proofs; break;
          case 6: returnValue = this.Tags; break;
          default: ProcessError("Index was outside of range "); break;
        }
        return returnValue;
      }
      set
      {
      switch(propertyIndex)
      {
          case 2:this.Title = value;break;
          case 3:this.Question = value;break;
          case 4:this.Answer = value;break;
          case 5:this.Proofs = value;break;
          case 6:this.Tags = value; break;
          default:ProcessError("Index was out of range");break;
          }
      }
    }
    public void Search(string term, bool truncate)
    {
      Log.Info("Filter", "Filtering Catechism Results");
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      Log.Debug("Timer", "Timer Started");
      Regex regex = new Regex(term, RegexOptions.IgnoreCase);
      string[] words;
      for (int i = 2; i <= 6; i++)
      {
        #region Fastest, inaccurate
      
          //if (regex.IsMatch(this[i], 0))
          //  this.Matches++;
          #endregion
        #region Best balanced
            words = this[i].Split(' ');
            foreach (string word in words)
            {
              if (regex.IsMatch(word))
                this.Matches++;
            }
        #endregion
        #region Most accurate search results
          //foreach (string word in this[i].Split(' '))
          //{
          //  if (regex.IsMatch(word))
          //    this.Matches++;
          //}
        #endregion
        if (truncate)
          this[i] = this.getBetween(this[i], term);
      }
      stopwatch.Stop();
      Log.Debug("Timer", String.Format("{0}ms passed", stopwatch.ElapsedMilliseconds.ToString()));
      stopwatch.Reset();
    }
  }

    public class Confession : Document
    {
      string chapter;
      public Confession() : base() { this.chapter = ""; }
      public Confession(string chString, int idValue, string titleValue, string proofValue, string typeValue) : base(titleValue, idValue, proofValue, typeValue)
      { this.Chapter = chString; }
      public string Chapter { get { return this.chapter; } set { this.chapter = Formatter(value); } }
     
      public string getBetween(string strSource, string strStart, string strEnd)
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
      public new Confession Clone() { return new Confession(this); }
      public Confession(Confession sourceConfession) { this.Copy(sourceConfession); }
      public void Copy(Confession sourceConfession) { this.chapter = sourceConfession.chapter; }
      public static new Confession Parse(string stringValue, bool proofs)
      {
        string[] words;
        Confession confession = new Confession();
        words = stringValue.Split('~');
        confession.Type = words[0];
        confession.IDNumber = Int32.Parse(words[1]);
        confession.Title = words[2];
        confession.Chapter = words[3];
        if (proofs)
          confession.Proofs = words[4];
        else
          confession.Proofs = "";
        confession.DocTitle = words[5];
        confession.Tags = words[6];
        return confession;
      }
      public string this[string propertyName]
      {
        get
        {
          string returnValue = "";
          switch (propertyName.ToUpper())
          {
            case "IDNUM": returnValue = this.IDNumber.ToString("d2"); break;
            case "TITLE": returnValue = this.Title; break;
            case "CHAPTER": returnValue = this.Chapter; break;
            case "DOCTITLE": returnValue = this.DocTitle; break;
            case "PROOFS": returnValue = this.Proofs; break;
            case "TAGS": returnValue = this.Tags; break;
            default: ProcessError("Property Name not valid"); break;
          }
          return returnValue;
        }
      }
      public string this[int propertyIndex]
      {
        get
        {
          string returnValue = "";
          switch (propertyIndex)
          {
            case 0: returnValue = this.IDNumber.ToString("d2"); break;
            case 1: returnValue = this.DocTitle; break;
            case 2: returnValue = this.Title; break;
            case 3: returnValue = this.Chapter; break;
            case 4: returnValue = this.Proofs; break;
            case 5: returnValue = this.Tags; break;
            default: ProcessError("Index was outside of range "); break;
          }
          return returnValue;
        }
        set
        {
        switch (propertyIndex)
        {
          case 2: this.Title = value; break;
          case 3: this.Chapter = value; break;
          case 4: this.Proofs = value; break;
          case 5: this.Tags = value; break;
          default: ProcessError("Index was out of range"); break;
        }
      }
      }
      public void Search(string term, bool truncate)
      {
      Log.Info("Filter", "Filtering Catechism Results");
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      Log.Debug("Timer", "Timer Started");
      Regex regex = new Regex(term, RegexOptions.IgnoreCase);
      //LibGList<string> verify = new LibGList<string>(),resultList=new LibGList<string>();
      List<string> verify = new List<string>(), resultList = new List<string>();
     // string[] words;
      for (int i = 2; i <= 5; i++)
      {
        foreach (string word in this[i].Split(' '))
        {
          if (regex.IsMatch(word))
            this.Matches++;
        }
        if (truncate)
          this[i] = this.getBetween(this[i], term);
      }
      stopwatch.Stop();
      Log.Debug("Timer", String.Format("{0}ms passed", stopwatch.ElapsedMilliseconds.ToString()));
      stopwatch.Reset();
    }
    }
  public class Creed : Document
  {
    string creedText;
    public Creed() { this.creedText = ""; }
    public Creed(string creedString, int idValue, string titleValue, string proofValue, string typeValue) : base(titleValue, idValue, proofValue, typeValue) { this.CreedText = creedString; }
    public Creed(Creed sourceCreed) { this.Copy(sourceCreed); }
    public new Creed Clone() { return new Creed(this); }
    public void Copy(Creed sourceCreed) { this.CreedText = sourceCreed.CreedText; }
    public String CreedText { get { return this.creedText; } set { creedText = Formatter(value); } }
    public static new Creed Parse(string stringField)
    {
      String[] words;
      Creed creed = new Creed();
      words = stringField.Split('~');
      creed.Type = words[0];
      creed.CreedText = words[1];
      return creed;
    }
  
  }
  public class DocumentList : LibGList<Document>
  {
    private string title; private bool truncate;
    public enum OrderEnum { IDOrder, TitleOrder,MatchOrder };
    public DocumentList()
    {
      this.title = "";
    }
    private DocumentList(int cap) {  }
    private DocumentList(DocumentList DocumentList) { this.Copy(DocumentList); }
    public void Copy(DocumentList sourceDocument) { this.Clear(); foreach (Document document in sourceDocument) this.Add(document.Clone()); }
    public bool Truncate{
    get
    { return this.truncate; }
    set
    { this.truncate = value; }

    }

    public string Title { get { return this.title; } set { this.title = value; } }
    public void Fill(string fileName,AssetManager asset,int ID,bool answer,bool proofs)
    {
    
      try
      {
        if (ID == 0)
          this.Clear();
        StreamReader fileIn = new StreamReader(asset.Open(fileName));

        if (ID == 0)
          Title = fileIn.ReadLine();
        else
          Title = "Results";
          while (fileIn.Peek() != -1)
            this.Add(Document.Parse(fileIn.ReadLine(),answer,proofs));
          fileIn.Close();
          MergeSort();
        
      }
     
      catch
      {
        this.Add(Document.Parse("CATECHISM~0~null~null~null~null",answer,proofs));
        this.Add(Document.Parse("CATECHISM~1~What is the chief end of man?~Man's chief end is to glorify God,[1] and to enjoy him forever.[2]~Mankind's Ultimate Purpose in life~[1] |Psalm 86. Bow down thine ear, O LORD, hear me: for I am poor and needy. Preserve my soul; for I am holy: O thou my God, save thy servant that trusteth in thee. Be merciful unto me, O Lord: for I cry unto thee daily. Rejoice the soul of thy servant: for unto thee, O Lord, do I lift up my soul. For thou, Lord, art good, and ready to forgive; and plenteous in mercy unto all them that call upon thee. Give ear, O LORD, unto my prayer; and attend to the voice of my supplications. In the day of my trouble I will call upon thee: for thou wilt answer me. Among the gods there is none like unto thee, O Lord; neither are there any works like unto thy works. All nations whom thou hast made shall come and worship before thee, O Lord; and shall glorify thy name. For thou art great, and doest wondrous things: thou art God alone. Teach me thy way, O LORD; I will walk in thy truth: unite my heart to fear thy name. I will praise thee, O Lord my God, with all my heart: and I will glorify thy name for evermore. For great is thy mercy toward me: and thou hast delivered my soul from the lowest hell. O God, the proud are risen against me, and the assemblies of violent men have sought after my soul; and have not set thee before them. But thou, O Lord, art a God full of compassion, and gracious, longsuffering, and plenteous in mercy and truth. O turn unto me, and have mercy upon me; give thy strength unto thy servant, and save the son of thine handmaid. Show me a token for good; that they which hate me may see it, and be ashamed: because thou, LORD, hast holpen me, and comforted me. |Isaiah 60:21. Thy people also shall be all righteous: they shall inherit the land for ever, the branch of my planting, the work of my hands, that I may be glorified. |Romans 11:36. For of him, and through him, and to him, are all things: to whom be glory for ever. Amen. |1 Corinthians 6:20, 10:31. For ye are bought with a price: therefore glorify God in your body, and in your spirit, which are God's.... Whether therefore ye eat, or drink, or whatsoever ye do, do all to the glory of God. |Revelation 4:11. Thou art worthy, O Lord, to receive glory and honour and power: for thou hast created all things, and for thy pleasure they are and were created.|[2] |Psalm 16:5-11. The LORD is the portion of mine inheritance and of my cup: thou maintainest my lot. The lines are fallen unto me in pleasant places; yea, I have a goodly heritage. I will bless the LORD, who hath given me counsel: my reins also instruct me in the night seasons. I have set the LORD always before me: because he is at my right hand, I shall not be moved. Therefore my heart is glad, and my glory rejoiceth: my flesh also shall rest in hope. For thou wilt not leave my soul in hell; neither wilt thou suffer thine Holy One to see corruption. Thou wilt show me the path of life: in thy presence is fulness of joy; at thy right hand there are pleasures for evermore. |Psalm 144:15. Happy is that people, that is in such a case: yea, happy is that people, whose God is the LORD. |Isaiah 12:2. Behold, God is my salvation; I will trust, and not be afraid: for the LORD JEHOVAH is my strength and my song; he also is become my salvation. |Luke 2:10. And the angel said unto them, Fear not: for, behold, I bring you good tidings of great joy, which shall be to all people. |Philippians 4:4. Rejoice in the Lord alway: and again I say, Rejoice. |Revelation 21:3-4. And I heard a great voice out of heaven saying, Behold, the tabernacle of God is with men, and he will dwell with them, and they shall be his people, and God himself shall be with them, and be their God. And God shall wipe away all tears from their eyes; and there shall be no more death, neither sorrow, nor crying, neither shall there be any more pain: for the former things are passed away.",answer,proofs));
        this.Add(Document.Parse("CATECHISM~2~What rule hath God given to direct us how we may glorify and enjoy him?~The Word of God, which is contained in the Scriptures of the Old and New Testaments,[3] is the only rule to direct us how we may glorify and enjoy him.[4]~The Authority of Scripture~[3] |Matthew 19:4-5. And he answered and said unto them, Have ye not read, that he which made them at the beginning made them male and female, And said, For this cause shall a man leave father and mother, and shall cleave to his wife: and they twain shall be one flesh? With |Genesis 2:24. Therefore shall a man leave his father and his mother, and shall cleave unto his wife: and they shall be one flesh. |Luke 24:27, 44. And beginning at Moses and all the prophets, he expounded unto them in all the scriptures the things concerning himself.... And he said unto them, These are the words which I spake unto you, while I was yet with you, that all things must be fulfilled, which were written in the law of Moses, and in the prophets, and in the psalms, concerning me. |1 Corinthians 2:13. Which things also we speak, not in the words which man's wisdom teacheth, but which the Holy Ghost teacheth; comparing spiritual things with spiritual. |1 Corinthians 14:37. If any man think himself to be a prophet, or spiritual, let him acknowledge that the things that I write unto you are the commandments of the Lord. 2 Peter 1:20-21. Knowing this first, that no prophecy of the scripture is of any private interpretation. For the prophecy came not in old time by the will of man: but holy men of God spake as they were moved by the Holy Ghost. 2 Peter 3:2, 15-16. That ye may be mindful of the words which were spoken before by the holy prophets, and of the commandment of us the apostles of the Lord and Saviour.... And account that the longsuffering of our Lord is salvation; even as our beloved brother Paul also according to the wisdom given unto him hath written unto you; As also in all his epistles, speaking in them of these things; in which are some things hard to be understood, which they that are unlearned and unstable wrest, as they do also the other scriptures, unto their own destruction.|[4] |Deuteronomy 4:2. Ye shall not add unto the word which I command you, neither shall ye diminish ought from it, that ye may keep the commandments of the LORD your God which I command you. |Psalm 19:7-11. The law of the LORD is perfect, converting the soul: the testimony of the LORD is sure, making wise the simple. The statutes of the LORD are right, rejoicing the heart: the commandment of the LORD is pure, enlightening the eyes. The fear of the LORD is clean, enduring for ever: the judgments of the LORD are true and righteous altogether. More to be desired are they than gold, yea, than much fine gold: sweeter also than honey and the honeycomb. Moreover by them is thy servant warned: and in keeping of them there is great reward. |Isaiah 8:20. To the law and to the testimony: if they speak not according to this word, it is because there is no light in them. |John 15:11. These things have I spoken unto you, that my joy might remain in you, and that your joy might be full. |John 20:30-31. And many other signs truly did Jesus in the presence of his disciples, which are not written in this book: But these are written, that ye might believe that Jesus is the Christ, the Son of God; and that believing ye might have life through his name. |Acts 17:11. These were more noble than those in Thessalonica, in that they received the word with all readiness of mind, and searched the scriptures daily, whether those things were so. |2 Timothy 3:15-17. And that from a child thou hast known the holy scriptures, which are able to make thee wise unto salvation through faith which is in Christ Jesus. All scripture is given by inspiration of God, and is profitable for doctrine, for reproof, for correction, for instruction in righteousness: That the man of God may be perfect, thoroughly furnished unto all good works. |1 John 1:4. And these things write we unto you, that your joy may be full.",answer,proofs));
      }
    }

    public void MergeSort(OrderEnum listOrder)
    {
      switch (listOrder)
      {
        case OrderEnum.IDOrder: this.MergeSort(); break;
        case OrderEnum.TitleOrder:this.MergeSort(Document.CompareTitles);break;
        case OrderEnum.MatchOrder:this.MergeSort(Document.CompareMatches);break;
        }
        
      } 
    public int BinarySearch(Document document, OrderEnum listOrder)
    {
      int listIndex = 0;
      switch (listOrder)
      {
        case OrderEnum.IDOrder: listIndex = this.BinarySearch(document); break;
        case OrderEnum.TitleOrder: listIndex = this.BinarySearch(document, OrderEnum.TitleOrder); break;
        case OrderEnum.MatchOrder:listIndex = this.BinarySearch(document, OrderEnum.MatchOrder);break;
        }
      return listIndex;
    }
    protected void ProcessError(string message)
    {
      Log.Error("DocumentList", message);
    }
    public bool BinaryContains(Document document, OrderEnum listOrder)
    {
      bool returnValue = false;
      switch (listOrder)
      {
        case OrderEnum.IDOrder:
          if (BinarySearch(document, OrderEnum.IDOrder) > 0)
            returnValue = true; break;
        case OrderEnum.TitleOrder:
          if (BinarySearch(document, OrderEnum.TitleOrder) > 0)
            returnValue = true; break;
        case OrderEnum.MatchOrder:
          if (BinarySearch(document, OrderEnum.MatchOrder) > 0)
            returnValue = true;break;
      }
      return returnValue;
    }
    
        //else
        // ProcessError(String.Format("GArrayList this[] Set index must be between 1 and {0}",this.count));
      
    //  public void PrintReport(string fileName)
    //  {
    //    StreamWriter docOut;
    //    int indention = 50;
    //    docOut = File.CreateText(fileName);
    //    docOut.WriteLine();
    //    docOut.WriteLine(("").PadRight(indention) + this.Title);
    //    docOut.WriteLine();
    //    docOut.WriteLine();
    //    if (this.Count > 0)
    //    {
    //      for(int i=1;i<=this.Count;i++)
    //      {
    //        if (this[i].Type == "CONFESSION")
    //        {
    //          Confession confession = (Confession)this[i];

    //          docOut.WriteLine("Chapter {0}: {1}", confession.IDNumber, confession.Title);
    //          docOut.WriteLine(confession.Chapter);

    //          docOut.WriteLine("Proofs for Chapter {0}:", confession.IDNumber);
    //          docOut.WriteLine(confession.Proofs);
    //          docOut.WriteLine();

    //        }
    //        else if (this[i].Type == "CATECHISM")
    //        {
    //          Catechism catechism = (Catechism)this[i];
    //          docOut.WriteLine("Question {0}  {1}", catechism.IDNumber, catechism.Title);
    //          docOut.WriteLine("Question: {0}", catechism.Question);
    //          docOut.WriteLine();
    //          docOut.WriteLine("Answer: {0}", catechism.Answer);
    //          docOut.WriteLine("");
    //          docOut.WriteLine("Proofs:");
    //          docOut.WriteLine("{0}", catechism.Proofs);
    //          docOut.WriteLine();

    //        }
    //        else if (this[i].Type == "CREED")
    //        {
    //          Creed creed = (Creed)this[i];
    //          docOut.WriteLine(creed.CreedText);
    //        }
    //      }
    //    }
    //    else
    //      docOut.WriteLine("Report is empty");


    //    docOut.Close(); //MessageBox.Show("Report Created");
    //  }
    //  public void PrintList(string fileName)
    //  {
    //    Confession confession; Catechism catechism;
    //    StreamWriter docOut = File.CreateText(fileName);
    //    docOut.WriteLine(this.Title);
    //    if (this.Count > 0)
    //    {
    //      foreach (Document document in this)
    //      {
    //        if (document.Type == "CONFESSION")
    //        {
    //          confession = (Confession)document;
    //          docOut.WriteLine("\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\"", confession.Type, confession.IDNumber, confession.Title, confession.Chapter, confession.proofWrite);

    //        }
    //        else if (document.Type == "CATECHISM")
    //        {
    //          catechism = (Catechism)document;
    //          docOut.WriteLine("\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\",\"{5}\"", catechism.Type, catechism.IDNumber, catechism.Question, catechism.Answer, catechism.Title, catechism.proofWrite);
    //        }
    //      }
    //    }
    //    docOut.Close();
    //  }
    //  public void PrintWeb(string fileName)
    //  {
    //    Confession confession; Catechism catechism;
    //    StreamWriter docOut = File.CreateText(fileName);
    //    docOut.WriteLine(this.Title);
    //    if (this.Count > 0)
    //    {
    //      foreach (Document document in this)
    //      {
    //        if (document.Type == "CONFESSION")
    //        {
    //          confession = (Confession)document;
    //          docOut.WriteLine("{0}~{1}~{2}~{3}~{4}", confession.Type, confession.IDNumber, confession.Title, confession.Chapter, confession.proofWrite);

    //        }
    //        else if (document.Type == "CATECHISM")
    //        {
    //          catechism = (Catechism)document;
    //          docOut.WriteLine("{0}~{1}~{2}~{3}~{4}~{5}", catechism.Type, catechism.IDNumber, catechism.Question, catechism.Answer, catechism.Title, catechism.proofWrite);
    //        }
    //      }
    //    }
    //    docOut.Close();
    //  }
    //}
  }
}