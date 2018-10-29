using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.ModelViews
{
    public class BookCartModelView
    {
        [DisplayName("Copy id")]
        public virtual int CopyId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }

    }
}