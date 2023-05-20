using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    //Ролики
    [EditorGroup("Equipment|ArmoryPlus|boots")]
    public class SpeedyBoots : Boots
    {
        float currentSpeed = 0;

        public SpeedyBoots(float xpos, float ypos) : base(xpos, ypos)
        {
            _pickupSprite = new Sprite(GetPath("WalkersRollers.png"));
            _sprite = new SpriteMap(GetPath("WalkersRollersWeared.png"), 32, 32);
            center = new Vec2(7f,-1f);
            graphic = _pickupSprite;
            _editorName = "Rollers";
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null && !this.destroyed)
            {
                center = new Vec2(16f, 13f);
                graphic = _sprite;
                collisionOffset = new Vec2(-6f, -6f);
                collisionSize = new Vec2(12f, 13f);
                this.solid = false;
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 12;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;

                speedWork();
                /*
                if (_sprite.flipH) hSpeed += hSpeed * 1.3f;
                else hSpeed += hSpeed * 1.3f;*/
            }
            else
            {
                center = new Vec2(7f, -1f);
                graphic = this._pickupSprite;
                collisionOffset = new Vec2(-6f, -6f);
                collisionSize = new Vec2(12f, 13f);
                solid = true;
                _sprite.frame = 0;
                _sprite.flipH = false;
            }
            if (destroyed)
            {
                alpha -= 0.05f;
            }
            if (alpha < 0.0)
                Level.Remove((Thing)this);            
        }

        bool setSpeed = false;
        float deltaSpeed = 0.06f;

        void speedWork()
        {
            if (!setSpeed)
            {
                currentSpeed += Math.Sign(_equippedDuck.hSpeed) * 4.8f;
                setSpeed = true;
            }

            if (currentSpeed == 0)
                setSpeed = false;

            if (currentSpeed > 0)
            {
                currentSpeed -= deltaSpeed;
                if (checkForEqualFloat(currentSpeed, 0, 0.2f))
                    currentSpeed = 0;
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deltaSpeed;
                if (checkForEqualFloat(currentSpeed, 0, 0.2f))
                    currentSpeed = 0;
            }
            if (_equippedDuck.grounded)
                _equippedDuck._hSpeed = currentSpeed;
            else currentSpeed = _equippedDuck.hSpeed;
        }
        private bool checkForEqualFloat(float first, float second, float error)
        {
            return (first >= second - error) && (first <= second + error);
        }
    }
}
