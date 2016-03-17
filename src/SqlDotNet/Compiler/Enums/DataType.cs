using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// List of available data types
    /// </summary>
    public enum DataType
    {
        None = 0x0000,

        Null = 0x0010,

        Boolean = 0x0022,
        Int32 = 0x0024,
        Int64 = 0x0028,

        Float32 = 0x0034,
        Float64 = 0x0038,

        Char = 0x0060,
        Str = 0x0061,

        AnonymObject = 0x0080,
        Object = 0x0090
    }

    public class DataTypeHelper
    {
        /// <summary>
        /// Convert a string to its datatype, i4 --> DataType.Int32
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public static DataType StrToDataType(string strType)
        {
            switch (strType)
            {
                case "null":
                    return DataType.Null;

                case "i2":
                    return DataType.Boolean;

                case "i4":
                    return DataType.Int32;

                case "i8":
                    return DataType.Int64;

                case "r4":
                    return DataType.Float32;

                case "r8":
                    return DataType.Float64;

                case "str":
                    return DataType.Str;

                default:
                    return DataType.None;
            }
        }

        public static object StringValueToObject(string value, DataType type)
        {
            switch (type)
            {
                case DataType.Null:
                    return null;

                case DataType.Boolean:
                    return Convert.ToBoolean(value);

                case DataType.Int32:
                    return Convert.ToInt32(value);

                case DataType.Int64:
                    return Convert.ToInt64(value);

                case DataType.Float32:
                    return ConvertHelper.ParseFloat(value);

                case DataType.Float64:
                    return ConvertHelper.ParseDouble(value);

                case DataType.Str:
                    return value.ToString();
            }

            return null;
        }
    }

    public class ConvertHelper
    {
        public static float ParseFloat(string val)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(val, NumberStyles.Any, ci);
        }

        public static double ParseDouble(string val)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            return double.Parse(val, NumberStyles.Any, ci);
        }

        public static bool TryParseFloat(string val, out float floatVal)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.TryParse(val, NumberStyles.Any, ci, out floatVal);
        }

        public static bool TryParseDouble(string val, out double doubleVal)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            return double.TryParse(val, NumberStyles.Any, ci, out doubleVal);
        }
    }
}
