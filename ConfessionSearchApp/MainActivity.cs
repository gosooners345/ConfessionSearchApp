using System;
using System.Collections.Generic;
using Android.Animation;
using Android.App;
using Android.Content;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Analytics;    
using Android.Content.Res;

using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
//using DocumentClass;
using Android.Support.V4.App;
using SQLite;
using Android;
using System.IO;
using Android.Support.V4.Content;
using Android.Content.PM;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ConfessionSearchApp
{


    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = true, ClearTaskOnLaunch = true)]
    public class MainActivity : AppCompatActivity
    {
        public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public int recurCall = 0,recurCall2=0;
        string shareList = "";
        string type = "";
        string newLine = "\r\n";
        string header = "";
        string dbName = "confessionSearchDB.db";
        string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(),
           "confessionSearchDB.db");
        private static bool menuOpen;
        private bool allOpen, confessionOpen, catechismOpen, creedOpen, helpOpen;
        private View view;
        private static string fileName = "", search = "";
        SearchFragmentActivity searchFragmentActivity;
        Intent intent;
        DocumentList documentList, resultList = new DocumentList();
        //App Loading screen
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main); SetTitle(Resource.String.app_name);
            AppCenter.Start("c8e96422-f89a-45cf-b331-0c3bc585ee00",
                   typeof(Analytics), typeof(Crashes));
            SetTheme(Resource.Style.AppTheme);
            SearchView search = FindViewById<SearchView>(Resource.Id.searchView1);
            search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
            search.QueryTextSubmit += Search_QueryTextSubmit;
            string[] permissions = new string[2];
            permissions[0] = Manifest.Permission.WriteExternalStorage;
            permissions[1] = Manifest.Permission.ReadExternalStorage;

            // Set our view from the "main" layout resource

            Spinner spinner1 = FindViewById<Spinner>(Resource.Id.spinner1), spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);
            //ActivityCompat.RequestPermissions(this, permissions, 1);

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

                }
                
                    using (var conn = new SQLite.SQLiteConnection(dbPath))
                    {
                        var cmd = new SQLite.SQLiteCommand(conn);
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
                        search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                        search.QueryTextSubmit += Search_QueryTextSubmit;
                        FindViewById(Resource.Id.helpButton).Click += delegate
                        {
                            SetContentView(Resource.Layout.main);
                            creedOpen = false; catechismOpen = false; confessionOpen = false; helpOpen = true;
                            // SetContentView(Resource.Layout.main);
                            FindViewById<TextView>(Resource.Id.appTitle).Text = "Help Page";
                            FindViewById<TextView>(Resource.Id.searchByLabel).Text = GetString(Resource.String.catechism_help);
                            FindViewById<TextView>(Resource.Id.catechismPgh).Text = "Hitting the No Answers Checkbox will remove answers from your search results, '\n"
       + "thus affecting the overall search results on the page.\n"
     + " Hit the No proofs checkbox to exclude proofs from the search results\n";
                            FindViewById<TextView>(Resource.Id.confessionHelp).Text = GetString(Resource.String.confession_help);
                            FindViewById<TextView>(Resource.Id.confessionPgh).Text = GetString(Resource.String.confessionPgh);
                            FindViewById<TextView>(Resource.Id.creedHelp).Text = "Creed Help:";
                            FindViewById<TextView>(Resource.Id.creedPgh).Text = GetString(Resource.String.creedPgh);
                            FindViewById<TextView>(Resource.Id.otherStuff).Text = "Other Stuff:";
                            FindViewById<TextView>(Resource.Id.otherPgh).Text = GetString(Resource.String.other_pgh);
                            FindViewById<TextView>(Resource.Id.sourceTV).Text = "All documents listed below are public domain, the websites below helped me with collecting them. \n A big thanks for inspiration for this app came from this website." +

                            "\n\nApostle's Creed: https://reformed.org/documents/apostles_creed.html " +
                           "\n1618 Belgic Confession: https://reformed.org/documents/BelgicConfession.html " +
                           "\n1646 Westminster Confession of Faith: https://reformed.org/documents/wcf_with_proofs/index.html" +
                           "\n1689 London Baptist Confession of Faith: https://reformed.org/documents/baptist_1689.html" +
                           "\n1658 Savoy Declaration of Faith and Order: https://reformed.org/documents/Savoy_Declaration/index.html" +
                           "\nWestminster Shorter Catechism: https://reformed.org/documents/wsc/index.html" +
                           " \nWestminster Larger Catechism: https://reformed.org/documents/wlc_w_proofs/index.html " +
                           "\nHeidelberg Catechism: https://reformed.org/documents/heidelberg.html " +
                           "\nNicean Creed: https://reformed.org/documents/nicene.html " +
                            "\nAthanasian Creed: https://reformed.org/documents/athanasian.html  ";

                            FindViewById(Resource.Id.floatingActionButton).Click += delegate { this.Recreate(); };
                        };
                    }
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, permissions, 1); this.Recreate();
                }
                FindViewById<FloatingActionButton>(Resource.Id.searchFAB).Click += delegate { Search(FindViewById<SearchView>(Resource.Id.searchView1).Query); };
            }
        
        [Java.Interop.Export("Help")]
        private void HelpLayout(View view)
        {
            SetContentView(Resource.Layout.main);


        }

        //Search View Text Submit
        private void Search_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            recurCall++;
            if (recurCall == 1)
            {
                string query = e.Query;
                Search(query);
            }

        }

        // Search Type Selection
        [Java.Interop.Export("SearchType")]
        //Change Search Types
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
                    text.Text = GetString(Resource.String.Search);
                }
            }
            //Chapter/Question Number Search
            else if (radio == FindViewById<RadioButton>(Resource.Id.chapterRadio))
            {
                if (radio.Checked)
                {
                    searchView.Enabled = true;
                    searchView.SetInputType(Android.Text.InputTypes.ClassNumber);
                    searchView.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    searchView.SetQueryHint("Search By Number...");
                    text.Text = GetString(Resource.String.Search);
                    searchView.QueryTextSubmit += Search_QueryTextSubmit;
                }
            }
            else if (radio == FindViewById<RadioButton>(Resource.Id.viewAllRadio))
            {
                text.Text = GetString(Resource.String.View_Button);
                searchView.Enabled = false;
            }

        }
        //Search by topic
        public void FilterResults(DocumentList documentList, bool truncate, bool answers, bool proofs, bool allDocs, string searchTerm)
        {
            DocumentList resultList = new DocumentList();
            Log.Info("Filter", "Filtering Confession Results");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            Log.Debug("Timer", "Timer Started");
            Regex regex = new Regex(searchTerm, RegexOptions.IgnoreCase);
            Log.Info("Filter Results", "Filtering has begun");
            foreach (Document document in documentList)
            {
                string[] searchEntries = new string[4];
                searchEntries[0] = document.ChName;
                searchEntries[1] = document.DocumentText;
                searchEntries[2] = document.ChProofs;
                searchEntries[3] = document.Tags;

                foreach (string word in searchEntries) //this[i].Split(' '))
                {
                    string[] pieces = word.Split(' ');
                    foreach (string chunks in pieces)
                        if (regex.IsMatch(chunks))
                            document.Matches++;

                }
                if (document.Matches >= 1)
                { resultList.Add(document);
                    if (!answers)
                    {
                        if (document.DocumentText.Contains("Question"))
                         document.DocumentText=GetBetween(document.DocumentText, "Question:", "Answer:");
                        else
                            continue;
                    }
                    else if (!proofs)
                    {
                        document.ChProofs = "No Proofs available!";
                    }
                    else if(truncate)
                    {
                       document.DocumentText= GetBetween(document.DocumentText, document.DocumentText.Substring(0), searchTerm);
                       document.DocumentText= GetBetween(document.ChProofs, document.ChProofs.Substring(0), searchTerm);

                    }
                    

                }
                stopwatch.Stop();
                Log.Debug("Timer", String.Format("{0}ms Passed", stopwatch.ElapsedMilliseconds.ToString()));
                stopwatch.Reset();
            }
            resultList.Sort(Document.CompareMatches);
            this.documentList = resultList;
        }
        //Search By Number
        public void FilterResults(DocumentList documentList, bool truncate, bool answers, bool proofs, bool allDocs, int searchTerm)
        {
            foreach (Document document in documentList)
            {
                if (document.ChNumber == searchTerm)
                    resultList.Add(document);
                if (!answers)
                {
                    if (document.DocumentText.Contains("Question"))
                     document.DocumentText=   GetBetween(document.DocumentText, "Question:", "Answer:");
                    else
                        continue;
                }
                else if(!proofs)
                {
                    document.ChProofs = "No Proofs available!";
                }
                
                else
                    continue;
            }
            this.documentList = resultList;
            this.documentList.Sort();
        }
        //Search Method
        private void Search(string query)
        {
            #region Variable Declaration 
            int docCount = 1;
            bool truncate = false;
            Log.Info("Search()", String.Format(Resource.String.search_execution_begins + ""));
            searchFragmentActivity = new SearchFragmentActivity();

            #endregion
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
                if (answerCheck.Checked)
                { answers = false; }
                else { answers = true; }
                if (searchCheck.Checked)
                    searchAll = true;
                else
                {
                    searchAll = false; accessString = DataTableAccess(string.Format(" and documenttitle.documentname = '{0}' ", fileName));
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
                //Doctype=3
                if (catechismOpen)
                {
                    if (searchAll)
                    {
                        fileString = TableAccess(" documenttitle.DocumentTypeID=3");
                        docCount = 3;
                    }
                    else
                        fileString = TableAccess(String.Format(" documenttitle.DocumentTypeID=3 and DocumentName='{0}' ", fileName));
                }
                //DocType = 2
                if (confessionOpen)
                {
                    if (searchAll)
                    { fileString = TableAccess(" documenttitle.DocumentTypeID=2' "); docCount = 2; }

                    else
                        fileString = TableAccess(String.Format(" documenttitle.DocumentTypeID=2 and DocumentName='{0}'  ", fileName));

                }
                //DocType=1
                if (creedOpen)
                {
                    if (searchAll)
                    { fileString = TableAccess(" documenttitle.DocumentTypeID=1 "); docCount = 1; }
                    else
                        fileString = TableAccess(string.Format(" documenttitle.DocumentTypeID=1 and DocumentName='{0}' ", fileName));
                }
                //Proofs enabled
                if (proofCheck.Checked)
                { proofs = false; }
                else
                    proofs = true;
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
                searchStr.CommandText = accessString;
                var r = cmd.ExecuteQuery<DocumentTitle>();
                var searchFields = searchStr.ExecuteQuery<Document>();
                documentList = new DocumentList();
                //Add Entries to DocumentList
                for (int x = 0; x < searchFields.Count; x++)
                {
                    
                    DocumentTitle docTitle = new DocumentTitle();
                    docTitle.DocumentID = searchFields[x].DocumentID;
                    
                    for (int y = 0; y < r.Count; y++)
                        if (!r[y].DocumentID.Equals(docTitle.DocumentID))
                        {
                            foreach (DocumentTitle doc in r)
                                if (doc.DocumentID == docTitle.DocumentID)
                                { docTitle.Title = doc.Title; docTitle.DocumentTypeID = doc.DocumentTypeID; }
                                else
                                    continue;
                        }
                        else
                        {
                            docTitle.Title = r[y].CompareIDs(docTitle.DocumentID);
                        }
                    if (docTitle.Title == fileName | searchAll==true & docTitle.DocumentTypeID==docCount | searchAll & allOpen)

                    {
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
                    else
                        continue;
                }
                if (FindViewById<CheckBox>(Resource.Id.truncateCheck).Checked)
                    truncate = true;
                if (viewRadio.Checked != true && query != "" && FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
                {
                    if (FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
                    {
                        stopwatch.Start();
                        FilterResults(documentList, truncate, answers, proofs, searchAll, query);
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
                    SearchAdapter adapter = new SearchAdapter(SupportFragmentManager, documentList, query,truncate);
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
                        Document document = this.documentList[this.documentList.Count - 1];
                        SetContentView(Resource.Layout.confession_results);
                        TextView chapterBox = FindViewById<TextView>(Resource.Id.chapterText);
                        TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
                        TextView chNumbBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
                        TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
                        proofBox.Text = document.ChProofs;
                        docTitleBox.Text = document.DocumentName;
                        chapterBox.Text = document.DocumentText;
                        if (chapterBox.Text.Contains("Question"))
                        { header = "Question "; chNumbBox.Text = string.Format("{0} {1}: {2}", header, document.ChNumber.ToString(), document.ChName); }
                        else if (chapterBox.Text.Contains("I."))
                        { header = "Chapter"; chNumbBox.Text = string.Format("{0} {1}: {2}", header, document.ChNumber.ToString(), document.ChName); }
                        else
                        {
                            chNumbBox.Text = string.Format("{0}",document.DocumentName);
                        }
                        
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



            //If there is only 1 search result  

            Toast.MakeText(this, String.Format("Search Completed for {0}" + "\r\n" + "{1} ms Passed", query, stopwatch.ElapsedMilliseconds.ToString()), ToastLength.Long).Show();
        }

        public string GetBetween(string strSource, string strStart, string strEnd)
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
        //On Screen Search Button Pressed
        [Java.Interop.Export("Search")]
        public void Search(View view)
        {
            //Execute Search Method
            Search("");
        }
        // Method for Combobox Item Selected
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
                    cmd.CommandText = LayoutString(type.ToUpper()); 
                }
                var r = cmd.ExecuteQuery<DocumentTitle>();
                List<string> items = new List<string>();
                foreach (var item in r)
                    items.Add(item.Title);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerz.ItemSelected += Spinner2_ItemSelected;
                spinnerz.Adapter = adapter;
                switch (type.ToUpper())
                {
                    case "ALL": this.allOpen = true; this.confessionOpen = false; this.catechismOpen = false; this.creedOpen = false; this.helpOpen = false; break;
                    case "CONFESSION": this.allOpen = false; this.confessionOpen = true; this.catechismOpen = false; header = "Chapter "; this.creedOpen = false; this.helpOpen = false; break;
                    case "CATECHISM": this.allOpen = false;header = "Question "; this.confessionOpen = false; this.catechismOpen = true; this.creedOpen = false; this.helpOpen = false; break;
                    case "CREED": allOpen = false; creedOpen = true; catechismOpen = false; confessionOpen = false; helpOpen = false; break;
                }
              
            }
        }
        private void Spinner2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            fileName = String.Format("{0}", spinner.GetItemAtPosition(e.Position));
            //Toast.MakeText(this, fileName, ToastLength.Long).Show();
        }
        //Return to home page
        [Java.Interop.Export("Home")]
        public void Home(View view)
        {
            Home();
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
        //Overrides back button settings
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
    //Share Content
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
        public class DocumentTitle
        {
            [PrimaryKey, AutoIncrement]
            public int DocumentID { get; set; }
            [Column("DocumentName")]
            public string Title { get; set; }
            [Column("DocumentTypeID")]
            public int DocumentTypeID { get; set; }
            public int CompareTo(DocumentTitle compareDocument)
            {
                return this.DocumentID.CompareTo(this.DocumentID);
            }
            public string CompareIDs(int id1)
            {
                if (id1 == this.DocumentID)
                    return this.Title;
                else
                    return "";
            }
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
            string title = "";
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
        //SQL Queries
        public string TableAccess(string var1)
        {
            string var2 = "";
            if (var1 != "")
                 var2 = String.Format("Select documenttitle.* from documenttitle where {0}", var1);
            else
             var2="Select documenttype.*,documenttitle.* from documenttitle natural join documenttype where documenttitle.documenttypeid= documenttype.documenttypeid";
           
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
        
    }
}
