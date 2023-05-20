using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|helmets")]
    class PropellerHat : Helmet
    {
        int a = 0;
        public PropellerHat(float xpos, float ypos) : base(xpos, ypos)
        {
            physicsMaterial = PhysicsMaterial.Plastic;
            _isArmor = true;
        }

        public override void Update()
        {
            if(_equippedDuck != null && !destroyed)
            if (!crushed) 
            {
                    if (a <= 100 && !_equippedDuck.grounded)
                    {
                        _equippedDuck.vSpeed -= 0.075f;
                        a++;
                    }
                    else if(a >= 0) a--;
            }

            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawString(a.ToString(), position + new Vec2(0, -16), Color.GreenYellow);

            base.Draw();
        }
    }
}
