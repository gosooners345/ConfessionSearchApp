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
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using DocumentClass;


namespace ConfessionSearchApp
{


    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme", MainLauncher = true, ClearTaskOnLaunch = true)]
    public class MainActivity : AppCompatActivity
    {
        public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public int recurCall = 0;
        string shareList = "";
        string newLine = "\r\n";
        private static bool menuOpen;
        private FloatingActionButton fab, fab1, fab2, fab3, fab4, fab5;
        private bool confessionOpen, catechismOpen, creedOpen,helpOpen;
        private View view;
        private static string fileName = "", search = "";
        SearchFragmentActivity searchFragmentActivity;
        List<KeyValuePair<string, string>> files;
        Intent intent;
        DocumentList documentList, resultList = new DocumentList();
#pragma warning disable IDE0044 // Add readonly modifier
        List<string> fileNameList;
#pragma warning restore IDE0044 // Add readonly modifier
        //App Loading screen
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState); 

            SetContentView(Resource.Layout.catechism_layout); SetTitle(Resource.String.app_name);
            AppCenter.Start("c8e96422-f89a-45cf-b331-0c3bc585ee00",
                   typeof(Analytics), typeof(Crashes));
            SetTheme(Resource.Style.AppTheme);
            SearchView search = FindViewById<SearchView>(Resource.Id.searchView1);
            search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
            fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
            fab.Click += Fab_Click;
            LayoutChanged("CATECHISM");

        }
        //Close search menu after selection is made
        private void CloseFabMenu()
        {
            fab4 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton4);
            fab1 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton1);
            fab2 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton2);
            fab3 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton3);
            view = FindViewById<View>(Resource.Id.fabMenu);
            menuOpen = false;
            //fab5.Animate().Rotation(0f);
            view.Animate().Alpha(0f);
            fab1.Visibility = ViewStates.Gone;
            fab2.Visibility = ViewStates.Gone;
            fab3.Visibility = ViewStates.Gone;
            fab4.Visibility = ViewStates.Gone;
        }
        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }

        // Open Search Menu
        private void Fab_Click(object o, EventArgs e)
        {
            TextView text1, text2, text3, text4, text5, text6;

            menuOpen = true;

            fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
            fab.Animate().Rotation(135f);
            SetContentView(Resource.Layout.fabLayout);
            fab5 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton5);
            fab5.Click += delegate { Toast.MakeText(this, "Select an option from above", ToastLength.Long).Show(); };
            fab1 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton1);
            fab1.Click += CreedLayout;
            fab2 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton2);
            fab2.Click += CatechismLayout;
            fab3 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton3);
            fab3.Click += ConfessionLayout;
            fab4 = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton4);
            fab4.Click += HelpLayout;
            fab1.Visibility = ViewStates.Visible;
            fab2.Visibility = ViewStates.Visible;
            fab3.Visibility = ViewStates.Visible;
            fab4.Visibility = ViewStates.Visible;

            view = FindViewById<View>(Resource.Id.fabMenu);

            view.Animate().Alpha(1f);
            text1 = FindViewById<TextView>(Resource.Id.searchItem);
            text2 = FindViewById<TextView>(Resource.Id.helpItem);
            text3 = FindViewById<TextView>(Resource.Id.creedItem);
            text4 = FindViewById<TextView>(Resource.Id.catechismItem);
            text5 = FindViewById<TextView>(Resource.Id.confessionItem);
            text6 = FindViewById<TextView>(Resource.Id.closeItem);
            text1.Visibility = ViewStates.Visible;
            text2.Visibility = ViewStates.Visible;
            text3.Visibility = ViewStates.Visible;
            text4.Visibility = ViewStates.Visible;
            text5.Visibility = ViewStates.Visible;
            text6.Visibility = ViewStates.Visible;
            text6.Text = "Choose item from above";
            fab5.Visibility = ViewStates.Visible;

            ChangeColor(false, Android.Graphics.Color.Black, text1, text2, text3, text4, text5, text6);


        }
        #region private class in Activity
        private class FabAnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
        {
            View[] viewsToHide;

            public FabAnimatorListener(params View[] viewsToHide)
            {
                this.viewsToHide = viewsToHide;
            }

            public void OnAnimationCancel(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                if (!menuOpen)
                    foreach (var view in viewsToHide)
                        view.Visibility = ViewStates.Gone;
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
            }
        }
        #endregion
        //Catechism Page Layout
        private void CatechismLayout(object o, EventArgs e)
        {
            string type = "CATECHISM";
            CloseFabMenu(); LayoutChanged(type);




        }
        // Confession Page layout
        private void ConfessionLayout(object o, EventArgs e)
        {
            string type = "CONFESSION";
            CloseFabMenu(); LayoutChanged(type);


        }

        //Creed Page layout
        private void CreedLayout(object o, EventArgs e)
        {
            string type = "CREED";
            CloseFabMenu(); LayoutChanged(type);
        }

        // Help page method
        private void HelpLayout(object o, EventArgs e)
        {
            string type = "HELP";
            CloseFabMenu();LayoutChanged(type);
        }

        // Change Document types
        private void LayoutChanged(string type)
        {
            switch (type)
            {
                case "CONFESSION":
                    confessionOpen = true; catechismOpen = false; creedOpen = false; helpOpen = false;
                    SetContentView(Resource.Layout.confession_layout);
                    //SearchView elements
                    SearchView search = FindViewById<SearchView>(Resource.Id.searchView1);
                    search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    search.QueryTextSubmit += Search_QueryTextSubmit;
                    //Spinner elements
                    Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
                    files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Confession of Faith 1646","Westminster Confession of Faith 1646"),
        new KeyValuePair<string, string>("2nd London Baptist Confession of Faith", "2nd London Baptist Confession of Faith"),
       new KeyValuePair<string,string>("1618 Belgic Confession Of Faith", "1618 Belgic Confession Of Faith"),
       new KeyValuePair<string, string>("1658 Savoy Declaration","1658 Savoy Declaration")
        };
                    List<string> catechismFiles = new List<string>();
                    foreach (var item in files)
                    {
                        catechismFiles.Add(item.Key);
                    }
                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
                    var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.confession_list, Android.Resource.Layout.SimpleSpinnerItem);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;
                    //Floating Action Button/TextView setup
                    ChangeColor(FindViewById<TextView>(Resource.Id.textView1), false, Android.Graphics.Color.Black);
                    FindViewById<RadioButton>(Resource.Id.topicRadio).PerformClick();
                    fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
                    fab.Click += Fab_Click;
                    FloatingActionButton button = FindViewById<FloatingActionButton>(Resource.Id.searchFAB);
                    button.Click += delegate { Search(search.Query); };
                    break;
                case "CATECHISM":
                    catechismOpen = true; confessionOpen = false; creedOpen = false; helpOpen = false;
                    SetContentView(Resource.Layout.catechism_layout);
                    //SearchView elements
                    search = FindViewById<SearchView>(Resource.Id.searchView1);
                    search.SetImeOptions(Android.Views.InputMethods.ImeAction.Go);
                    search.QueryTextSubmit += Search_QueryTextSubmit;
                    spinner = FindViewById<Spinner>(Resource.Id.spinner1);
                    ChangeColor(FindViewById<TextView>(Resource.Id.textView1), false, Android.Graphics.Color.Black);
                    files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Larger Catechism","Westminster Larger Catechism"),
        new KeyValuePair<string, string>("Westminster Shorter Catechism", "Westminster Shorter Catechism"),
        new KeyValuePair<string,string>("Heidelberg Catechism","Heidelberg Catechism")
        };
                    catechismFiles = new List<string>();
                    foreach (var item in files)
                    {
                        catechismFiles.Add(item.Key);

                    }
                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
                    adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, catechismFiles);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;
                    FindViewById<RadioButton>(Resource.Id.topicRadio).PerformClick();
                    fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
                    fab.Click += Fab_Click;
                    button = FindViewById<FloatingActionButton>(Resource.Id.searchFAB);
                    button.Click += delegate { Search(search.Query); };
                    break;
                case "CREED":
                    creedOpen = true; catechismOpen = false; confessionOpen = false; helpOpen = false;
                    SetContentView(Resource.Layout.creed_Layout);
                    spinner = FindViewById<Spinner>(Resource.Id.spinner1);
                    files = new List<KeyValuePair<string, string>>
       {
       new KeyValuePair<string, string>("Apostle\'s Creed","Apostle\'s Creed"),
       new KeyValuePair<string, string>("Nicene Creed", "Nicene Creed"),
       new KeyValuePair<string, string>("Athanasian Creed","Athanasian Creed")
       };
                    catechismFiles = new List<string>();
                    foreach (var item in files)
                    {
                        catechismFiles.Add(item.Key);
                    }
                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
                    adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, catechismFiles);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;
                    FindViewById<RadioButton>(Resource.Id.viewAllRadio).PerformClick();
                    ChangeColor(FindViewById<TextView>(Resource.Id.textView1), false, Android.Graphics.Color.Black);
                    fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
                    fab.Click += Fab_Click;
                    button = FindViewById<FloatingActionButton>(Resource.Id.searchFAB);
                    button.Click += delegate { Search(""); };
                    break;
                case "HELP":
                    creedOpen = false; catechismOpen = false; confessionOpen = false; helpOpen = true;
                    SetContentView(Resource.Layout.main);
                    FindViewById<TextView>(Resource.Id.appTitle).Text = "Help Page";
                    FindViewById<TextView>(Resource.Id.searchByLabel).Text = GetString(Resource.String.catechism_help);
                    FindViewById<TextView>(Resource.Id.catechismPgh).Text = GetString(Resource.String.catechismPgh);
                    FindViewById<TextView>(Resource.Id.confessionHelp).Text = GetString(Resource.String.confession_help);
                    FindViewById<TextView>(Resource.Id.confessionPgh).Text = GetString(Resource.String.confessionPgh);
                    FindViewById<TextView>(Resource.Id.creedHelp).Text = "Creed Help:";
                    FindViewById<TextView>(Resource.Id.creedPgh).Text = GetString(Resource.String.creedPgh);
                    FindViewById<TextView>(Resource.Id.otherStuff).Text = "Other Stuff:";
                    FindViewById<TextView>(Resource.Id.otherPgh).Text = GetString(Resource.String.other_pgh);
                    fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
                    fab.Click += Fab_Click;
                    FindViewById<TextView>(Resource.Id.sourceTV).Text = "All documents listed below are public domain, the websites below helped me with collecting them. \nThe formatting on the page was used as a guide to formatting the files needed for the app." +

