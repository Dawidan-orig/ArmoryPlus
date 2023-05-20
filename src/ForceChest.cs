using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|chestplates")]
    class ForceChest : ChestPlate
    {
        float charge = 100;

        private Sprite _sprite;
        private Sprite _spriteOver;
        private Sprite _pickupSprite;

        public ForceChest(float xpos, float ypos) : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("chestPlateAnim", 32, 32);
            this._spriteOver = new SpriteMap("chestPlateAnimOver", 32, 32);
            this._pickupSprite = new Sprite("chestPlatePickup");
            this._editorName = "Force Chestplate";
            _isArmor = true;
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
                this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;

                if (_equippedDuck.IsQuacking() && charge > 0)
                {
                    ForceField ff = new ForceField(_equippedDuck.topRight + new Vec2(-_equippedDuck.width / 2 - _equippedDuck.offDir * 5, -2), new Vec2(_equippedDuck.offDir*8, _equippedDuck.height*1.5f ), _equippedDuck);
                    ff.isLocal = this.isServerForObject;
                    Level.Add((Thing)ff);
                    charge -= 0.25f*3;
                }
                else
                {
                    this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                    this.solid = true;
                    this._sprite.flipH = false;
                    this.graphic = this._pickupSprite;
                }
                if (this.destroyed)
                {
                    this.alpha -= 0.05f;
                }
                if ((double)this.alpha < 0.0)
                {
                    Level.Remove((Thing)this);
                }

            }
            base.Update();
        }

        private class ForceLine : LaserLine
        {
            private Vec2 _move;
            private Vec2 _target;

            private Duck ignoreDuck;
            public ForceLine(
                Duck ignoreDuck,
                Vec2 pos,
                Vec2 target,
                Vec2 moveVector,
                float moveSpeed,
                Color color,
                float thickness) : base(pos, target, moveVector, moveSpeed, color, thickness) 
            {
                this.ignoreDuck = ignoreDuck;
                _move = moveVector;
                _target = target;
            }

            public override void Update()
            {
                Vec2 vec2 = new Vec2(x,y);

                foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(vec2, vec2 + _target))
                {
                    if (materialThing != ignoreDuck)
                        materialThing.ApplyForce(_move/10);
                }

                base.Update();
            }
        }

        private class ForceField : Thing
        {
            public float _field = 1f;
            private Vec2 _target;
            private Duck ignoreDuck;

            public ForceField(Vec2 pos, Vec2 target, Duck ignoreDuck) : base(pos.x, pos.y)
            {
                _target = target;
                this.ignoreDuck = ignoreDuck;
            }

            public override void Initialize()
            {                
                Vec2 normalized1 = ignoreDuck.OffsetLocal(new Vec2(1f, -1f));

                Vec2 vec2 = position + normalized1 * 2f;

                Level.Add((Thing)new ForceLine(ignoreDuck,vec2, _target, normalized1, 4f, Color.White, 1f));

                if (Recorder.currentRecording == null)
                    return;
                Recorder.currentRecording.LogBonus();
            }

            public override void Update()
            {
                this._field = Maths.CountDown(this._field, 0.1f);
                if ((double)this._field >= 0.0)
                    return;
                Level.Remove((Thing)this);
            }
        }
        public override void Draw()
        {
            Graphics.DrawString(charge.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            base.Draw();
        }
    }
}
