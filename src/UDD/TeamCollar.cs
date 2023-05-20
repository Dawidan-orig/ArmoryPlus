using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmoryPlus.src.Core;
using DuckGame;

namespace ArmoryPlus.src
{
    [EditorGroup("Equipment|ArmoryPlus|DEBUG")]
    class TeamCollar: HoverCollar
    {
        bool converted = false;

        public TeamCollar(float xpos, float ypos) : base(xpos, ypos)
        {
            bounces = new EditorProperty<int>(5, this, -1f, 10f, 0.5f);
            maxSpeed = new EditorProperty<float>(1, this, 0f, 3f, 0.5f);
        }

        public override void Update()
        {

            if (_equippedDuck != null && duck == null)
                return;

            if (_equippedDuck != null) 
            {
                if (controller != null && !converted) 
                {
                    converted = true;
                    _equippedDuck.ConvertDuck(_equippedDuck.converted != null ? duck.converted : controller);
                }
            }

            base.Update();
        }
    }
}