"    \n\nApostle's Creed: https://reformed.org/documents/apostles_creed.html " +
"    \n1618 Belgic Confession: https://reformed.org/documents/BelgicConfession.html " +
    "\n1646 Westminster Confession of Faith: https://reformed.org/documents/wcf_with_proofs/index.html" +
    "\n1689 London Baptist Confession of Faith: https://reformed.org/documents/baptist_1689.html" +
    "\n1658 Savoy Declaration of Faith and Order: https://reformed.org/documents/Savoy_Declaration/index.html" +
   "\nWestminster Shorter Catechism: https://reformed.org/documents/wsc/index.html" +
   " \nWestminster Larger Catechism: https://reformed.org/documents/wlc_w_proofs/index.html " +
"    \nHeidelberg Catechism: https://reformed.org/documents/heidelberg.html " +
    "\nNicean Creed: https://reformed.org/documents/nicene.html " +
   "\nAthanasian Creed: https://reformed.org/documents/athanasian.html  ";
                             
                    break;

            }
        }
        #region menu stuff
        //Help menu Items
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.layoutMenu, menu);
            return true;
            // return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            IMenuItem helpItem = menu.FindItem(Resource.Id.help_menu_item_layout);
            helpItem.SetVisible(true);


            return base.OnPrepareOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {

                case Resource.Id.help_menu_item_layout:
                    string type = "HELP"; LayoutChanged(type);break; 
            }

            return base.OnOptionsItemSelected(item);
        }
        #endregion
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

        //fill Document list
        private void Fill(DocumentList documentList, int ID, string[] files, bool answers, bool proofs, AssetManager asset, bool allSearch)
        {
            if (files[0] != null || files[0] != "" && allSearch)
            {
                foreach (string file1 in files)
                    this.documentList.Fill(file1, asset, 1, answers, proofs);
            }
            else
                this.documentList.Fill(fileName, asset, 0, answers, proofs);

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
                    searchView.SetQueryHint("Search By Number...");
                    text.Text = GetString(Resource.String.Search);
                }
            }
            else if (radio == FindViewById<RadioButton>(Resource.Id.viewAllRadio))
            {
                text.Text = GetString(Resource.String.View_Button);
                if (!creedOpen)
                {
                    searchView.SetIconifiedByDefault(true);
                 searchView.Enabled = false;

                }
                
            }

        }
        //Search by topic
        public void FilterResults(DocumentList documentList, bool truncate, bool answers, bool proofs, bool allDocs, string searchTerm)
        {
            resultList.Clear();
            Log.Info("Filter Results", "Filter has begun processing");
            //for (int y = 1; y <= documentList.Count; y++)
            foreach (Document document in documentList)
            {
                if (document.Type == "CONFESSION")
                {
                    Confession confession = (Confession)document;
                    confession.Filter(searchTerm, truncate);
                    if (confession.Matches >= 1)
                        resultList.Add(confession);
                }
                else if (document.Type == "CATECHISM")
                {
                    Catechism catechism = (Catechism)document;
                    catechism.Filter(searchTerm, truncate);
                    if (catechism.Matches >= 1)
                        resultList.Add(catechism);
                }
            }
            this.documentList = resultList;
            this.documentList.MergeSort(DocumentList.OrderEnum.MatchOrder);
        }
        //Search By Number
        public void FilterResults(DocumentList documentList, bool truncate, bool answers, bool proofs, bool allDocs, int searchTerm)
        {
            foreach (Document document in documentList)
                if (document.IDNumber == searchTerm)
                    resultList.Add(document);
            this.documentList = resultList;
            this.documentList.MergeSort();
        }
        //Search Method
        private void Search(string query)
        {
            #region Variable Declaration 
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            int docCount = 1;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            bool truncate = false;
            Log.Info("Search()", String.Format(Resource.String.search_execution_begins + ""));
            documentList = new DocumentList();
            RadioButton radio = FindViewById<RadioButton>(Resource.Id.viewAllRadio);
            this.searchFragmentActivity = new SearchFragmentActivity();
            AssetManager asset = this.Assets;
            //string searchTerm = "";
            bool proofs = true, answers = true, searchAll = false;
            string[] fileNames = new string[3];
            CheckBox answerCheck = FindViewById<CheckBox>(Resource.Id.AnswerBox), proofCheck = FindViewById<CheckBox>(Resource.Id.proofBox),
            searchCheck = FindViewById<CheckBox>(Resource.Id.searchAllCheckBox);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            #endregion
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner1_ItemSelected);
            //Search Options
            #region Search Options
            if (catechismOpen)
            {
                if (radio.Checked == false)
                {
                    if (answerCheck.Checked)
                        answers = false;
                    else
                        answers = true;
                    if (proofCheck.Checked)
                        proofs = false;
                    else
                        proofs = true;
                    if (searchCheck.Checked)
                    {
                        searchAll = true;
                        fileNames = new[] { "Westminster Larger Catechism.txt", "Westminster Shorter Catechism.txt", "Heidelberg Catechism.txt" };
                        docCount = 3;
                    }
                    else
                        searchAll = false;
                }
                else
                {
                    answers = true;
                    searchAll = false;
                    docCount = 1; proofs = true;
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
                        fileNames = new[] { "Westminster Confession of Faith 1646.txt", "2nd London Baptist Confession of Faith.txt", "1618 Belgic Confession Of Faith.txt","1658 Savoy Declaration.txt" };
                        docCount = 4;
                    }
                    else
                        searchAll = false;
                    docCount = 1;
                }
                else
                {
                    proofs = true; searchAll = false;
                    docCount = 1;
                }

            }
            else if (creedOpen)
            {
                if (searchCheck.Checked)
                {
                    searchAll = true;
                    docCount = 3;
                    fileNames = new[] { "Apostle\'s Creed.txt", "Nicene Creed.txt", "Athanasian Creed.txt" };
                }
                else
                {
                    proofs = false; searchAll = false; truncate = false;
                    docCount = 1;
                }
                answers = false;
            }
            Fill(this.documentList, 1, fileNames, answers, proofs, asset, searchAll);
            this.documentList.Truncate = truncate;
            
            search = query;
            //Results are truncated to the term if true
            if (FindViewById<CheckBox>(Resource.Id.truncateCheck).Checked)
                truncate = true;
            #endregion
            Log.Info("Search()", String.Format(Resource.String.search_status_sorting + ""));
            //If topic is filled
            if (radio.Checked != true && query != "" && FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
            {
                if (FindViewById<RadioButton>(Resource.Id.topicRadio).Checked)
                {
                    stopwatch.Start();
                    FilterResults(this.documentList, truncate, answers, proofs, searchAll,query);
                    this.documentList.Reverse();
                    stopwatch.Stop();
                }
            }
            //if Chapter is filled
            else if (FindViewById<RadioButton>(Resource.Id.chapterRadio).Checked & query != "")
            {
               // int searchInt;
                //if (Int32.TryParse(searchTerm, out searchInt))
                    int searchInt = Int32.Parse(query);
                    FilterResults(this.documentList, truncate, answers, proofs, searchAll, searchInt);

            }
            //if View Document is checked
            else if (FindViewById<RadioButton>(Resource.Id.viewAllRadio).Checked)
            {
                if (!FindViewById<CheckBox>(Resource.Id.searchAllCheckBox).Checked)
                    query = this.documentList.Title;
                else
                    query = "View All";
            }
            //Display Results
            Log.Info("Search Method", "Displaying Results");
            if (documentList.Count > 1)
            {
                SetContentView(Resource.Layout.search_results);
                ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                SearchAdapter adapter = new SearchAdapter(SupportFragmentManager, this.documentList, query, truncate);
                //Sets the final state of application
                this.searchFragmentActivity.DisplayResults(this.documentList, viewPager, adapter, query, 1, truncate);
                SetTitle(Resource.String.search_results_title);
            }
            //Single result or no results found
            else
            {
                stopwatch.Stop();
                //If There are no Search Results
                if (this.documentList.Empty)
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
                    alert.SetTitle(Resource.String.zero_results_title);
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
                    SetTitle(Resource.String.error_results);

                }
                //If there is only 1 search result  
                else
                {
                    SetTitle(Resource.String.search_results_title);
                    Document document = this.documentList[this.documentList.Count];
                    //Display Catechism Results
                    if (document.Type == "CATECHISM")
                    {

                        this.SetContentView(Resource.Layout.catechism_Results);
                        TextView questionBox = FindViewById<TextView>(Resource.Id.chapterText);
                        TextView answerBox = FindViewById<TextView>(Resource.Id.answerText);
                        TextView numberBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
                        TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
                        TextView proofView = FindViewById<TextView>(Resource.Id.proofLabel);
                        TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
                        Catechism catechism = (Catechism)document;
                        questionBox.Text = catechism.Question;
                        answerBox.Text = catechism.Answer;
                        numberBox.Text = String.Format("Question {0}: {1}", catechism.IDNumber.ToString(), catechism.Title);
                        proofBox.Text = catechism.Proofs;
                        docTitleBox.Text = catechism.DocTitle;
                        ChangeColor(true, Android.Graphics.Color.Black, questionBox, answerBox, proofBox, numberBox, docTitleBox);
                        ChangeColor(false, Android.Graphics.Color.Black, proofView, FindViewById<TextView>(Resource.Id.catechismAnswerLabel));
                        shareList = docTitleBox.Text + newLine + String.Format("Question {0}:", catechism.IDNumber.ToString("d3")) + newLine + questionBox.Text + newLine + "Answer:" + newLine +
        answerBox.Text + newLine + "Proofs:" + newLine + proofBox.Text;
                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
                        ChangeColor(fab, Android.Graphics.Color.Black);

                        fab.Click += ShareContent;
                    }
                    //Display Confession Results
                    else if (document.Type == "CONFESSION")
                    {
                        SetContentView(Resource.Layout.confession_results);
                        TextView chapterBox = FindViewById<TextView>(Resource.Id.chapterText);
                        TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
                        TextView chNumbBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
                        TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
                        Confession confession = (Confession)document;
                        chapterBox.Text = confession.Chapter;
                        chNumbBox.Text = String.Format("Chapter {0} : {1}", confession.IDNumber.ToString(), confession.Title);
                        proofBox.Text = confession.Proofs;
                        docTitleBox.Text = confession.DocTitle;
                        TextView proofView = FindViewById<TextView>(Resource.Id.proofLabel);
                        ChangeColor(true, Android.Graphics.Color.Black, chapterBox, proofBox, chNumbBox, docTitleBox);
                        ChangeColor(proofView, false, Android.Graphics.Color.Black);
                        shareList = docTitleBox.Text + newLine + chNumbBox.Text + newLine + chapterBox.Text + newLine + "Proofs" + newLine + proofBox.Text;
                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
                        ChangeColor(fab, Android.Graphics.Color.Black);

                        fab.Click += ShareContent;
                    }
                    //Display Creed Results
                    else if (document.Type == "CREED")
                    {
                        SetContentView(Resource.Layout.creed_Results);
                        Creed creed = (Creed)document;
                        TextView creedBox = FindViewById<TextView>(Resource.Id.chapterText);
                        TextView titleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
                        SetTitle(Resource.String.search_results_title);
                        creedBox.Text = creed.CreedText; titleBox.Text = documentList.Title;
                        shareList = titleBox.Text + newLine + creed.CreedText;
                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
                        ChangeColor(fab, Android.Graphics.Color.Black);
                        ChangeColor(true, Android.Graphics.Color.Black, creedBox, titleBox);
                        fab.Click += ShareContent;
                    }
                }
            }
            Toast.MakeText(this, String.Format("Search Completed for {0}" + "\r\n" + "{1} ms Passed",query, stopwatch.ElapsedMilliseconds.ToString()), ToastLength.Long).Show();
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
            fileName = String.Format("{0}.txt", spinner.GetItemAtPosition(e.Position));
        }
        //Return to home page
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
