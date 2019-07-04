using System;

using NUnit.Framework;

namespace net.esper.util
{
    [TestFixture]
    public class TestLikeUtil
    {
        [Test]
        public void testLike()
        {
            tryMatches(
                "%aa%", '\\',
                new String[] { "aa", " aa", "aa ", "0aa0", "%aa%" },
                new String[] { "ba", " bea", "a a", " a a a a", "yyya ay" });

            tryMatches(
                "a%", '\\',
                new String[] { "a", "allo", "aa ", "aa0", "aa%" },
                new String[] { " a", "ba", "\\a", " aa", "ya ay" });

            tryMatches(
                "%ab", '\\',
                new String[] { "dgdgab", "ab", " ab", "addhf ab ab", "  ab" },
                new String[] { " a", "ba", "a", "ac", "ay" });

            tryMatches(
                "c%ab", '\\',
                new String[] { "cab", "cgfhghab", "c ab", "cddhf ab ab", "c  ab" },
                new String[] { "c aa", "c ba", " ab", "c aba", "c b" });

            tryMatches(
                "c%ab", '\\',
                new String[] { "cab", "cgfhghab", "c ab", "cddhf ab ab", "c  ab" },
                new String[] { "c aa", "c ba", " ab", "c aba", "c b" });

            tryMatches(
                "c%ab%c", '\\',
                new String[] { "cabc", "cgfhghabc", "c ab  c", "cddhf ab ab c", "c  abc" },
                new String[] { "c aa", "c ab", " ab c", "c ab a", "c ba c" });

            tryMatches(
                "_d%c", '\\',
                new String[] { "adbc", "adc", "adbfhfhfhhfc", "xd99c", "4d%c" },
                new String[] { "ccdac", "qdtb", "yydc", "9d9e", "111gd" });

            tryMatches(
                "___a", '\\',
                new String[] { "aaaa", "736a", "   a", "oooa", "___a" },
                new String[] { "  a", "    a", "uua", "uuuua", "9999b" });

            tryMatches(
                "___a___", '\\',
                new String[] { "bbbabbb", "bbba   ", "   abbb", "   a   ", "%%%a%%%" },
                new String[] { "   a  ", "  a   ", "  a ", "    a    ", "x   a   " });

            tryMatches(
                "%_a_%", '\\',
                new String[] { "dhdhdhdh a djdjdj", " a ", "kdkd a ", " a dkdkd", "   a   ", "%%%a%%%" },
                new String[] { "a", " a", "a ", "    b    ", " qa" });

            tryMatches(
                "!%do", '!',
                new String[] { "%do" },
                new String[] { "!do", "do", "ado" });

            tryMatches(
                "!_do", '!',
                new String[] { "_do" },
                new String[] { "!do", "do", "ado" });
        }

        private void tryMatches(String pattern, char escape, String[] stringMatching, String[] stringNotMatching)
        {
            LikeUtil helper = new LikeUtil(pattern, escape, false);

            for (int i = 0; i < stringMatching.Length; i++)
            {
                String text = "Expected match for pattern '" + pattern + "' and string '" + stringMatching[i] + "'";
                bool? testComp = helper.Compare(stringMatching[i]);
                Assert.IsTrue(testComp.GetValueOrDefault(false), text);
            }

            for (int i = 0; i < stringNotMatching.Length; i++)
            {
                String text = "Expected mismatch for pattern '" + pattern + "' and string '" + stringNotMatching[i] + "'";
                bool? testComp = helper.Compare(stringNotMatching[i]);
                Assert.IsFalse(testComp.GetValueOrDefault(false), text);
            }
        }
    }
}