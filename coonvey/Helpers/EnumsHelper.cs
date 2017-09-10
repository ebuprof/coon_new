using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coonvey.Helpers
{
    public class EnumsHelper
    {
        public static EnumsHelper GetEmptyOption()
        {
            return new EnumsHelper { Text = "", Value = null };
        }

        public int? Value { set; get; }
        public string Text { get; set; }
    }
}