namespace Example;

public class Shapes
{
    public Shapes(int x, int y, int width, int height) { }

    public int Area(int width, int height) => width * height;

    public int Volume(int x, int y, int z, int scale) => x * y * z * scale;

    public int Long()
    {
        var a = 1;
        var b = 2;
        var c = 3;
        var d = 4;
        var e = 5;
        var f = 6;
        var g = 7;
        var h = 8;
        var i = 9;
        var j = 10;
        var k = 11;
        return a + b + c + d + e + f + g + h + i + j + k;
    }

    public int Short()
    {
        var a = 1;
        return a;
    }

    public int Nested(int[] values)
    {
        foreach (var value in values)
        {
            if (value > 0)
            {
                if (value > 10)
                {
                    return value;
                }
            }
        }
        return 0;
    }

    public string ElseIfChain(int value)
    {
        if (value > 2) return "big";
        else if (value > 1) return "medium";
        else if (value > 0) return "small";
        else return "none";
    }

    public string Classify(int a, string? b)
    {
        if (a > 100 && a < 200) return "range";
        if (a < 0 || a > 1000) return "outside";
        if (b == "x") return "x";
        var name = b ?? "none";
        var sign = a > 0 ? "positive" : "negative";
        switch (a)
        {
            case 1: return "one";
            case 2: return "two";
        }
        return name + sign;
    }

    public void Close(System.IO.Stream stream)
    {
        try { stream.Dispose(); }
        catch (System.IO.IOException) { }
    }

    public void Rethrow(System.IO.Stream stream)
    {
        try { stream.Dispose(); }
        catch (System.IO.IOException) { throw; }
    }
}
