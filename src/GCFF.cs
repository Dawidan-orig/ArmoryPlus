using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|backpacks")]
    //генератор контроллируемого силового поля; позволяет брать предметы и контроллировать их на расстоянии
    class GCFF : Backpack
    {
        bool inControl = false;
        bool init = false;

        float saveangle = 0;

        float cooldown = 0;

        CFF cff;

        Holdable controlled;

        private readonly Sprite _pickupSprite;
        private Sprite _sprite;
        public GCFF(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();
            _isArmor = true;
        }

        protected override void save()
        { }

        public override void Update()
        {
            if (_equippedDuck != null && duck == null)
                return;
            if (_equippedDuck != null && !destroyed)
            {
                center = new Vec2(16f, 16f);
                solid = false;
                _sprite.flipH = duck._sprite.flipH;
                graphic = _sprite;

                if (cooldown <= 0)
                {
                    if (_equippedDuck?.inputProfile.Pressed("SHOOT") == true && _equippedDuck.crouch && _equippedDuck.holdObject != null && !init && !(_equippedDuck.holdObject is Equipment))
                    {
                        init = true;
                        inControl = true;

                        controlled = _equippedDuck.holdObject;
                        saveangle = controlled.angle;
                        _equippedDuck.ThrowItem();

                        controlled.enablePhysics = false;
                        controlled.vSpeed = 0;
                        controlled.hSpeed = 0;
                        controlled.flipHorizontal = !_equippedDuck.flipHorizontal;
                        controlled.flipVertical = !_equippedDuck.flipVertical;

                        cff = new CFF(position, controlled, this);
                        Level.Add(cff);
                    }
                }

                Control();

                if (cooldown > 0) cooldown--;
            }
            else
            {
                inControl = false;
                Level.Remove(cff);
                cff = null;
                init = false;
                if (controlled != null)
                {
                    controlled.enablePhysics = true;
                }

                center = new Vec2(_pickupSprite.w / 2, _pickupSprite.h / 2);
                solid = true;
                _sprite.flipH = false;
                graphic = _pickupSprite;
            }

            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawString(cooldown.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            Graphics.DrawCircle(position, 170, Color.Red);
            base.Draw();
        }

        public class CFF : PhysicsObject
        {
            readonly GCFF _this;
            public CFF(Vec2 pos, Holdable h, GCFF _this)
            {
                position = pos;
                this._this = _this;

                center = h.center;
                collisionOffset -= new Vec2(h.collisionSize.x / 2, h.collisionSize.y / 2);
                collisionSize = h.collisionSize;
                weight = h.weight;
                h.angle = 0;
            }

            public override void Update()
            {
                vSpeed += -currentGravity;

                base.Update();
            }

            public override void Draw()
            {
                if (_this.inControl)
                {
                    Graphics.DrawCircle(position, 20, Color.Red);
                }

                base.Draw();
            }
        }

        readonly float cmaxspeed = 10f;
        private void Control()
        {

            if (controlled != null)
            {
                if (controlled.destroyed)
                {
                    inControl = false;
                }
            }

            Gun g = null;
            if (controlled is Gun gun) g = gun;

            bool found = false;
            if (controlled != null)
            {
                foreach (Holdable h in Level.CheckCircleAll<Holdable>(position, 170))
                {
                    if (controlled == h)
                        found = true;
                }
                if (!found && inControl)
                {
                    inControl = false;
                    controlled.ApplyForce((controlled.position - position).normalized * 50 / controlled.weight);

                    for (int r = 1; r < 10; r++)
                    {
                        Vec2 save = position;
                        foreach (Holdable holdable in Level.CheckCircleAll<Holdable>(save, r))
                        {
                            if (holdable == this) continue;
                            holdable.ApplyForce((holdable.position - position).normalized * 5);
                        }
                    }
                    cooldown = 500;
                }
            }

            if (!inControl)
            {
                Level.Remove(cff);
                cff = null;
                init = false;
                if (controlled != null)
                {
                    controlled.enablePhysics = true;
                }
                _equippedDuck.immobilized = false;
                return;
            }
            _equippedDuck.immobilized = true;

            controlled.position = cff.position;

            if (_equippedDuck?.inputProfile.Pressed("GRAB") == true)
            {
                if (cff.hSpeed != 0 || cff.vSpeed != 0)
                    cooldown = 500;

                controlled.ApplyForce(new Vec2(cff.hSpeed * 10 / weight, cff.vSpeed * 10 / weight));
                controlled.angle = saveangle;
                

                for (int r = 1; r < 10; r++)
                {
                    Vec2 save = position;
                    foreach (Holdable holdable in Level.CheckCircleAll<Holdable>(save, r))
                    {
                        if (holdable == this) continue;
                        holdable.ApplyForce((holdable.position - position).normalized * 5);
                    }
                }

                inControl = false;
            }

            cff.hSpeed = 0;
            cff.vSpeed = 0;

            controlled.angleDegrees = 90;

            if (_equippedDuck?.inputProfile.Down("UP") == true)
            {
                cff.vSpeed = -cmaxspeed / cff.weight;
            }
            if (_equippedDuck?.inputProfile.Down("DOWN") == true)
            {
                controlled.angleDegrees = 270;
                cff.vSpeed = cmaxspeed / cff.weight;
            }
            if (_equippedDuck?.inputProfile.Down("LEFT") == true)
            {
                if (cff.vSpeed == 0)
                    controlled.angleDegrees = 0;
                else if (cff.vSpeed > 0)
                    controlled.angleDegrees = 315;
                else
                    controlled.angleDegrees = 45;

                cff.hSpeed = -cmaxspeed / cff.weight;
            }
            if (_equippedDuck?.inputProfile.Down("RIGHT") == true)
            {
                if (cff.vSpeed == 0)
                    controlled.angleDegrees = 180;
                else if (cff.vSpeed > 0)
                    controlled.angleDegrees = 225;
                else
                    controlled.angleDegrees = 135;

                cff.hSpeed = cmaxspeed / cff.weight;
            }
            if (!found && inControl && g != null)
            {
                g.angleDegrees = controlled.angleDegrees;
                g.OnPressAction();
            }
            if (_equippedDuck?.inputProfile.Pressed("GRAB") == true)
            {
                g.angleDegrees = controlled.angleDegrees;
                g.OnPressAction();
            }
        }
    }
}
