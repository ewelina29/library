using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.ModelViews
{
    public class FineCopyView
    {
        [DisplayName("Id")]
        public virtual int Id { set; get; }

        [DisplayName("Signature")]
        public virtual int CopyId { set; get; }

        [DisplayName("Title")]
        public virtual string Title { set; get; }
        public virtual float Amount { set; get; }
        public virtual string Description { set; get; }
        public virtual bool Deleted { set; get; }


    }
}