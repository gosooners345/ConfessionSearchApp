using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.RecyclerView;
using Android.Support.V7.AppCompat;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using Android.Animation;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DialogWindow;


namespace ConfessionSearchApp2
{
  [Activity(Label = "@string/app_name", Theme = "@style/Theme.Dark", MainLauncher = true,LaunchMode =Android.Content.PM.LaunchMode.SingleTop)]
  public class MainActivity : AppCompatActivity
  {
    static SearchFragmentActivity searchFragmentActivity;
    public static int searchInt = 0;
    public static string searchTerm = "";
    public static DocumentList documentList=new DocumentList();
    List<KeyValuePair<string, string>> files;
    Intent intent;
    static string fileName = "";
    public static bool truncate = false;
    List<string> fileNameList;
    protected override void OnCreate(Bundle savedInstanceState)
    {
    base.OnCreate(savedInstanceState);
    SetContentView(Resource.Layout.activity_main);
    FindViewById<TextView>(Resource.Id.nothing).Text = "Use the switches above to navigate through the app.\r\n Search Catechisms by clicking the Catechism toggle. \r\n Search Confessions by hitting the Confession toggle.\r\n " +
      "Hit \"Search By Topic\" to search for a topic.\r\n Hit \"Search by chapter/question number\" to search by question or chapter number.\r\n" +
      "Hit the Proofs Check box to add proofs to your search.\r\n Hit the Answers checkbox to see answers in a catechism search. ";
     
    }
    /// <summary>
    /// Fills the list with entries
    /// </summary>
    /// <param name="documentList">The list</param>
    /// <param name="ID">Whether to clear the list or not</param>
    /// <param name="files">Array of files</param>
    /// <param name="answers">Answers enteries filled if Catechism search is performed</param>
    /// <param name="proofs">Proofs filled if search is performed</param>
    /// <param name="asset">App Assets</param>
    /// <param name="allSearch">Search All files for entry</param>
    private void Fill(DocumentList documentList,  string[] files, bool answers, bool proofs, AssetManager asset, bool allSearch)
    {
      if (files[0] != null || files[0] != "" && allSearch)
      {
        foreach (string file1 in files)
          documentList.Fill(file1, asset, 1, answers, proofs);
      }
      else
        documentList.Fill(fileName, asset, 0, answers, proofs);
    }
    [Java.Interop.Export("layoutChanged")]
    public void layoutChanged(View view)
    {
      bool layoutChange = ((ToggleButton)view).Checked;
      string toast = ((ToggleButton)view).Text;
      ToggleButton radio = (ToggleButton)view;
      //Home Layout
      if (radio == FindViewById<ToggleButton>(Resource.Id.homeBtn))
      {

        SetContentView(Resource.Layout.activity_main);
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.settingsBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = false;
        FindViewById<TextView>(Resource.Id.nothing).Text = "Use the switches above to navigate through the app.\r\n Search Catechisms by clicking the Catechism toggle. \r\n Search Confessions by hitting the Confession toggle.\r\n " +
       "Hit \"Search By Topic\" to search for a topic.\r\n Hit \"Search by chapter/question number\" to search by question or chapter number.\r\n" +
       "Hit the Proofs Check box to add proofs to your search.\r\n Hit the Answers checkbox to see answers in a catechism search. ";

      }
      else if (radio == FindViewById<ToggleButton>(Resource.Id.confessionBtn))
      {
        SetContentView(Resource.Layout.confession_layout);
        Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.settingsBtn).Checked = false;

        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = false;
        //Set Combo Box Elements
        files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Confession of Faith 1646","Westminster Confession of Faith 1646"),
        new KeyValuePair<string, string>("2nd London Baptist Confession of Faith", "2nd London Baptist Confession of Faith")
        };
        FindViewById<EditText>(Resource.Id.searchBox).Text = "";
        List<string> catechismFiles = new List<string>();
        foreach (var item in files)
        {
          catechismFiles.Add(item.Key);

        }
        spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
        var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.confession_list, Android.Resource.Layout.SimpleSpinnerItem);
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        spinner.Adapter = adapter;
      }
      //Catechism Search Layout
      else if (radio == FindViewById<ToggleButton>(Resource.Id.catechismBtn))
      {
        SetContentView(Resource.Layout.catechism_layout); //textbox.Text = "";
        Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = true;
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.settingsBtn).Checked = false;
        files = new List<KeyValuePair<string, string>>
        {
        new KeyValuePair<string, string> ("Westminster Larger Catechism","Westminster Larger Catechism"),
        new KeyValuePair<string, string>("Westminster Shorter Catechism", "Westminster Shorter Catechism"),
        new KeyValuePair<string,string>("Heidelberg Catechism","Heidelberg Catechism")
        };
        List<string> catechismFiles = new List<string>();
        foreach (var item in files)
        {
          catechismFiles.Add(item.Key);
        }
        spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, catechismFiles);
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        spinner.Adapter = adapter;
        FindViewById<EditText>(Resource.Id.searchBox).Text = "";
      }
      else if (radio == FindViewById<ToggleButton>(Resource.Id.settingsBtn))
      {
        SetContentView(Resource.Layout.settingsLayout);
        FindViewById<ToggleButton>(Resource.Id.catechismBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.confessionBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.homeBtn).Checked = false;
        FindViewById<ToggleButton>(Resource.Id.settingsBtn).Checked = true;
      }
    }
    /// <summary>
    /// The Type of Search Performed
    /// </summary>
    /// <param name="view"></param>
    [Java.Interop.Export("SearchType")]
    public void SearchType(View view)
    {
      RadioButton radio = ((RadioButton)view);
      EditText editText;
      if (radio == FindViewById<RadioButton>(Resource.Id.topicRadio))
      {
        if (radio.Checked)
        {
          editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
          editText.Enabled = false;
          editText = FindViewById<EditText>(Resource.Id.searchBox);
          editText.Enabled = true;
          editText.Text = "";
        }
      }
      else if (radio == FindViewById<RadioButton>(Resource.Id.chapterRadio))
      {
        if (radio.Checked)
        {
          editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
          editText.Enabled = true;
          editText.Text = "";
          editText = FindViewById<EditText>(Resource.Id.searchBox);
          editText.Enabled = false;
        }
      }
      else if (radio == FindViewById<RadioButton>(Resource.Id.viewAllRadio))
      {
        editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
        editText.Enabled = false;
        editText = FindViewById<EditText>(Resource.Id.searchBox);
        editText.Enabled = false;
      }

    }
    /// <summary>
    /// Search Button Pressed
    /// </summary>
    /// <param name="view">Search Button</param>
    [Java.Interop.Export("Search")]
    public void Search(View view)
    {
      
      documentList = new DocumentList();
      #region Variables
      AssetManager assets = this.Assets;
      //documentList = new DocumentList();
      EditText editText;
      Bundle bundle = new Bundle();
      ToggleButton catechismToggle = FindViewById<ToggleButton>(Resource.Id.catechismBtn), 
      confessionToggle = FindViewById<ToggleButton>(Resource.Id.confessionBtn);
      RadioButton radio = FindViewById<RadioButton>(Resource.Id.viewAllRadio),topicRadio=FindViewById<RadioButton>(Resource.Id.topicRadio),
      chapterRadio=FindViewById<RadioButton>(Resource.Id.chapterRadio);
      string[] fileNames = new string[3];
      bool proofs = true, answers = true, searchAll = false;
      CheckBox answerCheck = FindViewById<CheckBox>(Resource.Id.AnswerBox), 
      proofCheck = FindViewById<CheckBox>(Resource.Id.proofBox),searchCheck = FindViewById<CheckBox>(Resource.Id.searchAllCheckBox);
      Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
      spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner1_ItemSelected);
      #endregion
      #region Search Types
      //Sets the Textbox for the search entry to go in
      if (FindViewById<RadioButton>(Resource.Id.chapterRadio).Checked)
        editText = FindViewById<EditText>(Resource.Id.chNumberTextBox);
      else
        editText = FindViewById<EditText>(Resource.Id.searchBox);
     
      //Selects to display answers for catechism and proofs for both
      if (catechismToggle.Checked)
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
          }
          else
            searchAll = false;
        }
        else
        {
          answers = true;
          searchAll = false;
          proofs = true;
        }
      }
      else if (confessionToggle.Checked)
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
          }
          else
            searchAll = false;
        }
        else
        {
          proofs = true; searchAll = false;
        }
      }
      // Truncate Results?
      if (FindViewById<CheckBox>(Resource.Id.truncateCheck).Checked)
      truncate = true;
      #endregion
      #region Search Execution
      //Search Type Performed
      if (!documentList.Empty)
        documentList.Clear();
     
      Fill(documentList, fileNames, answers, proofs, assets, searchAll);
      if (chapterRadio.Checked)
        if (editText.Text != "")
        {
          searchInt = Int32.Parse(editText.Text);
          searchFragmentActivity = new SearchFragmentActivity(searchInt, documentList, truncate);
          intent = new Intent(this, searchFragmentActivity.Class);
          this.OnStop();
          StartActivity(intent);
        }
        //Error Code
        else
        {
          Toast.MakeText(this, "Search Results were null", ToastLength.Long).Show();
        }
      else if (topicRadio.Checked)
      {
        if (editText.Text != "")
        {
          searchTerm = editText.Text;
          searchFragmentActivity = new SearchFragmentActivity(searchTerm, documentList, truncate);
          intent = new Intent(this, searchFragmentActivity.Class);
          this.OnStop();
          StartActivity(intent);

        }


        //Error Code
        else { Toast.MakeText(this, "Fragment was null", ToastLength.Long).Show(); }
      }
      else if (FindViewById<RadioButton>(Resource.Id.viewAllRadio).Checked)
      {
        searchTerm = "";
        searchFragmentActivity = new SearchFragmentActivity(searchTerm,documentList,false);
        intent = new Intent(this, searchFragmentActivity.Class);
        this.OnPause();
        StartActivity(intent);
      }
      #endregion
    }
    

   
    public override void OnBackPressed()
    {
      
    
        string TITLE = "Exit App?", MESSAGE = "Are you sure you want to leave?";
        Bundle dialogBundle = new Bundle();
        dialogBundle.PutString(TITLE, TITLE);
        dialogBundle.PutString(MESSAGE, MESSAGE);
        Android.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
        Android.App.Fragment prev = FragmentManager.FindFragmentByTag("dialog");
        if (prev != null)
        {
          ft.Remove(prev);
        }
        ft.AddToBackStack(null);
        Button yesButton = new Button(this), noButton = new Button(this);
        ActivityDialog dialog = ActivityDialog.NewInstance(dialogBundle, yesButton, noButton, TITLE, MESSAGE);
        yesButton.Click += delegate { Toast.MakeText(this, "Don't forget to rate this app if you haven't already", ToastLength.Short).Show(); dialog.Dismiss(); Finish(); };
        noButton.Click += delegate { dialog.Dismiss(); this.Recreate(); };
        dialog.Show(ft, "dialog");
      
    }

    private void spinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
    {
      Spinner spinner = (Spinner)sender;
      fileName = String.Format("{0}.txt", spinner.GetItemAtPosition(e.Position));
    }
  }
}

