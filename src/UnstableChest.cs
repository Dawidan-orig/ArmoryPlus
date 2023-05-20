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
    public class UnstableChest : ChestPlate
    {
        float collectedMass = 0;
        int abilityCharge = 650;
        bool usedAbility = false;

        List<AlphaBullets> alphaBullets;

        string[] noicePaths;

        class AlphaBullets 
        {
            public Bullet bullet { get; set; }
            public float alpha { get; set; }
        }

        public UnstableChest(float xpos, float ypos) : base(xpos, ypos)
        {
            string[] noicePaths = new string[16];
            for (int i = 0; i < 16; i++) 
            {
                noicePaths[i] = GetPath("nicenoice/" + (i+1)+".wav");
            }
            this.noicePaths = noicePaths;

            alphaBullets = new List<AlphaBullets>();
            physicsMaterial = PhysicsMaterial.Plastic;
            _hasEquippedCollision = false;
            _isArmor = true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._equippedDuck == null || bullet.owner == this.duck || !bullet.isLocal)
                return false;

            if (bullet.isLocal)
            {
                Fondle(this, DuckNetwork.localConnection);
            }
            //if (bullet.isLocal && Network.isActive)
            NetSoundEffect.Play("equipmentTing");

            Level.Add(MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));

            for (int index = 0; index < 6; ++index)
                Level.Add(Spark.New(bullet.x, bullet.y, bullet.travelDirNormalized));
            if (usedAbility)
            {
                AlphaBullets ab = new AlphaBullets();
                ab.alpha = 2;
                ab.bullet = bullet;
                alphaBullets.Add(ab);
                abilityCharge -= 20;
                return true;
            }
            else
                return base.Hit(bullet, hitPos);

        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;

            if (_equippedDuck != null && !destroyed) 
            {
                if (_equippedDuck.holdObject != null && _equippedDuck?.inputProfile.Pressed("SHOOT") == true) 
                {
                    if (!(_equippedDuck.holdObject is Gun || _equippedDuck.holdObject is Equipment || _equippedDuck.holdObject is TV) && collectedMass <= 50)
                    {
                        collectedMass += _equippedDuck.holdObject.weight;
                        Level.Remove(_equippedDuck.holdObject);
                        SFX.Play(GetPath("consume.wav"), 1.5f, 1);
                    }
                }
            }

            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 8f);

            if (_equippedDuck != null) 
            {
                if ((_equippedDuck.IsQuacking() && collectedMass > 50) || usedAbility) 
                {                                     
                    if (_equippedDuck.holdObject != null) _equippedDuck.ThrowItem();
                    if (abilityCharge % 20 == 0)
                    {
                        int r = Rando.Int(1, 8);                        
                        SFX.Play(GetPath("nicenoice/"+r+".wav"), 0.5f);                        
                    }
                    
                    usedAbility = true;
                    collectedMass = 0;
                    _equippedCollisionOffset = new Vec2(-7*6f, -5*6f);
                    _equippedCollisionSize = new Vec2(12*6f,11*6f);
                    collisionOffset = new Vec2(-6*6f, -4*6f);
                    collisionSize = new Vec2(11*6f, 8*6f);
                }
            }

            if (abilityCharge <= 0 && usedAbility)
            {
                abilityCharge = 650;
                usedAbility = false;
            }
            else if (abilityCharge > 0 && usedAbility) abilityCharge--;
                base.Update();
        }

        public override void Draw()
        {
            if (usedAbility && alphaBullets.Count != 0) 
            {
                for(int i = 0; i < alphaBullets.Count; i++)
                {
                    Graphics.DrawLine(position, alphaBullets[i].bullet.position, Color.Wheat, alphaBullets[i].alpha);
                    alphaBullets[i].alpha -= 0.1f;
                }
                alphaBullets = alphaBullets.Where(ab =>( ab.alpha > 0)).ToList();
            }

            Graphics.DrawString(abilityCharge.ToString(CultureInfo.InvariantCulture), position + new Vec2(0, -16), Color.GreenYellow);
            //Graphics.DrawRect(rectangle, new Color(255, 0, 0));

            base.Draw();
        }
    }
}
