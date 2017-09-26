using System;
using System.Collections.Generic;
using Horse.Engine.Utils;
namespace Horse.Server.Helpers
{
    public sealed class StringHelper
    {
        public static string[] ReplaceAndToArray(string original, string targetToReplace)
        {
            if (!original.Contains(targetToReplace))
                return new[] {original};
            var toRet = original.Split(new []{targetToReplace}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < toRet.Length; i++)
            {
                toRet[i] = toRet[i].Trim();
            }
            return toRet;
        }
    }
}
