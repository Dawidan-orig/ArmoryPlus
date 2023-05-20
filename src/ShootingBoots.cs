using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|boots")]
    class ShootingBoots : Boots
    {
        int charges;
        bool used = false;
        public ShootingBoots(float xpos, float ypos) : base(xpos, ypos)
        {
            _pickupSprite = new Sprite(GetPath("Downshot.png"));
            _sprite = new SpriteMap(GetPath("DownshotWeared.png"), 32, 32);
            center = new Vec2(7f, -1f);
            graphic = _pickupSprite;

            charges = 2;
            physicsMaterial = PhysicsMaterial.Metal;
            _hasEquippedCollision = true;
            _isArmor = false;
        }

        public override void Update()
        {
            base.Update();            
            if (_equippedDuck != null && !destroyed) 
            {                
                if (!_equippedDuck.grounded && _equippedDuck?.inputProfile.Pressed("JUMP") == true && charges > 0  && !used) 
                {
                    center = new Vec2(16f, 12f);
                    graphic = _sprite;
                    charges--;
                    used = true;
                    for (int i = 0; i < 10; i++)
                    {
                        _equippedDuck._vSpeed -= 1;
                        var num = -90 + Rando.Float(-10, 10);
                        var at = new ATShotgun { range = 200f, bulletSpeed = 20f };
                        var bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 8.0),
                            y + 20 - (float)(Math.Sin(Maths.DegToRad(num)) * 8.0), at, num)
                        { firedFrom = this };
                        Level.Add(bullet);
                        SFX.Play("shotgun", pitch: 0.5f);
                    }
                }
                if (used && _equippedDuck.grounded) used = false;
            }
            else
            {
                center = new Vec2(7f, -1f);
            }
        }
    }
}
