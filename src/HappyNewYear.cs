using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|special")]
    class HappyNewYear : HoverCollar
    {
        SpriteMap map;
        SpriteMap inversedMap;

        bool Didboom = false;
        bool firsttime = false;
        bool isSpawnedVechile = false;
        bool maxSpeedSaved = false;

        float changingFlying = 0;
        int firstSpriteFlying = 0;

        int changingEquipped = 0;
        int spriteEquipped = 2;

        float currentMaxSpeed;

        int charge = 0;

        private class Vechile : HoverCollar
        {
            float changingFlying = 0;
            int firstSpriteFlying = 2;
            SpriteMap map;

            public Vechile(float xpos, float ypos) : base(xpos, ypos)
            {
                map = new SpriteMap(GetPath("Merry Assmass.png"), 25, 9);
                map.CenterOrigin();
                graphic = map;                
                flyHeight = 1;
                wasThrown = true;
                disableEquip = true;
                disableBounce = true;


                collisionOffset = new Vec2(6f, 3f);
                collisionSize = new Vec2(13f, 5f);
            }

            public override void Update()
            {
                if (changingFlying > 2 && firstSpriteFlying == 2)
                {
                    firstSpriteFlying = 3;
                    changingFlying = 0;
                }
                else if (changingFlying > 2 && firstSpriteFlying == 3)
                {
                    firstSpriteFlying = 2;
                    changingFlying = 0;
                }

                Vec2 checkdistance = position + new Vec2(Math.Sign(hSpeed) * 10f, 0);
                foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(position, checkdistance))
                {
                    if ((materialThing is Block) || (materialThing is Window))
                    {
                        wasThrown = false;
                    }
                }

                if (maxSpeed >= 0)
                    maxSpeed -= 0.01f;
                else maxSpeed = 0;
                changingFlying += _hSpeed;
                map.frame = firstSpriteFlying;
                base.Update();
                canPickUp = false;
            }
        }

        public HappyNewYear(float xpos, float ypos) : base(xpos, ypos)
        {
            bounces = new EditorProperty<int>(0, this, 0, 0);
            maxSpeed = new EditorProperty<float>(2, this, 2, 2);


            disableBounce = true;
            map = new SpriteMap(GetPath("Merry Assmass.png"), 25, 9);
            inversedMap = new SpriteMap(GetPath("Merry Assmass Inv.png"), 9, 25);

            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(20f, 8f);

            map.frame = 0;
            flyHeight = 1;

            center = new Vec2(map.w / 2 - 1, map.h / 2);
            _graphic = map;
        }

        public override void Update()
        {          

            if (_equippedDuck != null && duck == null)
                return;

            if (this._equippedDuck != null && !this.destroyed)
            {
                if (!isSpawnedVechile)
                {
                    isSpawnedVechile = true;
                    Vechile v = new Vechile(x, y-5);
                    v.hSpeed = hSpeed;
                    v.maxSpeed = maxSpeed;
                    v.flipHorizontal = flipHorizontal;
                    Level.Add(v);
                }

                _center = new Vec2(2, 6);
                _wearOffset = new Vec2(-2f, 4);
                solid = false;
                inversedMap.flipH = duck._sprite.flipH;

                if (changingEquipped > 3 && spriteEquipped < 7)
                {
                    inversedMap.frame = spriteEquipped++;
                    if (spriteEquipped == 4) spriteEquipped = 6;
                    changingEquipped = 0;
                }
                else if (changingEquipped > 3 && spriteEquipped >= 7)
                {
                    spriteEquipped = 7;
                    inversedMap.frame = spriteEquipped;
                    spriteEquipped = 2;
                    changingEquipped = 0;
                }
                graphic = inversedMap;
                changingEquipped++;
            }
            else if (duck != null)
            {
                inversedMap.frame = 0;
                inversedMap.flipH = duck._sprite.flipH;
                _holdOffset = new Vec2(6, -6);
                graphic = inversedMap;
            }
            else if (wasThrown)
            {
                if (!maxSpeedSaved) 
                {
                    maxSpeedSaved = true;
                    currentMaxSpeed = maxSpeed.value;
                }

                if (maxSpeed == 0) maxSpeed = currentMaxSpeed;

                if (maxSpeed >= 0)
                    maxSpeed -= 0.01f;
                else { maxSpeed = 0; wasThrown = false; }

                if (changingFlying > 2 && firstSpriteFlying == 0)
                {
                    firstSpriteFlying = 1;
                    changingFlying = 0;
                }
                else if (changingFlying > 2 && firstSpriteFlying == 1)
                {
                    firstSpriteFlying = 0;
                    changingFlying = 0;
                }
                changingFlying += _hSpeed;
                map.frame = firstSpriteFlying;
                graphic = map;
            }
            else
            {
                center = new Vec2((map.w / 2), (float)(map.h / 2));
                solid = true;
                map.flipH = false;
            }

            Vec2 checkdistance = position + new Vec2(Math.Sign(hSpeed) * 10f, 0);
            foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(position, checkdistance))
            {
                if ((materialThing is Block) || (materialThing is Window))
                {
                    wasThrown = false;
                }
            }

            if (_equippedDuck != null)
            {
                if (controller != null)
                {
                    SetDucktoSpace(_equippedDuck);
                    if (charge++ > 40) 
                        Explode();
                }
            }
            base.Update();
        }

        private void SetDucktoSpace(Duck duck)
        {
            if (!firsttime) { firsttime = true; SFX.Play(GetPath("content.wav"), 0.7f, Rando.Float(-0.3f, 0.3f)); }
            
            for (int index = 0; index < 3; ++index)
                Level.Add((Thing)new MusketSmoke(this.x - 5f + Rando.Float(10f), (float)((double)this.y + 50 + (double)Rando.Float(6f) - (double)index * 1.0))
                {
                    move = {
                        x = (Rando.Float(0.4f) - 0.2f),
                        y = (Rando.Float(0.4f) - 0.2f)
                    }
                });
                

            duck.vSpeed -= 3.5f;
            SFX.Play("explode", 0.1f, Rando.Float(0.3f) - 0.3f);
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
            for (var index = 0; index < 750; ++index)
            {
                var num = (float)(index * 30.0 - 10.0) + Rando.Float(20f);
                var atShrapnel = new ATShrapnel { range = 500f + Rando.Float(0f, Rando.Float(70f)), bulletSpeed = 1f, bulletColor = new Color(Rando.Int(128, 255), Rando.Int(128, 255), Rando.Int(128, 255))
            };
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
            Level.Remove(controlled);
        }
    }
}
