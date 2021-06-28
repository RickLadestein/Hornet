using HornetEngine.Ecs;
using OpenTK.Audio.OpenAL;
using System;
using System.Numerics;

namespace HornetEngine.Sound
{
    /// <summary>
    /// The SoundManager class can be used to manage the user's details within the application.
    /// </summary>
    public class Listener
    {
        private static Listener instance = null;
        private static readonly object padlock = new object();

        private Vector3 position;
        private Vector3 looking_dir;
        private Vector3 up;
        private float global_vol;

        /// <summary>
        /// The constructor of the listener class
        /// </summary>
        public Listener()
        {
            position = new Vector3(0.0f, 0.0f, 0.0f);
            looking_dir = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.0f, 0.0f, 0.0f);
            global_vol = 1.0f;
        }

        /// <summary>
        /// A function which will allow the user to change their own position
        /// </summary>
        /// <param name="pos">A vector with 3 elements, containing the X, Y and Z coords of the user.</param>
        public void SetPosition(Vector3 pos)
        {
            position = pos;
            AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
        }

        /// <summary>
        /// A function which can be used to set a new position
        /// </summary>
        /// <param name="pos">A vec3 containing the new position</param>
        public void SetPosition(GlmSharp.vec3 pos)
        {
            position = new Vector3(pos.x, pos.y, pos.z);
            AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
        }

        /// <summary>
        /// A function which will allow the user to change their looking direction
        /// </summary>
        /// <param name="dir">A vector with 3 elements, containing the X, Y and Z coords of the direction the user is looking at.</param>
        public void SetLookingDir(Vector3 dir)
        {
            looking_dir = dir;
            OpenTK.Mathematics.Vector3 looking_dir_tk = new OpenTK.Mathematics.Vector3(dir.X, dir.Y, dir.Z);
            OpenTK.Mathematics.Vector3 up_dir_tk = new OpenTK.Mathematics.Vector3(up.X, up.Y, up.Z);

            AL.Listener(ALListenerfv.Orientation, ref looking_dir_tk, ref up_dir_tk);
        }

        /// <summary>
        /// A function which can be used to set the looking direction
        /// </summary>
        /// <param name="dir">A vec3 containing the new looking direction</param>
        public void SetLookingDir(GlmSharp.vec3 dir)
        {
            looking_dir = new Vector3(dir.x, dir.y, dir.z);
            SetLookingDir(looking_dir);
        }

        /// <summary>
        /// A function which will allow the user to change their up direction
        /// </summary>
        /// <param name="upv">A vector with 3 elements, containing the X, Y and Z coords of the up vector</param>
        public void setUpDir(Vector3 upv)
        {
            up = upv;
            OpenTK.Mathematics.Vector3 looking_dir_tk = new OpenTK.Mathematics.Vector3(looking_dir.X, looking_dir.Y, looking_dir.Z);
            OpenTK.Mathematics.Vector3 up_dir_tk = new OpenTK.Mathematics.Vector3(upv.X, upv.Y, upv.Z);

            AL.Listener(ALListenerfv.Orientation, ref looking_dir_tk, ref up_dir_tk);
        }

        /// <summary>
        /// A function which will allow the user to change the global volume
        /// </summary>
        /// <param name="gvol">A float which contains the global volume</param>
        public void setGlobalVol(float gvol)
        {
            global_vol = gvol;
            AL.Listener(ALListenerf.Gain, gvol);
        }

        /// <summary>
        /// A function which will return the user's position
        /// </summary>
        /// <returns>A Vector3 containing the user's X, Y and Z coords.</returns>
        public Vector3 getPosition()
        {
            return this.position;
        }


        /// <summary>
        /// A function which will return the user's looking direction
        /// </summary>
        /// <returns>A Vector3 containing the user's looking direction X, Y and Z coords.</returns>
        public Vector3 getLookingDir()
        {
            return this.looking_dir;
        }

        /// <summary>
        /// A function which will return the user's up direction
        /// </summary>
        /// <returns>A Vector3 containing the user's up direction X, Y and Z coords.</returns>
        public Vector3 getUpDir()
        {
            return this.up;
        }

        /// <summary>
        /// A function which will return the global volume
        /// </summary>
        /// <returns>A flooat containing the global volume.</returns>
        public float getGlobalVolume()
        {
            return this.global_vol;
        }

        /// <summary>
        /// A method to get the instance of the Listener.
        /// The lock ensures that the singleton is thread-safe.
        /// </summary>
        public static Listener Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Listener();
                    }
                    return instance;
                }
            }
        }
    }
}
