using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Runtime;
using System.IO;
using System.Drawing;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Xamarin.Android;
//using DocumentClass;
using Android.Support.Design.Widget;
using static ConfessionSearchDBApp.Main.MainActivity;
using ConfessionSearchDBApp.Main;

namespace ConfessionSearchDBApp
{
    [Activity(Label = "PageTwo", Theme = "@style/AppTheme", AllowEmbedded = false, ParentActivity = typeof(MainActivity))]
    class SearchFragmentActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.ShareActionProvider actionProvider;
        public SearchAdapter adapter;
        public string shareList;// = new List<string>();
                                //Android.Widget.ShareActionProvider actionProvider;
        public List<Document> documentList = new List<Document>();
        string files;
        public int index;
        public SearchFragmentActivity() { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        //Search feed for Fragments
        public void DisplayResults(List<Document> sourcedocumentList, ViewPager view, SearchAdapter search, string searchTerm, int count, bool truncate)
        {
            //Syncs the document List objects together
            documentList = sourcedocumentList;
            //Calls GetItem Method from Search Adapter for Display. Required for fragments to display in viewPager
            search.GetItem(count);
            // index = count;
            shareList = "";
            //Saves current Adapter state
            search.SaveState();
            //Displays results
            view.Adapter = search;

        }
    }
    //Search Adapter for ViewPager
    class SearchAdapter : FragmentStatePagerAdapter
    {
        public DocumentList documentList = new DocumentList();//, documentList1 = new DocumentList();
        private int docPosition = 0;
        private string term = "";
        private bool truncate = false;
        public Android.Support.V4.App.FragmentManager news;
        /// <summary>
        /// Constructor for Search Adapter 
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="documents"></param>
        /// <param name="searchTerm"></param>
        /// <param name="truncate"></param>
        public SearchAdapter(Android.Support.V4.App.FragmentManager fm, DocumentList documents, string searchTerm, bool truncate) : base(fm)
        {
            documentList = documents;
            this.truncate = truncate;
            news = fm;
            term = searchTerm;
        }
        //Required for Fragment adapter
        public override int Count { get { return documentList.Count; } }
   
        public override Android.Support.V4.App.Fragment GetItem(int position)
        { string title = "";
            Android.Support.V4.App.Fragment frg;
            
            Document document = documentList[position];
            string docTitle = "";
           
            title = document.DocumentName;
            if (title == "Results" || title== "")
              docTitle = document.DocumentName;
            else
              docTitle = document.DocumentName;
            docPosition++;
            frg = SearchResultFragment.NewResult(document.DocumentText, document.ChProofs, document.ChName, document.ChNumber, document.DocumentName, false, document.Matches);
            
            Log.Info("Search Adapter", String.Format("{0} Panel Loaded", position));
            return frg;
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (term == "")
                term = documentList[position + 1].DocumentName;
            return new Java.Lang.String(String.Format("Result {0} of {1} for {2}", position + 1, documentList.Count, term));

        }
    }
    //Fragment class for results
    //This weekend
    class SearchResultFragment : Android.Support.V4.App.Fragment
    {
        //required variables for fragment class
       // private string documentTitle;
       // // ShareActionProvider share;
        private static string PROOFS = "proofs", CHAPTER = "chapter",
     //   QUESTION = "question", TYPE = "type",
        TITLE = "title", ANSWER = "answer", DOCTITLE = "titles", truncated = "truncate", matchNumb = "matches";
        private static string number, match;
        public const string newLine = "\r\n";
        private static int chNumber, queNumber;
        private static string CREED = "creed";
        private static bool confession1, catechism1, creed1;
        private static bool truncate = false;

