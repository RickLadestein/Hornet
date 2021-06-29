using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Input.Touch_Recognition;
using HornetEngine.Sound;
using HornetEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox
{
    public class HenkScript : MonoScript
    {
        private SoundSourceComponent scomp;
        private MaterialComponent mcomp;

        private Sample drum_kick;

        public override void Start()
        {
            base.Start();

            scomp = this.entity.GetComponent<SoundSourceComponent>();
            mcomp = this.entity.GetComponent<MaterialComponent>();

            if (scomp == null || mcomp == null)
            {
                throw new Exception("Component was not initialized");
            }

            mcomp.SetTextureUnit("guitar", HTextureUnit.Unit_0);

            drum_kick = new Sample("samples", "drum_kick.wav");
        }

        public override void Update()
        {
            base.Update();
            TouchManager.Instance.Refresh();
            try
            {
                List<TouchObject> objects = TouchManager.Instance.GetTouchObjects();
                TouchObject current_object = objects[0];
                
                switch(current_object.type)
                {
                    case TouchPointType.TYPE1:
                        HandleDrum();
                        break;
                    case TouchPointType.TYPE2:
                        HandleViolin();
                        break;
                    case TouchPointType.TYPE3:
                        HandleGuitar();
                        break;
                    default:
                        Console.WriteLine("Invalid type");
                        break;
                }
            } catch
            {
            }
        }

        private void HandleDrum()
        {
            if (!scomp.IsPlaying)
            {
                scomp.PlaySoundEffect(drum_kick, 1.0f, 1.0f);
            }

            if (mcomp.Textures.textures[0] != TextureResourceManager.Instance.GetResource("drum"))
            {
                mcomp.SetTextureUnit("drum", HTextureUnit.Unit_0);
            }
        }

        private void HandleViolin()
        {
            if (mcomp.Textures.textures[0] != TextureResourceManager.Instance.GetResource("violin"))
            {
                mcomp.SetTextureUnit("violin", HTextureUnit.Unit_0);
            }
        }

        private void HandleGuitar()
        {
            if (mcomp.Textures.textures[0] != TextureResourceManager.Instance.GetResource("guitar"))
            {
                mcomp.SetTextureUnit("guitar", HTextureUnit.Unit_0);
            }
        }
    }
}
