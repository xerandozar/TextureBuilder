namespace TextureBuilder.Geometry
{
    public class Rectangle
    {
        public bool IsEmpty => Width <= 0 || Height <= 0;

        public int X;
        public int Y;
        public int Width;
        public int Height;  
        
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
