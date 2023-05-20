using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;
using ArmoryPlus.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|backpacks")]
    public class CringeBackpack : Backpack
    {
        EditorProperty<int> charges;
        EditorProperty<int> uses;
        //Этот рюкзак при активации раскидывает всё то, что можно кинуть. Гранаты, мины, римские свечи и мечи
        public CringeBackpack(float xpos, float ypos) : base(xpos, ypos)
        {
            charges = new EditorProperty<int>(5, this, 1f, 25f, 0.5f);
            uses = new EditorProperty<int>(3, this, -1, 10, 0.5f);
        }

        protected override void save()
        {
            if (_equippedDuck?.inputProfile.Pressed("GRAB") == true && savething == null && _equippedDuck.holdObject != null && Сompatibility.throwables.Contains(_equippedDuck.holdObject.GetType().Name))
            {
                savething = _equippedDuck.holdObject;
                Level.Remove(_equippedDuck.holdObject);
            }
        }

        int r = 10;

        public override void Update()
        {
            if(_equippedDuck != null && uses > 0)
            if(savething != null && _equippedDuck.crouch && _equippedDuck.IsQuacking()) 
            {
                uses--;
                for (int i = 0; i < charges; i++) 
                {
                    if (!(Editor.CreateThing(savething.GetType()) is Gun thing))
                        return;
                    Level.Add(thing);
                        thing.position = new Vec2(Rando.Int(-15, 15), Rando.Int(-20, -5)) + position;
                        thing.OnPressAction();

                        thing.ApplyForce((thing.position - position).normalized*15);
                    }

                for (r = 1; r < 50; r++)
                {
                    Vec2 save = position;
                    foreach (Holdable holdable in Level.CheckCircleAll<Holdable>(save, r))
                    {
                        if (holdable == this) continue;
                        holdable.ApplyForce((holdable.position - position).normalized*5);
                    }
                        savething = null;
                }
            }
            if (uses == 0) Destroy();

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
