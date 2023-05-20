using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src.Core
{
    public class Collar : Equipment
    {
        public Collar(float xpos, float ypos) : base(xpos, ypos)
        {
            physicsMaterial = PhysicsMaterial.Metal;
            _hasEquippedCollision = false;
            _isArmor = false;
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;

            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw() => base.Draw();
    }
}
