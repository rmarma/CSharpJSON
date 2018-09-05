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

namespace CSharpJSON
{
    /**
 * A dense indexed sequence of values. Values may be any mix of
 * {@link JSONObject JSONObjects}, other {@link JSONArray JSONArrays}, Strings,
 * Booleans, Integers, Longs, Doubles, {@code null} or {@link JSONObject#NULL}.
 * Values may not be {@link Double#isNaN() NaNs}, {@link Double#isInfinite()
 * infinities}, or of any type not listed here.
 *
 * <p>{@code JSONArray} has the same type coercion behavior and
 * optional/mandatory accessors as {@link JSONObject}. See that class'
 * documentation for details.
 *
 * <p><strong>Warning:</strong> this class represents null in two incompatible
 * ways: the standard Java {@code null} reference, and the sentinel value {@link
 * JSONObject#NULL}. In particular, {@code get} fails if the requested index
 * holds the null reference, but succeeds if it holds {@code JSONObject.NULL}.
 *
 * <p>Instances of this class are not thread safe. Although this class is
 * nonfinal, it was not designed for inheritance and should not be subclassed.
 * In particular, self-use by overridable methods is not specified. See
 * <i>Effective Java</i> Item 17, "Design and Document or inheritance or else
 * prohibit it" for further information.
 */
    /// <summary>
    /// TODO summary
    /// </summary>
    public class JSONArray
    {
        private readonly List<object> values;

        /**
         * Creates a {@code JSONArray} with no values.
         */
        public JSONArray()
        {
            values = new List<object>();
        }

        /**
         * Creates a new {@code JSONArray} by copying all values from the given
         * collection.
         *
         * @param copyFrom a collection whose values are of supported types.
         *     Unsupported values are not permitted and will yield an array in an
         *     inconsistent state.
         */
        /* Accept a raw type for API compatibility */
        public JSONArray(IEnumerable<string> copyFrom) : this()
        {
            if (copyFrom != null)
            {
                foreach (var obj in copyFrom)
                {
                    Put(JSONObject.Wrap(obj));
                }
            }
        }

        /**
         * Creates a new {@code JSONArray} with values from the next array in the
         * tokener.
         *
         * @param readFrom a tokener whose nextValue() method will yield a
         *     {@code JSONArray}.
         * @throws JSONException if the parse fails or doesn't yield a
         *     {@code JSONArray}.
         */
        public JSONArray(JSONTokener readFrom)
        {
            /*
             * Getting the parser to populate this could get tricky. Instead, just
             * parse to temporary JSONArray and then steal the data from that.
             */
            object obj = readFrom.NextValue();
            if (obj is JSONArray)
            {
                values = ((JSONArray)obj).values;
            }
            else
            {
                throw JSON.TypeMismatch(obj, "JSONArray");
            }
        }

        /**
         * Creates a new {@code JSONArray} with values from the JSON string.
         *
         * @param json a JSON-encoded string containing an array.
         * @throws JSONException if the parse fails or doesn't yield a {@code
         *     JSONArray}.
         */
        public JSONArray(string json) : this(new JSONTokener(json))
        {
            
        }

        /**
         * Creates a new {@code JSONArray} with values from the given primitive array.
         */
        public JSONArray(object obj)
        {
            if (!obj.GetType().IsArray) {
                throw new JSONException("Not a primitive array: " + obj.GetType());
            }
            Array array = obj as Array;
            int length = array.Length;
            values = new List<object>(length);
            for (int i = 0; i < length; ++i) {
                Put(JSONObject.Wrap(array.GetValue(i)));
            }
        }

        /**
         * Returns the number of values in this array.
         */
        public int Length()
        {
            return values.Count;
        }

        /**
         * Appends {@code value} to the end of this array.
         *
         * @return this array.
         */
        public JSONArray Put(bool value)
        {
            values.Add(value);
            return this;
        }

        /**
         * Appends {@code value} to the end of this array.
         *
         * @param value a finite value. May not be {@link Double#isNaN() NaNs} or
         *     {@link Double#isInfinite() infinities}.
         * @return this array.
         */
        public JSONArray Put(double value)
        {
            values.Add(JSON.CheckDouble(value));
            return this;
        }

