using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;
using LibGArrayList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ConfessionSearchApp
{

  [Activity(Label = "@string/app_name",LaunchMode=Android.Content.PM.LaunchMode.SingleTask, Theme = "@style/AppTheme", MainLauncher = true,ClearTaskOnLaunch =true)]
  public class MainActivity :AppCompatActivity 
  {
    StreamReader fileIn;
    
    private const string CONFESSION_FILE_IN = "Westminster Confession of Faith 1646.txt";
    private const string CATECHISM_FILE_IN = "Westminster Shorter Catechism.txt";
    private static string fileName="";

    static SearchFragmentActivity searchFragmentActivity;
    List<KeyValuePair<string, string>> files;
    DocumentList documentList = new DocumentList(),resultList=new DocumentList();
    protected override void OnCreate(Bundle savedInstanceState)
    {

     
      base.OnCreate(savedInstanceState);     
      SetContentView(Resource.Layout.activity_main);
     
     



    }
    [Java.Interop.Export("layoutChanged")]
    public void layoutChanged(View view)
    {

      bool layoutChange = ((ToggleButton)view).Checked;
      string toast = ((ToggleButton)view).Text;
    //  Toast.MakeText(this,toast,ToastLength.Long).Show();
      ToggleButton radio = (ToggleButton)view;
     // EditText textbox = FindViewById<EditText>(Resource.Id.searchBox);
     
      
      if ( radio== FindViewById<ToggleButton>(Resource.Id.homeBtn))
      {
       
        SetContentView(Resource.Layout.activity_main);
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = false;
        FindViewById<TextView>(Resource.Id.nothing).Text = "Use the switches above to navigate the app. Search Catechisms by clicking the Catechism toggle. Use the search by Topic feature or hit View document to just view the document. Hit Go back home at the bottom of the results page when done";

      }
      else if (radio == FindViewById<ToggleButton>(Resource.Id.confessionBtn))
      {
        SetContentView(Resource.Layout.confession_layout);
        Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = false;
        //textbox.Text = "";

        files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Confession of Faith 1646","Westminster Confession of Faith 1646"),
        new KeyValuePair<string, string>("2nd London Baptist Confession of Faith", "2nd London Baptist Confession of Faith")
        };

        FindViewById<EditText>(Resource.Id.searchBox).Text = "";

        List<string> catechismFiles = new List<string>();
        foreach (var item in files)
          catechismFiles.Add(item.Key);
        spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
        var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.confession_list, Android.Resource.Layout.SimpleSpinnerItem);
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        spinner.Adapter=adapter;
      }
      else if (radio == FindViewById<ToggleButton>(Resource.Id.catechismBtn))
      {
        SetContentView(Resource.Layout.catechism_layout); //textbox.Text = "";
        Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
      FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = false;
        files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Larger Catechism","Westminster Larger Catechism"),
        new KeyValuePair<string, string>("Westminster Shorter Catechism", "Westminster Shorter Catechism"),
        new KeyValuePair<string,string>("Heidelberg Catechism","Heidelberg Catechism")
        };
        List<string> catechismFiles = new List<string>();
        foreach (var item in files)
          catechismFiles.Add(item.Key);
         spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, catechismFiles);
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        spinner.Adapter = adapter;
        FindViewById<EditText>(Resource.Id.searchBox).Text = "";
      }
    }
    [Java.Interop.Export("SearchType")]
    public void SearchType(View view)
    {
      RadioButton radio = ((RadioButton)view);
      EditText editText;
      if (radio == FindViewById<RadioButton>(Resource.Id.topicRadio)) {
        if (radio.Checked)
        {
          editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
          editText.Enabled = false;
          editText = FindViewById<EditText>(Resource.Id.searchBox);
          editText.Enabled = true;
          editText.Text = "";
        } }
      else if (radio == FindViewById<RadioButton>(Resource.Id.chapterRadio))
      {
      if(radio.Checked)
      {
          editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
          editText.Enabled = true;
          editText.Text = "";
          editText = FindViewById<EditText>(Resource.Id.searchBox);
          editText.Enabled = false;
      }
      }
      else if (radio==FindViewById<RadioButton>(Resource.Id.viewAllRadio))
      {
        editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
        editText.Enabled = false;
        editText = FindViewById<EditText>(Resource.Id.searchBox);
        editText.Enabled = false;
      }

    }
    [Java.Interop.Export("Search")]
    public void Search(View view)
    {
      int docCount=1;
      #region Variable Declaration
      ToggleButton toggle1 = FindViewById<ToggleButton>(Resource.Id.catechismBtn), toggle2 = FindViewById<ToggleButton>(Resource.Id.confessionBtn);
      RadioButton radio = FindViewById<RadioButton>(Resource.Id.viewAllRadio);
      searchFragmentActivity = new SearchFragmentActivity();
      AssetManager asset = this.Assets;
      string searchTerm = "";
      bool proofs=true, answers=true,searchAll=false;
      string[] fileNames= new string[3];
      CheckBox answerCheck = FindViewById<CheckBox>(Resource.Id.AnswerBox), proofCheck = FindViewById<CheckBox>(Resource.Id.proofBox), 
      searchCheck = FindViewById<CheckBox>(Resource.Id.searchAllCheckBox);
      Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
      EditText editText = FindViewById<EditText>(Resource.Id.searchBox);
      #endregion
      spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
      #region Search Options
      if (toggle1.Checked)
      {
        if (radio.Checked != true)
        {

          if (answerCheck.Checked)
            answers = true;

          else
            answers = false;
          if (proofCheck.Checked)
            proofs = true;
          else
            proofs = false;
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
      else if (toggle2.Checked)
      {
        if (radio.Checked != true)
        {
          if (proofCheck.Checked)
            proofs = true;
          else
            proofs = false;
          if (searchCheck.Checked)
          {
            searchAll = true;
            fileNames = new[] { "Westminster Confession of Faith 1646.txt", "2nd London Baptist Confession of Faith.txt" };
            docCount = 2;
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
        if(fileNames[0]!=null || fileNames[0]!=""&&searchAll)
        {
        foreach(string file1 in fileNames)
          documentList.Fill(file1, asset, 1, answers, proofs);
      }
      else
        documentList.Fill(fileName, asset, 0, answers, proofs);
     for(int x=0; x<=2;x++)
        for (int i = 0; i <= documentList.Count; i++)
        if (documentList[i].Type == "")
          documentList.RemoveAt(i);
      #endregion
      searchTerm = editText.Text;
      if (radio.Checked != true && searchTerm!="")
      {
        for (int y = 0; y <= documentList.Count; y++)
        {
          if (documentList[y].Type == "CONFESSION")
          {
            Confession confession = (Confession)documentList[y];
            confession.SearchTitle(confession, searchTerm);
            confession.SearchChapter(confession, searchTerm);
            if (proofs)
              confession.SearchProof(confession, searchTerm);
            if (confession.Matches > 0)
              resultList.Add(confession);
          }
          else if (documentList[y].Type == "CATECHISM")
          {
            Catechism catechism = (Catechism)documentList[y];
            catechism.SearchTitle(catechism, searchTerm);
            catechism.SearchQuestion(catechism, searchTerm);
            if (proofs)
              catechism.SearchProof(catechism, searchTerm);
            if (answers)
              catechism.SearchAnswer(catechism, searchTerm);
            if (catechism.Matches > 0)
              resultList.Add(catechism);
          }


        }
        documentList = resultList;
        documentList.MergeSort();       
        }

      for (int x = 0; x <= 2; x++)
        for (int i = 0; i <= documentList.Count; i++)
          if (documentList[i].Type == "")
            documentList.RemoveAt(i);
      SetContentView(Resource.Layout.search_results);
      searchFragmentActivity = new SearchFragmentActivity(documentList, this);
      ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
      SearchAdapter adapter = new SearchAdapter(SupportFragmentManager, documentList);
      searchFragmentActivity.Search(documentList, viewPager, adapter, searchTerm,docCount);


    }
    string WordInBetween(string sentence, string wordOne, string wordTwo)
    {

      int start = sentence.IndexOf(wordOne);

      int end = sentence.IndexOf(wordTwo)+wordTwo.Length+1;

      return sentence.Substring(start, end);


    }
    public static string getBetween(string strSource, string strStart, string strEnd)
    {
      int Start, End;
      if (strSource.Contains(strStart) && strSource.Contains(strEnd))
      {
        Start = strSource.IndexOf(strStart, 0) + strStart.Length;
        End = strSource.IndexOf(strEnd, Start);
        return strSource.Substring(Start, End - Start);
      }
      else
      {
        return "";
      }
    }
    private void spinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
    {
      Spinner spinner = (Spinner)sender;
      fileName = String.Format("{0}.txt", spinner.GetItemAtPosition(e.Position));
     // Toast.MakeText(this, fileName, ToastLength.Short).Show();
      }
    [Java.Interop.Export("BackHome")]
    public void GoHome(View view)
    {
      searchFragmentActivity.Finish();
      SetContentView(Resource.Layout.activity_main);
      this.Recreate();
    }


  }
  }
    