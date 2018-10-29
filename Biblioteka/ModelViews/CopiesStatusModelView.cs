using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.ModelViews
{
    public class CopiesStatusesModelView
    {
        [DisplayName("Id")]
        public virtual int Id { set; get; }

        [DisplayName("Signature")]
        public virtual int CopyId { set; get; }

        [DisplayName("Reader")]
        public virtual string Reader { set; get; }

        [DisplayName("Title")]
        public virtual string Title { set; get; }

        public virtual int BookId { set; get; }

        [DisplayName("Date from")]
        public virtual string DateFrom { set; get; }

        [DisplayName("Date to")]
        public virtual string DateTo { set; get; }


    }
}