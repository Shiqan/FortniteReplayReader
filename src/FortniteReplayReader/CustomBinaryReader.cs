using System;
using System.IO;
using System.Text;

namespace FortniteReplayReader
{
    /// <summary>
    /// Custom Binary Reader with methods for Unreal Engine replay files
    /// </summary>
    public class CustomBinaryReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the CustomBinaryReader class based on the specified stream.
        /// </summary>
        /// <param name="input">An stream.</param>
        /// <seealso cref="System.IO.BinaryReader"/> 
        public CustomBinaryReader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Reads a string from the current stream. The string is prefixed with the length as an 4-byte signed integer.
        /// </summary>
        /// <returns>A string read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual string ReadFString()
        {
            var length = ReadInt32();

            if (length == 0)
            {
                return "";
            }

            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -2 * length;
                data = ReadBytes(length);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by four bytes.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual T ReadUInt32AsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadUInt32());
        }

        /// <summary>
        /// Reads a byte from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by 1 byte.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }

        /// <summary>
        /// Reads a Boolean value from the current stream and advances the current position of the stream by 4-bytes.
        /// </summary>
        /// <returns>true if the byte is nonzero; otherwise, false.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual bool ReadUInt32AsBoolean()
        {
            return ReadUInt32() == 1;
        }


        /// <summary>
        /// Reads 16 bytes from the current stream and advances the current position of the stream by 16-bytes.
        /// </summary>
        /// <returns>A GUID in string format read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual string ReadGUID()
        {
            var guid = new Guid(ReadBytes(16));
            return guid.ToString();
        }


        /// <summary>
        /// Advances the current position of the stream by <paramref name="byteCount"/> bytes.
        /// </summary>
        /// <param name="byteCount">The amount of bytes to skip. This value must be 0 or a non-negative number.</param>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        protected void SkipBytes(uint byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