        //default constructor for fragment
        public SearchResultFragment() { }
        public string shareList = "";
        //Displays Search Results
        public static SearchResultFragment NewResult(string Chapter, string Proofs, string Title, int ID, string ListTitle, bool truncate, int matchnum)
        {
            Log.Info("Fragment Forming", "Grabbing new Confession Fragment to Display");
            SearchResultFragment fragment = new SearchResultFragment();
            Bundle spaces = new Bundle();
            confession1 = true; catechism1 = false; creed1 = false;
            spaces.PutInt(number, ID);
            spaces.PutInt(matchNumb, matchnum);
            spaces.PutString(CHAPTER, Chapter);
            spaces.PutString(PROOFS, Proofs);
            spaces.PutString(TITLE, Title);
            spaces.PutString(DOCTITLE, ListTitle);
            fragment.Arguments = spaces;
            return fragment;
        }
        //Displays the search Results
        //public static SearchResultFragment NewResult(string Question, string Answer, string Proofs, string Title, int ID, string ListTitle, int matchNum, bool truncate)
        //{
        //    Log.Info("Fragment Forming", "Grabbing new Catechism Fragment to Display");
        //    SearchResultFragment fragment = new SearchResultFragment();
        //    Bundle spaces = new Bundle();
        //    catechism1 = true; confession1 = false; creed1 = false;
        //    spaces.PutInt(number, ID);
        //    spaces.PutString(ANSWER, Answer);
        //    spaces.PutString(QUESTION, Question);
        //    spaces.PutInt(matchNumb, matchNum);
        //    spaces.PutString(TITLE, Title);
        //    spaces.PutString(PROOFS, Proofs);
        //    spaces.PutString(DOCTITLE, ListTitle);
        //    spaces.PutBoolean(truncated, truncate);
        //    fragment.Arguments = spaces;
        //    return fragment;
        //}
        //public static SearchResultFragment NewResult(string Title, string ListTitle, bool truncate, string creed, int ID, string Proofs, int matchNum)
        //{

        //    Log.Info("Fragment forming", "Grabbing new Creed fragment to display");
        //    SearchResultFragment fragment = new SearchResultFragment();
        //    Bundle spaces = new Bundle();
        //    creed1 = true; catechism1 = false; confession1 = false;
        //    spaces.PutInt(number, ID);
        //    spaces.PutString(CREED, creed);
        //    spaces.PutInt(matchNumb, matchNum);
        //    spaces.PutString(PROOFS, Proofs);
        //    spaces.PutString(DOCTITLE, ListTitle);
        //    spaces.PutBoolean(truncated, truncate);
        //    fragment.Arguments = spaces;
        //    return fragment;
        //}
        //Displays everything
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // If the document type is Confession the code below will execute
           
                //Variable Declaration
                string resultChapter = Arguments.GetString(CHAPTER, "");
                string resultProofs = Arguments.GetString(PROOFS, "");
                string listTitles = Arguments.GetString(DOCTITLE, "");
                string resultTitle = Arguments.GetString(TITLE, "");
                int resultMatch = Arguments.GetInt(matchNumb, -1);
                bool resultBool = Arguments.GetBoolean(truncated, false);
                int resultID = Arguments.GetInt(number, 0);
                View view = inflater.Inflate(Resource.Layout.confession_results, container, false);
                TextView chapterBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
                TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
                TextView chNumbBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
                TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
                TextView matchView = (TextView)view.FindViewById<TextView>(Resource.Id.matchView);
                TextView proofView = (TextView)view.FindViewById<TextView>(Resource.Id.proofLabel);
                //Variable Assignment
                chapterBox.Text = resultChapter;
                proofBox.Text = resultProofs;
                docTitleBox.Text = listTitles;

                chNumbBox.Text = string.Format(" {0}: {1}", resultID.ToString(), resultTitle);
                matchView.Text = string.Format("Matches: {0}", resultMatch);
                shareList = docTitleBox.Text + newLine + chNumbBox.Text + newLine + chapterBox.Text + newLine + "Proofs" + newLine + proofBox.Text;
                TextView[] views = new TextView[5];
                views[0] = chapterBox; views[1] = proofBox; views[2] = chNumbBox;
                views[3] = docTitleBox; views[4] = matchView;
                ChangeColor(true, views, Android.Graphics.Color.Black);
                matchView.SetTextIsSelectable(false);
                ChangeColor(proofView, false, Android.Graphics.Color.Black);
                FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
                ChangeColor(fab, Android.Graphics.Color.Black);
                fab.Click += ShareContent;
                return view;

            
        }
        //Color changes on display
        public void ChangeColor(TextView view, bool selectable, Android.Graphics.Color color)
        {
            view.SetTextColor(color);
            view.SetTextIsSelectable(selectable);
        }
        //TextView Text Color change event
        public void ChangeColor(bool selectable, TextView[] views, Android.Graphics.Color color)
        {
            foreach (TextView view in views)
            {
                view.SetTextColor(color);
                view.SetTextIsSelectable(selectable);
            }
        }
        
        public void ChangeColor(FloatingActionButton button, Android.Graphics.Color color)
        {
            button.SetBackgroundColor(color);
            button.SetImageResource(Resource.Drawable.abc_ic_menu_share_mtrl_alpha);
        }

        //Share Result Content
        public void ShareContent(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);

            string INTENTNAME = "SHARE";
            sendIntent.PutExtra(Intent.ExtraText, shareList);
            sendIntent.SetType("text/plain");
            StartActivity(Intent.CreateChooser(sendIntent, INTENTNAME));
        }

    }


}