namespace SubtitleAlchemist.Logic.Ocr;

public class NOcrChar
{
    public string Text { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int MarginTop { get; set; }
    public bool Italic { get; set; }
    public List<NOcrLine> LinesForeground { get; }
    public List<NOcrLine> LinesBackground { get; }
    public int ExpandCount { get; set; }
    public bool LoadedOk { get; }

    public double WidthPercent => Height * 100.0 / Width;

    public NOcrChar()
    {
        LinesForeground = new List<NOcrLine>();
        LinesBackground = new List<NOcrLine>();
        Text = string.Empty;
    }

    public NOcrChar(NOcrChar old)
    {
        LinesForeground = new List<NOcrLine>(old.LinesForeground.Count);
        LinesBackground = new List<NOcrLine>(old.LinesBackground.Count);
        Text = old.Text;
        Width = old.Width;
        Height = old.Height;
        MarginTop = old.MarginTop;
        Italic = old.Italic;
        foreach (var p in old.LinesForeground)
        {
            LinesForeground.Add(new NOcrLine(new OcrPoint(p.Start.X, p.Start.Y), new OcrPoint(p.End.X, p.End.Y)));
        }

        foreach (var p in old.LinesBackground)
        {
            LinesBackground.Add(new NOcrLine(new OcrPoint(p.Start.X, p.Start.Y), new OcrPoint(p.End.X, p.End.Y)));
        }
    }

    public NOcrChar(string text)
        : this()
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }

    public bool IsSensitive => Text == "O" || Text == "o" || Text == "0" || Text == "'" || Text == "-" || Text == ":" || Text == "\"";

