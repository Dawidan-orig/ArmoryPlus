using System;
using System.Globalization;
using DuckGame;
using JetBrains.Annotations;

namespace ArmoryPlus.src
{

    [EditorGroup("Equipment|ArmoryPlus|special")]
    public class LaserArm : Equipment
    {
        private bool laserSight = true;
        private Vec2 laserRawOffset = new Vec2(7f,2f);
        private Vec2 _wallPoint = new Vec2();
        private bool _laserInit;
        private Sprite _sightHit;
        private Tex2D _laserTex;
        private float _laserAngle = 0f;
        private Vec2 _laserOffset = new Vec2(0f,0f);
        private float _laserRange;
        private readonly Sprite _pickupSprite;
        private Sprite _sprite;

        public LaserArm(float xpos, float ypos) : base(xpos, ypos) {
            this._sightHit = new Sprite("laserSightHit");
            this._sightHit.CenterOrigin();
            this._editorName = "Laser Arm";
            this._sprite = new Sprite(GetPath("ArmLaser.png"), 2, 2);
            _pickupSprite = _sprite;
            _pickupSprite.CenterOrigin();
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 8f);
        }

        bool _canDraw = false;

        public override void Update()
        {
            _canDraw = false;
            if (equippedDuck != null)
            {
                if(_equippedDuck.sliding) laserRawOffset = new Vec2(2f, 7f);
                else laserRawOffset = new Vec2(7f, 2f);

                if (equippedDuck.gun != null)
                {
                    var a = equippedDuck.gun.OffsetLocal(new Vec2(1f, 0f));
                    //var b = equippedDuck.OffsetLocal(new Vec2(1f, 0f));
                    _laserAngle = -1* Maths.RadToDeg((float)(Math.Atan2(a.y,a.x)));
                    _laserOffset = equippedDuck.gun.handOffset;
                    _laserRange = equippedDuck.gun.ammoType.range;                    
                    _canDraw = true;
                }
            }

            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 16f);
                this._sprite.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;
            }
            else 
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
                base.Update();
        }

        public override void DoUpdate()
        {
            if (this.laserSight && this._laserTex == null)
            {
                this._laserTex = Content.Load<Tex2D>("pointerLaser");
            }
            base.DoUpdate();
        }

        public override void Draw()
        {
            if (laserSight && owner != null  && _canDraw)
            {
                ATTracer atTracer = new ATTracer();
                atTracer.range = _laserRange;
                float ang = _laserAngle;
                Vec2 vec2 = Offset(this.laserRawOffset);
                atTracer.penetration = 0.4f;
                _wallPoint = new Bullet(vec2.x, vec2.y, (AmmoType)atTracer, ang, owner, tracer: true).end;
                _laserInit = true;
            }

            base.Draw();
        }
        public override void DrawGlow()
        {
            if (this.laserSight && this.owner != null && (this._laserTex != null && this._laserInit) && _canDraw)
            {
                Vec2 p1 = this.Offset(this.laserRawOffset);
                //p1.Rotate(handAngle, _laserOffset);
                float length = (p1 - this._wallPoint).length;
                float val1 = 1000f;               
                Vec2 normalized = (this._wallPoint - p1).normalized;
                Vec2 vec2 = p1 + normalized * Math.Min(val1, length);
                vec2.Rotate(handAngle, _laserOffset);
                Graphics.DrawTexturedLine(this._laserTex, p1, vec2, Color.Red, 0.5f, this.depth - 1);
                if ((double)length > (double)val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(this._laserTex, vec2, vec2 + normalized * 2f, Color.Red * (float)(1.0 - (double)index * 0.2), 0.5f, this.depth - 1);
                        vec2 += normalized * 2f;
                    }
                }

                if (this._sightHit != null && (double)length < (double)val1)
                {
                    this._sightHit.alpha = 1f;
                    _sightHit.color = Color.Red;
                    Graphics.Draw(_sightHit, this._wallPoint.x, this._wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}
