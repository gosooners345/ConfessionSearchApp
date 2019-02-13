using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Content.Res;
using Android.Util;

namespace ConfessionSearchDBApp
{
    [Table("Documents")]
    public class Document
    {
        protected int idNumber, matches;
        protected string title, proofs, type, docTitle, tags,text;
#pragma warning disable IDE0044 // Add readonly modifier
        private Document document;
#pragma warning restore IDE0044 // Add readonly modifier

        public Document()
        {
            this.idNumber = 0; this.title = ""; this.proofs = ""; this.type = ""; this.matches = 0; this.tags = "";
            this.docTitle = ""; this.text = "";
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
        //foreign Key
        public int DocIDNumber
        {
            get;set;
        }

        [PrimaryKey, AutoIncrement]
        public int DocDetailIDNumber { get; set; }

        public int ChapterNumber
        {
            get
            {
                return idNumber;
            }
            set
            {
                if (value >= 0)
                {
                    idNumber = value;
                }
            }
        }
        protected static void ProcessError(string message)
        {

            Log.Error("Error", message);

        }
        public int Matches
        {
            get { return this.matches; }
            set { this.matches = value; }
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
                {
                    this.type = value;
                }
                else
                {
                    ProcessError(String.Format("{0} can not be assigned to an Type property", value));
                }
            }
        }
        public string Tags { get { return this.tags; } set { this.tags = value; } }
        public string Proofs { get { return this.proofs; } set { this.proofs = Formatter(value); } }
        public string Title { get { return this.title; } set { title = value; } }
        public string DocTitle { get { return docTitle;  } set { docTitle = value; } }
        public string Formatter(string stringField)
        {
            int x = 0;
            string formatter = "";
            String[] words;
            words = stringField.Split('|');
            for (int i = 0; i <= words.Length - 1; i++, x++)
            {
                formatter += words[i];
                formatter += "\r\n\n";

            }

            stringField = formatter;
            return stringField;
        }
        public string Text { get { return this.text; } set { this.text= Formatter(value); } }
        public static Document Parse(string stringValue, bool proofs,int documentTitleID)
        {
            string[] words;
            Document document = new Document();
            document.DocIDNumber = documentTitleID;
            words = stringValue.Split('~');
            document.Type = words[0];
            document.ChapterNumber = Int32.Parse(words[1]);
            document.Title = words[2];
            document.Text = words[3];
            if (proofs)
            {
                if (words[4] == "")
                    document.Proofs = "No Proofs Available";
                else
                    document.Proofs = words[4];
            }
            else
            {
                document.Proofs = "";
            }

            document.DocTitle = words[5];
            document.Tags = words[6];
            return document;

        }

    }
    [Table("DocumentList")]
    public class DocumentTitle:List<Document>
    {
        
        protected string title;
        //protected int documentID;
        public DocumentTitle()
        { this.title = "";  }
        public DocumentTitle(string stringField)
        {
            this.title = stringField;
        }
       