        /**
         * Appends {@code value} to the end of this array.
         *
         * @return this array.
         */
        public JSONArray Put(int value)
        {
            values.Add(value);
            return this;
        }

        /**
         * Appends {@code value} to the end of this array.
         *
         * @return this array.
         */
        public JSONArray Put(long value)
        {
            values.Add(value);
            return this;
        }

        /**
         * Appends {@code value} to the end of this array.
         *
         * @param value a {@link JSONObject}, {@link JSONArray}, String, Boolean,
         *     Integer, Long, Double, {@link JSONObject#NULL}, or {@code null}. May
         *     not be {@link Double#isNaN() NaNs} or {@link Double#isInfinite()
         *     infinities}. Unsupported values are not permitted and will cause the
         *     array to be in an inconsistent state.
         * @return this array.
         */
        public JSONArray Put(object value)
        {
            values.Add(value);
            return this;
        }

        /**
         * Same as {@link #put}, with added validity checks.
         */
        public void CheckedPut(object value)
        {
            if (value is double) {
                JSON.CheckDouble((double)value);
            }
            Put(value);
        }

        /**
         * Sets the value at {@code index} to {@code value}, null padding this array
         * to the required length if necessary. If a value already exists at {@code
         * index}, it will be replaced.
         *
         * @return this array.
         */
        public JSONArray Put(int index, bool value)
        {
            return Put(index, value);
        }

        /**
         * Sets the value at {@code index} to {@code value}, null padding this array
         * to the required length if necessary. If a value already exists at {@code
         * index}, it will be replaced.
         *
         * @param value a finite value. May not be {@link Double#isNaN() NaNs} or
         *     {@link Double#isInfinite() infinities}.
         * @return this array.
         */
        public JSONArray Put(int index, double value)
        {
            return Put(index, value);
        }

        /**
         * Sets the value at {@code index} to {@code value}, null padding this array
         * to the required length if necessary. If a value already exists at {@code
         * index}, it will be replaced.
         *
         * @return this array.
         */
        public JSONArray Put(int index, int value)
        {
            return Put(index, value);
        }

        /**
         * Sets the value at {@code index} to {@code value}, null padding this array
         * to the required length if necessary. If a value already exists at {@code
         * index}, it will be replaced.
         *
         * @return this array.
         */
        public JSONArray Put(int index, long value)
        {
            return Put(index, value);
        }

        /**
         * Sets the value at {@code index} to {@code value}, null padding this array
         * to the required length if necessary. If a value already exists at {@code
         * index}, it will be replaced.
         *
         * @param value a {@link JSONObject}, {@link JSONArray}, String, Boolean,
         *     Integer, Long, Double, {@link JSONObject#NULL}, or {@code null}. May
         *     not be {@link Double#isNaN() NaNs} or {@link Double#isInfinite()
         *     infinities}.
         * @return this array.
         */
        public JSONArray Put(int index, object value)
        {
            if (value is double)
            {
                // deviate from the original by checking all Numbers, not just floats & doubles
                JSON.CheckDouble((double)value);
            }
            while (values.Count <= index)
            {
                values.Add(null);
            }
            values[index] = value;
            return this;
        }

        /**
         * Returns true if this array has no value at {@code index}, or if its value
         * is the {@code null} reference or {@link JSONObject#NULL}.
         */
        public bool IsNull(int index)
        {
            object value = Opt(index);
            return value == null || value == JSONObject.NULL;
        }

        /**
         * Returns the value at {@code index}.
         *
         * @throws JSONException if this array has no value at {@code index}, or if
         *     that value is the {@code null} reference. This method returns
         *     normally if the value is {@code JSONObject#NULL}.
         */
        public object Get(int index)
        {
            try
            {
                object value = values[index];
                if (value == null)
                {
                    throw new JSONException("Value at " + index + " is null.");
                }
                return value;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new JSONException("Index " + index + " out of range [0.." + values.Count + ")", e);
            }
        }

        /**
         * Returns the value at {@code index}, or null if the array has no value
         * at {@code index}.
         */
        public object Opt(int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return null;
            }
            return values[index];
        }

