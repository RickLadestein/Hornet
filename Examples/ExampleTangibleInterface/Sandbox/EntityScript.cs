using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Input;
using HornetEngine.Input.Touch_Recognition;
using HornetEngine.Sound;
using HornetEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox
{
    public class EntityScript : MonoScript
    {
        private string curr_model;
        private bool has_modifier;

        private bool modifier_state_switched;

        public MeshComponent meshcomp;
        public MaterialComponent matcomp;
        public SoundSourceComponent soundsourcecomp;
        public override void Start()
        {
            base.Start();
            curr_model = "";
            modifier_state_switched = false;
            entity.Transform.Position = new GlmSharp.vec3(0.0f, -10.0f, 100.0f);
            entity.Transform.Scale = new GlmSharp.vec3(0.5f, 0.5f, 0.5f);
            entity.Transform.Rotate(new GlmSharp.vec3(0.0f, 1.0f, 0.0f), 90);
        }

        public override void Update()
        {
            base.Update();
            entity.Transform.Rotate(new GlmSharp.vec3(0.0f, 1.0f, 0.0f), 5.0f * Time.FrameDelta);

            List<TouchObject> detected = TouchManager.Instance.GetTouchObjects();
            if(detected == null || detected.Count == 0 || detected.Count > 2)
            {
                SetVisibleMesh("surface");
            } else
            {
                int index = 0;
                foreach(TouchObject to in detected)
                {
                    Console.WriteLine(to);
                    index += 1;
                }


                TouchObject first_found = detected.Find((e) =>
                {
                    return (e.type == TouchPointType.TYPE2 || e.type == TouchPointType.TYPE3);
                });

                if(first_found == null)
                {
                    SetVisibleMesh("surface");
                } else if(first_found.type == TouchPointType.TYPE2)
                {
                    SetVisibleMesh("cat");
                } else if(first_found.type == TouchPointType.TYPE3)
                {
                    SetVisibleMesh("dog");
                }


                TouchObject modifier = detected.Find((e) => { return e.type == TouchPointType.TYPE4; });
                if(modifier == null)
                {
                    matcomp.SetTextureUnit("default", HTextureUnit.Unit_0);
                    SetVisibleMesh(curr_model);

                    if(has_modifier)
                    {
                        modifier_state_switched = true;
                    }
                    has_modifier = false;
                } else
                {
                    matcomp.SetTextureUnit("laminate", HTextureUnit.Unit_0);

                    if (!has_modifier)
                    {
                        modifier_state_switched = true;
                    }
                    has_modifier = true;
                }


                if(modifier_state_switched)
                {
                    PlaySound(curr_model);
                }
            }
        }

        private void SetVisibleMesh(string mesh_id)
        {
            if(curr_model != mesh_id)
            {
                meshcomp.SetTargetMesh(mesh_id);
                PlaySound(mesh_id);
                curr_model = mesh_id;
            }
        }

        private void PlaySound(string mesh_id)
        {
            //Play the cat or dog sound effect
            if (mesh_id != "surface")
            {
                if (has_modifier)
                {
                    soundsourcecomp.PlaySoundEffect(SoundResourceManager.Instance.GetResource(mesh_id), 1.0f, 0.75f);
                }
                else
                {
                    soundsourcecomp.PlaySoundEffect(SoundResourceManager.Instance.GetResource(mesh_id), 0.75f, 0.75f);
                }
            }
            modifier_state_switched = false;
        }
    }
}
