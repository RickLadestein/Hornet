using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Graphics;
using HornetEngine.Util;
using HornetEngine.Util.Exceptions;

namespace HornetEngine.Ecs
{
    public class MaterialComponent : Component
    {
        public MultiTexture Texture { get; private set; }
        public ShaderProgram Shader { get; private set; }

        public MaterialComponent()
        {
            this.Texture = new MultiTexture();
            this.Shader = null;
        }

        public MaterialComponent(ShaderProgram prg)
        {
            if(prg == null)
            {
                throw new ArgumentNullException("ShaderProgram");
            }
            this.Texture = new MultiTexture();
            this.Shader = prg;
        }

        public void SetShaderFromId(String matId)
        {
            ShaderProgram result = ShaderResourceManager.GetInstance().GetResource(matId);
            if(result == null)
            {
                throw new MissingResourceException(matId);
            }
            this.Shader = result;
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
