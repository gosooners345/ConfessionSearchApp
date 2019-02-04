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
using DocumentClass;
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
    //Fetches the fragment for display
    public override Android.Support.V4.App.Fragment GetItem(int position)
    {
      Android.Support.V4.App.Fragment frg;
      if (docPosition > 0)
        position = position + 1;
      string docTitle = "";
      if (documentList.Title == "Results" || documentList.Title == "")
        docTitle = documentList[position].DocTitle;
      else
        docTitle = documentList.Title;
      if (documentList[position].Type == "CONFESSION")
      {
        docPosition++;
        Confession confession = (Confession)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.NewResult(confession.Chapter, confession.Proofs, confession.Title, confession.IDNumber, docTitle, documentList.Truncate, confession.Matches);
      }
      else if (documentList[position].Type == "CATECHISM")
      {
        docPosition++;
        Catechism catechism = (Catechism)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.NewResult(catechism.Question, catechism.Answer, catechism.Proofs, catechism.Title, catechism.IDNumber, docTitle, catechism.Matches, documentList.Truncate);
      }
      else if (documentList[position].Type == "CREED")
      {
        docPosition++;
        Creed creed = (Creed)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.NewResult(creed.DocTitle, documentList.Title, truncate, creed.CreedText, creed.IDNumber, creed.Proofs, creed.Matches);
      }
      else
      {
        frg = GetItem(position + 1);
      }
      Log.Info("Search Adapter", String.Format("{0} Panel Loaded", position));
      return frg;
    }
    public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
    {
      if (term == "")
        term = documentList[position + 1].DocTitle;
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
    public static SearchResultFragment NewResult(string Question, string Answer, string Proofs, string Title, int ID, string ListTitle, int matchNum, bool truncate)
    {
      Log.Info("Fragment Forming", "Grabbing new Catechism Fragment to Display");
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();
      catechism1 = true; confession1 = false; creed1 = false;
      spaces.PutInt(number, ID);
      spaces.PutString(ANSWER, Answer);
      spaces.PutString(QUESTION, Question);
      spaces.PutInt(matchNumb, matchNum);
      spaces.PutString(TITLE, Title);
      spaces.PutString(PROOFS, Proofs);
      spaces.PutString(DOCTITLE, ListTitle);
      spaces.PutBoolean(truncated, truncate);
      fragment.Arguments = spaces;
      return fragment;
    }
    public static SearchResultFragment NewResult(string Title, string ListTitle, bool truncate, string creed, int ID, string Proofs, int matchNum)
    {

      Log.Info("Fragment forming", "Grabbing new Creed fragment to display");
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();
      creed1 = true; catechism1 = false; confession1 = false;
      spaces.PutInt(number, ID);
      spaces.PutString(CREED, creed);
      spaces.PutInt(matchNumb, matchNum);
      spaces.PutString(PROOFS, Proofs);
      spaces.PutString(DOCTITLE, ListTitle);
      spaces.PutBoolean(truncated, truncate);
      fragment.Arguments = spaces;
      return fragment;
    }
    //Displays everything
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      // If the document type is Confession the code below will execute
      if (confession1)
      {
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
       
        chNumbBox.Text = string.Format("Chapter {0}: {1}", resultID.ToString(), resultTitle);
        matchView.Text = string.Format("Matches: {0}", resultMatch);
        shareList = docTitleBox.Text + newLine + chNumbBox.Text + newLine + chapterBox.Text + newLine + "Proofs" + newLine + proofBox.Text;
       TextView[] views = new TextView[5];
                views[0] = chapterBox;views[1] = proofBox;views[2] = chNumbBox;
                views[3] = docTitleBox;views[4] = matchView;
                ChangeColor(true, views, Android.Graphics.Color.Black);
                matchView.SetTextIsSelectable(false);
                ChangeColor(proofView, false, Android.Graphics.Color.Black);
                FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
        ChangeColor(fab,Android.Graphics.Color.Black);
        fab.Click += ShareContent;
        return view;
      }
      //if the document type is a creed, the code below will execute
      else if (creed1)
      {
        string resultCreed = Arguments.GetString(CREED, "");
        string listTitle = Arguments.GetString(DOCTITLE, "");
        View view = inflater.Inflate(Resource.Layout.creed_Results, container, false);
        TextView creedBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView titleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        creedBox.Text = resultCreed; titleBox.Text = listTitle; 
        FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
        ChangeColor(fab,Android.Graphics.Color.Black);
                TextView[] views = new TextView[2];
                views[0] = creedBox; views[1] = titleBox;
                ChangeColor(true, views, Android.Graphics.Color.Black);
                titleBox.SetTextIsSelectable(false);
                fab.Click += ShareContent;
        return view;
      }
      //if the document type is catechism the code below will execute
      else
      {
        //Variable Declaration
        string resultQuestion = Arguments.GetString(QUESTION, "");
        int resultID = Arguments.GetInt(number, 0);
        string resultAnswers = Arguments.GetString(ANSWER, "");
        string listTitle = Arguments.GetString(DOCTITLE, "");
        int resultMatch = Arguments.GetInt(matchNumb, -1);
        string resultProofs = Arguments.GetString(PROOFS, "");
        string resultTitle = Arguments.GetString(TITLE, "");
        bool resultBool = Arguments.GetBoolean(truncated, false);
        View view = inflater.Inflate(Resource.Layout.catechism_Results, container, false);
        TextView questionBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView answerBox = (TextView)view.FindViewById<TextView>(Resource.Id.answerText);
        TextView numberBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
        TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
        TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        TextView matchView = (TextView)view.FindViewById<TextView>(Resource.Id.matchView);
                TextView proofView = (TextView)view.FindViewById<TextView>(Resource.Id.proofLabel);
                //Results display on screen        
                questionBox.Text = resultQuestion;
        numberBox.Text = String.Format("Question {0}: {1}", resultID, resultTitle);
        answerBox.Text = resultAnswers;
        matchView.Text = string.Format("Matches: {0}", resultMatch);
        docTitleBox.Text = listTitle;
        proofBox.Text = resultProofs;
 //Share action string building
        shareList = docTitleBox.Text + newLine + String.Format("Question {0}:", resultID) + newLine + questionBox.Text + newLine + "Answer:" + newLine +
                    answerBox.Text + newLine + "Proofs:" + newLine + proofBox.Text;
                //Text color changes
                TextView[] views = new TextView[6];
                views[0] = questionBox; views[1] = proofBox; views[2] = numberBox;
                views[3] = docTitleBox; views[4] = matchView; views[5] = answerBox;
                ChangeColor(true, views, Android.Graphics.Color.Black);
                ChangeColor(proofView, false, Android.Graphics.Color.Black);
                ChangeColor((TextView)view.FindViewById<TextView>(Resource.Id.catechismAnswerLabel), false, Android.Graphics.Color.Black);
                matchView.SetTextIsSelectable(false);
                FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.shareActionButton);
        ChangeColor(fab,Android.Graphics.Color.Black);
        fab.Click += ShareContent;
        return view;
      }
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
        private string ShareListBuilder(string shares, Document document)
    {
      string qHeader = "", aHeader = "Answer:", pHeader = "Proofs:", chHeader = "";
      List<string> listBuilder = new List<string>();
      if (document.Type == "CREED")
      {
        Creed creed = (Creed)document;
        shares += creed.Title;
        shares += "\r\n";
        shares += creed.CreedText;
      }
      else if (document.Type == "CATECHISM")
      {
        Catechism catechism = (Catechism)document;

        qHeader = String.Format("QUESTION {0}:", catechism.IDNumber.ToString("d3"));
        listBuilder.Add(catechism.DocTitle);
        listBuilder.Add(qHeader);
        listBuilder.Add(catechism.Question);
        listBuilder.Add(aHeader);
        listBuilder.Add(catechism.Answer);
        listBuilder.Add(pHeader);
        listBuilder.Add(catechism.Proofs);
        shares = "";
        foreach (string shareElement in listBuilder)
        {
          shares += shareElement + "\r\n";
        }
      }
      else if (document.Type == "CONFESSION")
      {
        Confession confession = (Confession)document;
        chHeader = String.Format("Chapter {0}: {1}", confession.IDNumber, confession.DocTitle);
        listBuilder.Add(confession.DocTitle); listBuilder.Add(chHeader);
        listBuilder.Add(confession.Chapter); listBuilder.Add(pHeader);
        listBuilder.Add(confession.Proofs); shares = "";
        foreach (string element in listBuilder)
          shares += element + "\r\n";
      }
      return shares;
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