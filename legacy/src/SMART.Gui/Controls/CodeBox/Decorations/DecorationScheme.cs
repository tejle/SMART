using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBoxControl.Decorations
{
 public  class DecorationScheme
    {
     List<Decoration> mDecorations = new List<Decoration>();

         public List<Decoration> BaseDecorations 
         {   get { return mDecorations;}
             set { mDecorations = value; } 
         }

         public string Name { get; set; }
    }
}
