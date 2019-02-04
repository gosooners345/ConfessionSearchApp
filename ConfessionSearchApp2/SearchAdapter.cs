using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.App;
using System.Diagnostics;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Runtime;
using System.IO;
using System.Drawing;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using DialogWindow;
using Xamarin.Android;


namespace ConfessionSearchApp2
{
  [Activity(Label = "PageTwo", Theme = "@style/Theme.Dark",AllowEmbedded =false, ParentActivity = typeof(MainActivity))]
  class SearchFragmentActivity:AppCompatActivity
  {

    Intent intent ;
    public static Stopwatch stopwatch ;
    public  DocumentList documentList = new DocumentList();
    static int searchInt = 0;
     bool truncate = false;
   public static string searchTerm="";
    public SearchFragmentActivity() { }
    /// <summary>
    /// Search for chapter/question
    /// </summary>
    public SearchFragmentActivity(int sourceInt,DocumentList sourceList,bool truncate)
    {
      searchInt = sourceInt;
     this.documentList = sourceList;
     this.truncate = truncate;
      searchTerm = "";
    }
    /// <summary>
    /// Search for topic 
    /// </summary>
    public SearchFragmentActivity(string sourceTerm,DocumentList sourceList,bool truncate)
    {
     searchTerm = sourceTerm;
     this.documentList = sourceList;
      this.truncate = truncate;
    
      searchInt = 0;
    }
  /// <summary>
  /// Back button pressed
  /// </summary>
    public override void OnBackPressed()
    {
      string TITLE = "Search Again?", MESSAGE = "Do you want to perform another search?";
      Bundle dialogBundle = new Bundle();
      dialogBundle.PutString(TITLE, TITLE);
      dialogBundle.PutString(MESSAGE, MESSAGE);
      if (MainActivity.documentList.Count>1)
      {
        Android.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
        Android.App.Fragment prev = FragmentManager.FindFragmentByTag("dialog");
        if (prev != null)
        {
          ft.Remove(prev);
        }
        ft.AddToBackStack(null);
        Button yesButton = new Button(this), noButton = new Button(this);
        ActivityDialog dialog = ActivityDialog.NewInstance(dialogBundle, yesButton, noButton, TITLE, MESSAGE);
        yesButton.Click += delegate
        {
          MainActivity.documentList.Clear();
          this.Finish();
          this.NavigateUpTo(intent);
          base.OnBackPressed();

        };
        noButton.Click += delegate
        {
          dialog.Dismiss();
        };
        dialog.Show(ft, "dialog");
      }
      else
      {
        this.Finish();
        this.NavigateUpTo(intent);
        base.OnBackPressed();
      }
    }
    /// <summary>
    /// Activity Initiation
    /// </summary>
    /// <param name="savedInstanceState"></param>
    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.search_results);
      stopwatch = new Stopwatch();
      intent = new Intent(this, typeof(MainActivity));
      this.documentList = MainActivity.documentList;
      if (MainActivity.searchInt > 0)
      {
        searchInt = MainActivity.searchInt;
        searchTerm = "";
      }
      else if (MainActivity.searchTerm != "")
      {
        searchTerm = MainActivity.searchTerm;
        searchInt = 0;
        }
        this.truncate = MainActivity.truncate;
     
      if (searchTerm == "" && searchInt != 0)
        MainActivity.documentList=Search(searchInt, MainActivity.documentList);
      else if (searchTerm != "" && searchInt == 0)
       MainActivity.documentList=Search(searchTerm, MainActivity.documentList, this.truncate);
      Log.Debug("Timer", String.Format("{0} ms passed", stopwatch.ElapsedMilliseconds.ToString()));
      Display(MainActivity.documentList);
    }
