using System;

    public sealed class LittleEndianBitConverter : EndianBitConverter
    {
        //
        // Properties
        //
        public sealed override Endianness Endianness
        {
            get
            {
                return Endianness.LittleEndian;
            }
        }

        //
        // Methods
        //
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            for (int i = 0; i < bytes; i++)
            {
                buffer[i + index] = (byte)(value & 255);
                value >>= 8;
            }
        }

        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long num = 0;
            for (int i = 0; i < bytesToConvert; i++)
            {
                num = (num << 8 | (long)buffer[startIndex + bytesToConvert - 1 - i]);
            }
            return num;
        }

        public sealed override bool IsLittleEndian()
        {
            return true;
        }
    }
