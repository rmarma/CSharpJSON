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

 // R.M.A., 2018

using System;
using System.Globalization;

namespace CSharpJSON
{
    public static class JSON
    {
        /// <summary>
        /// Returns the input if it is a JSON-permissible value; throws otherwise.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double CheckDouble(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                throw new JSONException("Forbidden numeric value: " + d);
            }
            return d;
        }

        public static bool? ToBoolean(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is string)
            {
                string stringValue = (string)value;
                if ("true".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if ("false".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return null;
        }

        public static double? ToDouble(object value)
        {
            if (value is double)
            {
                return (double)value;
            }
            else if (value is string)
            {
                try
                {
                    return double.Parse((string)value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static double? ToDouble(object value, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            if (value is double)
            {
                return (double)value;
            }
            else if (value is string)
            {
                try
                {
                    return double.Parse((string)value, numberStyles, formatProvider);
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static int? ToInteger(object value)
        {
            if (value is int)
            {
                return (int)value;
            }
            else if (value is string)
            {
                try
                {
                    return int.Parse((string)value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static int? ToInteger(object value, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            if (value is int)
            {
                return (int)value;
            }
            else if (value is string)
            {
                try
                {
                    return int.Parse((string)value, numberStyles, formatProvider);
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static long ToLong(object value)
        {
            if (value is long)
            {
                return (long)value;
            }
            else if (value is string)
            {
                try
                {
                    return long.Parse((string)value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }
            }
            return 0L;
        }

        public static long ToLong(object value, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            if (value is long)
            {
                return (long)value;
            }
            else if (value is string)
            {
                try
                {
                    return long.Parse((string)value, numberStyles, formatProvider);
                }
                catch
                {
                    // ignored
                }
            }
            return 0L;
        }

        public static string ToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            else if (value != null)
            {
                return value.ToString();
            }
            return null;
        }

        public static JSONException TypeMismatch(object indexOrName, object actual, string requiredType)
        {
            if (actual == null)
            {
                throw new JSONException("Value at " + indexOrName + " is null.");
            }
            else
            {
                throw new JSONException("Value " + actual + " at " + indexOrName
                        + " of type " + actual.GetType().Name
                        + " cannot be converted to " + requiredType);
            }
        }

        public static JSONException TypeMismatch(object actual, string requiredType)
        {
            if (actual == null)
            {
                throw new JSONException("Value is null.");
            }
            else
            {
                throw new JSONException("Value " + actual
                        + " of type " + actual.GetType().Name
                        + " cannot be converted to " + requiredType);
            }
        }
    }
}
