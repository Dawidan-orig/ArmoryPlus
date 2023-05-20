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
    public class TracerChest : ChestPlate
    {
        Vec2 _trrSavedPos = new Vec2(0,0);
        bool _hasSaved = false;
        bool _hasUsed = false;
        private readonly Sprite _pickupSprite;
        private Sprite _sprite;
        private float a = 255;

        public TracerChest(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();
            _isArmor = true;
        }

        public override void Update()
        {
            if (a < 255)
                a += 0.5f;
            if (a == 200)
                SFX.Play("equip");
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;
                
                if ((_equippedDuck.grounded || _equippedDuck.sliding) && a >= 200) 
                {                    
                    positionWork(_equippedDuck);
                }
                
            }
            else
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this.solid = true;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        private void positionWork(Duck _equippedDuck) 
        {
            if (_equippedDuck.crouch && _equippedDuck.IsQuacking() && _trrSavedPos.x == 0 && _trrSavedPos.y == 0 && !_hasUsed) //установка
            {
                _trrSavedPos = _equippedDuck.position;
                _hasSaved = true;
            }
            else if (!_equippedDuck.crouch || !_equippedDuck.IsQuacking() && _trrSavedPos.x == 0 && _trrSavedPos.y == 0)  //сброс
            {
                _hasSaved = false;
                _hasUsed = false;
            }
            else if (_equippedDuck.crouch && _equippedDuck.IsQuacking() && !(_trrSavedPos.x == 0 && _trrSavedPos.y == 0) && !_hasSaved) //телепортация
            {
                BigBeam deathBeam = new BigBeam(_trrSavedPos, _equippedDuck.position - _trrSavedPos);
                _equippedDuck.position = _trrSavedPos;
                deathBeam.isLocal = this.isServerForObject;
                Level.Add((Thing)deathBeam);
                _trrSavedPos = new Vec2(0, 0);
                _hasUsed = true;
                a = 10;
                SFX.Play("laserBlast");
            }
        }

        public override void Draw() 
        { 
            Graphics.DrawString(a.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            base.Draw();
        }

        private class BigBeam : Thing
        {
            public float _blast = 1f;
            private Vec2 _target;

            public BigBeam(Vec2 pos, Vec2 target)
              : base(pos.x, pos.y)
              => this._target = target;

            public override void Initialize()
            {
                Vec2 normalized1 = this._target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
                Vec2 normalized2 = this._target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
                Level.Add((Thing)new LaserLine(this.position, this._target, normalized1, 4f, Color.White, 1f));
                Level.Add((Thing)new LaserLine(this.position, this._target, normalized2, 4f, Color.White, 1f));
                Level.Add((Thing)new LaserLine(this.position, this._target, normalized1, 2.5f, Color.White, 2f));
                Level.Add((Thing)new LaserLine(this.position, this._target, normalized2, 2.5f, Color.White, 2f));                
                if (Recorder.currentRecording == null)
                    return;
                Recorder.currentRecording.LogBonus();
            }

            public override void Update()
            {
                this._blast = Maths.CountDown(this._blast, 0.1f);
                if ((double)this._blast >= 0.0)
                    return;
                Level.Remove((Thing)this);
            }

            public override void Draw()
            {
                double num1 = (double)Maths.NormalizeSection(this._blast, 0.0f, 0.2f);
                double num2 = (double)Maths.NormalizeSection(this._blast, 0.6f, 1f);
                double blast = (double)this._blast;
                Vec2 normalized1 = this._target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
                Vec2 normalized2 = this._target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
                Vec2 vec2 = this.position + normalized1 * 16f;
                for (int index = 0; index < 5; ++index)
                {
                    Vec2 p1 = vec2 + normalized2 * 8f * (float)index;
                    Graphics.DrawLine(p1, p1 + this._target, Color.Red * (this._blast * 0.5f), 2f, (Depth)0.9f);
                }
            }
        }
    }
}
