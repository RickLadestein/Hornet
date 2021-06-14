using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Input;
using HornetEngine.Sound;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox
{
    public class PlayerScript : MonoScript, IDisposable
    {
        public Keyboard keyboard;
        public Mouse mouse;
        public Scene scene;

        public override void Start()
        {
            base.Start();
            mouse.MouseMove += Mouse_MouseMove;
        }

        private void Mouse_MouseMove(double xpos, double ypos, double deltaX, double deltaY)
        {
            Camera cam = Camera.Primary;
            float mouse_modifier = 0.5f;
            if (mouse.GetMode() == MouseMode.FPS)
            {
                cam.Rotate(new GlmSharp.vec3(1.0f, 0.0f, 0.0f), (float)-deltaY * mouse_modifier);
                cam.Rotate(new GlmSharp.vec3(0.0f, 1.0f, 0.0f), (float)-deltaX * mouse_modifier);
                cam.UpdateViewMatrix();
            }
        }

        public override void Update()
        {
            base.Update();

            Camera cam = Camera.Primary;
            char[] btns = keyboard.GetPressedButtons();
            float key_modifier = 2.0f;
            for (int i = 0; i < Keyboard.MAX_PRESSED_BUTTONS; i++)
            {
                switch ((Silk.NET.GLFW.Keys)btns[i])
                {
                    case Silk.NET.GLFW.Keys.W:
                        cam.Position += cam.Foreward * key_modifier;
                        break;
                    case Silk.NET.GLFW.Keys.S:
                        cam.Position -= cam.Foreward * key_modifier;
                        break;
                    case Silk.NET.GLFW.Keys.A:
                        cam.Position -= cam.Right * key_modifier;
                        break;
                    case Silk.NET.GLFW.Keys.D:
                        cam.Position += cam.Right * key_modifier;
                        break;
                    case Silk.NET.GLFW.Keys.F:
                        this.PlaySound();
                        break;
                }
            }
            cam.UpdateViewMatrix();
        }

        private void PlaySound()
        {
            Entity found = scene.FindEntityById("line");
            Sample sample = SoundManager.Instance.GetResource("bonk");
            if (found != null && sample != null)
            {
                SoundSourceComponent ssc = found.GetComponent<SoundSourceComponent>();
                if(ssc != null)
                {
                    ssc.PlaySoundEffect(sample, 1.0f, 1.0f);
                }
            }
        }

        public void Dispose()
        {
            mouse.MouseMove -= Mouse_MouseMove;
        }
    }
}
