using QuickPad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPad
{
    public class SavedDocuments
    {
        public List<Document> Documents { get; set; }

        public SavedDocuments()
        {
            Documents = new List<Document>();
        }
    }
}
