using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src.Core
{
    public class Clone : PhysicsObject
    {
        bool isTransparent;
        private Sprite _sprite;

        public Clone(bool isTransparent, Duck toClone) : base(toClone.position.x, toClone.position.y)
        {
            _featherVolume = new FeatherVolume(toClone);
            _featherVolume.anchor = this;
            centerx = toClone.centerx;
            centery = toClone.centery;
            flammable = 1f;
            thickness = toClone.thickness;
            collideSounds.Add("land", ImpactedFrom.Bottom);
            _impactThreshold = 1.3f;
            _impactVolume = 0.4f;
            this.SetCollisionMode("normal");
            physicsMaterial = PhysicsMaterial.Duck;
            _sprite = toClone._sprite.Clone();
            _sprite.CenterOrigin();
            position = toClone.position;

            this.isTransparent = isTransparent;
            PathGrid.init();
        }

        public override void Update()
        {
            graphic = _sprite;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.DrawRect(rectangle, Color.Red);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
            {
                for (int index = 0; index < 3; ++index)
                    Level.Add(new MusketSmoke(x + Rando.Float(10f), (float)((double)y + Rando.Float(6f) - index * 1.0)));
                SFX.Play("thwip", pitch: Rando.Float(-0.1f, 0.1f));
            }
            Level.Remove(this);
            return base.Hit(bullet, hitPos);
        }

        //
        //!!!Копия свойств!!!
        //


        private string _collisionMode = "normal";
        protected FeatherVolume _featherVolume;
        private float _duckWidth = 1f;
        private float _duckHeight = 1f;
        public float duckWidth
        {
            get => this._duckWidth;
            set
            {
                this._duckWidth = value;
                this.xscale = this._duckWidth;
            }
        }

        public float duckHeight
        {
            get => this._duckHeight;
            set
            {
                this._duckHeight = value;
                this.yscale = this._duckHeight;
            }
        }

        public void SetCollisionMode(string mode)
        {
            this._collisionMode = mode;
            if (this.offDir > (sbyte)0)
                _featherVolume.anchor.offset = new Vec2(0.0f, 0.0f);
            else
                this._featherVolume.anchor.offset = new Vec2(1f, 0.0f);
            if (mode == "normal")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 22f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, -7f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 26f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -9f * this.duckHeight);
            }
            else if (mode == "slide")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 11f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, 4f * this.duckHeight);
                if (this.offDir > (sbyte)0)
                {
                    this._featherVolume.collisionSize = new Vec2(25f * this.duckWidth, 13f * this.duckHeight);
                    this._featherVolume.collisionOffset = new Vec2(-13f * this.duckWidth, 3f * this.duckHeight);
                }
                else
                {
                    this._featherVolume.collisionSize = new Vec2(25f * this.duckWidth, 13f * this.duckHeight);
                    this._featherVolume.collisionOffset = new Vec2(-12f * this.duckWidth, 3f * this.duckHeight);
                }
            }
            else if (mode == "crouch")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 16f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, -1f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 20f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -3f * this.duckHeight);
            }
            else if (mode == "netted")
            {
                this.collisionSize = new Vec2(16f * this.duckWidth, 17f * this.duckHeight);
                this.collisionOffset = new Vec2(-8f * this.duckWidth, -9f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(18f * this.duckWidth, 19f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-9f * this.duckWidth, -10f * this.duckHeight);
            }
            this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 12f * this.duckHeight);
            this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -6f * this.duckHeight);
        }
    }
}
