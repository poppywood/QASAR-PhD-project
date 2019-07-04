using System;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ImportOffersAttribute : Attribute
    {

        public string Path;

        public ImportOffersAttribute()
        {
            Path = "\uFFFD";
        }

    } // class ImportOffersAttribute

}

