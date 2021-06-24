using HornetEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    class DeferredRenderComponent : Component
    {
        private MaterialComponent default_material;
        public DeferredRenderComponent()
        {
            default_material = new MaterialComponent();
            default_material.SetShaderFromId("default");
            default_material.SetTextureUnit("default", HTextureUnit.Unit_0);
        }

        public void Render(Camera target)
        {
            
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