/// <summary>
/// Searches the List for matching Chapter/Question ID
/// </summary>
/// <param name="searchInt">ID being searched for</param>
/// <param name="documentList">Documents being searched</param>
    public DocumentList Search(int searchInt,DocumentList documentList)
    {
      DocumentList resultList = new DocumentList();
      foreach (Document document in documentList)
        if (document.IDNumber == searchInt)
          resultList.Add(document);
      resultList.MergeSort();
      return resultList;
    }
    /// <summary>
    /// Searches the List for relevant entries with the topic provided, Tags are included in search
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="documentList">Documents being searched</param>
    /// <param name="truncate">Truncate results if true</param>
    public DocumentList Search(string searchTerm,DocumentList documentList,bool truncate)
    {
      DocumentList resultList = new DocumentList();
      stopwatch.Start();
      for (int y = 1; y <= documentList.Count; y++)
      {
        if (documentList[y].Type == "CONFESSION")
        {
          Confession confession = (Confession)documentList[y];
          confession.Search(searchTerm, truncate);
          if (confession.Matches >= 1)
            resultList.Add(confession);
        }
        else if (documentList[y].Type == "CATECHISM")
        {
          Catechism catechism = (Catechism)documentList[y];
          catechism.Search(searchTerm, truncate);
          if (catechism.Matches >= 1)
            resultList.Add(catechism);
        }
      }
      resultList.MergeSort(DocumentList.OrderEnum.MatchOrder);
      resultList.Reverse();
    //  documentList = resultList;
      stopwatch.Stop();
      return resultList;
    }
    /// <summary>
    /// Displays the results
    /// </summary>
    /// <param name="documentList">Documents being searched</param>
    public void Display(DocumentList documentList)
    {
      Toast.MakeText(this, String.Format("{0}ms have passed", stopwatch.ElapsedMilliseconds), ToastLength.Long).Show();
      if (documentList.Count > 1)
      {

        SetContentView(Resource.Layout.search_results);
        ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
        if (searchInt > 0)
        {
          if (documentList[1].Type == "CATECHISM")
            searchTerm = String.Format("Question {0}", searchInt);
          else if (documentList[1].Type == "CONFESSION")
            searchTerm = String.Format("Chapter {0}", searchInt);
        }
        SearchAdapter adapter = new SearchAdapter(SupportFragmentManager, documentList, searchTerm, truncate);
        Android.Support.V4.App.FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
        ft.AddToBackStack("Home");
        ft.Add(adapter.GetItem(1), "Search");
        ft.Commit();
        
        adapter.SaveState();
        viewPager.Adapter = adapter;
        ft.Show(adapter.GetItem(1));
      }
      else
      {

        if (documentList.Empty)
        {
          Log.Info("Search Empty", "Search Results came back empty");
          Toast.MakeText(this, String.Format("0 search Results were found for {0}", searchTerm), ToastLength.Long).Show();
          Confession confession = new Confession();
          confession.DocTitle = String.Format("0 results found for {0}", searchTerm);
          confession.Chapter = "";
          confession.Proofs = "";
          confession.Title = "";
          confession.IDNumber = 1;
          SetContentView(Resource.Layout.confession_results);
          TextView chapterBox = FindViewById<TextView>(Resource.Id.chapterText);
          TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
          TextView chNumbBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
          TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
          chapterBox.Text = confession.Chapter;
          proofBox.Text = confession.Proofs;
          docTitleBox.Text = confession.DocTitle;
          chNumbBox.Text = String.Format(" {0}", confession.DocTitle);
        }
        else
        {
          Document document = documentList[documentList.Count];
          if (document.Type == "CATECHISM")
          {

            this.SetContentView(Resource.Layout.catechism_Results);
            TextView questionBox = FindViewById<TextView>(Resource.Id.chapterText);
            TextView answerBox = FindViewById<TextView>(Resource.Id.answerText);
            TextView numberBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
            TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
            TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
            TextView matchView = FindViewById<TextView>(Resource.Id.matchView);
            Catechism catechism = (Catechism)document;
            questionBox.Text = catechism.Question;
            answerBox.Text = catechism.Answer;
            numberBox.Text = String.Format("Question {0}: {1}", catechism.IDNumber.ToString(), catechism.Title);
            proofBox.Text = catechism.Proofs;
            docTitleBox.Text = catechism.DocTitle;
            questionBox.SetTextIsSelectable(true);
            proofBox.SetTextIsSelectable(true);
            answerBox.SetTextIsSelectable(true);
          }
          else if (document.Type == "CONFESSION")
          {
            SetContentView(Resource.Layout.confession_results);
            TextView chapterBox = FindViewById<TextView>(Resource.Id.chapterText);
            TextView proofBox = FindViewById<TextView>(Resource.Id.proofText);
            TextView chNumbBox = FindViewById<TextView>(Resource.Id.confessionChLabel);
            TextView matchView = FindViewById<TextView>(Resource.Id.matchView); 
            TextView docTitleBox = FindViewById<TextView>(Resource.Id.documentTitleLabel);
            Confession confession = (Confession)document;
            chapterBox.Text = confession.Chapter;
            proofBox.Text = confession.Proofs;
            docTitleBox.Text = confession.DocTitle;
            chapterBox.SetTextIsSelectable(true);
            proofBox.SetTextIsSelectable(true);
          }
        }
      }
    }
  }
  //Search Adapter for ViewPager
  class SearchAdapter : FragmentStatePagerAdapter
  {
    public DocumentList documentList = new DocumentList();
    private int docPosition = 0;
    private string term = "";
    private bool truncate = false;
    public Android.Support.V4.App.FragmentManager news;
    public SearchAdapter(Android.Support.V4.App.FragmentManager fm, DocumentList documents,string searchTerm,bool truncate) : base(fm)
    {
      documentList = documents;
      news = fm;
      term = searchTerm;
    }
    public override int Count { get { return documentList.Count; } }
    public override Android.Support.V4.App.Fragment GetItem(int position)
    {
      Android.Support.V4.App.Fragment frg;
      
      if(docPosition>0)
      position = position + 1;

     // bool truncateD = truncate;
      string docTitle = "";
      if (documentList.Title == "Results"||documentList.Title=="")
        docTitle = documentList[position].DocTitle;
      else
        docTitle = documentList.Title;
      
      if (documentList[position].Type == "CONFESSION")
      {
      docPosition++;
        Confession confession = (Confession)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.NewResult(confession.Chapter, confession.Proofs, confession.Title, confession.IDNumber, docTitle,documentList.Truncate,confession.Matches);
     
      }
      else if (documentList[position].Type == "CATECHISM")
      {
        docPosition++;
        Catechism catechism = (Catechism)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.NewResult(catechism.Question, catechism.Answer, catechism.Proofs, catechism.Title, catechism.IDNumber, docTitle,catechism.Matches,documentList.Truncate);

       
      }
      else
      {
       frg= GetItem(position+1);      
      }
      
      Log.Info("Search Adapter", String.Format("{0} Panel Loaded", position));
     return frg;
    }

    public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
    {
      if (term == "")
        term = documentList[position+1].DocTitle;
      return new Java.Lang.String(String.Format("Result {0} of {1} for {2}",position+1,documentList.Count,term));
      
    }
    
  }
  //Fragment Adapter for display
  class SearchResultFragment : Android.Support.V4.App.Fragment 
  {
  private string documentTitle;
    private static string PROOFS="proofs", CHAPTER="chapter", 
    QUESTION="question", TYPE="type", 
    TITLE="title", ANSWER="answer",DOCTITLE="titles",truncated="truncate";
    private static string number,match,matchNumb = "matches";
#pragma warning disable IDE0044 // Add readonly modifier
    private static int chNumber, queNumber;
#pragma warning restore IDE0044 // Add readonly modifier
    private static bool confession1, catechism1;
    private static bool truncate = false;
    
  public SearchResultFragment(){}
   //Displays Search Results
    public static SearchResultFragment NewResult(string Chapter, string Proofs, string Title, int ID,string ListTitle,bool truncate,int matchNum)
    {
      Log.Info("Fragment Forming", "Grabbing new Confession Fragment to Display");
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();
      confession1 = true; catechism1 = false;
      spaces.PutInt(number, ID);
      spaces.PutString(CHAPTER, Chapter);
      spaces.PutString(PROOFS, Proofs);
      spaces.PutString(TITLE, Title);
      spaces.PutInt(matchNumb, matchNum);
      spaces.PutString(DOCTITLE, ListTitle);
      fragment.Arguments = spaces;
      return fragment;
    }
    //Displays the search Results
    public static SearchResultFragment NewResult(string Question,string Answer,string Proofs,string Title,int ID,string ListTitle,int matchNum,bool teruncate)
    {
      Log.Info("Fragment Forming", "Grabbing new Catechism Fragment to Display");
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();
      catechism1 = true; confession1 = false; truncate = teruncate;
        spaces.PutInt(number, ID);
      spaces.PutInt(matchNumb, matchNum);
      spaces.PutString(ANSWER, Answer);
        spaces.PutString(QUESTION,Question);
        spaces.PutString(TITLE,Title);
        spaces.PutString(PROOFS,Proofs);
      spaces.PutString(DOCTITLE, ListTitle);
      spaces.PutBoolean(truncated, truncate);
      fragment.Arguments = spaces;
      return fragment;
    }
    //Displays everything
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      
     
      if (confession1)
      {
      //Variable Declaration
        string resultChapter = Arguments.GetString(CHAPTER,"");
        string resultProofs = Arguments.GetString(PROOFS,"");
        int resultMatch = Arguments.GetInt(matchNumb, -1);
        string listTitles = Arguments.GetString(DOCTITLE,"");
        string resultTitle = Arguments.GetString(TITLE,"");
        bool resultBool = Arguments.GetBoolean(truncated, false);
        int resultID = Arguments.GetInt(number,0);
        View view = inflater.Inflate(Resource.Layout.confession_results, container, false);
        TextView chapterBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
        TextView chNumbBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
        TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        TextView matchView = (TextView)view.FindViewById<TextView>(Resource.Id.matchView);
        //Variable Assignment
        chapterBox.Text = resultChapter;
        proofBox.Text = resultProofs;
        docTitleBox.Text = listTitles;
        chapterBox.SetTextIsSelectable(true);
        proofBox.SetTextIsSelectable(true);
        chNumbBox.Text = "Chapter" + " " + resultID.ToString() + ":" + resultTitle;
        matchView.Text = string.Format("Matches: {0}", resultMatch);
        return view;
}
      else 
      {
      //Variable Declaration
        string resultQuestion = Arguments.GetString(QUESTION,"");
        int resultID = Arguments.GetInt(number,0);
        int resultMatch = Arguments.GetInt(matchNumb, -1);
        string resultAnswers = Arguments.GetString(ANSWER,"");
        string listTitle = Arguments.GetString(DOCTITLE,"");
        string resultProofs = Arguments.GetString(PROOFS,"");
        string resultTitle = Arguments.GetString(TITLE, "");
        bool resultBool = Arguments.GetBoolean(truncated, false);
        View view = inflater.Inflate(Resource.Layout.catechism_Results, container,false);
        TextView questionBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView answerBox = (TextView)view.FindViewById<TextView>(Resource.Id.answerText);
        TextView numberBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
        TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
        TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        TextView matchView = (TextView)view.FindViewById<TextView>(Resource.Id.matchView);
        //Variable Assignment        
        questionBox.Text = resultQuestion; 
        answerBox.Text = resultAnswers;
        docTitleBox.Text = listTitle;
        proofBox.Text = resultProofs;
        questionBox.SetTextIsSelectable(true);
      proofBox.SetTextIsSelectable(true);
        answerBox.SetTextIsSelectable(true);
        numberBox.Text = "Question" + " " + resultID.ToString() + ": "+resultTitle;
        matchView.Text = string.Format("Matches: {0}", resultMatch);
        return view;
       
      }
      }
  }
  
}