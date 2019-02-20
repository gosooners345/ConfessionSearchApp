using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using SQLite;
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
using Android.Content;
using Android.Support.Design.Widget;

namespace ConfessionSearchDBApp.Main
{//657 Lines of code
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        int recurCall = 0, recurCall1 = 0;
        string fileName = "";
        List<KeyValuePair<string, string>> files, documents;
        List<KeyValuePair<int, String>> types;
        Intent intent;
        string header = "";
        string shareList = "", newLine = "\r\n";
        SearchFragmentActivity searchFragmentActivity;
        DocumentList documentList, resultList = new DocumentList();
        Stopwatch stopwatch = new Stopwatch();
        private bool confessionOpen, catechismOpen, creedOpen, helpOpen, allOpen;
        string type = "";
        string dbName = "confessionSearchDB.db";
        string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(),
           "confessionSearchDB.db");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //base.OnCreate();
base.OnCreate(savedInstanceState);
            string[] permissions = new string[2];
            permissions[0] = Manifest.Permission.WriteExternalStorage;
            permissions[1] = Manifest.Permission.ReadExternalStorage;
            
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
                  //  var cmd2= new SQLite.SQLiteCommand(conn); 
                    var cmd = new SQLite.SQLiteCommand(conn); //var cmd1 = new SQLite.SQLiteCommand(conn);
                    cmd.CommandText = "select * from DocumentType";
                    var docTypes = cmd.ExecuteQuery<DocumentType>();
                    cmd.CommandText = "select * from DocumentTitle"; 
                    var r = cmd.ExecuteQuery<DocumentTitle>();
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
                    SearchView search = FindViewById<SearchView>(Resource.Id.searchView1);
                    search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    search.QueryTextSubmit += Search_QueryTextSubmit;

                }
            }

            else
            {
                ActivityCompat.RequestPermissions(this, permissions, 1); this.Recreate();
                
            }
            FindViewById<FloatingActionButton>(Resource.Id.searchFAB).Click +=delegate { Search(FindViewById<SearchView>(Resource.Id.searchView1).Query); };
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
        //Add to main app
        //Spinner Selection Statements
        //34 Lines of code
        private void Spinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            type = String.Format("{0}", spinner.GetItemAtPosition(e.Position));
            // Toast.MakeText(this, type, ToastLength.Long).Show();
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                Spinner spinnerz = FindViewById<Spinner>(Resource.Id.spinner2);
                List<string> catechismFiles = new List<string>();
                var cmd = new SQLite.SQLiteCommand(conn);

                if (type.ToUpper() == "ALL")
                    cmd.CommandText = "select * from DocumentTitle";
                else
                {
                    cmd.CommandText = LayoutString(type.ToUpper()); //"Select documenttype.*,documenttitle.* from documenttitle natural join documenttype where documenttitle.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName='CATECHISM' ";
                }
                var r = cmd.ExecuteQuery<DocumentTitle>();
                List<string> items = new List<string>();
                foreach (var item in r)
                    items.Add(item.Title);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerz.ItemSelected += Spinner2_ItemSelected;
                spinnerz.Adapter = adapter;
            switch(type.ToUpper())
                {
                    case "ALL": this.allOpen = true; this.confessionOpen = false; this.catechismOpen = false; this.creedOpen = false; this.helpOpen = false;break;
                    case "CONFESSION": this.allOpen = false; this.confessionOpen = true; this.catechismOpen = false; this.creedOpen = false; this.helpOpen = false; break;
                    case "CATECHISM": this.allOpen = false; this.confessionOpen = false; this.catechismOpen = true; this.creedOpen = false; this.helpOpen = false; break;
                    case "CREED": allOpen = false; creedOpen = true; catechismOpen = false; confessionOpen = false; helpOpen = false;break;
                }
                Toast.MakeText(this, type, ToastLength.Short).Show();
            }
        }
      //Add or modify in existing app
        //42 lines of code
        [Java.Interop.Export("SearchType")]
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
                    searchView.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    searchView.QueryTextSubmit += Search_QueryTextSubmit;
                    searchView.SetInputType(Android.Text.InputTypes.ClassNumber);
                    searchView.SetQueryHint("Search By Number...");
                    text.Text = "Search";
                }
            }
            else if (radio == FindViewById<RadioButton>(Resource.Id.viewAllRadio))
            {
                text.Text = "Read";
                searchView.Enabled = false;
                //if (!creedOpen)
                //{
                //    searchView.SetIconifiedByDefault(true);
                //    searchView.Enabled = false;

                //}
            }
        }
        #region Add To Main App
        //SQL Queries
        public string TableAccess(string var1)
        {
            string var2 = String.Format("Select documenttype.*,documenttitle.* from documenttitle natural join documenttype where documenttitle.documenttypeid= documenttype.documenttypeid {0} ", var1);
            return var2;
        }
        public string LayoutString(string var1)
        {
            string var2 = string.Format("Select documenttype.*,documenttitle.* from documenttitle natural join documenttype where documenttitle.documenttypeid= documenttype.documenttypeid and documenttype.DocumentTypeName='{0}'", var1);

            return var2;
        }
        public string DataTableAccess(string var1)
        {
            string var2 = string.Format("Select Documenttitle.documentName, document.documentid,documenttitle.documentid, document.DocIndexNum, " + "document.chname, document.chText, document.chproofs,document.ChTags," + " document.ChMatches from documentTitle natural join document where document.DocumentID=DocumentTitle.DocumentID {0}", var1);
            return var2;
        } 
        #endregion
        private void Search_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            recurCall++;
            if (recurCall == 1)
            {
                string query = e.Query;
                Search(query);
            }
        }
        //Add to original app
        private void Spinner2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            fileName = String.Format("{0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, fileName, ToastLength.Long).Show();
        }
        //Modify Existing Code on Main App
        //207 Lines of Code
        private void Search(string query)
        {
            int docCount = 1;
            bool truncate = false;
            Log.Info("Search()", String.Format("Search Begins" + ""));
            searchFragmentActivity = new SearchFragmentActivity();
           // RadioButton radio = FindViewById<RadioButton>(Resource.Id.viewAllRadio);
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                var cmd = new SQLite.SQLiteCommand(conn); var searchStr = new SQLite.SQLiteCommand(conn);
                bool proofs = true, answers = true, searchAll = false, viewDocs = false;
                CheckBox answerCheck = FindViewById<CheckBox>(Resource.Id.AnswerBox), proofCheck = FindViewById<CheckBox>(Resource.Id.proofBox),
            searchCheck = FindViewById<CheckBox>(Resource.Id.searchAllCheckBox);
                RadioButton viewRadio = FindViewById<RadioButton>(Resource.Id.viewAllRadio);
                Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1), spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);
                spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
                spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner2_ItemSelected);
               
                string fileString = "", accessString = "";
                accessString = DataTableAccess("");
                if (searchCheck.Checked)
                    searchAll = true;
                else
                { searchAll = false; accessString = DataTableAccess(string.Format(" and documenttitle.documentname = '{0}' ", fileName));
                }
                //Data filters
                if (allOpen)
                {
                    if (searchAll)
                    {
                        fileString = "select * from Documenttitle";
                    }
                    else
                    {
                        fileString = String.Format("select * from Documenttitle where Documenttitle.DocumentName='{0}'", fileName);

                    }

                }
                if (catechismOpen)
                {
                    if (searchAll)
                    {
                        fileString = TableAccess("and documenttype.DocumentTypeName='CATECHISM'");
                    }
                    else
                        fileString = TableAccess(String.Format(" and documenttype.DocumentTypeName='CATECHISM' and DocumentName='{0}' ", fileName));
                }
                if (confessionOpen)
                {
                    if (searchAll)
                        fileString = TableAccess("and documenttype.DocumentTypeName='CONFESSION' ");
                    else
                        fileString = TableAccess(String.Format(" and documenttype.DocumentTypeName='CONFESSION' and DocumentName='{0}'  ", fileName));

                }
                if (creedOpen)
                {
                    if (searchAll)
                        fileString = TableAccess("and documenttype.DocumentTypeName='CREED' ");
                    else
                        fileString = TableAccess(string.Format("and documenttype.DocumentTypeName='CREED' and DocumentName='{0}' ", fileName));
                }
                //Proofs enabled
                if (proofCheck.Checked)
                { proofs = true; }
                else
                    proofs = false;
                //Read Document
                if (viewRadio.Checked)
                {
                    viewDocs = true;
                }
                else
                {
                    viewDocs = false;
                }
                cmd.CommandText = fileString;
                searchStr.CommandText =accessString;
                var r = cmd.ExecuteQuery<DocumentTitle>();
                var searchFields = searchStr.ExecuteQuery<Document>();
                documentList = new DocumentList();
                //Add Entries to DocumentList
                for (int x = 0; x < searchFields.Count; x++)
                {
                    DocumentTitle docTitle = new DocumentTitle();
                    docTitle.DocumentID = searchFields[x].DocumentID;
                    for(int y=0;y<r.Count;y++)
                    if (!r[y].DocumentID.Equals(docTitle.DocumentID))
                    {
                        foreach (DocumentTitle doc in r)
                            if (doc.DocumentID == docTitle.DocumentID)
                            { docTitle.Title = doc.Title; }
                            else
                                continue;
                    }
                    else
                    {
                        docTitle.Title = r[y].CompareIDs(docTitle.DocumentID);
                    }
                        searchFields[x].DocumentName = docTitle.Title;
                        Document document = new Document();
                        document.ChName = searchFields[x].ChName;
                        document.DocDetailID = searchFields[x].DocDetailID;
                        document.DocumentText = Formatter(searchFields[x].DocumentText);
                        document.DocumentName = searchFields[x].DocumentName;
                        document.ChNumber = searchFields[x].ChNumber;
                        document.ChProofs = Formatter(searchFields[x].ChProofs);
                        document.Tags = searchFields[x].Tags;
                        documentList.Add(document);
                    
                }
                if (FindViewById<CheckBox>(Resource.Id.truncateCheck).Checked)
                    truncate = true;
                if (viewRadio.Checked != true && query != "" && FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
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
                else if (viewDocs)
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
                    searchFragmentActivity.DisplayResults(documentList, viewPager, adapter, query, 0, truncate);
                }
                else
                {
                    stopwatch.Stop();
                    if (this.documentList.Count == 0)
                    {
                        #region Error Logging
                        Log.Info("Search()", String.Format("No Results were found for {0}", query));
                        Toast.MakeText(this, String.Format("No results were found for  {0}", query), ToastLength.Long).Show();
                        #endregion
                        #region Variable Declaration and Assignment

                        SetContentView(Resource.Layout.errorLayout);
                        TextView errorMsg = FindViewById<TextView>(Resource.Id.errorTV);
                        errorMsg.Text = String.Format("No Search Results were found for {0}\r\n\r\n" +
                            "Go back to home page to search for another topic", query);


                        #endregion
                        #region Dialog Box
                        Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                        alert.SetTitle("No Results Found");
                        alert.SetMessage(String.Format("No Results were found for {0}.\r\n\r\n" +
                            "Do you want to go back and search for another topic?", query));
                        alert.SetPositiveButton("Yes", (senderAlert, args) =>
                        {
                            intent = new Intent(this, Class);
                            searchFragmentActivity = null;
                            this.OnStop();
                            this.Finish();
                            StartActivity(intent);
                        });
                        alert.SetNegativeButton("No", (senderAlert, args) => { alert.Dispose(); });

                        Dialog dialog = alert.Create();
                        dialog.Show();
                        #endregion

                    }
                    else
                    {
                        //SetTitle();
                        Document document = this.documentList[this.documentList.Count-1];
                        SetContentView(Resource.Layout.confession_results);
                        TextView chapterBox = FindViewById<TextView>(Resource.Id.chapterText);
                        TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
                        TextView chNumbBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
                        TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);

                            chapterBox.Text = document.DocumentText;
                        chNumbBox.Text = String.Format("Chapter {0} : {1}", document.ChNumber.ToString(), document.ChName);
                        proofBox.Text = document.ChProofs;
                        docTitleBox.Text = document.DocumentName;
                        TextView proofView = FindViewById<TextView>(Resource.Id.proofLabel);
                        ChangeColor(true, Android.Graphics.Color.Black, chapterBox, proofBox, chNumbBox, docTitleBox);
                        ChangeColor(proofView, false, Android.Graphics.Color.Black);
                        shareList = docTitleBox.Text + newLine + chNumbBox.Text + newLine + chapterBox.Text + newLine + "Proofs" + newLine + proofBox.Text;
                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
                        ChangeColor(fab, Android.Graphics.Color.Black);

                        fab.Click += ShareContent;
                    }
                }
            }
        }

        //Modify existing methods in original app
        public void FilterResults(DocumentList list, bool truncate, bool answers, bool proofs, bool allDocs, string searchTerm)
        {
            DocumentList resultList = new DocumentList();
            Log.Info("Filter", "Filtering Confession Results");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            Log.Debug("Timer", "Timer Started");
            Regex regex = new Regex(searchTerm, RegexOptions.IgnoreCase);

            Log.Info("Filter Results", "Filtering has begun");
            foreach (Document document in list)
            {
                string[] searchEntries = new string[4];
                searchEntries[0] = document.ChName;
                searchEntries[1] = document.DocumentText;
                searchEntries[2] = document.ChProofs;
                searchEntries[3] = document.Tags;
                
                foreach (string word in searchEntries) //this[i].Split(' '))
                {
                    string[] pieces = word.Split(' ');
                    foreach(string chunks in pieces)
                    if (regex.IsMatch(chunks))
                        document.Matches++;

                }
                if (document.Matches >= 1)
                    resultList.Add(document);
                stopwatch.Stop();
                Log.Debug("Timer", String.Format("{0}ms Passed", stopwatch.ElapsedMilliseconds.ToString()));
                stopwatch.Reset();
            }
    
            list = resultList;
            list.Sort(Document.CompareMatches);
         //   list.Reverse();
            this.documentList = (DocumentList)list;
        }


        ////Search By Number
        public void FilterResults(DocumentList list, bool truncate, bool answers, bool proofs, bool allDocs, int searchTerm)
        {
            DocumentList resultList = new DocumentList();
            foreach (Document document in list)
                if (document.ChNumber == searchTerm)
                    resultList.Add(document);

            list = resultList;
            list.Sort();
            this.documentList = list;
            
        }
        //Add to Main app
        public class DocumentTitle
        {
            [PrimaryKey, AutoIncrement]
            public int DocumentID { get; set; }
            [Column("DocumentName")]
            public string Title { get; set; }
            [Column("DocumentTypeID")]
            private int DocumentTypeID { get; set; }
            public int CompareTo(DocumentTitle compareDocument)
            {
                return this.DocumentID.CompareTo(this.DocumentID);
            }
           public  string CompareIDs(int id1)
            {
                if (id1 == this.DocumentID)
                    return this.Title;
                else
                    return "";
            }
            // public int ArtistId { get; set; }
        }

        public class Document
        {
            [PrimaryKey, AutoIncrement]
            public int DocDetailID { get; set; }
            [Column("ChText")]
            public string DocumentText { get; /*{ return Formatter(this.DocumentText); }*/ set; /*{ this.DocumentText = Formatter(value); }*/ }
            [Ignore]
            public string DocumentName { get; set; }
            [Column("DocumentID")]
            public int DocumentID
            { get; set; }
            [Column("DocIndexNum")]
            public int ChNumber { get; set; }
            [Column("ChProofs")]
            public string ChProofs { get; /*{ return Formatter(this.ChProofs); }*/ set; } //{ this.ChProofs = Formatter(value); }}
            [Column("ChName")]
            public string ChName { get; set; }
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
            [PrimaryKey, AutoIncrement]
            public int DocumentTypeID { get; set; }

            [Column("DocumentTypeName")]
            public string DocumentTypeName { get; set; }

        }
        public class DocumentList : List<Document>
        {
            string title ="";
            public DocumentList()
            {
                this.title = "";
            }
            public DocumentList(string titleValue)
            {
                this.title = titleValue;
            }
            public string Title
            {
                get { return this.title; }
                set { this.title = value; }
            }
        }
        public void Home()
        {
            string TITLE = "Go Home?", MESSAGE = "Do you want to search for another topic?";
            #region Dialog Box Creation   
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle(TITLE);
            alert.SetMessage(MESSAGE);
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                searchFragmentActivity.Finish();
                Log.Debug("Timer", String.Format("{0} ms passed", stopwatch.ElapsedMilliseconds.ToString()));
                stopwatch.Reset();
                intent = new Intent(this, Class);
                searchFragmentActivity = null;
                this.OnStop();
                this.Finish();
                StartActivity(intent);
            });
            alert.SetNegativeButton("No", (senderAlert, args) => { alert.Dispose(); });
            Dialog dialog = alert.Create();
            dialog.Show();
            #endregion
        }
        public override void OnBackPressed()
        {
            //If results are showing, Go to opening page
            if (searchFragmentActivity != null)
                //Home method executed
                Home();
            //Exit app
            else
            { //Dialog Box Creation
                string TITLE = "Exit App?", MESSAGE = "Are you sure you want to leave?";
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle(TITLE);
                alert.SetMessage(MESSAGE);
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    Finish();
                    Toast.MakeText(this, "Don't forget to rate this app if you haven't already", ToastLength.Short).Show(); alert.Dispose();
                });
                alert.SetNegativeButton("No", (senderAlert, args) => { alert.Dispose(); });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }
        private void ShareContent(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            string INTENTNAME = "SHARE";
            sendIntent.PutExtra(Intent.ExtraText, shareList);
            sendIntent.SetType("text/plain");
            StartActivity(Intent.CreateChooser(sendIntent, INTENTNAME));
        }
        //Floating Action Button Rendering
        public void ChangeColor(FloatingActionButton button, Android.Graphics.Color color)
        {
            button.SetBackgroundColor(color);
            button.SetImageResource(Resource.Drawable.abc_ic_menu_share_mtrl_alpha);
        }
        public void ChangeColor(TextView view, bool selectable, Android.Graphics.Color color)
        {
            view.SetTextColor(color);
            view.SetTextIsSelectable(selectable);
        }
        //TextView Text Color change event
        public void ChangeColor(bool selectable, Android.Graphics.Color color, params TextView[] views)
        {
            foreach (TextView view in views)
            {
                view.SetTextColor(color);
                view.SetTextIsSelectable(selectable);
            }
        }
    }
}