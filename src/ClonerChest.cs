using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|chestplates")]
    class ClonerChest : ChestPlate
    { 
        Duck prevFrameUser;
        int framesToNullify = 20;

        bool triggered = false;

        private readonly Sprite _pickupSprite;
        private Sprite _sprite;
        private Sprite _spriteOver;

        public ClonerChest(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _spriteOver = new SpriteMap("chestPlateAnimOver", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();
            _isArmor = true;
        }

        public override void Update()
        {
            base.Update();
            if (_equippedDuck != null && duck == null)
                return;
            if (destroyed && prevFrameUser != null)
            {
                triggered = true;

                Level.Add(new Clone(false, prevFrameUser));
                prevFrameUser = null;
            }
            if (_equippedDuck != null && !destroyed)
            {
                center = new Vec2(16f, 16f);
                _sprite.flipH = duck._sprite.flipH;
                graphic = _sprite;

                prevFrameUser = _equippedDuck;
            }            
            /*
            if (framesToNullify <= 0)
            {
                prevFrameUser = null;
            }
            else framesToNullify--;
            */
        }
    }
}
