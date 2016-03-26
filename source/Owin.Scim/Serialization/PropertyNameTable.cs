namespace Owin.Scim.Serialization
{
    using System;

    internal class PropertyNameTable
    {
        private int _mask = 31;
        private static readonly int HashCodeRandomizer = Environment.TickCount;
        private int _count;
        private Entry[] _entries;

        public PropertyNameTable()
        {
            _entries = new Entry[_mask + 1];
        }

        public string Get(char[] key, int start, int length)
        {
            if (length == 0)
                return string.Empty;
            int num1 = length + HashCodeRandomizer;
            int num2 = num1 + (num1 << 7 ^ key[start]);
            int num3 = start + length;
            for (int index = start + 1; index < num3; ++index)
                num2 += num2 << 7 ^ key[index];
            int num4 = num2 - (num2 >> 17);
            int num5 = num4 - (num4 >> 11);
            int num6 = num5 - (num5 >> 5);
            for (Entry entry = _entries[num6 & _mask]; entry != null; entry = entry.Next)
            {
                if (entry.HashCode == num6 && TextEquals(entry.Value, key, start, length))
                    return entry.Value;
            }
            return null;
        }

        public string Add(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            int length = key.Length;
            if (length == 0)
                return string.Empty;
            int num1 = length + HashCodeRandomizer;
            for (int index = 0; index < key.Length; ++index)
                num1 += num1 << 7 ^ key[index];
            int num2 = num1 - (num1 >> 17);
            int num3 = num2 - (num2 >> 11);
            int hashCode = num3 - (num3 >> 5);
            for (Entry entry = _entries[hashCode & _mask]; entry != null; entry = entry.Next)
            {
                if (entry.HashCode == hashCode && entry.Value.Equals(key))
                    return entry.Value;
            }
            return AddEntry(key, hashCode);
        }

        private string AddEntry(string str, int hashCode)
        {
            int index = hashCode & _mask;
            Entry entry = new Entry(str, hashCode, _entries[index]);
            _entries[index] = entry;
            int num = _count;
            _count = num + 1;
            if (num == _mask)
                Grow();
            return entry.Value;
        }

        private void Grow()
        {
            Entry[] entryArray1 = _entries;
            int num = _mask * 2 + 1;
            Entry[] entryArray2 = new Entry[num + 1];
            Entry entry1;
            for (int index1 = 0; index1 < entryArray1.Length; ++index1)
            {
                for (Entry entry2 = entryArray1[index1]; entry2 != null; entry2 = entry1)
                {
                    int index2 = entry2.HashCode & num;
                    entry1 = entry2.Next;
                    entry2.Next = entryArray2[index2];
                    entryArray2[index2] = entry2;
                }
            }
            _entries = entryArray2;
            _mask = num;
        }

        private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
        {
            if (str1.Length != str2Length)
                return false;
            for (int index = 0; index < str1.Length; ++index)
            {
                if (str1[index] != str2[str2Start + index])
                    return false;
            }
            return true;
        }

        private class Entry
        {
            internal readonly string Value;
            internal readonly int HashCode;
            internal Entry Next;

            internal Entry(string value, int hashCode, Entry next)
            {
                Value = value;
                HashCode = hashCode;
                Next = next;
            }
        }
    }
}