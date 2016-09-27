using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ModelHelpers
{
    public class BomInterestData
    {
        public string componentId { get; set; }
        public string referenceDesignator { get; set; }
        
        public BomInterestData(string componentId, string referenceDesignator)
        {
            this.componentId = componentId;
            this.referenceDesignator = referenceDesignator;
        }
    }
}
