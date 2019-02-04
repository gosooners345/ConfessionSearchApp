 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7;


namespace DialogWindow
{
  public class ActivityDialog : DialogFragment
  {
    
    private static string TITLE = "title";
    private static string MESSAGE = "message";
    public static Button yesButton, noButton;

    public static ActivityDialog NewInstance(Bundle bundle, Button yes, Button no,
        string titleValue,string msgValue)
    {
      ActivityDialog alertDialog = new ActivityDialog();
      bundle.PutString(MESSAGE, msgValue);
      bundle.PutString(TITLE, titleValue);
      yesButton = yes; noButton = no;
      alertDialog.Arguments = bundle;
      return alertDialog;
    }
     public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.dialogLayout, container, false);
     Button cancelButton = view.FindViewById<Button>(Resource.Id.NoButton);
      view.FindViewById<TextView>(Resource.Id.titleView).Text=Arguments.GetString(TITLE,TITLE);
      view.FindViewById<TextView>(Resource.Id.subTitleView).Text = Arguments.GetString(MESSAGE,MESSAGE);
      cancelButton.Click += delegate { noButton.PerformClick();Dismiss(); };
      Button confirmButton = view.FindViewById<Button>(Resource.Id.YesButton);
      confirmButton.Click += delegate { yesButton.PerformClick(); Dismiss(); };
      return view;
    }
    public int Title{get{ return this.title; }set{ this.title = value; }}
    public int Message{get{ return this.message; }set{ this.message = value; }}
  }
}
