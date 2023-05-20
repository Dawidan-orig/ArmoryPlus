using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|chestplates")]
    public class PlumbumArmor : ChestPlate
    {
        private Sprite _sprite;
        private Sprite _spriteOver;
        private Sprite _pickupSprite;

        Vec2 lastPos = new Vec2(0, 0);

        public PlumbumArmor(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap(GetPath("LeadChestplate.png"), 32, 32);
            _spriteOver = new SpriteMap(GetPath("LeadChestplateOver.png"), 32, 32);
            _pickupSprite = new Sprite(GetPath("LeadChestplateItem.png"));
            _editorName = "Plumbum Chest";
            center = new Vec2(7f, 4f);
            graphic = _pickupSprite;
            _isArmor = true;
            weight = 10;
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                lastPos = _equippedDuck.position;

                solid = false;
                _sprite.flipH = this.duck._sprite.flipH;
                _spriteOver.flipH = this.duck._sprite.flipH;
                graphic = (Sprite)this._sprite;

                if (_equippedDuck.ragdoll != null)
                    _equippedDuck.vSpeed += 1.2f;
                _equippedDuck.hSpeed *= 0.87f;
                Vec2 checkdistance = _equippedDuck.position + new Vec2(0, 25f);
                if (_equippedDuck.vSpeed > 0)
                    foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(_equippedDuck.position + new Vec2(0, 20f), checkdistance))
                    {
                        if (materialThing != _equippedDuck && materialThing is Duck duck)
                        {
                            if (duck != null)
                                OnSoftImpact(_equippedDuck, ImpactedFrom.Top, duck);
                        }
                    }
            }
            else
            {
                center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                solid = true;
                _sprite.flipH = false;
                graphic = this._pickupSprite;
            }
            if (this.destroyed)
            {
                this.alpha -= 0.05f;
            }
            if ((double)this.alpha < 0.0)
            {
                Level.Remove((Thing)this);
            }

            if (_equippedDuck != null)
            {
                if (_equippedDuck.crouch && equippedDuck.grounded)
                {
                    _equippedDuck.hSpeed = 0;
                }
            }

            bool nearGround = false;

            if (_equippedDuck != null)
            {
                    foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(_equippedDuck.position + new Vec2(0, _equippedDuck.height+8f), _equippedDuck.position + new Vec2(0, _equippedDuck.height + 5f)))
                    {
                        if (materialThing is Block || materialThing is IPlatform)
                        {
                            nearGround = true;
                        }
                    }
            }

            if (_equippedDuck != null && _equippedDuck._vSpeed > 8 && nearGround)
            {
                _equippedDuck.scale /= 2;
                new ATMissileShrapnel().MakeNetEffect(lastPos, false);
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 12; ++index)
                {
                    float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                    ATMissileShrapnel atMissileShrapnel = new ATMissileShrapnel();
                    atMissileShrapnel.range = 15f + Rando.Float(5f);
                    Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                    Bullet bullet = new Bullet(lastPos.x + vec2.x * 8f, lastPos.y - vec2.y * 8f, (AmmoType)atMissileShrapnel, num);
                    Level.Add((Thing)Spark.New(lastPos.x + Rando.Float(-8f, 8f), lastPos.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f))));
                    Level.Add((Thing)SmallSmoke.New(lastPos.x + vec2.x * 8f + Rando.Float(-8f, 8f), lastPos.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                }
                foreach (Window window in Level.CheckCircleAll<Window>(lastPos, 30f))
                {
                    if (Level.CheckLine<Block>(lastPos, window.position, (Thing)window) == null)
                        window.Destroy((DestroyType)new DTImpact((Thing)this));
                }
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(lastPos, 70f))
                {
                    if ((double)(physicsObject.position - lastPos).length < 30.0 && physicsObject != this)
                        physicsObject.Destroy((DestroyType)new DTImpact((Thing)this));
                    physicsObject.sleeping = false;
                    physicsObject.vSpeed = -2f;
                }
                HashSet<ushort> varBlocks = new HashSet<ushort>();
                foreach (BlockGroup blockGroup1 in Level.CheckCircleAll<BlockGroup>(lastPos, 50f))
                {
                    if (blockGroup1 != null)
                    {
                        BlockGroup blockGroup2 = blockGroup1;
                        List<Block> blockList = new List<Block>();
                        foreach (Block block in blockGroup2.blocks)
                        {
                            if (Collision.Circle(lastPos, 28f, block.rectangle))
                            {
                                block.shouldWreck = true;
                                if (block is AutoBlock)
                                    varBlocks.Add((block as AutoBlock).blockIndex);
                            }
                        }
                        blockGroup2.Wreck();
                    }
                }
                foreach (Block block in Level.CheckCircleAll<Block>(lastPos, 28f))
                {
                    switch (block)
                    {
                        case AutoBlock _:
                            block.skipWreck = true;
                            block.shouldWreck = true;
                            if (block is AutoBlock)
                            {
                                varBlocks.Add((block as AutoBlock).blockIndex);
                                continue;
                            }
                            continue;
                        case Door _:
                        case VerticalDoor _:
                            Level.Remove((Thing)block);
                            block.Destroy((DestroyType)new DTRocketExplosion((Thing)null));
                            continue;
                        default:
                            continue;
                    }
                }


            }

        }

        public override void Draw() 
        {
            base.Draw();
            if(_equippedDuck != null)
            Graphics.DrawLine(_equippedDuck.position + new Vec2(0, _equippedDuck.height), _equippedDuck.position + new Vec2(0, _equippedDuck.height + 5f), Color.Wheat);
        }

        void OnSoftImpact(MaterialThing with, ImpactedFrom from, Duck toKill)
        {
            Holdable holdable = with as Holdable;
            if (with is Duck && (double)with.weight >= 5.0)
            {
                Duck duck = with as Duck;

                this.vSpeed = with.impactDirectionV * 0.5f;
                with.vSpeed = (float)(-(double)with.vSpeed * 0.7);
                duck._groundValid = 7;
                if (with.isServerForObject)
                    toKill.MakeStars();
                if (toKill.GetEquipment(typeof(Helmet)) is Helmet equipment)
                {
                    SFX.Play("metalRebound");
                    equipment.Crush();
                }
                else
                {
                    if (!with.isServerForObject)
                        return;
                    toKill.Kill((DestroyType)new DTCrush(with as PhysicsObject));
                }
            }
        }
    }
}