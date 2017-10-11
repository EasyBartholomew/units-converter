using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    class ConvertedDimention
    {

        public string Dimension { get; private set; }

        public string Value { get; private set; }

        public ConvertedDimention(string dimension, string value)
        {
            Dimension = dimension;
            Value = value;
        }


    }
}
