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

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSharpJSON
{
    /// <summary>
    /// TODO summary
    /// Implements {@link JSONObject#toString} and {@link JSONArray#toString}. Most
    /// application developers should use those methods directly and disregard this
    /// API. For example:<pre>
    /// JSONObject object = ...
    /// String json = object.toString();</pre>
    ///
    /// <p>Stringers only encode well-formed JSON strings. In particular:
    /// <ul>
    ///   <li>The stringer must have exactly one top-level array or object.
    ///   <li>Lexical scopes must be balanced: every call to {@link #array} must
    ///       have a matching call to {@link #endArray} and every call to {@link
    ///       #object} must have a matching call to {@link #endObject}.
    ///   <li>Arrays may not contain keys (property names).
    ///   <li>Objects must alternate keys (property names) and values.
    ///   <li>Values are inserted with either literal {@link #value(Object) value}
    ///      calls, or by nesting arrays or objects.
    /// </ul>
    /// Calls that would result in a malformed JSON string will fail with a
    /// {@link JSONException}.
    ///
    /// <p>This class provides no facility for pretty-printing (ie. indenting)
    /// output. To encode indented output, use {@link JSONObject#toString(int)} or
    /// {@link JSONArray#toString(int)}.
    ///
    /// <p>Some implementations of the API support at most 20 levels of nesting.
    /// Attempts to create more than 20 levels of nesting may fail with a {@link
    /// JSONException}.
    ///
    /// <p>Each stringer may be used to encode a single top level value. Instances of
    /// this class are not thread safe. Although this class is nonfinal, it was not
    /// designed for inheritance and should not be subclassed. In particular,
    /// self-use by overrideable methods is not specified. See <i>Effective Java</i>
    /// Item 17, "Design and Document or inheritance or else prohibit it" for further
    /// information.
    /// </summary>
    public class JSONStringer
    {
        /// <summary>
        /// The output data, containing at most one top-level array or object.
        /// </summary>
        public readonly StringBuilder sbOut = new StringBuilder();

        /// <summary>
        /// Lexical scoping elements within this stringer, necessary to insert the
        /// appropriate separator characters(ie.commas and colons) and to detect
        /// nesting errors.
        /// </summary>
        public enum Scope
        {
            /// <summary>
            /// An array with no elements requires no separators or newlines before
            /// it is closed.
            /// </summary>
            EMPTY_ARRAY,

            /// <summary>
            /// A array with at least one value requires a comma and newline before
            /// the next element.
            /// </summary>
            NONEMPTY_ARRAY,

            /// <summary>
            /// An object with no keys or values requires no separators or newlines
            /// before it is closed.
            /// </summary>
            EMPTY_OBJECT,

            /// <summary>
            /// An object whose most recent element is a key. The next element must
            /// be a value.
            /// </summary>
            DANGLING_KEY,

            /// <summary>
            /// An object with at least one name/value pair requires a comma and
            /// newline before the next element.
            /// </summary>
            NONEMPTY_OBJECT,

            /// <summary>
            /// A special bracketless array needed by JSONStringer.Join() and
            /// JSONObject.Quote() only. Not used for JSON encoding.
            /// </summary>
            NULL
        }

        /// <summary>
        /// Unlike the original implementation, this stack isn't limited to 20
        /// levels of nesting.
        /// </summary>
        private readonly List<Scope> stack = new List<Scope>();

        /// <summary>
        /// A string containing a full set of spaces for a single level of
        /// indentation, or null for no pretty printing.
        /// </summary>
        private readonly string indent;

        public JSONStringer()
        {
            indent = null;
        }

        public JSONStringer(int indentSpaces)
        {
            char[] indentChars = new char[indentSpaces];
            for (int i = 0; i < indentSpaces; ++i)
            {
                indentChars[i] = ' ';
            }
            indent = new string(indentChars);
        }

        /**
         * Begins encoding a new array. Each call to this method must be paired with
         * a call to {@link #endArray}.
         *
         * @return this stringer.
         */
        public JSONStringer Array()
        {
            return Open(Scope.EMPTY_ARRAY, "[");
        }

        /**
         * Ends encoding the current array.
         *
         * @return this stringer.
         */
        public JSONStringer EndArray()
        {
            return Close(Scope.EMPTY_ARRAY, Scope.NONEMPTY_ARRAY, "]");
        }

        /**
         * Begins encoding a new object. Each call to this method must be paired
         * with a call to {@link #endObject}.
         *
         * @return this stringer.
         */
        public JSONStringer Obj()
        {
            return Open(Scope.EMPTY_OBJECT, "{");
        }

        /**
         * Ends encoding the current object.
         *
         * @return this stringer.
         */
        public JSONStringer EndObject()
        {
            return Close(Scope.EMPTY_OBJECT, Scope.NONEMPTY_OBJECT, "}");
        }

        /**
         * Enters a new scope by appending any necessary whitespace and the given
         * bracket.
         */
        public JSONStringer Open(Scope empty, string openBracket)
        {
            if (stack.Count <= 0 && sbOut.Length > 0)
            {
                throw new JSONException("Nesting problem: multiple top-level roots");
            }
            BeforeValue();
            stack.Add(empty);
            sbOut.Append(openBracket);
            return this;
        }

        /**
         * Closes the current scope by appending any necessary whitespace and the
         * given bracket.
         */
        public JSONStringer Close(Scope empty, Scope nonempty, string closeBracket)
        {
            Scope context = Peek();
            if (context != nonempty && context != empty)
            {
                throw new JSONException("Nesting problem");
            }

            stack.RemoveAt(stack.Count - 1);
            if (context == nonempty)
            {
                Newline();
            }
            sbOut.Append(closeBracket);
            return this;
        }

        /**
         * Returns the value on the top of the stack.
         */
        private Scope Peek()
        {
            if (stack.Count <= 0)
            {
                throw new JSONException("Nesting problem");
            }
            return stack[stack.Count - 1];
        }

        /**
         * Replace the value on the top of the stack with the given value.
         */
        private void ReplaceTop(Scope topOfStack)
        {
            stack[stack.Count - 1] = topOfStack;
        }

        /**
         * Encodes {@code value}.
         *
         * @param value a {@link JSONObject}, {@link JSONArray}, String, Boolean,
         *     Integer, Long, Double or null. May not be {@link Double#isNaN() NaNs}
         *     or {@link Double#isInfinite() infinities}.
         * @return this stringer.
         */
        public JSONStringer Value(object value)
        {
            if (stack.Count <= 0)
            {
                throw new JSONException("Nesting problem");
            }

            if (value is JSONArray)
            {
                ((JSONArray)value).WriteTo(this);
                return this;

            }
            else if (value is JSONObject)
            {
                ((JSONObject)value).WriteTo(this);
                return this;
            }

            BeforeValue();

            if (value == null || value == JSONObject.NULL)
            {
                sbOut.Append(value);

            }
            else if (value is bool)
            {
                sbOut.Append(((bool)value).ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            }
            else if (value is double
                || value is float
                || value is int
                || value is long
                || value is short)
            {
                sbOut.Append(JSONObject.NumberToString(value));
            }
            else
            {
                Str(value.ToString());
            }

            return this;
        }

        /**
         * Encodes {@code value} to this stringer.
         *
         * @return this stringer.
         */
        public JSONStringer Value(bool value)
        {
            if (stack.Count <= 0)
            {
                throw new JSONException("Nesting problem");
            }
            BeforeValue();
            sbOut.Append(value);
            return this;
        }

        /**
         * Encodes {@code value} to this stringer.
         *
         * @param value a finite value. May not be {@link Double#isNaN() NaNs} or
         *     {@link Double#isInfinite() infinities}.
         * @return this stringer.
         */
        public JSONStringer Value(double value)
        {
            if (stack.Count <= 0)
            {
                throw new JSONException("Nesting problem");
            }
            BeforeValue();
            sbOut.Append(JSONObject.NumberToString(value));
            return this;
        }

        /**
         * Encodes {@code value} to this stringer.
         *
         * @return this stringer.
         */
        public JSONStringer Value(long value)
        {
            if (stack.Count <= 0)
            {
                throw new JSONException("Nesting problem");
            }
            BeforeValue();
            sbOut.Append(value);
            return this;
        }

        private void Str(string value)
        {
            sbOut.Append("\"");
            for (int i = 0, length = value.Length; i < length; ++i)
            {
                char c = value[i];

                /*
                 * From RFC 4627, "All Unicode characters may be placed within the
                 * quotation marks except for the characters that must be escaped:
                 * quotation mark, reverse solidus, and the control characters
                 * (U+0000 through U+001F)."
                 */
                switch (c)
                {
                    case '"':
                    case '\\':
                    case '/':
                        sbOut.Append('\\').Append(c);
                        break;

                    case '\t':
                        sbOut.Append("\\t");
                        break;

                    case '\b':
                        sbOut.Append("\\b");
                        break;

                    case '\n':
                        sbOut.Append("\\n");
                        break;

                    case '\r':
                        sbOut.Append("\\r");
                        break;

                    case '\f':
                        sbOut.Append("\\f");
                        break;

                    default:
                        if (c <= 0x1F)
                        {
                            sbOut.Append(string.Format("\\u{0:x4}", (int)c));
                        }
                        else
                        {
                            sbOut.Append(c);
                        }
                        break;
                }

            }
            sbOut.Append("\"");
        }

        private void Newline()
        {
            if (indent == null)
            {
                return;
            }

            sbOut.Append("\n");
            for (int i = 0; i < stack.Count; ++i)
            {
                sbOut.Append(indent);
            }
        }

        /**
         * Encodes the key (property name) to this stringer.
         *
         * @param name the name of the forthcoming value. May not be null.
         * @return this stringer.
         */
        public JSONStringer Key(string name)
        {
            if (name == null)
            {
                throw new JSONException("Names must be non-null");
            }
            BeforeKey();
            Str(name);
            return this;
        }

        /**
         * Inserts any necessary separators and whitespace before a name. Also
         * adjusts the stack to expect the key's value.
         */
        private void BeforeKey()
        {
            Scope context = Peek();
            if (context == Scope.NONEMPTY_OBJECT)
            { // first in object
                sbOut.Append(',');
            }
            else if (context != Scope.EMPTY_OBJECT)
            { // not in an object!
                throw new JSONException("Nesting problem");
            }
            Newline();
            ReplaceTop(Scope.DANGLING_KEY);
        }

        /**
         * Inserts any necessary separators and whitespace before a literal value,
         * inline array, or inline object. Also adjusts the stack to expect either a
         * closing bracket or another element.
         */
        private void BeforeValue()
        {
            if (stack.Count <= 0)
            {
                return;
            }

            Scope context = Peek();
            if (context == Scope.EMPTY_ARRAY)
            { // first in array
                ReplaceTop(Scope.NONEMPTY_ARRAY);
                Newline();
            }
            else if (context == Scope.NONEMPTY_ARRAY)
            { // another in array
                sbOut.Append(',');
                Newline();
            }
            else if (context == Scope.DANGLING_KEY)
            { // value for key
                sbOut.Append(indent == null ? ":" : ": ");
                ReplaceTop(Scope.NONEMPTY_OBJECT);
            }
            else if (context != Scope.NULL)
            {
                throw new JSONException("Nesting problem");
            }
        }

        /**
         * Returns the encoded JSON string.
         *
         * <p>If invoked with unterminated arrays or unclosed objects, this method's
         * return value is undefined.
         *
         * <p><strong>Warning:</strong> although it contradicts the general contract
         * of {@link Object#toString}, this method returns null if the stringer
         * contains no data.
         */
        public override string ToString()
        {
            return sbOut.Length == 0 ? null : sbOut.ToString();
        }
    }
}