        [PrimaryKey,AutoIncrement]
        public int DocumentID
        { get; set; }
        public enum OrderEnum { IDOrder, TitleOrder, MatchOrder };
        public string Title { get { return this.title; } set { this.title = value; } }
        public void Fill(string fileName, AssetManager asset, int ID, bool answer, bool proofs)
        {

            try
            {
                if (ID == 0)
                {
                    this.Clear();
                }

                StreamReader fileIn = new StreamReader(asset.Open(fileName));

                if (ID == 0)
                {
                    Title = fileIn.ReadLine();
                    this.DocumentID++;
                }
                else
                {
                    Title = "Results";
                }

                string dbConnection = "DocumentList";
                var db = new SQLiteConnection(dbConnection);
              db.CreateTable<DocumentTitle>();
                while (fileIn.Peek() != -1)
                {
                    Document document = new Document();
                    db.CreateTable<Document>();
                    document =Document.Parse(fileIn.ReadLine(), proofs, this.DocumentID);
                    document.DocTitle = this.Title;
                    db.Insert(document);
                    this.Add(document);
                }

                fileIn.Close();
               
               
                db.InsertAll(this);
                

            }

            catch
            {
                DocumentID = 1;
                this.Add(Document.Parse("CATECHISM~0~null~null~null~null",proofs,DocumentID));
                this.Add(Document.Parse("CATECHISM~1~What is the chief end of man?~Man's chief end is to glorify God,[1] and to enjoy him forever.[2]~Mankind's Ultimate Purpose in life~[1] |Psalm 86. Bow down thine ear, O LORD, hear me: for I am poor and needy. Preserve my soul; for I am holy: O thou my God, save thy servant that trusteth in thee. Be merciful unto me, O Lord: for I cry unto thee daily. Rejoice the soul of thy servant: for unto thee, O Lord, do I lift up my soul. For thou, Lord, art good, and ready to forgive; and plenteous in mercy unto all them that call upon thee. Give ear, O LORD, unto my prayer; and attend to the voice of my supplications. In the day of my trouble I will call upon thee: for thou wilt answer me. Among the gods there is none like unto thee, O Lord; neither are there any works like unto thy works. All nations whom thou hast made shall come and worship before thee, O Lord; and shall glorify thy name. For thou art great, and doest wondrous things: thou art God alone. Teach me thy way, O LORD; I will walk in thy truth: unite my heart to fear thy name. I will praise thee, O Lord my God, with all my heart: and I will glorify thy name for evermore. For great is thy mercy toward me: and thou hast delivered my soul from the lowest hell. O God, the proud are risen against me, and the assemblies of violent men have sought after my soul; and have not set thee before them. But thou, O Lord, art a God full of compassion, and gracious, longsuffering, and plenteous in mercy and truth. O turn unto me, and have mercy upon me; give thy strength unto thy servant, and save the son of thine handmaid. Show me a token for good; that they which hate me may see it, and be ashamed: because thou, LORD, hast holpen me, and comforted me. |Isaiah 60:21. Thy people also shall be all righteous: they shall inherit the land for ever, the branch of my planting, the work of my hands, that I may be glorified. |Romans 11:36. For of him, and through him, and to him, are all things: to whom be glory for ever. Amen. |1 Corinthians 6:20, 10:31. For ye are bought with a price: therefore glorify God in your body, and in your spirit, which are God's.... Whether therefore ye eat, or drink, or whatsoever ye do, do all to the glory of God. |Revelation 4:11. Thou art worthy, O Lord, to receive glory and honour and power: for thou hast created all things, and for thy pleasure they are and were created.|[2] |Psalm 16:5-11. The LORD is the portion of mine inheritance and of my cup: thou maintainest my lot. The lines are fallen unto me in pleasant places; yea, I have a goodly heritage. I will bless the LORD, who hath given me counsel: my reins also instruct me in the night seasons. I have set the LORD always before me: because he is at my right hand, I shall not be moved. Therefore my heart is glad, and my glory rejoiceth: my flesh also shall rest in hope. For thou wilt not leave my soul in hell; neither wilt thou suffer thine Holy One to see corruption. Thou wilt show me the path of life: in thy presence is fulness of joy; at thy right hand there are pleasures for evermore. |Psalm 144:15. Happy is that people, that is in such a case: yea, happy is that people, whose God is the LORD. |Isaiah 12:2. Behold, God is my salvation; I will trust, and not be afraid: for the LORD JEHOVAH is my strength and my song; he also is become my salvation. |Luke 2:10. And the angel said unto them, Fear not: for, behold, I bring you good tidings of great joy, which shall be to all people. |Philippians 4:4. Rejoice in the Lord alway: and again I say, Rejoice. |Revelation 21:3-4. And I heard a great voice out of heaven saying, Behold, the tabernacle of God is with men, and he will dwell with them, and they shall be his people, and God himself shall be with them, and be their God. And God shall wipe away all tears from their eyes; and there shall be no more death, neither sorrow, nor crying, neither shall there be any more pain: for the former things are passed away.", proofs,DocumentID));
                this.Add(Document.Parse("CATECHISM~2~What rule hath God given to direct us how we may glorify and enjoy him?~The Word of God, which is contained in the Scriptures of the Old and New Testaments,[3] is the only rule to direct us how we may glorify and enjoy him.[4]~The Authority of Scripture~[3] |Matthew 19:4-5. And he answered and said unto them, Have ye not read, that he which made them at the beginning made them male and female, And said, For this cause shall a man leave father and mother, and shall cleave to his wife: and they twain shall be one flesh? With |Genesis 2:24. Therefore shall a man leave his father and his mother, and shall cleave unto his wife: and they shall be one flesh. |Luke 24:27, 44. And beginning at Moses and all the prophets, he expounded unto them in all the scriptures the things concerning himself.... And he said unto them, These are the words which I spake unto you, while I was yet with you, that all things must be fulfilled, which were written in the law of Moses, and in the prophets, and in the psalms, concerning me. |1 Corinthians 2:13. Which things also we speak, not in the words which man's wisdom teacheth, but which the Holy Ghost teacheth; comparing spiritual things with spiritual. |1 Corinthians 14:37. If any man think himself to be a prophet, or spiritual, let him acknowledge that the things that I write unto you are the commandments of the Lord. 2 Peter 1:20-21. Knowing this first, that no prophecy of the scripture is of any private interpretation. For the prophecy came not in old time by the will of man: but holy men of God spake as they were moved by the Holy Ghost. 2 Peter 3:2, 15-16. That ye may be mindful of the words which were spoken before by the holy prophets, and of the commandment of us the apostles of the Lord and Saviour.... And account that the longsuffering of our Lord is salvation; even as our beloved brother Paul also according to the wisdom given unto him hath written unto you; As also in all his epistles, speaking in them of these things; in which are some things hard to be understood, which they that are unlearned and unstable wrest, as they do also the other scriptures, unto their own destruction.|[4] |Deuteronomy 4:2. Ye shall not add unto the word which I command you, neither shall ye diminish ought from it, that ye may keep the commandments of the LORD your God which I command you. |Psalm 19:7-11. The law of the LORD is perfect, converting the soul: the testimony of the LORD is sure, making wise the simple. The statutes of the LORD are right, rejoicing the heart: the commandment of the LORD is pure, enlightening the eyes. The fear of the LORD is clean, enduring for ever: the judgments of the LORD are true and righteous altogether. More to be desired are they than gold, yea, than much fine gold: sweeter also than honey and the honeycomb. Moreover by them is thy servant warned: and in keeping of them there is great reward. |Isaiah 8:20. To the law and to the testimony: if they speak not according to this word, it is because there is no light in them. |John 15:11. These things have I spoken unto you, that my joy might remain in you, and that your joy might be full. |John 20:30-31. And many other signs truly did Jesus in the presence of his disciples, which are not written in this book: But these are written, that ye might believe that Jesus is the Christ, the Son of God; and that believing ye might have life through his name. |Acts 17:11. These were more noble than those in Thessalonica, in that they received the word with all readiness of mind, and searched the scriptures daily, whether those things were so. |2 Timothy 3:15-17. And that from a child thou hast known the holy scriptures, which are able to make thee wise unto salvation through faith which is in Christ Jesus. All scripture is given by inspiration of God, and is profitable for doctrine, for reproof, for correction, for instruction in righteousness: That the man of God may be perfect, thoroughly furnished unto all good works. |1 John 1:4. And these things write we unto you, that your joy may be full.", proofs,DocumentID));
            }
        }
    }
    public class DocumentList : List<DocumentTitle> {

        public DocumentList() { }

        public DocumentList(string[] documentNames)
        {

            foreach (string docName in documentNames)
                this.Add(new DocumentTitle(docName));
        }

    }
}