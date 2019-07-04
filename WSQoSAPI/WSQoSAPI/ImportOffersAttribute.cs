using System;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ImportOffersAttribute : System.Attribute
    {

        public string Path;

        public ImportOffersAttribute()
        {
            Path = "\uFFFD";
        }

    } // class ImportOffersAttribute

}

