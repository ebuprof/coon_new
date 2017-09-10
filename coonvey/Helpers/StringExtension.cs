using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coonvey.Helpers
{
    public static class StringExtension
    {
        public static string Quoted(this string str)
        {
            return "\"" + str + "\"";
        }
    }
}