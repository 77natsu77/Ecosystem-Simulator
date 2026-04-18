namespace Ecosystem_Simulator.Core
{
    public interface IRenderable
    {
        Vector2 Position { get; }
        // We define what "Shape" the entity is so the renderer knows what to draw
        Appearance GetAppearance();
    }

    public struct Appearance
    {
        public float Size;
        public System.Drawing.Color Color;
        public ShapeType Shape;
    }

    public enum ShapeType { Circle, Square, Triangle }
}