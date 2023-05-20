using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace ArmoryPlus.src.Core
{
    public class HoverCollar : Collar
    {
        float saveSpeed;

        protected bool wasThrown = false;
        public EditorProperty<int> bounces;
        public EditorProperty<float> maxSpeed;
        protected float flyHeight = 17.5f;

        protected Duck controller;
        protected Duck controlled;

        protected bool disableBounce = false;
        protected bool disableEquip = false;

        public HoverCollar(float xpos, float ypos) : base(xpos, ypos)
        {
            bounces = new EditorProperty<int>(5, this, -1f, 10f, 0.5f);
            maxSpeed = new EditorProperty<float>(1, this, 0f, 5f, 0.5f);

            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Thrown()
        {
            controller = duck;
            wasThrown = true;            
            base.Thrown();
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;

            if (wasThrown)
            {
                //Поддержание высоты
                Vec2 height = position + new Vec2(0, flyHeight);
                foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(position, height))
                {
                    if (materialThing is Block || materialThing is IPlatform)
                    {
                        vSpeed -= 0.205f;
                    }
                }
                //Поддержание скорости
                if (hSpeed > 0 && hSpeed < maxSpeed) hSpeed = maxSpeed;
                else if (hSpeed > maxSpeed * -1 && hSpeed < 0) hSpeed = maxSpeed * -1;
                if (hSpeed == 0) hSpeed = saveSpeed;
                saveSpeed = hSpeed;

                //отскок ИЛИ присобачивание к утке
                Vec2 checkdistance = position + new Vec2(Math.Sign(hSpeed)*10f, 0);
                
                foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(position, checkdistance))
                {
                    if (((materialThing is Block)||(materialThing is Window)) && bounces != 0  && !disableBounce)
                    {
                        hSpeed *= -1;
                        bounces--;
                        SFX.Play("swordClash", 0.7f, 0.5f);
                    }
                    else if (materialThing is Duck duck && materialThing != controller && !disableEquip)
                    {
                        controlled = duck;
                        duck.Equip(this);
                        wasThrown = false;
                    }
                }
            }

            canPickUp = !wasThrown;

            if (bounces == 0 && !disableBounce)
            {
                this.Destroy();
                SFX.Play("ting2");
            }
            base.Update();
        }

        public override void Draw() => base.Draw();        
    }
}
