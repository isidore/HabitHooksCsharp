namespace Example;

public class Shapes
{
    public Shapes(int x, int y, int width, int height) { }

    public int Area(int width, int height) => width * height;

    public int Volume(int x, int y, int z, int scale) => x * y * z * scale;
}
