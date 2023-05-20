using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|boots")]
    class FireBoots : Boots
    {
        float charge = 0;
        int timeToEmit = 0;

        public FireBoots(float xpos, float ypos) : base(xpos, ypos)
        {
            _pickupSprite = new Sprite(GetPath("Firestarter.png"));
            _sprite = new SpriteMap(GetPath("FirestarterWeared.png"), 32, 32);
            center = new Vec2(7f, -1f);
            graphic = _pickupSprite;
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 12f);
                this.graphic = (Sprite)this._sprite;
                this.collisionOffset = new Vec2(0.0f, -9999f);
                this.collisionSize = new Vec2(0.0f, 0.0f);
                this.solid = false;
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 12;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;

                if (!_equippedDuck.crouch && !_equippedDuck.sliding && _equippedDuck.hSpeed != 0 && _equippedDuck.grounded && charge < 100)                
                    charge += 0.5f;                
                else if(charge > 0)
                    charge -= 0.5f;
                if (charge >= 75 && timeToEmit == 0)
                {
                    Random rand = new Random();
                    timeToEmit = rand.Next(30, 100);
                    Level.Add((Thing)SmallFire.New(this.x , this.y, 0,0, firedFrom: ((Thing)this)));
                }
                else if (timeToEmit != 0) 
                {
                    --timeToEmit;
                }
                if (_equippedDuck.burnt <= 0.75) {
                    _equippedDuck.burnt -= 0.003f;
                }

            }
            else
            {
                center = new Vec2(7f, -1f);
                this.graphic = this._pickupSprite;
                this.collisionOffset = new Vec2(-6f, -6f);
                this.collisionSize = new Vec2(12f, 13f);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);            
        }

        public override void Draw()
        {            
            if (this._equippedDuck != null && !this.destroyed)
            {
                Graphics.DrawString(_equippedDuck.burnt.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            }
                base.Draw();
        }
    }
}
