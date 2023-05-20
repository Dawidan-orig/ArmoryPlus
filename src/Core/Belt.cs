using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{    
    public class Belt : Equipment
    {
        public Belt(float xpos, float ypos) : base(xpos, ypos)
        {
            physicsMaterial = PhysicsMaterial.Metal;
            _hasEquippedCollision = true;
            _isArmor = false;

            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 8f);
        }

        public override void Update()
        {            
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw() => base.Draw();
        
    }
}