        /**
         * Removes and returns the value at {@code index}, or null if the array has no value
         * at {@code index}.
         */
        public object Remove(int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return null;
            }
            var obj = values[index];
            values.RemoveAt(index);
            return obj;
        }

        /**
         * Returns the value at {@code index} if it exists and is a boolean or can
         * be coerced to a boolean.
         *
         * @throws JSONException if the value at {@code index} doesn't exist or
         *     cannot be coerced to a boolean.
         */
        public bool GetBoolean(int index)
        {
            object obj = Get(index);
            bool? result = JSON.ToBoolean(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(index, obj, "bool");
            }
            return (bool)result;
        }

        /**
         * Returns the value at {@code index} if it exists and is a boolean or can
         * be coerced to a boolean. Returns false otherwise.
         */
        public bool OptBoolean(int index)
        {
            return OptBoolean(index, false);
        }

        /**
         * Returns the value at {@code index} if it exists and is a boolean or can
         * be coerced to a boolean. Returns {@code fallback} otherwise.
         */
        public bool OptBoolean(int index, bool fallback)
        {
            object obj = Opt(index);
            bool? result = JSON.ToBoolean(obj);
            return result != null ? (bool)result : fallback;
        }

        /**
         * Returns the value at {@code index} if it exists and is a double or can
         * be coerced to a double.
         *
         * @throws JSONException if the value at {@code index} doesn't exist or
         *     cannot be coerced to a double.
         */
        public double GetDouble(int index)
        {
            object obj = Get(index);
            double? result = JSON.ToDouble(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(index, obj, "double");
            }
            return (double)result;
        }

        /**
         * Returns the value at {@code index} if it exists and is a double or can
         * be coerced to a double. Returns {@code NaN} otherwise.
         */
        public double OptDouble(int index)
        {
            return OptDouble(index, double.NaN);
        }

        /**
         * Returns the value at {@code index} if it exists and is a double or can
         * be coerced to a double. Returns {@code fallback} otherwise.
         */
        public double OptDouble(int index, double fallback)
        {
            object obj = Opt(index);
            double? result = JSON.ToDouble(obj);
            return result != null ? (double)result : fallback;
        }

        /**
         * Returns the value at {@code index} if it exists and is an int or
         * can be coerced to an int.
         *
         * @throws JSONException if the value at {@code index} doesn't exist or
         *     cannot be coerced to a int.
         */
        public int GetInt(int index)
        {
            object obj = Get(index);
            int? result = JSON.ToInteger(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(index, obj, "int");
            }
            return (int)result;
        }

        /**
         * Returns the value at {@code index} if it exists and is an int or
         * can be coerced to an int. Returns 0 otherwise.
         */
        public int OptInt(int index)
        {
            return OptInt(index, 0);
        }

        /**
         * Returns the value at {@code index} if it exists and is an int or
         * can be coerced to an int. Returns {@code fallback} otherwise.
         */
        public int OptInt(int index, int fallback)
        {
            object obj = Opt(index);
            int? result = JSON.ToInteger(obj);
            return result != null ? (int)result : fallback;
        }

        /**
         * Returns the value at {@code index} if it exists and is a long or
         * can be coerced to a long.
         *
         * @throws JSONException if the value at {@code index} doesn't exist or
         *     cannot be coerced to a long.
         */
        public long GetLong(int index)
        {
            object obj = Get(index);
            long? result = JSON.ToLong(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(index, obj, "long");
            }
            return (long)result;
        }

        /**
         * Returns the value at {@code index} if it exists and is a long or
         * can be coerced to a long. Returns 0 otherwise.
         */
        public long OptLong(int index)
        {
            return OptLong(index, 0L);
        }

        /**
         * Returns the value at {@code index} if it exists and is a long or
         * can be coerced to a long. Returns {@code fallback} otherwise.
         */
        public long OptLong(int index, long fallback)
        {
            object obj = Opt(index);
            long? result = JSON.ToLong(obj);
            return result != null ? (long)result : fallback;
        }

        /**
         * Returns the value at {@code index} if it exists, coercing it if
         * necessary.
         *
         * @throws JSONException if no such value exists.
         */
        public String GetString(int index)
        {
            object obj = Get(index);
            string result = JSON.ToString(obj);
            if (result == null)
            {
                throw JSON.TypeMismatch(index, obj, "string");
            }
            return result;
        }

        /**
         * Returns the value at {@code index} if it exists, coercing it if
         * necessary. Returns the empty string if no such value exists.
         */
        public String OptString(int index)
        {
            return OptString(index, "");
        }

        /**
         * Returns the value at {@code index} if it exists, coercing it if
         * necessary. Returns {@code fallback} if no such value exists.
         */
        public string OptString(int index, string fallback)
        {
            object obj = Opt(index);
            string result = JSON.ToString(obj);
            return result != null ? result : fallback;
        }

        /**
         * Returns the value at {@code index} if it exists and is a {@code
         * JSONArray}.
         *
         * @throws JSONException if the value doesn't exist or is not a {@code
         *     JSONArray}.
         */
        public JSONArray GetJSONArray(int index)
        {
            object obj = Get(index);
            if (obj is JSONArray)
            {
                return (JSONArray)obj;
            }
            else
            {
                throw JSON.TypeMismatch(index, obj, "JSONArray");
            }
        }

        /**
         * Returns the value at {@code index} if it exists and is a {@code
         * JSONArray}. Returns null otherwise.
         */
        public JSONArray OptJSONArray(int index)
        {
            object obj = Opt(index);
            return obj is JSONArray ? (JSONArray)obj : null;
        }

        /**
         * Returns the value at {@code index} if it exists and is a {@code
         * JSONObject}.
         *
         * @throws JSONException if the value doesn't exist or is not a {@code
         *     JSONObject}.
         */
        public JSONObject GetJSONObject(int index)
        {
            object obj = Get(index);
            if (obj is JSONObject)
            {
                return (JSONObject)obj;
            }
            else
            {
                throw JSON.TypeMismatch(index, obj, "JSONObject");
            }
        }

        /**
         * Returns the value at {@code index} if it exists and is a {@code
         * JSONObject}. Returns null otherwise.
         */
        public JSONObject OptJSONObject(int index)
        {
            object obj = Opt(index);
            return obj is JSONObject ? (JSONObject)obj : null;
        }

        /**
         * Returns a new object whose values are the values in this array, and whose
         * names are the values in {@code names}. Names and values are paired up by
         * index from 0 through to the shorter array's length. Names that are not
         * strings will be coerced to strings. This method returns null if either
         * array is empty.
         */
        public JSONObject ToJSONObject(JSONArray names)
        {
            JSONObject result = new JSONObject();
            int length = Math.Min(names.Length(), values.Count);
            if (length == 0)
            {
                return null;
            }
            for (int i = 0; i < length; ++i)
            {
                result.Put(JSON.ToString(names.Opt(i)), Opt(i));
            }
            return result;
        }

        /**
         * Returns a new string by alternating this array's values with {@code
         * separator}. This array's string values are quoted and have their special
         * characters escaped. For example, the array containing the strings '12"
         * pizza', 'taco' and 'soda' joined on '+' returns this:
         * <pre>"12\" pizza"+"taco"+"soda"</pre>
         */
        public string Join(string separator)
        {
            JSONStringer stringer = new JSONStringer();
            stringer.Open(JSONStringer.Scope.NULL, "");
            for (int i = 0, size = values.Count; i < size; ++i)
            {
                if (i > 0) {
                    stringer.sbOut.Append(separator);
                }
                stringer.Value(values[i]);
            }
            stringer.Close(JSONStringer.Scope.NULL, JSONStringer.Scope.NULL, "");
            return stringer.sbOut.ToString();
        }

        /**
         * Encodes this array as a compact JSON string, such as:
         * <pre>[94043,90210]</pre>
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
         * Encodes this array as a human readable JSON string for debugging, such
         * as:
         * <pre>
         * [
         *     94043,
         *     90210
         * ]</pre>
         *
         * @param indentSpaces the number of spaces to indent for each level of
         *     nesting.
         */
        public String ToString(int indentSpaces)
        {
            JSONStringer stringer = new JSONStringer(indentSpaces);
            WriteTo(stringer);
            return stringer.ToString();
        }

        public void WriteTo(JSONStringer stringer)
        {
            stringer.Array();
            foreach (object value in values)
            {
                stringer.Value(value);
            }
            stringer.EndArray();
        }

        public override bool Equals(object obj)
        {
            return obj is JSONArray && ((JSONArray)obj).values.Equals(values);
        }

        public override int GetHashCode()
        {
            return values.GetHashCode();
        }
    }
}
