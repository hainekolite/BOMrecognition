using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ModelHelpers
{
    public class AoiInterestData
    {
        public string partNumber { get; set; }
        public string referenceDesignator { get; set; }

        public AoiInterestData(string partNumber, string referenceDesignator)
        {
            this.partNumber = partNumber;
            this.referenceDesignator = referenceDesignator;
        }
    }

}
