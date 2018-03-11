/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// Note: this class was written without inspecting the non-free org.json sourcecode.

// R.M.A., 2018

using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSharpJSON
{
    /**
 * A modifiable set of name/value mappings. Names are unique, non-null strings.
 * Values may be any mix of {@link JSONObject JSONObjects}, {@link JSONArray
 * JSONArrays}, Strings, Booleans, Integers, Longs, Doubles or {@link #NULL}.
 * Values may not be {@code null}, {@link Double#isNaN() NaNs}, {@link
 * Double#isInfinite() infinities}, or of any type not listed here.
 *
 * <p>This class can coerce values to another type when requested.
 * <ul>
 *   <li>When the requested type is a boolean, strings will be coerced using a
 *       case-insensitive comparison to "true" and "false".
 *   <li>When the requested type is a double, other {@link Number} types will
 *       be coerced using {@link Number#doubleValue() doubleValue}. Strings
 *       that can be coerced using {@link Double#valueOf(String)} will be.
 *   <li>When the requested type is an int, other {@link Number} types will
 *       be coerced using {@link Number#intValue() intValue}. Strings
 *       that can be coerced using {@link Double#valueOf(String)} will be,
 *       and then cast to int.
 *   <li><a name="lossy">When the requested type is a long, other {@link Number} types will
 *       be coerced using {@link Number#longValue() longValue}. Strings
 *       that can be coerced using {@link Double#valueOf(String)} will be,
 *       and then cast to long. This two-step conversion is lossy for very
 *       large values. For example, the string "9223372036854775806" yields the
 *       long 9223372036854775807.</a>
 *   <li>When the requested type is a String, other non-null values will be
 *       coerced using {@link String#valueOf(Object)}. Although null cannot be
 *       coerced, the sentinel value {@link JSONObject#NULL} is coerced to the
 *       string "null".
 * </ul>
 *
 * <p>This class can look up both mandatory and optional values:
 * <ul>
 *   <li>Use <code>get<i>Type</i>()</code> to retrieve a mandatory value. This
 *       fails with a {@code JSONException} if the requested name has no value
 *       or if the value cannot be coerced to the requested type.
 *   <li>Use <code>opt<i>Type</i>()</code> to retrieve an optional value. This
 *       returns a system- or user-supplied default if the requested name has no
 *       value or if the value cannot be coerced to the requested type.
 * </ul>
 *
 * <p><strong>Warning:</strong> this class represents null in two incompatible
 * ways: the standard Java {@code null} reference, and the sentinel value {@link
 * JSONObject#NULL}. In particular, calling {@code put(name, null)} removes the
 * named entry from the object but {@code put(name, JSONObject.NULL)} stores an
 * entry whose value is {@code JSONObject.NULL}.
 *
 * <p>Instances of this class are not thread safe. Although this class is
 * nonfinal, it was not designed for inheritance and should not be subclassed.
 * In particular, self-use by overrideable methods is not specified. See
 * <i>Effective Java</i> Item 17, "Design and Document or inheritance or else
 * prohibit it" for further information.
 */
    /// <summary>
    /// TODO summary
    /// </summary>
    public class JSONObject
    {

        public sealed class NullObject
        {

            public override bool Equals(object obj)
            {
                return obj == this || obj == null; // API specifies this broken equals implementation
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "null";
            }
        }

        private const double NEGATIVE_ZERO = -0d;

        /**
         * A sentinel value used to explicitly define a name with no value. Unlike
         * {@code null}, names with this value:
         * <ul>
         *   <li>show up in the {@link #names} array
         *   <li>show up in the {@link #keys} iterator
         *   <li>return {@code true} for {@link #has(String)}
         *   <li>do not throw on {@link #get(String)}
         *   <li>are included in the encoded JSON string.
         * </ul>
         *
         * <p>This value violates the general contract of {@link Object#equals} by
         * returning true when compared to {@code null}. Its {@link #toString}
         * method returns "null".
         */
        public static readonly object NULL = new NullObject();

        private readonly Dictionary<string, object> nameValuePairs;

        /**
         * Creates a {@code JSONObject} with no name/value mappings.
         */
        public JSONObject()
        {
            nameValuePairs = new Dictionary<string, object>();
        }

        /**
         * Creates a new {@code JSONObject} by copying all name/value mappings from
         * the given map.
         *
         * @param copyFrom a map whose keys are of type {@link String} and whose
         *     values are of supported types.
         * @throws NullPointerException if any of the map's keys are null.
         */
        /* (accept a raw type for API compatibility) */
        public JSONObject(IDictionary<string, object> copyFrom) : this()
        {
            foreach (var entry in copyFrom)
            {
                if (entry.Key == null)
                {
                    throw new NullReferenceException("entry.Key == null");
                }
                if (!nameValuePairs.ContainsKey(entry.Key))
                {
                    nameValuePairs.Add(entry.Key, Wrap(entry.Value));
                }
                else
                {
                    nameValuePairs[entry.Key] = Wrap(entry.Value);
                }
            }
        }

        /**
         * Creates a new {@code JSONObject} with name/value mappings from the next
         * object in the tokener.
         *
         * @param readFrom a tokener whose nextValue() method will yield a
         *     {@code JSONObject}.
         * @throws JSONException if the parse fails or doesn't yield a
         *     {@code JSONObject}.
         */
        public JSONObject(JSONTokener readFrom)
        {
            /*
             * Getting the parser to populate this could get tricky. Instead, just
             * parse to temporary JSONObject and then steal the data from that.
             */
            object obj = readFrom.NextValue();
            if (obj is JSONObject)
            {
                nameValuePairs = ((JSONObject)obj).nameValuePairs;
            }
            else
            {
                throw JSON.TypeMismatch(obj, "JSONObject");
            }
        }

        /**
         * Creates a new {@code JSONObject} with name/value mappings from the JSON
         * string.
         *
         * @param json a JSON-encoded string containing an object.
         * @throws JSONException if the parse fails or doesn't yield a {@code
         *     JSONObject}.
         */
        public JSONObject(string json) : this(new JSONTokener(json))
        {

        }

        /**
         * Creates a new {@code JSONObject} by copying mappings for the listed names
         * from the given object. Names that aren't present in {@code copyFrom} will
         * be skipped.
         */
        public JSONObject(JSONObject copyFrom, String[] names) : this()
        {
            object value;
            foreach (string name in names)
            {
                value = copyFrom.Opt(name);
                if (value != null)
                {
                    if (!nameValuePairs.ContainsKey(name))
                    {
                        nameValuePairs.Add(name, value);
                    }
                    else
                    {
                        nameValuePairs[name] = value;
                    }
                }
            }
        }

        /**
         * Returns the number of name/value mappings in this object.
         */
        public int Length()
        {
            return nameValuePairs.Count;
        }

        /**
         * Maps {@code name} to {@code value}, clobbering any existing name/value
         * mapping with the same name.
         *
         * @return this object.
         */
        public JSONObject Put(string name, bool value)
        {
            CheckName(name);
            if (!nameValuePairs.ContainsKey(name))
            {
                nameValuePairs.Add(name, value);
            }
            else
            {
                nameValuePairs[name] = value;
            }
            return this;
        }

        /**
         * Maps {@code name} to {@code value}, clobbering any existing name/value
         * mapping with the same name.
         *
         * @param value a finite value. May not be {@link Double#isNaN() NaNs} or
         *     {@link Double#isInfinite() infinities}.
         * @return this object.
         */
        public JSONObject Put(string name, double value)
        {
            CheckName(name);
            if (!nameValuePairs.ContainsKey(name))
            {
                nameValuePairs.Add(name, value);
            }
            else
            {
                nameValuePairs[name] = value;
            }
            return this;
        }

        /**
         * Maps {@code name} to {@code value}, clobbering any existing name/value
         * mapping with the same name.
         *
         * @return this object.
         */
        public JSONObject Put(string name, int value)
        {
            CheckName(name);
            if (!nameValuePairs.ContainsKey(name))
            {
                nameValuePairs.Add(name, value);
            }
            else
            {
                nameValuePairs[name] = value;
            }
            return this;
        }

        /**
         * Maps {@code name} to {@code value}, clobbering any existing name/value
         * mapping with the same name.
         *
         * @return this object.
         */
        public JSONObject Put(string name, long value)
        {
            CheckName(name);
            if (!nameValuePairs.ContainsKey(name))
            {
                nameValuePairs.Add(name, value);
            }
            else
            {
                nameValuePairs[name] = value;
            }
            return this;
        }

        /**
         * Maps {@code name} to {@code value}, clobbering any existing name/value
         * mapping with the same name. If the value is {@code null}, any existing
         * mapping for {@code name} is removed.
         *
         * @param value a {@link JSONObject}, {@link JSONArray}, String, Boolean,
         *     Integer, Long, Double, {@link #NULL}, or {@code null}. May not be
         *     {@link Double#isNaN() NaNs} or {@link Double#isInfinite()
         *     infinities}.
         * @return this object.
         */
        public JSONObject Put(string name, object value)
        {
            CheckName(name);
            if (value == null)
            {
                if (nameValuePairs.ContainsKey(name))
                {
                    nameValuePairs.Remove(name);
                }
                return this;
            }
            if (!nameValuePairs.ContainsKey(name))
            {
                nameValuePairs.Add(name, value);
            }
            else
            {
                nameValuePairs[name] = value;
            }
            return this;
        }

        /**
         * Equivalent to {@code put(name, value)} when both parameters are non-null;
         * does nothing otherwise.
         */
        public JSONObject PutOpt(string name, object value)
        {
            if (name == null || value == null)
            {
                return this;
            }
            return Put(name, value);
        }

        /**
         * Appends {@code value} to the array already mapped to {@code name}. If
         * this object has no mapping for {@code name}, this inserts a new mapping.
         * If the mapping exists but its value is not an array, the existing
         * and new values are inserted in order into a new array which is itself
         * mapped to {@code name}. In aggregate, this allows values to be added to a
         * mapping one at a time.
         *
         * <p> Note that {@code append(String, Object)} provides better semantics.
         * In particular, the mapping for {@code name} will <b>always</b> be a
         * {@link JSONArray}. Using {@code accumulate} will result in either a
         * {@link JSONArray} or a mapping whose type is the type of {@code value}
         * depending on the number of calls to it.
         *
         * @param value a {@link JSONObject}, {@link JSONArray}, String, Boolean,
         *     Integer, Long, Double, {@link #NULL} or null. May not be {@link
         *     Double#isNaN() NaNs} or {@link Double#isInfinite() infinities}.
         */
        // TODO: Change {@code append) to {@link #append} when append is
        // unhidden.
        public JSONObject Accumulate(string name, object value)
        {
            object current = null;
            if (nameValuePairs.ContainsKey(CheckName(name)))
            {
                current = nameValuePairs[name];
            }
            if (current == null)
            {
                return Put(name, value);
            }

            if (current is JSONArray)
            {
                JSONArray array = (JSONArray)current;
                array.CheckedPut(value);
            }
            else
            {
                JSONArray array = new JSONArray();
                array.CheckedPut(current);
                array.CheckedPut(value);

                nameValuePairs.Add(name, array);
            }
            return this;
        }

        /**
         * Appends values to the array mapped to {@code name}. A new {@link JSONArray}
         * mapping for {@code name} will be inserted if no mapping exists. If the existing
         * mapping for {@code name} is not a {@link JSONArray}, a {@link JSONException}
         * will be thrown.
         *
         * @throws JSONException if {@code name} is {@code null} or if the mapping for
         *         {@code name} is non-null and is not a {@link JSONArray}.
         *
         * @hide
         */
        public JSONObject Append(String name, Object value)
        {
            object current = null;
            if (nameValuePairs.ContainsKey(CheckName(name)))
            {
                current = nameValuePairs[name];
            }

            JSONArray array;
            if (current is JSONArray)
            {
                array = (JSONArray)current;
            }
            else if (current == null)
            {
                JSONArray newArray = new JSONArray();
                if (!nameValuePairs.ContainsKey(name))
                {
                    nameValuePairs.Add(name, newArray);
                }
                else
                {
                    nameValuePairs[name] = newArray;
                }
                array = newArray;
            }
            else
            {
                throw new JSONException("Key " + name + " is not a JSONArray");
            }

            array.CheckedPut(value);

            return this;
        }

        private string CheckName(string name)
        {
            if (name == null)
            {
                throw new JSONException("Names must be non-null");
            }
            return name;
        }

        /// <summary>
        /// Removes the named mapping if it exists; does nothing otherwise.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the value previously mapped by {@code name}, or null if there was no such mapping.</returns>
        public object Remove(string name)
        {
            object obj = null;
            if (nameValuePairs.ContainsKey(name))
            {
                obj = nameValuePairs[name];
                nameValuePairs.Remove(name);
            }
            return obj;
        }

        /**
         * Returns true if this object has no mapping for {@code name} or if it has
         * a mapping whose value is {@link #NULL}.
         */
        public bool IsNull(string name)
        {
            object value = null;
            if (nameValuePairs.ContainsKey(name))
            {
                value = nameValuePairs[name];
            }
            return value == null || value == NULL;
        }

        /**
         * Returns true if this object has a mapping for {@code name}. The mapping
         * may be {@link #NULL}.
         */
        public bool Has(string name)
        {
            return nameValuePairs.ContainsKey(name);
        }

        /**
         * Returns the value mapped by {@code name}, or throws if no such mapping exists.
         *
         * @throws JSONException if no such mapping exists.
         */
        public Object Get(string name)
        {
            object result = Opt(name);
            if (result == null)
            {
                throw new JSONException("No value for " + name);
            }
            return result;
        }

        /**
         * Returns the value mapped by {@code name}, or null if no such mapping
         * exists.
         */
        public object Opt(string name)
        {
            return Has(name) ? nameValuePairs[name] : null;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a boolean or
         * can be coerced to a boolean, or throws otherwise.
         *
         * @throws JSONException if the mapping doesn't exist or cannot be coerced
         *     to a boolean.
         */
        public bool GetBoolean(string name)
        {
            object obj = Get(name);
            bool? result = JSON.ToBoolean(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(name, obj, "bool");
            }
            return (bool)result;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a boolean or
         * can be coerced to a boolean, or false otherwise.
         */
        public bool OptBoolean(string name)
        {
            return OptBoolean(name, false);
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a boolean or
         * can be coerced to a boolean, or {@code fallback} otherwise.
         */
        public bool OptBoolean(String name, bool fallback)
        {
            object obj = Opt(name);
            bool? result = JSON.ToBoolean(obj);
            return result != null ? (bool)result : fallback;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a double or
         * can be coerced to a double, or throws otherwise.
         *
         * @throws JSONException if the mapping doesn't exist or cannot be coerced
         *     to a double.
         */
        public double GetDouble(string name)
        {
            object obj = Get(name);
            double? result = JSON.ToDouble(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(name, obj, "double");
            }
            return (double)result;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a double or
         * can be coerced to a double, or {@code NaN} otherwise.
         */
        public double OptDouble(string name)
        {
            return OptDouble(name, double.NaN);
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a double or
         * can be coerced to a double, or {@code fallback} otherwise.
         */
        public double OptDouble(string name, double fallback)
        {
            object obj = Opt(name);
            double? result = JSON.ToDouble(obj);
            return result != null ? (double)result : fallback;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is an int or
         * can be coerced to an int, or throws otherwise.
         *
         * @throws JSONException if the mapping doesn't exist or cannot be coerced
         *     to an int.
         */
        public int GetInt(string name)
        {
            object obj = Get(name);
            int? result = JSON.ToInteger(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(name, obj, "int");
            }
            return (int)result;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is an int or
         * can be coerced to an int, or 0 otherwise.
         */
        public int OptInt(string name)
        {
            return OptInt(name, 0);
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is an int or
         * can be coerced to an int, or {@code fallback} otherwise.
         */
        public int OptInt(string name, int fallback)
        {
            object obj = Opt(name);
            int? result = JSON.ToInteger(obj);
            return result != null ? (int)result : fallback;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a long or
         * can be coerced to a long, or throws otherwise.
         * Note that JSON represents numbers as doubles,
         * so this is <a href="#lossy">lossy</a>; use strings to transfer numbers via JSON.
         *
         * @throws JSONException if the mapping doesn't exist or cannot be coerced
         *     to a long.
         */
        public long GetLong(string name)
        {
            object obj = Opt(name);
            long? result = JSON.ToLong(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(name, obj, "long");
            }
            return (long)result;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a long or
         * can be coerced to a long, or 0 otherwise. Note that JSON represents numbers as doubles,
         * so this is <a href="#lossy">lossy</a>; use strings to transfer numbers via JSON.
         */
        public long OptLong(string name)
        {
            return OptLong(name, 0L);
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a long or
         * can be coerced to a long, or {@code fallback} otherwise. Note that JSON represents
         * numbers as doubles, so this is <a href="#lossy">lossy</a>; use strings to transfer
         * numbers via JSON.
         */
        public long OptLong(string name, long fallback)
        {
            object obj = Opt(name);
            long? result = JSON.ToLong(obj);
            return result != null ? (long)result : fallback;
        }

        /**
         * Returns the value mapped by {@code name} if it exists, coercing it if
         * necessary, or throws if no such mapping exists.
         *
         * @throws JSONException if no such mapping exists.
         */
        public string GetString(string name)
        {
            object obj = Opt(name);
            string result = JSON.ToString(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(name, obj, "string");
            }
            return result;
        }

        /**
         * Returns the value mapped by {@code name} if it exists, coercing it if
         * necessary, or the empty string if no such mapping exists.
         */
        public string OptString(string name)
        {
            return OptString(name, string.Empty);
        }

        /**
         * Returns the value mapped by {@code name} if it exists, coercing it if
         * necessary, or {@code fallback} if no such mapping exists.
         */
        public string OptString(string name, string fallback)
        {
            object obj = Opt(name);
            string result = JSON.ToString(obj);
            return result != null ? result : fallback;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a {@code
         * JSONArray}, or throws otherwise.
         *
         * @throws JSONException if the mapping doesn't exist or is not a {@code
         *     JSONArray}.
         */
        public JSONArray GetJSONArray(string name)
        {
            object obj = Opt(name);
            if (obj is JSONArray)
            {
                return (JSONArray)obj;
            }
            else
            {
                throw JSON.TypeMismatch(name, obj, "JSONArray");
            }
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a {@code
         * JSONArray}, or null otherwise.
         */
        public JSONArray OptJSONArray(string name)
        {
            object obj = Opt(name);
            return obj is JSONArray ? (JSONArray)obj : null;
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a {@code
         * JSONObject}, or throws otherwise.
         *
         * @throws JSONException if the mapping doesn't exist or is not a {@code
         *     JSONObject}.
         */
        public JSONObject GetJSONObject(String name)
        {
            object obj = Opt(name);
            if (obj is JSONObject)
            {
                return (JSONObject)obj;
            }
            else
            {
                throw JSON.TypeMismatch(name, obj, "JSONObject");
            }
        }

        /**
         * Returns the value mapped by {@code name} if it exists and is a {@code
         * JSONObject}, or null otherwise.
         */
        public JSONObject OptJSONObject(string name)
        {
            object obj = Opt(name);
            return obj is JSONObject ? (JSONObject)obj : null;
        }

        /**
         * Returns an array with the values corresponding to {@code names}. The
         * array contains null for names that aren't mapped. This method returns
         * null if {@code names} is either null or empty.
         */
        public JSONArray ToJSONArray(JSONArray names)
        {
            JSONArray result = new JSONArray();
            if (names == null)
            {
                return null;
            }
            int length = names.Length();
            if (length == 0)
            {
                return null;
            }
            for (int i = 0; i < length; ++i)
            {
                string name = JSON.ToString(names.Opt(i));
                result.Put(Opt(name));
            }
            return result;
        }

        /**
         * Returns an iterator of the {@code String} names in this object. The
         * returned iterator supports {@link Iterator#remove() remove}, which will
         * remove the corresponding mapping from this object. If this object is
         * modified after the iterator is returned, the iterator's behavior is
         * undefined. The order of the keys is undefined.
         */
        public IEnumerator<string> Keys()
        {
            return nameValuePairs.Keys.GetEnumerator();
        }

        /**
         * Returns the set of {@code String} names in this object. The returned set
         * is a view of the keys in this object. {@link Set#remove(Object)} will remove
         * the corresponding mapping from this object and set iterator behaviour
         * is undefined if this object is modified after it is returned.
         *
         * See {@link #keys()}.
         *
         * @hide.
         */
        public IEnumerable<string> KeyCollection()
        {
            return nameValuePairs.Keys;
        }

        /**
         * Returns an array containing the string names in this object. This method
         * returns null if this object contains no mappings.
         */
        public JSONArray Names()
        {
            return nameValuePairs.Count <= 0
                    ? null
                    : new JSONArray(new List<string>(KeyCollection()));
        }

        /**
         * Encodes this object as a compact JSON string, such as:
         * <pre>{"query":"Pizza","locations":[94043,90210]}</pre>
         */
        public override string ToString()
        {
            try
            {
                JSONStringer stringer = new JSONStringer();
                WriteTo(stringer);
                return stringer.ToString();
            }
            catch (JSONException e)
            {
                return null;
            }
        }

        /**
         * Encodes this object as a human readable JSON string for debugging, such
         * as:
         * <pre>
         * {
         *     "query": "Pizza",
         *     "locations": [
         *         94043,
         *         90210
         *     ]
         * }</pre>
         *
         * @param indentSpaces the number of spaces to indent for each level of
         *     nesting.
         */
        public string ToString(int indentSpaces)
        {
            JSONStringer stringer = new JSONStringer(indentSpaces);
            WriteTo(stringer);
            return stringer.ToString();
        }

        public void WriteTo(JSONStringer stringer)
        {
            stringer.Obj();
            foreach (var entry in nameValuePairs)
            {
                stringer.Key(entry.Key).Value(entry.Value);
            }
            stringer.EndObject();
        }

        /**
         * Encodes the number as a JSON string.
         *
         * @param number a finite value. May not be {@link Double#isNaN() NaNs} or
         *     {@link Double#isInfinite() infinities}.
         */
        public static string NumberToString(object number)
        {
            if (number == null)
            {
                throw new JSONException("Number must be non-null");
            }

            double? doubleValue = JSON.ToDouble(number);
            if (doubleValue != null)
            {
                JSON.CheckDouble((double)doubleValue);
            }

            // the original returns "-0" instead of "-0.0" for negative zero
            if (doubleValue != null && number.Equals(NEGATIVE_ZERO))
            {
                return "-0";
            }

            long? longValue = JSON.ToLong(number);
            if (doubleValue == (double)longValue)
            {
                return longValue.ToString();
            }

            if (number is double)
            {
                return ((double)number).ToString(CultureInfo.InvariantCulture);
            }
            else if (number is float)
            {
                return ((float)number).ToString(CultureInfo.InvariantCulture);
            }
            return number.ToString();
        }

        /**
         * Encodes {@code data} as a JSON string. This applies quotes and any
         * necessary character escaping.
         *
         * @param data the string to encode. Null will be interpreted as an empty
         *     string.
         */
        public static string Quote(string data)
        {
            if (data == null)
            {
                return "\"\"";
            }
            try
            {
                JSONStringer stringer = new JSONStringer();
                stringer.Open(JSONStringer.Scope.NULL, "");
                stringer.Value(data);
                stringer.Close(JSONStringer.Scope.NULL, JSONStringer.Scope.NULL, "");
                return stringer.ToString();
            }
            catch (JSONException e)
            {
                throw e;
            }
        }

        /**
         * Wraps the given object if necessary.
         *
         * <p>If the object is null or , returns {@link #NULL}.
         * If the object is a {@code JSONArray} or {@code JSONObject}, no wrapping is necessary.
         * If the object is {@code NULL}, no wrapping is necessary.
         * If the object is an array or {@code Collection}, returns an equivalent {@code JSONArray}.
         * If the object is a {@code Map}, returns an equivalent {@code JSONObject}.
         * If the object is a primitive wrapper type or {@code String}, returns the object.
         * Otherwise if the object is from a {@code java} package, returns the result of {@code toString}.
         * If wrapping fails, returns null.
         */
        public static object Wrap(object o)
        {
            if (o == null)
            {
                return NULL;
            }
            if (o is JSONArray || o is JSONObject)
            {
                return o;
            }
            if (o.Equals(NULL))
            {
                return o;
            }
            try
            {
                if (o is IEnumerable<string>)
                {
                    return new JSONArray((IEnumerable<string>)o);
                }
                else if (o.GetType().IsArray)
                {
                    return new JSONArray(o);
                }
                if (o is IDictionary<string, object>)
                {
                    return new JSONObject((IDictionary<string, object>)o);
                }
                if (o is bool ||
                    o is byte ||
                    o is char ||
                    o is double ||
                    o is float ||
                    o is int ||
                    o is long ||
                    o is short ||
                    o is string)
                {
                    return o;
                }
                return o.ToString();
            }
            catch (Exception ignored)
            {
            }
            return null;
        }
    }
}
