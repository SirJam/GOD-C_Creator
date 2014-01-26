using System;
using System.Collections.Generic;
using System.Text;

namespace God_C_Creator
{
    class GOD_C_SyntaxProvider
    {
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        #region ctor
        static GOD_C_SyntaxProvider()
        {
            string[] strs = {
                "main",
                "int",
                "bool",
                "write",
                "read",
                "return",
                "TRUE",
                "FALSE"
            };
            tags = new List<string>(strs);

            char[] chrs = {
                ',',
                ' ',
                ')',
                '(',
                '[',
                ']',
                '{',
                '}',
                '>',
                '<',
                '=',
                '!',
                ';',
                '\r',
                '\x9',
                '\x13',
                '\n',
                '\t'
            };
            specials = new List<char>(chrs);
        }
        #endregion
        public static List<char> GetSpecials
        {
            get { return specials; }
        }
        public static List<string> GetTags
        {
            get { return tags; }
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
}
