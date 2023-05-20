using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|chestplates")]
    class AmmoChest : ChestPlate
    {
        private readonly Sprite _pickupSprite;
        private Sprite _sprite;
        List<Gun> alreadyReloaded = new List<Gun>();
        public AmmoChest(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");            
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                //this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;
                if (_equippedDuck.gun != null)
                {
                    if (_equippedDuck.gun.ammo == 0 && !alreadyReloaded.Contains(_equippedDuck.gun))
                    {
                        addAmmo(_equippedDuck.gun);
                        Destroy();
                    }
                }
            }
            else
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this.solid = true;
                //this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
            
            base.Update();
        }

        private void addAmmo(Gun gun)
        {
            _equippedDuck.gun._wait += 14;
            SFX.Play("click");
            if (Сompatibility.lowAmmo.Contains(gun.GetType().Name)) //Имеющие мало боеприпасов изначально (<=5)
            {
                gun.ammo += 3;
            }
            else if (Сompatibility.highAmmo.Contains(gun.GetType().Name)) //Много боеприпасов (>50)
            {
                gun.ammo += 50;
            }
            else
            {
                gun.ammo += 12;
            }
            alreadyReloaded.Add(gun);
        }
    }
}
