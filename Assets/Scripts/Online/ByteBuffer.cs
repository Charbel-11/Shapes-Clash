using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Test the efficient implementation using implemented functions instead
//IDisposable provides a mechanism to release unmanaged data
public class ByteBuffer : IDisposable
{
    //We need the array of bytes to be able to use BitConverter 
    private List<Byte> Buff;        //Main buffer (data container)
    private byte[] readBuff;        //Data ready to be read
    private int readPos;            //Index of first unread byte
    private bool buffUpdated = false;

    public ByteBuffer()
    {
        Buff = new List<Byte>();
        readPos = 0;
    }

    public int GetReadPos() { return readPos; }
    public byte[] ToArray() { return Buff.ToArray(); }
    public int Count() { return Buff.Count(); }
    public int Length() { return Count() - readPos; } //remaining length

    public void Clear()
    {
        Buff.Clear();
        readPos = 0;
    }

    public void WriteByte(byte input)
    {
        Buff.Add(input);
        buffUpdated = true;
    }
    public void WriteBytes(byte[] input)
    {
        Buff.AddRange(input);
        buffUpdated = true;
    }
    public void WriteShort(short input)     //short = 2 bytes
    {
        Buff.AddRange(BitConverter.GetBytes(input));    //converts short to byte[] then adds it
        buffUpdated = true;
    }
    public void WriteInteger(int input)
    {
        Buff.AddRange(BitConverter.GetBytes(input));
        buffUpdated = true;
    }
    public void WriteLong(long input)
    {
        Buff.AddRange(BitConverter.GetBytes(input));
        buffUpdated = true;
    }
    public void WriteFloat(float input)
    {
        Buff.AddRange(BitConverter.GetBytes(input));
        buffUpdated = true;
    }
    public void WriteBool(bool input)
    {
        Buff.AddRange(BitConverter.GetBytes(input));
        buffUpdated = true;
    }
    //String length not bound at design time
    //So we need to add its length first
    //so that readString knows how much to read
    public void WriteString(string input)
    {
        Buff.AddRange(BitConverter.GetBytes(input.Length));
        Buff.AddRange(Encoding.ASCII.GetBytes(input));  //converts each character to a byte, i.e. a string to byte[]
        buffUpdated = true;
    }

    //Reads the next byte
    public byte ReadByte(bool MoveIndex = true)
    {
        if (readPos >= Buff.Count) { throw new Exception("No unread byte available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        byte value = readBuff[readPos];
        if (MoveIndex) { readPos++; }

        return value;
    }
    //We get a shallow copy!
    public byte[] ReadBytes(int length, bool MoveIndex = true)
    {
        if (length == 0) { return new byte[] { }; }
        if (readPos + length - 1 >= Buff.Count) { throw new Exception("Not enough unread bytes available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        byte[] value = Buff.GetRange(readPos, length).ToArray();
        if (MoveIndex) { readPos += length; }

        return value;
    }
    public short ReadShort(bool MoveIndex = true)
    {
        if (readPos + 1 >= Buff.Count) { throw new Exception("No unread short available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        short value = BitConverter.ToInt16(readBuff, readPos);
        if (MoveIndex) { readPos += 2; }

        return value;
    }
    public int ReadInteger(bool MoveIndex = true)
    {
        if (readPos + 3 >= Buff.Count) { throw new Exception("No unread int available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        int value = BitConverter.ToInt32(readBuff, readPos);
        if (MoveIndex) { readPos += 4; }

        return value;
    }
    public long ReadLong(bool MoveIndex = true)
    {
        if (readPos + 7 >= Buff.Count) { throw new Exception("No unread long available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        long value = BitConverter.ToInt64(readBuff, readPos);
        if (MoveIndex) { readPos += 8; }

        return value;
    }
    public float ReadFloat(bool MoveIndex = true)
    {
        if (readPos + 3 >= Buff.Count) { throw new Exception("No unread float available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        float value = BitConverter.ToSingle(readBuff, readPos);
        if (MoveIndex) { readPos += 4; }

        return value;
    }
    //A bool is considered as a byte
    public bool ReadBool(bool MoveIndex = true)
    {
        if (readPos >= Buff.Count) { throw new Exception("No unread Bool available"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }

        bool value = BitConverter.ToBoolean(readBuff, readPos);
        if (MoveIndex) { readPos++; }

        return value;
    }
    public string ReadString(bool MoveIndex = true)
    {
        int length = ReadInteger(true);
        if (readPos + length - 1 >= Buff.Count) { throw new Exception("Not enough unread bytes available for this string"); }

        if (buffUpdated)
        {
            readBuff = Buff.ToArray();
            buffUpdated = false;
        }
        string value = Encoding.ASCII.GetString(readBuff, readPos, length);

        if (MoveIndex && value.Length > 0) { readPos += length; }

        return value;
    }

    private bool disposedValue = false;
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue) { return; }

        if (disposing)
        {
            //Clean up all managed resources
            Buff.Clear();
            readPos = 0;
        }
        disposedValue = true;

        //Clean up all unmanaged resources (e.g. database connections, tcp connections...)
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);  //In case Dispose(true) is called, no need to call Finalize (i.e. the destructor)
    }
}