using System;
using System.Collections.Generic;

namespace Horse.Server.Helpers
{
    public sealed class StringHelper
    {
        public static string[] ReplaceAndToArray(string original, string targetToReplace)
        {
            if (!original.Contains(targetToReplace))
                return new[] {original};
            var toReturn = new List<string>();
            while (original.Contains(targetToReplace))
            {
                if (original.Length - targetToReplace.Length <= 2)
                    break;
                string toAdd;
                if (original.StartsWith(targetToReplace))
                {
                    toAdd = original.Substring(targetToReplace.Length+1);
                    toAdd = toAdd.Substring(0, toAdd.IndexOf(targetToReplace, StringComparison.Ordinal)).Trim();
                }
                else
                {
                    toAdd = original.Substring(0, original.IndexOf(targetToReplace, StringComparison.Ordinal)).Trim();
                }
                original = original.Length > toAdd.Length ? original.Substring(toAdd.Length) : original.Substring(toAdd.Length - 1);
                toReturn.Add(toAdd);
            }
            return toReturn.ToArray();
        }
    }
}
