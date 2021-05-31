using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;

namespace HornetEngine.Ecs
{
    public struct Transform
    {
        /// <summary>
        /// The current Position
        /// </summary>
        public vec3 Position;

        /// <summary>
        /// The current Rotation in degrees
        /// </summary>
        public vec3 Rotation;

        /// <summary>
        /// The current Scale
        /// </summary>
        public vec3 Scale;

        /// <summary>
        /// The matrix that describes the orientation, scale and position
        /// </summary>
        public mat4 ModelMat
        {
            get
            {
                CalcModelMatrix();
                return _model;
            }
        }
        private mat4 _model;

        /// <summary>
        /// Resets the transform rotation, position and scale to default values
        /// </summary>
        public void Reset()
        {
            this.Position = vec3.Zero;
            this.Rotation = vec3.Zero;
            this.Scale = new vec3(1.0f, 1.0f, 1.0f);
        }

        private void CalcModelMatrix()
        {
            float rad_x = OpenTK.Mathematics.MathHelper.DegreesToRadians(Rotation.x);
            float rad_y = OpenTK.Mathematics.MathHelper.DegreesToRadians(Rotation.y);
            float rad_z = OpenTK.Mathematics.MathHelper.DegreesToRadians(Rotation.z);

            quat quat_x = quat.FromAxisAngle(rad_x, new vec3(1.0f, 0.0f, 0.0f));
            quat quat_y = quat.FromAxisAngle(rad_y, new vec3(0.0f, 1.0f, 0.0f));
            quat quat_z = quat.FromAxisAngle(rad_z, new vec3(0.0f, 0.0f, 1.0f));
            quat quat_fin = quat_y * quat_z * quat_x;

            mat4 rot = quat_fin.ToMat4;
            mat4 trans = mat4.Translate(Position);
            mat4 scl = mat4.Scale(Scale);
            this._model = mat4.Identity * trans * scl * rot;
        }
        public override string ToString()
        {
            return $"Position: {Position.x}, {Position.y}, {Position.z}" +
                $"Rotation: {Rotation.x}, {Rotation.y}, {Rotation.z}" +
                $"Scale: {Scale.x}, {Scale.y}, {Scale.z}";
        }
    }
}
