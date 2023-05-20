using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|collars")]
    class ExploCollar : HoverCollar
    {
        public bool Didboom;

        private SpriteMap _sprite;
        private SpriteMap _spriteOver;
        private Sprite _pickupSprite;
        private Sprite _lyingSprite;
        private Sprite _flyingSprite;

        private float angleDelta = 0.4f;

        public ExploCollar(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "Explosive Collar";
            _sprite = new SpriteMap(GetPath("Chloride Eye Anima.png"), 32, 32);
            _spriteOver = new SpriteMap(GetPath("Chloride Eye Anima.png"), 32, 32);
            _pickupSprite = new Sprite(GetPath("Chloride Eye Picked.png"));
            _lyingSprite = new Sprite(GetPath("Chloride Eye Lying.png"));
            _flyingSprite = new Sprite(GetPath("Chloride Eye Flying.png"));       
            _lyingSprite.CenterOrigin();

            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 10f);

            center = new Vec2(_lyingSprite.w / 2, _lyingSprite.h / 2);
            graphic = _lyingSprite;            
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                center = new Vec2(16, 20f);
                solid = false;
                _sprite.flipH = duck._sprite.flipH;
                _spriteOver.flipH = duck._sprite.flipH;
                graphic = _sprite;
                if (controller != null)
                    if (controller.IsQuacking())
                    {
                        Explode();
                    }
            }
            else if (duck != null)
            {
                graphic = _pickupSprite;
            }
            else if (wasThrown) 
            {                
                angle += angleDelta;
                graphic = _flyingSprite;
            }
            else
            {
                center = new Vec2(_lyingSprite.w / 2, _lyingSprite.h / 2);
                solid = true;
                _sprite.flipH = false;
                graphic = _lyingSprite;
            }

            if (controller != null) if (controller.IsQuacking()) { this.Destroy(); SFX.Play("jump", 1, -0.5f); }

            base.Update();
        }

        public override void Draw() 
        {
            base.Draw();
            if (this._equippedDuck != null && this.duck == null || this._equippedDuck == null)
                return;
            _spriteOver.flipH = graphic.flipH;
            _spriteOver.angle = angle;
            _spriteOver.alpha = alpha;
            _spriteOver.scale = scale;
            _spriteOver.depth = owner.depth + (this.duck.holdObject != null ? 3 : 11);
            _spriteOver.center = center;
            Graphics.Draw(_spriteOver, x, y);
        }

        private void Explode()
        {
            if (Didboom) return;
            Didboom = true;
            Graphics.FlashScreen();
            for (var index = 0; index < 1; ++index)
            {
                var explosionPart = new ExplosionPart(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add(explosionPart);
            }
            SFX.Play("explode");
            if (!isServerForObject) return;


            var varBullets = new List<Bullet>();
            for (var index = 0; index < 150; ++index)
            {
                var num = (float)(index * 30.0 - 10.0) + Rando.Float(20f);
                var atShrapnel = new ATShrapnel { range = 30f + Rando.Float(0f, Rando.Float(70f)) };
                var bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 8.0),
                        y - (float)(Math.Sin(Maths.DegToRad(num)) * 8.0), atShrapnel, num)
                { firedFrom = this };
                varBullets.Add(bullet);
                Level.Add(bullet);
            }

            if (Network.isActive)
            {
                Send.Message(new NMExplodingProp(varBullets), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            foreach (var window in Level.CheckCircleAll<Window>(position, 40f))
                if (Level.CheckLine<Block>(position, window.position, window) == null)
                    window.Destroy(new DTImpact(this));
            foreach (var thing in Level.CheckCircleAll<Thing>(position, 500f))
            {
                if (Level.CheckLine<Block>(position, thing.position, thing) != null) continue;
                //else
                var dVec2 = thing.position - position + new Vec2(Rando.Float(-6f, 6f), Rando.Float(-6f, 6f));
                var l = dVec2.length + 0.1f;
                var force = dVec2 * (1000f / (l * l * l));
                //force.y *= 0.8f;
                //force.x *= 1.1f;
                thing.ApplyForce(force);
            }
            Level.Remove(this);
        }
    }
}
