using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.IO;
using System.Drawing;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

namespace ConfessionSearchApp
{
  [Activity(Label = "PageTwo", Theme = "@style/AppTheme",AllowEmbedded =false, ParentActivity = typeof(MainActivity))]
  class SearchFragmentActivity:FragmentActivity
  {
    //public static 
    public  SearchAdapter adapter;
    public DocumentList documentList,documentList1;
    public Android.Support.V4.App.FragmentManager news1;
    public MainActivity home;

    string files;
    public SearchFragmentActivity() { this.documentList = new DocumentList(); }
    public SearchFragmentActivity(DocumentList sourcedocumentList,MainActivity main)
    {
      home = main;
      documentList = sourcedocumentList;
    }
    protected override void OnCreate(Bundle savedInstanceState)
    {

      base.OnCreate(savedInstanceState);
      
    }
   
    public void Search(DocumentList sourcedocumentList,ViewPager view, SearchAdapter search,string searchTerm,int count)
    {
      documentList = sourcedocumentList;

      news1 = search.news;
      search.GetItem(count);
      
      search.SaveState();
      
     // news1.BeginTransaction().DisallowAddToBackStack().Commit();
        view.Adapter =search;
     // adapter = search;
     
    }
    public override void OnBackPressed()
    {
    //  adapter.SaveState();
    }
    
  }
  class SearchAdapter : FragmentStatePagerAdapter
  {
    public DocumentList documentList = new DocumentList(),documentList1=new DocumentList();
    private int docPosition = 0;
    public Android.Support.V4.App.FragmentManager news;
    public SearchAdapter(Android.Support.V4.App.FragmentManager fm, DocumentList documents) : base(fm)
    {
      documentList = documents;
      news = fm;
     
    }
    public override int Count { get { return documentList.Count; } }
    public override Android.Support.V4.App.Fragment GetItem(int position)
    {
      Android.Support.V4.App.Fragment frg;
      position = position + 1;
      string docTitle = "";
      if (documentList.Title == "Results"||documentList.Title=="")
        docTitle = documentList[position].DocTitle;
      else
        docTitle = documentList.Title;
      docPosition = position;
      if (documentList[position].Type == "CONFESSION")
      {
        Confession confession = (Confession)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.newResult(confession.Chapter, confession.Proofs, confession.Title, confession.IDNumber, docTitle);
      //  return frg;
      }
      else if (documentList[position].Type == "CATECHISM")
      {
        Catechism catechism = (Catechism)documentList[position];
        frg = (Android.Support.V4.App.Fragment)SearchResultFragment.newResult(catechism.Question, catechism.Answer, catechism.Proofs, catechism.Title, catechism.IDNumber, docTitle,catechism.Matches);

        //return frg;
      }
      else
      {
       
       frg= GetItem(position);
       
      }
     return frg;
    }
    
    public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
    {
     // GetItem(position);
      return new Java.Lang.String("Result " + (position+1));
      
    }
    
  }
  class SearchResultFragment : Android.Support.V4.App.Fragment 
  {
  private string documentTitle;
    private static string PROOFS="proofs", CHAPTER="chapter", 
    QUESTION="question", TYPE="type", 
    TITLE="title", ANSWER="answer",DOCTITLE="titles";
    private static string number,match;
    private static int chNumber, queNumber;
    private static bool confession1, catechism1;
    
  public SearchResultFragment(){}
   
    public static SearchResultFragment newResult(string Chapter, string Proofs, string Title, int ID,string ListTitle)
    {
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();
      confession1 = true; catechism1 = false;
      spaces.PutInt(number, ID);
      spaces.PutString(CHAPTER, Chapter);
      spaces.PutString(PROOFS, Proofs);
      spaces.PutString(TITLE, Title);
      spaces.PutString(DOCTITLE, ListTitle);
      fragment.Arguments = spaces;
      return fragment;
      
    }
    public static SearchResultFragment newResult(string Question,string Answer,string Proofs,string Title,int ID,string ListTitle,int matchNum)
    {
      SearchResultFragment fragment = new SearchResultFragment();
      Bundle spaces = new Bundle();



      catechism1 = true; confession1 = false;

        spaces.PutInt(number, ID);
        spaces.PutString(ANSWER, Answer);
        spaces.PutString(QUESTION,Question);
        spaces.PutString(TITLE,Title);
        spaces.PutString(PROOFS,Proofs);
      //spaces.PutInt(match, matchNum);
      spaces.PutString(DOCTITLE, ListTitle);
      fragment.Arguments = spaces;
      return fragment;
    }
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      
     
      if (confession1)
      {
        string resultChapter = Arguments.GetString(CHAPTER,"");
        string resultProofs = Arguments.GetString(PROOFS,"");
        string listTitles = Arguments.GetString(DOCTITLE,"");
        string resultTitle = Arguments.GetString(TITLE,"");
        int resultID = Arguments.GetInt(number,0);
        
        View view = inflater.Inflate(Resource.Layout.confession_results, container, false);
        TextView chapterBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
        TextView chNumbBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
        TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        

        chapterBox.Text = resultChapter;
        proofBox.Text = resultProofs;
        docTitleBox.Text = listTitles;
        chapterBox.SetTextIsSelectable(true);
        proofBox.SetTextIsSelectable(true);
        
      
        chNumbBox.Text = "Chapter" + " " + resultID.ToString() + ":" + resultTitle;
        return view;

      }
      else 
      {
        string resultQuestion = Arguments.GetString(QUESTION,"");
        int resultID = Arguments.GetInt(number,0);
        string resultAnswers = Arguments.GetString(ANSWER,"");
        string listTitle = Arguments.GetString(DOCTITLE,"");
        string resultProofs = Arguments.GetString(PROOFS,"");
        string resultTitle = Arguments.GetString(TITLE,"");
       
        View view = inflater.Inflate(Resource.Layout.catechism_Results, container,false);
        TextView questionBox = (TextView)view.FindViewById<TextView>(Resource.Id.chapterText);
        TextView answerBox = (TextView)view.FindViewById<TextView>(Resource.Id.answerText);
        TextView numberBox = (TextView)view.FindViewById<TextView>(Resource.Id.confessionChLabel);
        TextView proofBox = (TextView)view.FindViewById<TextView>(Resource.Id.proofText);
        TextView docTitleBox = (TextView)view.FindViewById<TextView>(Resource.Id.documentTitleLabel);
        TextView matchBox = (TextView)view.FindViewById<TextView>(Resource.Id.matchLabel);
        questionBox.Text = resultQuestion; 
        answerBox.Text = resultAnswers;
        docTitleBox.Text = listTitle;
        proofBox.Text = resultProofs;
        questionBox.SetTextIsSelectable(true);
      
        proofBox.SetTextIsSelectable(true);
        answerBox.SetTextIsSelectable(true);
        numberBox.Text = "Question" + " " + resultID.ToString() + ": "+resultTitle;
        return view;
       
      }
      }
   public  void RefreshView()
   {
     base.FragmentManager.BeginTransaction().Detach(this).Attach(this).CommitNow();
   }
  }
  
}