    public NOcrChar(Stream stream, bool isVersion2)
    {
        try
        {
            if (isVersion2)
            {
                var buffer = new byte[4];
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read < buffer.Length)
                {
                    LoadedOk = false;
                    return;
                }

                var isShort = (buffer[0] & 0b0001_0000) > 0;
                Italic = (buffer[0] & 0b0010_0000) > 0;

                if (isShort)
                {
                    ExpandCount = buffer[0] & 0b0000_1111;
                    Width = buffer[1];
                    Height = buffer[2];
                    MarginTop = buffer[3];
                }
                else
                {
                    ExpandCount = stream.ReadByte();
                    Width = stream.ReadByte() << 8 | stream.ReadByte();
                    Height = stream.ReadByte() << 8 | stream.ReadByte();
                    MarginTop = stream.ReadByte() << 8 | stream.ReadByte();
                }

                var textLen = stream.ReadByte();
                if (textLen > 0)
                {
                    buffer = new byte[textLen];
                    stream.Read(buffer, 0, buffer.Length);
                    Text = System.Text.Encoding.UTF8.GetString(buffer);
                }
                else
                {
                    Text = string.Empty;
                }

                if (isShort)
                {
                    LinesForeground = ReadPointsBytes(stream);
                    LinesBackground = ReadPointsBytes(stream);
                }
                else
                {
                    LinesForeground = ReadPoints(stream);
                    LinesBackground = ReadPoints(stream);
                }

                if (Width > 0 && Height > 0 && Width <= 1920 && Height <= 1080 && Text.IndexOf('\0') < 0)
                {
                    LoadedOk = true;
                }
                else
                {
                    LoadedOk = false;
                }
            }
            else
            {
                var buffer = new byte[9];
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read < buffer.Length)
                {
                    LoadedOk = false;
                    return;
                }

                Width = buffer[0] << 8 | buffer[1];
                Height = buffer[2] << 8 | buffer[3];

                MarginTop = buffer[4] << 8 | buffer[5];

                Italic = buffer[6] != 0;

                ExpandCount = buffer[7];

                int textLen = buffer[8];
                if (textLen > 0)
                {
                    buffer = new byte[textLen];
                    stream.Read(buffer, 0, buffer.Length);
                    Text = System.Text.Encoding.UTF8.GetString(buffer);
                }
                else
                {
                    Text = string.Empty;
                }

                LinesForeground = ReadPoints(stream);
                LinesBackground = ReadPoints(stream);

                if (Width > 0 && Height > 0 && Width <= 1920 && Height <= 1080 && Text.IndexOf('\0') < 0)
                {
                    LoadedOk = true;
                }
                else
                {
                    LoadedOk = false;
                }
            }
        }
        catch
        {
            LoadedOk = false;
        }
    }

    private static List<NOcrLine> ReadPoints(Stream stream)
    {
        var length = stream.ReadByte() << 8 | stream.ReadByte();
        var list = new List<NOcrLine>(length);
        var buffer = new byte[8];
        for (var i = 0; i < length; i++)
        {
            stream.Read(buffer, 0, buffer.Length);
            var point = new NOcrLine
            {
                Start = new OcrPoint(buffer[0] << 8 | buffer[1], buffer[2] << 8 | buffer[3]),
                End = new OcrPoint(buffer[4] << 8 | buffer[5], buffer[6] << 8 | buffer[7])
            };
            list.Add(point);
        }
        return list;
    }

    private static List<NOcrLine> ReadPointsBytes(Stream stream)
    {
        var length = stream.ReadByte();
        var list = new List<NOcrLine>(length);
        var buffer = new byte[4];
        for (var i = 0; i < length; i++)
        {
            stream.Read(buffer, 0, buffer.Length);
            var point = new NOcrLine
            {
                Start = new OcrPoint(buffer[0], buffer[1]),
                End = new OcrPoint(buffer[2], buffer[3])
            };
            list.Add(point);
        }
        return list;
    }

    internal void Save(Stream stream)
    {
        if (IsAllByteValues())
        {
            SaveOneBytes(stream);
        }
        else
        {
            SaveTwoBytes(stream);
        }
    }

    private bool IsAllByteValues()
    {
        return Width <= byte.MaxValue && Height <= byte.MaxValue && ExpandCount < 16 &&
               LinesBackground.Count <= byte.MaxValue && LinesForeground.Count <= byte.MaxValue &&
               IsAllPointByteValues(LinesForeground) && IsAllPointByteValues(LinesForeground);
    }

    private static bool IsAllPointByteValues(List<NOcrLine> lines)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            var point = lines[index];
            if (point.Start.X > byte.MaxValue || point.Start.Y > byte.MaxValue ||
                point.End.X > byte.MaxValue || point.End.Y > byte.MaxValue)
            {
                return false;
            }
        }

        return true;
    }

    private void SaveOneBytes(Stream stream)
    {
        var flags = 0b0001_0000;

        if (Italic)
        {
            flags |= 0b0010_0000;
        }

        if (ExpandCount > 0)
        {
            flags |= (byte)ExpandCount;
        }

        stream.WriteByte((byte)flags);

        stream.WriteByte((byte)Width);
        stream.WriteByte((byte)Height);
        stream.WriteByte((byte)MarginTop);

        if (Text == null)
        {
            stream.WriteByte(0);
        }
        else
        {
            var textBuffer = System.Text.Encoding.UTF8.GetBytes(Text);
            stream.WriteByte((byte)textBuffer.Length);
            stream.Write(textBuffer, 0, textBuffer.Length);
        }
        WritePointsAsOneByte(stream, LinesForeground);
        WritePointsAsOneByte(stream, LinesBackground);
    }

    private void SaveTwoBytes(Stream stream)
    {
        var flags = 0b0000_0000;

        if (Italic)
        {
            flags |= 0b0010_0000;
        }

        stream.WriteByte((byte)flags);
        stream.WriteByte((byte)ExpandCount);

        WriteInt16(stream, (ushort)Width);
        WriteInt16(stream, (ushort)Height);
        WriteInt16(stream, (ushort)MarginTop);

        if (Text == null)
        {
            stream.WriteByte(0);
        }
        else
        {
            var textBuffer = System.Text.Encoding.UTF8.GetBytes(Text);
            stream.WriteByte((byte)textBuffer.Length);
            stream.Write(textBuffer, 0, textBuffer.Length);
        }
        WritePoints(stream, LinesForeground);
        WritePoints(stream, LinesBackground);
    }

    private static void WritePointsAsOneByte(Stream stream, List<NOcrLine> points)
    {
        stream.WriteByte((byte)points.Count);
        foreach (var nOcrPoint in points)
        {
            stream.WriteByte((byte)nOcrPoint.Start.X);
            stream.WriteByte((byte)nOcrPoint.Start.Y);
            stream.WriteByte((byte)nOcrPoint.End.X);
            stream.WriteByte((byte)nOcrPoint.End.Y);
        }
    }

    private static void WritePoints(Stream stream, List<NOcrLine> points)
    {
        WriteInt16(stream, (ushort)points.Count);
        foreach (var nOcrPoint in points)
        {
            WriteInt16(stream, (ushort)nOcrPoint.Start.X);
            WriteInt16(stream, (ushort)nOcrPoint.Start.Y);
            WriteInt16(stream, (ushort)nOcrPoint.End.X);
            WriteInt16(stream, (ushort)nOcrPoint.End.Y);
        }
    }

    private static void WriteInt16(Stream stream, ushort val)
    {
        var buffer = new byte[2];
        buffer[0] = (byte)((val & 0xFF00) >> 8);
        buffer[1] = (byte)(val & 0x00FF);
        stream.Write(buffer, 0, buffer.Length);
    }
}