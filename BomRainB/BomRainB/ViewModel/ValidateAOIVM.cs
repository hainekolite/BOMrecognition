using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class ValidateAOIVM : ViewModelBase
    {
        

        private readonly string title;
        public string Title => title;

        public ValidateAOIVM()
        {
            title = "Validate AOI";
        }

    }
}
