using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|boots")]
    //Ботинки, которые позволяют при двойном нажатии активировать рывок
    class DashBoots : Boots
    {
        int cooldown = 0;
        int useFrames = 7;
        bool isusing = false;

        float saveDelta = 0;
        bool firstSaveWas = false;

        int ctimefir = 0;
        int ctimesec = 0;
        readonly int pressedTime = 20;
        readonly int releasedTime = 20;

        public DashBoots(float xpos, float ypos) : base(xpos, ypos)
        {
            _pickupSprite = new Sprite(GetPath("Booster.png"));
            _sprite = new SpriteMap(GetPath("BoosterWeared.png"), 32, 32);
            graphic = this._pickupSprite;
            center = new Vec2(6f, -2f);            
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedDepth = 3;
            flammable = 0.3f;
            charThreshold = 0.8f;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (isusing)
            {
                bullet.position += new Vec2(bullet.vSpeed*5, bullet.hSpeed*5);
                return false;                
            }
            else
                return base.Hit(bullet, hitPos);
        }

        bool firstPressL = false;
        bool firstReleaseL = false;
        private bool doubleTapCheckLeft()
        {
            if (_equippedDuck?.inputProfile.Pressed("LEFT") == true && !firstReleaseL)
            {
                if (ctimefir == 0) {
                    firstPressL = true;
                    ctimefir = pressedTime; }
                else if (ctimefir > 0)
                {
                    ctimefir--;
                    if (ctimefir == 1) { firstPressL = false; ctimefir = 0; }
                }
            }
            else if (_equippedDuck?.inputProfile.Pressed("LEFT") == false && firstPressL)
            {
                if (ctimesec == 0)
                {
                    ctimesec = releasedTime;
                    firstReleaseL = true;
                }
                else if (ctimesec > 0)
                {
                    ctimesec--;
                    if (ctimesec == 1) {
                        firstReleaseL = false;
                        firstPressL = false;
                        ctimefir = 0;
                        ctimesec = 0;
                    }
                }
            }
            else if (_equippedDuck?.inputProfile.Pressed("LEFT") == true && firstReleaseL)
            {
                firstReleaseL = false;
                firstPressL = false;
                ctimefir = 0;
                ctimesec = 0;
                return true;
            }
            return false;
        }

        bool firstPressR = false;
        bool firstReleaseR = false;
        private bool doubleTapCheckRight()
        {
            if (_equippedDuck?.inputProfile.Pressed("RIGHT") == true && !firstReleaseR)
            {
                if (ctimefir == 0)
                {
                    firstPressR = true;
                    ctimefir = pressedTime;
                }
                else if (ctimefir > 0)
                {
                    ctimefir--;
                    if (ctimefir == 1) { firstPressR = false; ctimefir = 0; }
                }
            }
            else if (_equippedDuck?.inputProfile.Pressed("RIGHT") == false && firstPressR)
            {
                if (ctimesec == 0)
                {
                    ctimesec = releasedTime;
                    firstReleaseR = true;
                }
                else if (ctimesec > 0)
                {
                    ctimesec--;
                    if (ctimesec == 1)
                    {
                        firstReleaseR = false;
                        firstPressR = false;
                        ctimefir = 0;
                        ctimesec = 0;
                    }
                }
            }
            else if (_equippedDuck?.inputProfile.Pressed("RIGHT") == true && firstReleaseR)
            {
                firstReleaseR = false;
                firstPressR = false;
                ctimefir = 0;
                ctimesec = 0;
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
            if(!(_equippedDuck != null && !destroyed))
            {
                center = new Vec2(6f, -2f);
            }

            if (_equippedDuck != null)
            {
                float d = 0;
                if (doubleTapCheckLeft() && cooldown <= 0)
                {
                    d = 60;
                    foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(_equippedDuck.topLeft + new Vec2(-60, 3), _equippedDuck.bottomLeft + new Vec2(0, -3)))
                    {
                        if (materialThing is Block)
                            d = Math.Min(d, _equippedDuck.bottomLeft.x - materialThing.bottomRight.x);
                    }
                    d = -d;
                }
                else if (doubleTapCheckRight() && cooldown <= 0)
                {
                    d = 60;
                    foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(_equippedDuck.topRight + new Vec2(0, 3), _equippedDuck.bottomRight + new Vec2(60, -3)))
                    {
                        if (materialThing is Block)
                            d = Math.Min(d, -_equippedDuck.bottomRight.x + materialThing.bottomLeft.x);
                    }
                }
                if (d != 0 || isusing)
                {
                    if (useFrames > 0)
                    {
                        if (!firstSaveWas) { saveDelta = d / useFrames; firstSaveWas = true; }
                        _equippedCollisionOffset = new Vec2(-7 * 3f, -5 * 3f);
                        _equippedCollisionSize = new Vec2(12 * 3f, 11 * 3f);
                        collisionOffset = new Vec2(-6 * 3f, -4 * 3f);
                        collisionSize = new Vec2(11 * 3f, 8 * 3f);
                        isusing = true;
                        useFrames--;
                        cooldown = 80;
                        sprites.Add(new SpriteAlpha(_equippedDuck._sprite, _equippedDuck.position, 1));
                        _equippedDuck.position += new Vec2(saveDelta, 0);
                    }
                    else
                    {
                        collisionOffset = new Vec2(-6f, -6f);
                        collisionSize = new Vec2(12f, 13f);
                        _equippedDuck.sleeping = false;
                        useFrames = 7;
                        isusing = false;
                        saveDelta = 0;
                        firstSaveWas = false;
                    }
                }
            }

            if (cooldown > 0) cooldown--;
        }

        class SpriteAlpha
        {
            public SpriteAlpha(SpriteMap s, Vec2 v, int a) {
                sprite = s.Clone();
                sprite.flipH = s.flipH;
                x = v.x;
                y = v.y;
                this.a = a;
            }

            public Sprite sprite;
            public float x, y;
            public float a;
        }

        List<SpriteAlpha> sprites = new List<SpriteAlpha>();
        
        public override void Draw()
        {
            if (_equippedDuck != null)
            {
                Graphics.DrawString(cooldown.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
                Graphics.DrawRect(rectangle, Color.Red);
            }
            if (sprites.Count() > 0) 
            {
                foreach (SpriteAlpha sa in sprites) 
                {
                    sa.a -= 0.05f;
                    sa.sprite.alpha = sa.a;
                    Graphics.Draw(sa.sprite, sa.x, sa.y);                    
                }
                sprites = sprites.Where(sa => sa.a > 0).ToList();
            }
            base.Draw();
        }
    }
}
