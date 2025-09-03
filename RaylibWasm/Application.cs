using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;

using Frent;
using Frent.Components;
using System.Numerics;
using Frent.Systems;
using Frent.Updating;

namespace RaylibWasm
{
    public partial class Application
    {
        private static Texture2D logo;
        private static World _world = null!;

        const int Width = 1024;
        const int Height = 512;

        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main()
        {
            Raylib.InitWindow(Width, Height, "RaylibWasm");
            Raylib.SetTargetFPS(60);


            logo = Raylib.LoadTexture("Resources/raylib_logo.png");

            _world = new();
            for (int i = 1; i < 10; i++)
                _world.Create<Square, Position, Velocity, Color, V, float>(
                    new(30),
                    new(new(i * 50, 0)),
                    new(Vector2.One * i),
                    Color.Red, default, default);
        }

        /// <summary>
        /// Updates frame
        /// </summary>
        [JSExport]
        public static void UpdateFrame()
        {
            _world.Update<Tick>();

            _world.Query<Square, Position, Velocity>()
                .Delegate((ref Square s, ref Position pos, ref Velocity v) =>
                {
                    if (pos.Value.X < 0 || pos.Value.X + s.Size > Width)
                        v.Delta = v.Delta with { X = -v.Delta.X };
                    if (pos.Value.Y < 0 || pos.Value.Y + s.Size > Height)
                        v.Delta = v.Delta with { Y = -v.Delta.Y };
                });


            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);

            Raylib.DrawFPS(4, 4);
            Raylib.DrawText("All systems operational!", 4, 32, 20, Color.Maroon);
            Raylib.DrawTexture(logo, 4, 64, Color.White);

            _world.Update<Draw>();

            Raylib.EndDrawing();
        }
    }

    record struct Square(int Size) : IComponent<Position, Color>
    {
        [Draw]
        public void Update(ref Position pos, ref Color c) => 
            Raylib.DrawRectangle((int)pos.Value.X, (int)pos.Value.Y, Size, Size, c);
    }
    record struct Position(Vector2 Value);

    record struct Velocity(Vector2 Delta) : IComponent<Position>
    {
        [Tick]
        public void Update(ref Position pos) => pos.Value += Delta;
    }

    record struct V : IUniformComponent<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float>
    {
        public void Update(float uniform, ref float arg1, ref float arg2, ref float arg3, ref float arg4, ref float arg5, ref float arg6, ref float arg7, ref float arg8, ref float arg9, ref float arg10, ref float arg11, ref float arg12, ref float arg13, ref float arg14, ref float arg15, ref float arg16)
        {

        }
    }

    class Tick : UpdateTypeAttribute;
    class Draw : UpdateTypeAttribute;
}
