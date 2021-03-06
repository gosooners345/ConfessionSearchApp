﻿using System;
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
using static ConfessionSearchApp.MainActivity;
using ConfessionSearchApp;
using Android.Support.Design.Widget;


namespace ConfessionSearchApp
{
  [Activity(Label = "PageTwo", Theme = "@style/AppTheme", AllowEmbedded = false, ParentActivity = typeof(MainActivity))]
  class SearchFragmentActivity : AppCompatActivity
  {
    private Android.Support.V7.Widget.ShareActionProvider actionProvider;
    public SearchAdapter adapter;
    public string shareList;// = new List<string>();
    //Android.Widget.ShareActionProvider actionProvider;
    public DocumentList documentList = new DocumentList();
    string files;
        string header = "";
    public int index;
    public SearchFragmentActivity() { }
 
    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
      
    }
    //Search feed for Fragments
    public void DisplayResults(DocumentList sourcedocumentList, ViewPager view, SearchAdapter search, string searchTerm, int count, bool truncate)
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
    public DocumentList documentList = new DocumentList(), documentList1 = new DocumentList();
    private int docPosition = 0;
    private string term = "";
        string header = "";
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
          //  this.header = header;
      news = fm;
      term = searchTerm;
    }
    //Required for Fragment adapter
    public override int Count { get { return documentList.Count; } }
        //Fetches the fragment for display
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            string title = "";
            Android.Support.V4.App.Fragment frg;

            Document document = documentList[position];
            string docTitle = "";

            title = document.DocumentName;
            if (title == "Results" || title == "")
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
  class SearchResultFragment : Android.Support.V4.App.Fragment
  {
    //required variables for fragment class
    private string documentTitle;
    // ShareActionProvider share;
    private static string PROOFS = "proofs", CHAPTER = "chapter",
    QUESTION = "question", TYPE = "type",
    TITLE = "title", ANSWER = "answer", DOCTITLE = "titles", truncated = "truncate", matchNumb = "matches";
    private static string number, match;
    public const string newLine = "\r\n";
        public static string HEADER = "header";
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
        //Displays everything
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Variable Declaration
            string header = "";// Arguments.GetString(HEADER, "");
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
            if (resultChapter.Contains("Question"))
            { header = "Question "; chNumbBox.Text = string.Format("{0} {1}: {2}", header, resultID.ToString(), resultTitle); }
            else if (resultChapter.Contains("I."))
            { header = "Chapter"; chNumbBox.Text = string.Format("{0} {1}: {2}", header, resultID.ToString(), resultTitle); }
            else
            {
                chNumbBox.Text = string.Format("{0} ",  resultTitle);
            }
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
        public void ChangeColor(TextView view, bool selectable,Android.Graphics.Color color)
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
     
      //  Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
      //  .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
    }

  }
  
  
}  