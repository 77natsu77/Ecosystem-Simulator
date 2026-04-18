namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IRenderable
    {
        Vector2 Position { get; }
        void Draw();
    }
}
