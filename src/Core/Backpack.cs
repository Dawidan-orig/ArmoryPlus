using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.Core;
using DuckGame;

namespace ArmoryPlus.src.Core
{
    [EditorGroup("Equipment|ArmoryPlus|backpacks")]
    public class Backpack : Equipment
    {
        protected Holdable savething = null;

        public Backpack(float xpos, float ypos) : base(xpos, ypos)
        {
            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 8f);
        }

        protected virtual void save() 
        {
            if (_equippedDuck?.inputProfile.Pressed("GRAB") == true && savething == null && _equippedDuck.holdObject is Gun holdobj)
            {
                if (!Сompatibility.throwables.Contains(holdobj.GetType().Name))
                {
                    savething = _equippedDuck.holdObject;
                    Level.Remove(_equippedDuck.holdObject);
                }
            }
            else if (_equippedDuck?.inputProfile.Pressed("GRAB") == true && savething == null && _equippedDuck.holdObject != null && !(_equippedDuck.holdObject is RagdollPart || _equippedDuck.holdObject is Equipment))
            {
                savething = _equippedDuck.holdObject;
                Level.Remove(_equippedDuck.holdObject);
            }            
            else if (_equippedDuck?.inputProfile.Pressed("GRAB") == true && savething != null && _equippedDuck.holdObject == null && _equippedDuck.crouch)
            {
                Level.Add(savething);
                _equippedDuck.GiveHoldable(savething);
                _equippedDuck.resetAction = true;
                savething = null;
            }
            
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;

            if (_equippedDuck != null && !destroyed) 
            {
                save();
            }

            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw() => base.Draw();
    }
}
