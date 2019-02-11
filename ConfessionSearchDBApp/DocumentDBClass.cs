using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ConfessionSearchDBApp
{
    class Document
    {
        protected int idNumber, matches;
        protected string title, proofs, type, docTitle, tags;
#pragma warning disable IDE0044 // Add readonly modifier
        private Document document;
#pragma warning restore IDE0044 // Add readonly modifier

        public Document()
        {
            this.idNumber = 0; this.title = ""; this.proofs = ""; this.type = ""; this.matches = 0; this.tags = "";
            this.docTitle = "";
        }
        public Document(string titleValue, int idValue, string proofValue, string typeValue)
        {
            this.idNumber = idValue; this.title = titleValue; this.Proofs = proofValue; this.type = typeValue;
        }
        public string Truncate(string value, int maxLength)
        {
            return value?.Substring(0, Math.Min(value.Length, maxLength));
        }

        public Document(Document sourceDocument)
        {
            this.document = sourceDocument;
        }


        [PrimaryKey, AutoIncrement]
        public int IDNumber
        {
            get
            {
                return idNumber;
            }
            set
            {
                if (value >= 0)
                {
                    idNumber = value;
                }
            }
        }





    }
}