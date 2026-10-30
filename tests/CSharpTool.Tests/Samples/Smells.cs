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
}
