using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Linq;

namespace App
{
    public class StringUtil
    {

        public static int[] GetIntArrayFromString(string commaSeparatedString)
        {
            if (String.IsNullOrEmpty(commaSeparatedString))
            {
                return new int[0];
            }
            else
            {
                return commaSeparatedString.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            }
        }

        //public static string GetJSBeautifyString(string source)
        //{
        //    var jsb = new JSBeautifyLib.JSBeautify(source, new JSBeautifyLib.JSBeautifyOptions());
        //    return jsb.GetResult();
        //}

    }
}
