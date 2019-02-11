using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DBSampleApp;

namespace DBSampleApp.Resources.Model
    {
        public class ViewHolder : Java.Lang.Object
        {
            public TextView txtName { get; set; }
            public TextView txtDepartment { get; set; }
            public TextView txtEmail { get; set; }
        }
        class ListViewAdapter : BaseAdapter
    {
        private Activity activity;
        private List<Person> listPerson;
        public ListViewAdapter(Activity activity, List<Person> listPerson)
        {
            this.activity = activity;
            this.listPerson = listPerson;
        }
        public override int Count
        {
            get { return listPerson.Count; }
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public override long GetItemId(int position)
        {
            return listPerson[position].Id;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.list_view, parent, false);
            var txtName = view.FindViewById<TextView>(Resource.Id.txtView_Name);
            var txtDepart = view.FindViewById<TextView>(Resource.Id.txtView_Depart);
            var txtEmail = view.FindViewById<TextView>(Resource.Id.txtView_Email);
            txtName.Text = listPerson[position].Name;
            txtDepart.Text = listPerson[position].Department;
            txtEmail.Text = listPerson[position].Email;
            return view;
        }
    }
}