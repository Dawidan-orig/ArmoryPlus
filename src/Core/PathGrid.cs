using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmoryPlus.src.Core
{
    public class PathGrid : Thing
    {
        static string pathFormed;

        static List<PathStep> steps = new List<PathStep>();

        private class PathStep : Thing
        {
            public PathStep(Vec2 pos)
            {
                position = pos;
            }
            public override void Update()
            {
                base.Update();
            }

            public override void Draw()
            {
                Graphics.DrawCircle(position, 2f, Color.Red, 4);

                base.Draw();
            }
        }

        public static void init() //Формирование точек хода
        {
            steps = new List<PathStep>();
            foreach (Thing t in Level.core.currentLevel.things)
            {
                if (t is Block || t is IPlatform)
                {
                    if (t is BlockGroup bg)
                    {
                        foreach (Block b in bg.blocks)
                            if (Level.CheckLineAll<Block>(b.position + new Vec2(0, -8 - 2), b.position + new Vec2(0, -8 - 16 - 16 + 2)).Count() == 0)
                            {
                                steps.Add(new PathStep(b.position + new Vec2(0, -16)));
                                Level.Add(new PathStep(b.position + new Vec2(0, -16))); //DEBUG
                            }
                    }
                    else if (Level.CheckLineAll<Block>(t.position + new Vec2(0, -8 - 2), t.position + new Vec2(0, -8 - 16 - 16 + 2)).Count() == 0)
                    {
                        steps.Add(new PathStep(t.position + new Vec2(0, -16)));
                        Level.Add(new PathStep(t.position + new Vec2(0, -16))); //DEBUG
                    }
                }
            }
            pathFormed = Level.activeLevel.level;

        }
    }
}
