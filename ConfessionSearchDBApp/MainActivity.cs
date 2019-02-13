using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using SQLite;
//using Android.Graphics;
using System.IO;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android;
using Android.Util;
using Android.Support.V4.App;
using Android.Support.V4.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Android.Views;

namespace ConfessionSearchDBApp.Main
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        int recurCall = 0,recurCall1=0;
        string fileName = "";
        List<KeyValuePair<string, string>> files, documents;
        List<KeyValuePair<int, String>> types;
        List<Document> documentList = new List<Document>();
        Stopwatch stopwatch = new Stopwatch();
        private bool confessionOpen, catechismOpen, creedOpen, helpOpen, allOpen;
        string type = "";
        string dbName = "confessionSearchDB.db";
        string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(),
           "confessionSearchDB.db");
        protected override void OnCreate(Bundle savedInstanceState)
        {


            string[] permissions = new string[2];
            permissions[0] = Manifest.Permission.WriteExternalStorage;
            permissions[1] = Manifest.Permission.ReadExternalStorage;
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            Spinner spinner1 = FindViewById<Spinner>(Resource.Id.spinner1), spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);


            ActivityCompat.RequestPermissions(this, permissions, 1);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                if (!File.Exists(dbPath))
                {
                    using (BinaryReader br = new BinaryReader(Android.App.Application.Context.Assets.Open(dbName)))
                    {
                        using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                        {
                            byte[] buffer = new byte[2048];
                            int len = 0;
                            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, len);
                            }
                        }
                    }
                }  // We have permission, go ahead and use the camera.
                using (var conn = new SQLite.SQLiteConnection(dbPath))
                {
                    var cmd = new SQLite.SQLiteCommand(conn); var cmd1 = new SQLite.SQLiteCommand(conn);
                    cmd1.CommandText = "select * from DocumentType";
                    cmd.CommandText = "select * from DocumentList";
                    var docTypes = cmd1.ExecuteQuery<DocumentType>();
                    var r = cmd.ExecuteQuery<DocumentList>();

                    List<string> items = new List<string>();
                    items.Add("All");
                    foreach (var item in docTypes)
                        items.Add(item.DocumentTypeName);
                    var adapter1 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                    adapter1.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner1.ItemSelected += Spinner1_ItemSelected;
                    spinner1.Adapter = adapter1;

                    items = new List<string>();
                    foreach (var item in r)
                        items.Add(item.Title);
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner2.ItemSelected += Spinner2_ItemSelected;
                    spinner2.Adapter = adapter;


                }
            }

            else
            {
                ActivityCompat.RequestPermissions(this, permissions, 1); this.Recreate();
                // Camera permission is not granted. If necessary display rationale & request.
            }
        }
        public string Formatter(string stringField)
        {
            int x = 0;
            string formatter = "";
            string[] words;
            words = stringField.Split('|');
            for (int i = 0; i <= words.Length - 1; i++, x++)
            {
                formatter += words[i];
                formatter += "\r\n\n";

            }

            stringField = formatter;
            return stringField;
        }
        //Spinner Selection Statements
        private void Spinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            type = String.Format("{0}", spinner.GetItemAtPosition(e.Position));
            // Toast.MakeText(this, type, ToastLength.Long).Show();
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                switch (type.ToUpper())
                {
                    case "ALL":
                        allOpen = true; confessionOpen = false; catechismOpen = false; creedOpen = false; helpOpen = false;
                        Spinner spinnerz = FindViewById<Spinner>(Resource.Id.spinner2);
                        List<string> catechismFiles = new List<string>();

                        var cmd = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = "select * from DocumentList";
                        var r = cmd.ExecuteQuery<DocumentList>();
                        List<string> items = new List<string>();
                        foreach (var item in r)
                            items.Add(item.Title);
                        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        spinnerz.ItemSelected += Spinner2_ItemSelected;
                        spinnerz.Adapter = adapter;

                        break;
                    case "CATECHISM":
                        allOpen = false; catechismOpen = true; confessionOpen = false; creedOpen = false; helpOpen = false;
                        spinnerz = FindViewById<Spinner>(Resource.Id.spinner2);
                        cmd = new SQLiteCommand(conn);
                        cmd.CommandText = "Select documenttype.*,documentlist.* from documentlist natural join documenttype where documentlist.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName='CATECHISM' ";
                        r = cmd.ExecuteQuery<DocumentList>();
                        items = new List<string>();
                        foreach (var item in r)
                            items.Add(item.Title);
                        adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        spinnerz.ItemSelected += Spinner2_ItemSelected;
                        spinnerz.Adapter = adapter; break;
                    case "CONFESSION":
                        allOpen = false; confessionOpen = true; catechismOpen = false; creedOpen = false; helpOpen = false;
                        spinnerz = FindViewById<Spinner>(Resource.Id.spinner2);
                        cmd = new SQLiteCommand(conn);
                        cmd.CommandText = "Select documenttype.*,documentlist.* from documentlist natural join documenttype where documentlist.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName='CONFESSION' ";
                        r = cmd.ExecuteQuery<DocumentList>();
                        items = new List<string>();
                        foreach (var item in r)
                            items.Add(item.Title);
                        adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        spinnerz.ItemSelected += Spinner2_ItemSelected;
                        spinnerz.Adapter = adapter; break;
                    case "CREED":
                        allOpen = false; creedOpen = true; catechismOpen = false; confessionOpen = false; helpOpen = false;
                        spinnerz = FindViewById<Spinner>(Resource.Id.spinner2);
                        cmd = new SQLiteCommand(conn);
                        cmd.CommandText = "Select documenttype.*,documentlist.* from documentlist natural join documenttype where documentlist.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName='CREED' ";
                        r = cmd.ExecuteQuery<DocumentList>();
                        items = new List<string>();
                        foreach (var item in r)
                            items.Add(item.Title);
                        adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        spinnerz.ItemSelected += Spinner2_ItemSelected;
                        spinnerz.Adapter = adapter; break;
                }
                Toast.MakeText(this, type, ToastLength.Short).Show();

            }

        }
        public void SearchType(View view)
        {
            KeyEvent enter = new KeyEvent(KeyEventActions.Down, Keycode.Enter);
            SearchView searchView = FindViewById<SearchView>(Resource.Id.searchView1);
            RadioButton radio = ((RadioButton)view);
            TextView text = FindViewById<TextView>(Resource.Id.searchTVFAB);
            if (radio == FindViewById<RadioButton>(Resource.Id.topicRadio))
            {
                if (radio.Checked)
                {
                    searchView.Enabled = true;

                    searchView.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    searchView.SetInputType(Android.Text.InputTypes.ClassText);
                    searchView.QueryTextSubmit += Search_QueryTextSubmit;
                    searchView.SetQueryHint("Search By topic");
                    text.Text = "Search";

                }
            }
            //Chapter/Question Number Search
            else if (radio == FindViewById<RadioButton>(Resource.Id.chapterRadio))
            {
                if (radio.Checked)
                {
                    searchView.Enabled = true;
                    searchView.SetInputType(Android.Text.InputTypes.ClassNumber);
                    searchView.SetQueryHint("Search By Number...");
                    text.Text = "Search";
                }
            }
            else if (radio == FindViewById<RadioButton>(Resource.Id.viewAllRadio))
            {
                text.Text = "Read";
                if (!creedOpen)
                {
                    searchView.SetIconifiedByDefault(true);
                    searchView.Enabled = false;

                }

            }

        }
        public string TableAccess(string var1)
        {
            string var2 = String.Format("Select documenttype.*,documentlist.* from documentlist natural join documenttype where documentlist.documenttypeid= documenttype.documenttypeid {0} ",var1);
            return var2;
        }
        public string DataTableAccess(string var1)
        {
            string var2 = string.Format("Select Documentlist.documentName, document.DocIndexNum, document.chname, document.chText, document.chproofs,document.ChTags,document.ChMatches from Documentlist natural join document where document.DocumentID=documentlist.DocumentID {0}",var1);
            return var2;
        }
        private void Search_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            recurCall++;
            if (recurCall == 1)
            {
                string query = e.Query;
                Search(query);
            }

        }
        private void Spinner2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            fileName = String.Format("{0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, fileName, ToastLength.Long).Show();


        }

        private void Search(string query)
        {
            int docCount = 1;
            bool truncate = false;
            Log.Info("Search()", String.Format("Search Begins" + ""));
            // documentList = new DocumentList();
            RadioButton radio = FindViewById<RadioButton>(Resource.Id.viewAllRadio);
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                var cmd = new SQLite.SQLiteCommand(conn); var searchStr = new SQLite.SQLiteCommand(conn);
                bool proofs = true, answers = true, searchAll = false;
                CheckBox answerCheck = FindViewById<CheckBox>(Resource.Id.AnswerBox), proofCheck = FindViewById<CheckBox>(Resource.Id.proofBox),
            searchCheck = FindViewById<CheckBox>(Resource.Id.searchAllCheckBox);
                Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1), spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);
                spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
                spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner2_ItemSelected);
                List<Document> searchField1; List<DocumentList> tortures;
                //   var r, searchFields;
                if (allOpen)
                {
                    if (radio.Checked == false)
                    {
                        #region
                        if (proofCheck.Checked)
                        {
                            proofs = false;

                        }
                        else
                            proofs = true;
                        #endregion
                        if (searchCheck.Checked)
                        {
                            searchAll = true;
                            cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                            cmd.CommandText = "select * from DocumentList";
                            searchStr.CommandText = "Select Documentlist.documentName, document.DocIndexNum, document.chname, document.chText, document.chproofs,document.ChTags,document.ChMatches from Documentlist natural join document where document.DocumentID=documentlist.DocumentID";
                            var r = cmd.ExecuteQuery<DocumentList>();
                            var searchFields = searchStr.ExecuteQuery<Document>();
                            foreach (var item in searchFields)
                            {
                                Document document = new Document();
                                document.DocDetailID = item.DocDetailID;
                                document.DocumentText = item.DocumentText;
                                document.ChNumber = item.ChNumber;
                                document.ChProofs = item.ChProofs;
                                document.Tags = item.Tags;
                                documentList.Add(document);
                            }
                            //Add Document Fields
                        }
                        //search 1
                        else
                        {
                            searchAll = false;
                            cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                            cmd.CommandText = String.Format("select * from DocumentList where DocumentList.DocumentName='{0}'", fileName);
                            searchStr.CommandText = "Select Documentlist.documentName, document.DocIndexNum, document.chname, document.chText, document.chproofs,document.ChTags,document.ChMatches from Documentlist natural join document where document.DocumentID=documentlist.DocumentID";
                            var r = cmd.ExecuteQuery<DocumentList>();
                            var searchFields = searchStr.ExecuteQuery<Document>();
                            foreach (var item in searchFields)
                            {
                                Document document = new Document();
                                document.DocDetailID = item.DocDetailID;
                                document.DocumentText = item.DocumentText;
                                document.ChNumber = item.ChNumber;
                                document.ChProofs = item.ChProofs;
                                document.Tags = item.Tags;
                                document.DocTitle = fileName;
                                documentList.Add(document);
                            }
                        }
                    }
                    else
                    {
                        //  answers = true;
                        searchAll = false;
                        docCount = 1; proofs = true;
                        cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = String.Format("select * from DocumentList where DocumentList.DocumentName='{0}'", fileName);
                        searchStr.CommandText = "Select Documentlist.documentName, document.DocIndexNum, document.chname, document.chText, document.chproofs,document.ChTags,document.ChMatches from Documentlist natural join document where document.DocumentID=documentlist.DocumentID";
                        var r = cmd.ExecuteQuery<DocumentList>();
                        var searchFields = searchStr.ExecuteQuery<Document>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                                document.DocTitle = fileName;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                            //
                        }   // docCount = 10;
                    }
                }

                else if (catechismOpen)
                {
                    //Read Document Button
                    if (radio.Checked == false)
                    {
                        #region
                        if (proofCheck.Checked)
                            proofs = false;
                        else
                            proofs = true;
                        #endregion
                        //Search All
                        if (searchCheck.Checked)
                        {
                            searchAll = true;
                            cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                            cmd.CommandText = TableAccess("and documenttype.DocumentTypeName='CATECHISM'");
                            searchStr.CommandText = DataTableAccess("");
                            var r = cmd.ExecuteQuery<DocumentList>();
                            var searchFields = searchStr.ExecuteQuery<Document>();
                            foreach (var item in searchFields)
                            {
                                Document document = new Document();
                                document.DocDetailID = item.DocDetailID;
                                document.DocumentText = item.DocumentText;
                                document.DocTitle = fileName;
                                document.ChNumber = item.ChNumber;
                                document.ChProofs = item.ChProofs;
                                document.Tags = item.Tags;
                                documentList.Add(document);
                            }
                        }
                        //Search One
                        else
                        {
                            searchAll = false;

                            cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                            cmd.CommandText = TableAccess(String.Format(" and documenttype.DocumentTypeName='CATECHISM' and DocumentName='{0}' ", fileName));
                            searchStr.CommandText = DataTableAccess("");
                            var r = cmd.ExecuteQuery<DocumentList>();
                            var searchFields = searchStr.ExecuteQuery<Document>();
                            foreach (var item in searchFields)
                            {
                                Document document = new Document();
                                document.DocDetailID = item.DocDetailID;
                                document.DocumentText = item.DocumentText;
                                document.DocTitle = fileName;
                                document.ChNumber = item.ChNumber;
                                document.ChProofs = item.ChProofs;
                                document.Tags = item.Tags;
                                documentList.Add(document);
                            }
                        }
                    }
                    //Read Button = true
                    else
                    {

                        searchAll = false;
                        docCount = 1; proofs = true;
                        cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = TableAccess(String.Format(" and documenttype.DocumentTypeName='CATECHISM' and DocumentName='{0}' ", fileName));
                        searchStr.CommandText = DataTableAccess("");
                        var r = cmd.ExecuteQuery<DocumentList>();
                        var searchFields = searchStr.ExecuteQuery<Document>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                                document.DocTitle = fileName;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                        }
                    }
                }
                else if (confessionOpen)
                {
                    if (radio.Checked != true)
                    {
                        if (proofCheck.Checked)
                            proofs = false;
                        else
                            proofs = true;
                        if (searchCheck.Checked)
                        {
                            searchAll = true;
                            cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                            cmd.CommandText = TableAccess("and documenttype.DocumentTypeName='CONFESSION' ");
                            searchStr.CommandText = DataTableAccess("");//"Select Documentlist.documentName, document.DocIndexNum, document.chname, document.chText, document.chproofs,document.ChTags,document.ChMatches from Documentlist natural join document where document.DocumentID=documentlist.DocumentID";
                            var rs = cmd.ExecuteQuery<DocumentList>();
                            var ssearchFields = searchStr.ExecuteQuery<Document>();
                            foreach (var item in ssearchFields)
                            {
                                Document document = new Document();
                                document.DocDetailID = item.DocDetailID;
                                document.DocumentText = item.DocumentText;
                                document.ChNumber = item.ChNumber;
                                document.ChProofs = item.ChProofs;
                                document.Tags = item.Tags;
                                documentList.Add(document);
                            }
                        }
                        else
                            searchAll = false;
                        docCount = 1;
                        cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = TableAccess("and documenttype.DocumentTypeName='CONFESSION' ");
                        searchStr.CommandText = DataTableAccess("");
                        var r = cmd.ExecuteQuery<DocumentList>();
                        var searchFields = searchStr.ExecuteQuery<Document>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                        }
                    }
                    else
                    {
                        proofs = true; searchAll = false;
                        docCount = 1;
                        cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = TableAccess(String.Format(" and documenttype.DocumentTypeName='CONFESSION' and DocumentName='{0}' ", fileName));
                        searchStr.CommandText = DataTableAccess("");
                        var r = cmd.ExecuteQuery<DocumentList>();
                        var searchFields = searchStr.ExecuteQuery<Document>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                        }
                    }

                }
                else if (creedOpen)
                {
                    if (searchCheck.Checked)
                    {
                        searchAll = true;
                        cmd = new SQLite.SQLiteCommand(conn); searchStr = new SQLite.SQLiteCommand(conn);
                        cmd.CommandText = TableAccess("and documenttype.DocumentTypeName='CREED' ");
                        searchStr.CommandText = DataTableAccess("");
                        var r = cmd.ExecuteQuery<DocumentList>();
                        var searchFields = searchStr.ExecuteQuery<Document>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                        }
                    }
                    else
                    {
                        searchStr.CommandText = DataTableAccess("");
                        proofs = false; searchAll = false; truncate = false;
                        var searchFields = searchStr.ExecuteQuery<Document>();

                        cmd.CommandText = TableAccess(string.Format("and documenttype.DocumentTypeName='CREED' and DocumentName='{0}' ", fileName));// String.Format("Select documenttype.*,documentlist.* from documentlist natural join documenttype where documentlist.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName=);
                        var r = cmd.ExecuteQuery<DocumentList>();
                        foreach (var item in searchFields)
                        {
                            Document document = new Document();
                            document.DocDetailID = item.DocDetailID;
                            document.DocumentText = item.DocumentText;
                                document.DocTitle = fileName;
                            document.ChNumber = item.ChNumber;
                            document.ChProofs = item.ChProofs;
                            document.Tags = item.Tags;
                            documentList.Add(document);
                        }
                    }
                    // answers = false;

                }
                if (FindViewById<CheckBox>(Resource.Id.truncateCheck).Checked)
                    truncate = true;
                if (radio.Checked != true && query != "" && FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
                {
                    if (FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
                    {
                        stopwatch.Start();
                        FilterResults(documentList, truncate, true, proofs, searchAll, query);
                        documentList.Reverse();
                        stopwatch.Stop();
                    }

                }
                else if (FindViewById<RadioButton>(Resource.Id.chapterRadio).Checked & query != "")
                {

                    int searchInt = Int32.Parse(query);
                    FilterResults(this.documentList, truncate, answers, proofs, searchAll, searchInt);

                }
                else if (FindViewById<RadioButton>(Resource.Id.viewAllRadio).Checked)
                {
                    if (!FindViewById<CheckBox>(Resource.Id.searchAllCheckBox).Checked)
                        query = "Results for All";
                    else
                        query = "View All";
                }
                if (documentList.Count > 1)
                {
                    SetContentView(Resource.Layout.search_results);
                    ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                    SearchAdapter adapter = new SearchAdapter(SupportFragmentManager, documentList, query, truncate);

                }
            }
        }
    

        public void FilterResults(List<Document> list, bool truncate, bool answers, bool proofs, bool allDocs, string searchTerm)
        {
            List<Document> resultList = new List<Document>();
            Log.Info("Filter", "Filtering Confession Results");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            Log.Debug("Timer", "Timer Started");
            Regex regex = new Regex(searchTerm, RegexOptions.IgnoreCase);

            Log.Info("Filter Results", "Filtering has begun");
            foreach (Document document in list)
            {
                string[] searchEntries = new string[3];

                #region old code
                foreach (string word in searchEntries) //this[i].Split(' '))
                {
                    if (regex.IsMatch(word))
                        document.Matches++;

                }
                if(document.Matches>1)
                resultList.Add(document);
                stopwatch.Stop();
                Log.Debug("Timer", String.Format("{0}ms Passed", stopwatch.ElapsedMilliseconds.ToString()));
                stopwatch.Reset();
            }
            list = resultList;
            list.Sort(Document.CompareMatches);

        }
    
   
    ////Search By Number
    public void FilterResults(List<Document> list, bool truncate, bool answers, bool proofs, bool allDocs, int searchTerm)
    {
            List<Document> resultList = new List<Document>();
        foreach (Document document in list)
            if (document.ChNumber == searchTerm)
                resultList.Add(document);

        list= resultList;
            list.Sort();
    }
    
    public class DocumentList
        {
            [PrimaryKey, AutoIncrement]
            public int DocumentID { get; set; }
            [Column("DocumentName")]
            public string Title { get; set; }
           [Column("DocumentTypeID")]
            private int DocumentTypeID { get; set; }
            // public int ArtistId { get; set; }
        }

        public class Document
        {
            [PrimaryKey,AutoIncrement]
            public int DocDetailID { get; set; }
            [Column("ChText")]
            public string DocumentText { get; /*{ return Formatter(this.DocumentText); }*/ set; /*{ this.DocumentText = Formatter(value); }*/ }
            [Ignore]
            public string DocTitle { get; set; }
            [Column("DocIndexNum")]
            public int ChNumber { get; set; }
[Column("ChProofs")]
            public string ChProofs { get; /*{ return Formatter(this.ChProofs); }*/ set; } //{ this.ChProofs = Formatter(value); }}

            [Column("ChMatches")]
            public int Matches { get; set; }// { return this.Matches; } set { this.Matches = value; } }

            [Column("ChTags")]
            public string Tags { get; set; }
            
            public int CompareTo(Document compareDocument)
            {
                return this.ChNumber.CompareTo(compareDocument.ChNumber);
            }
            
            public static int CompareMatches(Document document1, Document document2)
            {
                string string1, string2;
                if (document1.Matches >= document2.Matches)
                {
                    return 1;
                }

                if (document1.Matches <= document2.Matches)
                {
                    return -1;
                }
                else
                {
                    string1 = document1.ChNumber.ToString() + document1.DocDetailID;
                    string2 = document2.ChNumber.ToString() + document2.DocDetailID;
                    return string1.CompareTo(string2);
                }
            }
            //[Ignore]
            // public this[int ]

        }
        public class DocumentType
        {
            [PrimaryKey,AutoIncrement]
            public int DocumentTypeID { get; set; }
            
            [Column("DocumentTypeName")]
            public string DocumentTypeName { get; set; }

        }

    }
}