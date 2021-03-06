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
        public vec3 Rotation { 
            get {
                dvec3 _rot = glm.EulerAngles(Orientation);
                return new vec3((float)_rot.x, (float)_rot.y, (float)_rot.z);
            } 
        }


        /// <summary>
        /// The current orientation
        /// </summary>
        public quat Orientation;

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
        /// The matrix that describes the translation of the normal vector
        /// </summary>
        public mat3 NormalMat
        {
            get
            {
                CalcModelMatrix();
                return new mat3(_model.Transposed.Inverse);
            }
        }

        /// <summary>
        /// Resets the transform rotation, position and scale to default values
        /// </summary>
        public void Reset()
        {
            this.Position = vec3.Zero;
            this.Orientation = quat.Identity;
            this.Scale = new vec3(1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        /// Set the current orientation
        /// </summary>
        /// <param name="roll">A float containing the roll of the orientation</param>
        /// <param name="pitch">A float containing the pitch of the orientation</param>
        /// <param name="yaw">A float containing the yaw of the orientation</param>
        public void SetOrientation(float roll, float pitch, float yaw)
        {
            float rad_x = OpenTK.Mathematics.MathHelper.DegreesToRadians(pitch);
            float rad_y = OpenTK.Mathematics.MathHelper.DegreesToRadians(yaw);
            float rad_z = OpenTK.Mathematics.MathHelper.DegreesToRadians(roll);

            quat quat_x = quat.FromAxisAngle(rad_x, new vec3(1.0f, 0.0f, 0.0f));
            quat quat_y = quat.FromAxisAngle(rad_y, new vec3(0.0f, 1.0f, 0.0f));
            quat quat_z = quat.FromAxisAngle(rad_z, new vec3(0.0f, 0.0f, 1.0f));
            quat quat_fin = quat_y * quat_z * quat_x;
            this.Orientation = quat_fin;
        }

        /// <summary>
        /// Rotate the object
        /// </summary>
        /// <param name="rotation_quat">The quat used for the rotation</param>
        public void Rotate(quat rotation_quat)
        {
            this.Orientation = this.Orientation * rotation_quat;
        }

        /// <summary>
        /// Rotate the object
        /// </summary>
        /// <param name="axis_angle">A vec3 which contains the axis angle over which the object should be rotated</param>
        /// <param name="degrees">A float which contains the degrees of the rotation</param>
        public void Rotate(vec3 axis_angle, float degrees)
        {
            this.Orientation = this.Orientation.Rotated(OpenTK.Mathematics.MathHelper.DegreesToRadians(degrees), axis_angle);
        }

        private void CalcModelMatrix()
        {
            mat4 rot = this.Orientation.ToMat4;
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
