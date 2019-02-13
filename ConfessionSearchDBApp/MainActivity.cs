using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using SQLite;
//using Android.Graphics;
using System.IO;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android;
using Android.Util;
using Android.Support.V4.App;

namespace ConfessionSearchDBApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        string dbName = "confessionSearchDB.db";
        string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(), 
           "confessionSearchDB.db");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            string[] permissions = new string[2];
            permissions[0] = Manifest.Permission.WriteExternalStorage;
            permissions[1] = Manifest.Permission.ReadExternalStorage;
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            TextView textView1 = FindViewById<TextView>(Resource.Id.textView1), text2 = FindViewById<TextView>(Resource.Id.textView2);
            // if(ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteExternalStorage != (int)Permission.Granted));
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                if (!File.Exists(dbPath))
                {
                    using (BinaryReader br = new BinaryReader(Android.App.Application.Context.Assets.Open(dbName)))
                    {
                        using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                        {
                            byte[] buffer = new byte[2048];
                            int len = 0;
                            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, len);
                            }
                        }
                    }
                }  // We have permission, go ahead and use the camera.
                using (var conn = new SQLite.SQLiteConnection(dbPath))
                {
                    var cmd = new SQLite.SQLiteCommand(conn);
                    cmd.CommandText = "select * from DocumentList";
                    var r = cmd.ExecuteQuery<DocumentLists>();
                    //foreach(string strings in r)
                    foreach (var item in r)
                    {
                        textView1.Text += item.DocumentID + ". \r\n";
                        text2.Text += item.Title + ". \r\n";
                    }



                }
            }
           
           

 
             else
            {
                ActivityCompat.RequestPermissions(this, permissions, 1);
                // Camera permission is not granted. If necessary display rationale & request.
            }
        }
        public class DocumentLists
        {
            [PrimaryKey, AutoIncrement]
            public int DocumentID { get; set; }
            [Column("DocumentName")]
            public string Title { get; set; }
           // public int ArtistId { get; set; }
        }
       

    }
}