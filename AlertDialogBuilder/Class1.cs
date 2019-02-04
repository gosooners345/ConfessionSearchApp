using System;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlertDialogBuilder
{
    public class DialogPic:Android.App.Fragment
    {
    public string Title = "Title";
    public string shareText;
    public DialogPic newInstance(Bundle bundle) 
    {
      DialogPic dialogPic = new DialogPic();
        
      return dialogPic;
    };
    }
}
