using System;

using net.esper.compat;

namespace net.esper.support.bean
{
    [Serializable]
    public class SupportBeanComplexProps
    {
        private String _simpleProperty;
        private readonly EDictionary<string, string> _mappedProps;
        private readonly int[] _indexedProps;
        private readonly SupportBeanSpecialGetterNested _nested;
        private readonly Properties _mapProperty;
        private readonly int[] _arrayProperty;

        public static String[] PROPERTIES = {
    	    "SimpleProperty",
    	    "mapped()",
    	    "indexed[]",
    	    "MapProperty",
    	    "ArrayProperty",
    	    "Nested"
        };

        public static SupportBeanComplexProps MakeDefaultBean()
        {
            Properties properties = new Properties();
            properties.Add("keyOne", "valueOne");
            properties.Add("keyTwo", "valueTwo");

            Properties mapProp = new Properties();
            mapProp.Add("xOne", "yOne");
            mapProp.Add("xTwo", "yTwo");

            int[] arrayProp = new int[] { 10, 20, 30 };

            return new SupportBeanComplexProps(
                "simple", properties, new int[] { 1, 2 }, mapProp, arrayProp,
                "nestedValue",
                "nestedNestedValue");
        }

        public SupportBeanComplexProps(int[] indexedProps)
        {
            this._indexedProps = indexedProps;
        }

        public SupportBeanComplexProps(
            String simpleProperty,
            Properties mappedProps,
            int[] indexedProps,
            Properties mapProperty,
            int[] arrayProperty,
            String nestedValue,
            String nestedNestedValue)
        {
            this._simpleProperty = simpleProperty;
            this._mappedProps = mappedProps;
            this._indexedProps = indexedProps;
            this._mapProperty = mapProperty;
            this._arrayProperty = arrayProperty;
            this._nested = new SupportBeanSpecialGetterNested(nestedValue, nestedNestedValue);
        }

        public String SimpleProperty
        {
            get { return _simpleProperty; }
            set { _simpleProperty = value; }
        }

        public Properties MapProperty
        {
            get { return _mapProperty; }
        }

        public String GetMapped(String key)
        {
            return _mappedProps.Get(key);
        }

        public int GetIndexed(int index)
        {
            return _indexedProps[index];
        }

        public SupportBeanSpecialGetterNested Nested
        {
            get { return _nested; }
        }

        public int[] ArrayProperty
        {
            get { return _arrayProperty; }
        }

        public void SetIndexed(int index, int value)
        {
            _indexedProps[index] = value;
        }

        [Serializable]
        public class SupportBeanSpecialGetterNested
        {
            private String _nestedValue;
            private SupportBeanSpecialGetterNestedNested _nestedNested;

            public SupportBeanSpecialGetterNested(String nestedValue, String nestedNestedValue)
            {
                this._nestedValue = nestedValue;
                this._nestedNested = new SupportBeanSpecialGetterNestedNested(nestedNestedValue);
            }

            public String NestedValue
            {
                get { return _nestedValue; }
            }

            public SupportBeanSpecialGetterNestedNested NestedNested
            {
                get { return _nestedNested; }
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (o == null || GetType() != o.GetType())
                {
                    return false;
                }

                SupportBeanSpecialGetterNested that = (SupportBeanSpecialGetterNested)o;

                if (!Equals(_nestedValue, that._nestedValue))
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                return _nestedValue.GetHashCode();
            }
        }

        [Serializable]
        public class SupportBeanSpecialGetterNestedNested
        {
            private String _nestedNestedValue;

            public SupportBeanSpecialGetterNestedNested(String nestedNestedValue)
            {
                this._nestedNestedValue = nestedNestedValue;
            }

            public String NestedNestedValue
            {
                get { return _nestedNestedValue; }
            }
        }
    }
}
