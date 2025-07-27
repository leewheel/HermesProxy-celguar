﻿/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace System
{
    public static class Extensions
    {

        /// <summary>
        /// Returns the remaining bytes on the stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static uint Remaining(this Stream reader)
        {
            if (reader.Position > reader.Length)
                throw new InvalidOperationException();

            return (uint)(reader.Length - reader.Position);
        }

        public static string ToHexString(this byte[] array)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < array.Length; ++i)
                builder.Append(array[i].ToString("X2"));

            return builder.ToString();
        }

        /// <summary>
        /// places a non-negative value (0) at the MSB, then converts to a BigInteger.
        /// This ensures a non-negative value without changing the binary representation.
        /// </summary>
        public static BigInteger ToBigInteger(this byte[] array)
        {
            byte[] temp;
            if ((array[array.Length - 1] & 0x80) == 0x80)
            {
                temp = new byte[array.Length + 1];
                temp[array.Length] = 0;
            }
            else
                temp = new byte[array.Length];

            Array.Copy(array, temp, array.Length);
            return new BigInteger(temp);
        }

        /// <summary>
        /// Removes the MSB if it is 0, then converts to a byte array.
        /// </summary>
        public static byte[] ToCleanByteArray(this BigInteger b)
        {
            byte[] array = b.ToByteArray();
            if (array[array.Length - 1] != 0)
                return array;

            byte[] temp = new byte[array.Length - 1];
            Array.Copy(array, temp, temp.Length);
            return temp;
        }

        public static BigInteger ModPow(this BigInteger value, BigInteger pow, BigInteger mod)
        {
            return BigInteger.ModPow(value, pow, mod);
        }

        public static byte[] SubArray(this byte[] array, int start, int count)
        {
            byte[] subArray = new byte[count];
            Array.Copy(array, start, subArray, 0, count);
            return subArray;
        }

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
            where T : Attribute
        {
            return (T[])member.GetCustomAttributes(typeof(T), inherit) ?? new T[] { };
        }

        public static bool TryGetAttributes<T>(this MemberInfo member, bool inherit, out IEnumerable<T> attributes)
            where T : Attribute
        {
            var attrs = (T[])member.GetCustomAttributes(typeof(T), inherit) ?? new T[] { };
            attributes = attrs;
            return attrs.Length > 0;
        }

        public static IEnumerable<TSource> TakeRandom<TSource>(this IEnumerable<TSource> source, int count)
        {
            Random random = new Random();
            List<int> indexes = new List<int>(source.Count());
            for (int index = 0; index < indexes.Capacity; index++)
                indexes.Add(index);

            List<TSource> result = new List<TSource>(count);
            for (int index = 0; index < count && indexes.Count() > 0; index++)
            {
                int randomIndex = random.Next(indexes.Count());
                result.Add(source.ElementAt(randomIndex));
                indexes.Remove(randomIndex);
            }

            return result;
        }

        /// <summary>
        /// Returns true if flag exists in value (&)
        /// </summary>
        /// <param name="value">An enum, int, ...</param>
        /// <param name="flag">An enum, int, ...</param>
        /// <returns>A boolean</returns>
        public static bool HasAnyFlag(this IConvertible value, IConvertible flag)
        {
            var uFlag = flag.ToUInt64(null);
            var uThis = value.ToUInt64(null);

            return (uThis & uFlag) != 0;
        }

        public static string ToHexString(this byte[] byteArray, bool reverse = false)
        {
            if (reverse)
                return byteArray.Reverse().Aggregate("", (current, b) => current + b.ToString("X2"));
            else
                return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        static uint LeftRotate(this uint value, int shiftCount)
        {
            return (value << shiftCount) | (value >> (0x20 - shiftCount));
        }

        public static byte[] GenerateRandomKey(this byte[] s, int length)
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var key = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int randValue;

                do
                {
                    randValue = (int)((uint)random.Next(0xFF)).LeftRotate(1) ^ i;
                } while (randValue > 0xFF && randValue <= 0);

                key[i] = (byte)randValue;
            }

            return key;
        }

        public static bool Compare(this byte[] b, byte[] b2)
        {
            for (int i = 0; i < b2.Length; i++)
                if (b[i] != b2[i])
                    return false;

            return true;
        }

        public static byte[] Combine(this byte[] data, params byte[][] pData)
        {
            var combined = data;

            foreach (var arr in pData)
            {
                var currentSize = combined.Length;

                Array.Resize(ref combined, currentSize + arr.Length);

                Buffer.BlockCopy(arr, 0, combined, currentSize, arr.Length);
            }

            return combined;
        }

        public static object[] Combine(this object[] data, params object[][] pData)
        {
            var combined = data;

            foreach (var arr in pData)
            {
                var currentSize = combined.Length;

                Array.Resize(ref combined, currentSize + arr.Length);

                Array.Copy(arr, 0, combined, currentSize, arr.Length);
            }

            return combined;
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            T temp = left;
            left = right;
            right = temp;
        }

        public static uint[] SerializeObject<T>(this T obj)
        {
            //if (obj.GetType()<StructLayoutAttribute>() == null)
                //return null;

            var size = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(size);
            byte[] array = new byte[size];

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, array, 0, size);

            Marshal.FreeHGlobal(ptr);

            uint[] result = new uint[size / 4];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            return result;
        }

        public static List<T> DeserializeObjects<T>(this ICollection<uint> data)
        {
            List<T> list = new List<T>();

            if (data.Count == 0)
                return list;

            if (typeof(T).GetCustomAttribute<StructLayoutAttribute>() == null)
                return list;

            byte[] result = new byte[data.Count * sizeof(uint)];
            Buffer.BlockCopy(data.ToArray(), 0, result, 0, result.Length);

            var typeSize = Marshal.SizeOf(typeof(T));
            var objCount = data.Count / (typeSize / sizeof(uint));

            for (var i = 0; i < objCount; ++i)
            {
                var ptr = Marshal.AllocHGlobal(typeSize);
                Marshal.Copy(result, typeSize * i, ptr, typeSize);
                list.Add((T)Marshal.PtrToStructure(ptr, typeof(T)));
                Marshal.FreeHGlobal(ptr);
            }

            return list;
        }

