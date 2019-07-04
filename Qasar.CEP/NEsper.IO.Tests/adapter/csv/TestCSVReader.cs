///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;

using net.esper.adapter;
using net.esper.client;

using NUnit.Framework;

namespace net.esper.adapter.csv
{
    [TestFixture]
	public class TestCSVReader
	{
        internal class EquatableList<T> : List<T>
        {
            public override bool Equals(object obj)
            {
                EquatableList<T> tempList = obj as EquatableList<T>;
                if ( tempList == null )
                {
                    return false;
                }

                if ( tempList.Count != this.Count )
                {
                    return false;
                }

                for( int ii = tempList.Count - 1 ; ii >= 0 ; ii-- )
                {
                    if ( !Equals( tempList[ii], this[ii] ) )
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static List<T> AsList<T>( T[] itemArray )
        {
            List<T> itemList = new EquatableList<T>();
            itemList.AddRange(itemArray);
            return itemList;
        }

        [Test]
		public void TestParsing()
		{
			String path = "regression/parseTests.csv";
			CSVReader reader = new CSVReader(new AdapterInputSource(path));

			String[] nextRecord = reader.GetNextRecord();
			String[] expected = new String[] {"8", "8.0", "c", "'c'", "string", "string"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"", "string","", "string","","",""};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"leading spaces", "trailing spaces", ""};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"unquoted value 1", "unquoted value 2", ""};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"value with embedded \"\" quotes"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"value\r\nwith newline"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"value after empty lines"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"value after comments"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			try
			{
				reader.GetNextRecord();
				Assert.Fail();
			}
			catch(EndOfStreamException ex)
			{
				// Expected
			}
		}

        [Test]
		public void TestNonLooping()
		{
			AssertNonLooping("regression/endOnNewline.csv");
			AssertNonLooping("regression/endOnEOF.csv");
			AssertNonLooping("regression/endOnCommentedEOF.csv");
		}

        [Test]
		public void TestLooping()
		{
			AssertLooping("regression/endOnNewline.csv");
			AssertLooping("regression/endOnEOF.csv");
			AssertLooping("regression/endOnCommentedEOF.csv");
		}

		[Test]
        public void TestClose()
		{
			String path = "regression/parseTests.csv";
			CSVReader reader = new CSVReader(new AdapterInputSource(path));

			reader.Close();
			try
			{
				reader.GetNextRecord();
				Assert.Fail();
			}
			catch (EPException e)
			{
				// Expected
			}
			try
			{
				reader.Close();
				Assert.Fail();
			}
			catch (EPException e)
			{
				// Expected
			}
		}

        [Test]
		public void TestReset()
		{
			CSVReader reader = new CSVReader(new AdapterInputSource("regression/endOnNewline.csv"));

			String[] nextRecord = reader.GetNextRecord();
			String[] expected = new String[] {"first line", "1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			reader.Reset();

			nextRecord = reader.GetNextRecord();
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			reader.Reset();

			nextRecord = reader.GetNextRecord();
			Assert.AreEqual(AsList(expected), AsList(nextRecord));
		}

        [Test]
		public void TestTitleRow()
		{
			CSVReader reader = new CSVReader(new AdapterInputSource("regression/titleRow.csv"));
            reader.Looping = true;

			// isUsingTitleRow is false by default, so get the title row
			String[] nextRecord = reader.GetNextRecord();
			String[] expected = new String[] {"myString", "myInt", "timestamp", "myDouble"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			// Acknowledge the title row and reset the file afterwards
			reader.IsUsingTitleRow = true;
			reader.Reset();

			// First time through the file
			nextRecord = reader.GetNextRecord();
			expected = new String[] {"one", "1", "100", "1.1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"three", "3", "300", "3.3"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"five", "5", "500", "5.5"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			// Second time through the file
			nextRecord = reader.GetNextRecord();
			expected = new String[] {"one", "1", "100", "1.1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"three", "3", "300", "3.3"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"five", "5", "500", "5.5"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			// Pretend no title row again
			reader.IsUsingTitleRow = false;

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"myString", "myInt", "timestamp", "myDouble"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			reader.Reset();

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"myString", "myInt", "timestamp", "myDouble"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));
		}

        private static void AssertLooping(String path)
		{
			CSVReader reader = new CSVReader(new AdapterInputSource(path));
			reader.Looping = true;

			String[] nextRecord = reader.GetNextRecord();
			String[] expected = new String[] {"first line", "1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			Assert.IsTrue(reader.GetAndClearIsReset());

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"second line", "2"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			Assert.IsFalse(reader.GetAndClearIsReset());

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"first line", "1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			Assert.IsTrue(reader.GetAndClearIsReset());

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"second line", "2"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			Assert.IsFalse(reader.GetAndClearIsReset());

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"first line", "1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			Assert.IsTrue(reader.GetAndClearIsReset());

			reader.Close();
		}

		private static void AssertNonLooping(String path)
		{
			CSVReader reader = new CSVReader(new AdapterInputSource(path));

			String[] nextRecord = reader.GetNextRecord();
			String[] expected = new String[] {"first line", "1"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			nextRecord = reader.GetNextRecord();
			expected = new String[] {"second line", "2"};
			Assert.AreEqual(AsList(expected), AsList(nextRecord));

			try
			{
				reader.GetNextRecord();
				Assert.Fail();
			}
			catch(EndOfStreamException ex)
			{
				// Expected
			}

			reader.Close();
		}
	}
} // End of namespace
