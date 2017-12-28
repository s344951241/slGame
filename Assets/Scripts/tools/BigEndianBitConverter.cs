using System;

    public sealed class BigEndianBitConverter : EndianBitConverter
    {
        //
        // Properties
        //
        public sealed override Endianness Endianness
        {
            get
            {
                return Endianness.BigEndian;
            }
        }

        //
        // Methods
        //
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            int num = index + bytes - 1;
            for (int i = 0; i < bytes; i++)
            {
                buffer[num - i] = (byte)(value & 255);
                value >>= 8;
            }
        }

        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long num = 0;
            for (int i = 0; i < bytesToConvert; i++)
            {
                num = (num << 8 | (long)buffer[startIndex + i]);
            }
            return num;
        }

        public sealed override bool IsLittleEndian()
        {
            return false;
        }
    }
