using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|belts")]
    public class ShieldBelt : Belt
    {
        private readonly Sprite _pickupSprite;
        private Sprite _sprite;

        public ShieldBelt(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "One-side Shield Belt";
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();
            _isArmor = true;
            _equippedCollisionOffset = new Vec2(10f, -10f);
            _equippedCollisionSize = new Vec2(2f, 10f);
            _hasEquippedCollision = true;
        }

        float a = 255;
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._equippedDuck == null || bullet.owner == this.duck || !bullet.isLocal)
                return false;
            if (bullet.isLocal)
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            }
            //if (bullet.isLocal && Network.isActive)
            NetSoundEffect.Play("equipmentTing");

            Level.Add((Thing)MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));

            for (int index = 0; index < 6; ++index)
                Level.Add((Thing)Spark.New(this.x, this.y, bullet.travelDirNormalized));
            if (a >= 200)
            {
                a = 10;
                return true;                
            }
            else            
                return base.Hit(bullet, hitPos);
            
        }

        public override void Update()
        {
            if (a < 255) 
                a += 0.35f;

            

            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                //this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;
            }
            else
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this.solid = true;
                //this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawRect(rectangle, new Color(255, 0, 0, a));
            Graphics.DrawString(a.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            base.Draw();
        }
    }
}
