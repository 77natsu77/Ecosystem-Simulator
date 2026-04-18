namespace Ecosystem_Simulator.Core
{
    public struct SimpleColor
    {
        public byte R, G, B;
        public SimpleColor(byte r, byte g, byte b) { R = r; G = g; B = b; }

        // Some defaults for convenience
        public static SimpleColor Green => new SimpleColor(0, 255, 0);
        public static SimpleColor Red => new SimpleColor(255, 0, 0);
    }
}