#if NET5_0
        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(this IEnumerable<TValue> values, int chunkSize)
        {
            return values
                   .Select((v, i) => new { v, groupIndex = i / chunkSize })
                   .GroupBy(x => x.groupIndex)
                   .Select(g => g.Select(x => x.v));
        }
#endif

        public static T CastFlags<T> (this Enum input) where T : struct, Enum
        {
            uint result = 0;
            foreach (Enum value in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(value) && Enum.IsDefined(typeof(T), value.ToString()))
                {
                    result |= (uint)Enum.Parse(typeof(T), value.ToString());
                }
            }
            return (T)(object)result;
        }

        #region Strings
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static T ToEnum<T>(this string str) where T : struct
        {
            T value;
            if (!Enum.TryParse(str, out value))
                return default;

            return value;
        }

        public static string Reverse(this string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static byte[] ToCString(this string str)
        {
            byte[] utf8StringBytes = Encoding.UTF8.GetBytes(str);
            byte[] data = new byte[utf8StringBytes.Length + 1];
            Array.Copy(utf8StringBytes, data, utf8StringBytes.Length);
            data[data.Length - 1] = 0;
            return data;
        }

        public static byte[] ParseAsByteArray(this string str)
        {
            str = str.Replace(" ", String.Empty);

            var res = new byte[str.Length / 2];
            for (int i = 0; i < res.Length; ++i)
            {
                string temp = String.Concat(str[i * 2], str[i * 2 + 1]);
                res[i] = Convert.ToByte(temp, 16);
            }
            return res;
        }

        public static byte[] ToByteArray(this string str)
        {
            str = str.Replace(" ", String.Empty);

            var res = new byte[str.Length / 2];
            for (int i = 0; i < res.Length; ++i)
            {
                string temp = String.Concat(str[i * 2], str[i * 2 + 1]);
                res[i] = Convert.ToByte(temp, 16);
            }
            return res;
        }

        public static byte[] ToByteArray(this string value, char separator)
        {
            return Array.ConvertAll(value.Split(separator), byte.Parse);
        }

        public static string ConvertFormatSyntax(this string str)
        {
            string pattern = @"(%\W*\d*[a-zA-Z]*)";
            
            int count = 0;
            string result = Regex.Replace(str, pattern, m => string.Concat("{", count++, "}"));

            return result;
        }

        public static bool Like(this string toSearch, string toFind)
        {
            return toSearch.ToLower().Contains(toFind.ToLower());
        }

        public static bool IsNumber(this string str)
        {
            double value;
            return double.TryParse(str, out value);
        }

        public static int GetByteCount(this string str)
        {
            if (str.IsEmpty())
                return 0;

            return Encoding.UTF8.GetByteCount(str);
        }

        public static bool isExtendedLatinCharacter(char wchar)
        {
            if (isBasicLatinCharacter(wchar))
                return true;
            if (wchar >= 0x00C0 && wchar <= 0x00D6)                  // LATIN CAPITAL LETTER A WITH GRAVE - LATIN CAPITAL LETTER O WITH DIAERESIS
                return true;
            if (wchar >= 0x00D8 && wchar <= 0x00DE)                  // LATIN CAPITAL LETTER O WITH STROKE - LATIN CAPITAL LETTER THORN
                return true;
            if (wchar == 0x00DF)                                     // LATIN SMALL LETTER SHARP S
                return true;
            if (wchar >= 0x00E0 && wchar <= 0x00F6)                  // LATIN SMALL LETTER A WITH GRAVE - LATIN SMALL LETTER O WITH DIAERESIS
                return true;
            if (wchar >= 0x00F8 && wchar <= 0x00FE)                  // LATIN SMALL LETTER O WITH STROKE - LATIN SMALL LETTER THORN
                return true;
            if (wchar >= 0x0100 && wchar <= 0x012F)                  // LATIN CAPITAL LETTER A WITH MACRON - LATIN SMALL LETTER I WITH OGONEK
                return true;
            if (wchar == 0x1E9E)                                     // LATIN CAPITAL LETTER SHARP S
                return true;
            return false;
        }

        public static bool isBasicLatinCharacter(char wchar)
        {
            if (wchar >= 'a' && wchar <= 'z')                      // LATIN SMALL LETTER A - LATIN SMALL LETTER Z
                return true;
            if (wchar >= 'A' && wchar <= 'Z')                      // LATIN CAPITAL LETTER A - LATIN CAPITAL LETTER Z
                return true;
            return false;
        }
        
        public static uint HashFnv1a(this string data)
        {
            uint hash = 0x811C9DC5u;
            foreach (char c in data)
            {
                hash ^= c;
                hash *= 0x1000193u;
            }
            return hash;
        }
        #endregion

        #region BinaryReader
        public static string ReadCString(this BinaryReader reader)
        {
            byte num;
            List<byte> temp = new List<byte>();

            while ((num = reader.ReadByte()) != 0)
                temp.Add(num);

            return Encoding.UTF8.GetString(temp.ToArray());
        }

        public static string ReadString(this BinaryReader reader, int count)
        {
            var array = reader.ReadBytes(count);
            return Encoding.ASCII.GetString(array);
        }

        public static string ReadStringFromChars(this BinaryReader reader, int count)
        {
            return new string(reader.ReadChars(count));
        }

        public static T[] ReadArray<T>(this BinaryReader reader, uint size) where T : struct
        {
            int numBytes = Unsafe.SizeOf<T>() * (int)size;

            byte[] source = reader.ReadBytes(numBytes);

            T[] result = new T[source.Length / Unsafe.SizeOf<T>()];

            if (source.Length > 0)
            {
                unsafe
                {
                    Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref result[0]), Unsafe.AsPointer(ref source[0]), (uint)source.Length);
                }
            }

            return result;
        }

        public static T Read<T>(this BinaryReader reader) where T : struct
        {
            byte[] result = reader.ReadBytes(Unsafe.SizeOf<T>());

            return Unsafe.ReadUnaligned<T>(ref result[0]);
        }
        #endregion
    }
}
