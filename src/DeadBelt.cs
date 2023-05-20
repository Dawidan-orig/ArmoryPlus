using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    //Пояс, который взрывается при смерти или снятии
    [EditorGroup("Equipment|ArmoryPlus|belts")]
    public class DeadBelt : Belt
    {
        bool ownerIs = false;
        Vec2 lastPos = new Vec2(0,0);

        private readonly Sprite _pickupSprite;
        private Sprite _sprite;
        public DeadBelt(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "Explosive Belt";
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();            
        }

        public override void Update()
        {
            base.Update();
            if (_equippedDuck == null && ownerIs)
            {
                Destroy();
                if (destroyed)
                {
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
                        if ((double)(physicsObject.position - lastPos).length < 30.0)
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

            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                ownerIs = true;
                lastPos = _equippedDuck.position;
                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                //this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;
            }
            else
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this.solid = true;
                //this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            
        }        
    }
}

