using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace HornetEngine.Ecs
{
    public class Transform : Component
    {
        /// <summary>
        /// The current Position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                this.update_matrix = true;
            }
        }
        private Vector3 _position;

        /// <summary>
        /// The current Rotation in degrees
        /// </summary>
        public Vector3 Rotation {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                this.update_matrix = true;
            }
        }
        private Vector3 _rotation;

        /// <summary>
        /// The current Scale
        /// </summary>
        public Vector3 Scale { 
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                this.update_matrix = true;
            }
        }
        private Vector3 _scale;

        /// <summary>
        /// The matrix that describes the orientation, scale and position
        /// </summary>
        public Matrix4x4 ModelMat
        {
            get
            {
                CalcModelMatrix();
                return _model;
            }
        }
        private Matrix4x4 _model;

        private bool update_matrix;

        public Transform()
        {
            this.Reset();
        }

        public Transform(Vector3 pos, Vector3 rot, Vector3 scl)
        {
            this._position = pos;
            this._rotation = rot;
            this._scale = scl;
        }

        /// <summary>
        /// Resets the transform rotation, position and scale to default values
        /// </summary>
        public void Reset()
        {
            this._position = Vector3.Zero;
            this._rotation = Vector3.Zero;
            this._scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        private void CalcModelMatrix()
        {
            if (update_matrix)
            {
                float rad_x = OpenTK.Mathematics.MathHelper.DegreesToRadians(_rotation.X);
                float rad_y = OpenTK.Mathematics.MathHelper.DegreesToRadians(_rotation.Y);
                float rad_z = OpenTK.Mathematics.MathHelper.DegreesToRadians(_rotation.Z);
                Quaternion quat = Quaternion.CreateFromYawPitchRoll(rad_y, rad_x, rad_z);

                Matrix4x4 rot = Matrix4x4.CreateFromQuaternion(quat);
                Matrix4x4 trans = Matrix4x4.CreateTranslation(_position);
                Matrix4x4 scl = Matrix4x4.CreateScale(_scale);
                this._model = rot * trans * scl;
            }
        }
        public override string ToString()
        {
            return $"Position: {_position.X}, {_position.Y}, {_position.Z}" +
                $"Rotation: {_rotation.X}, {_rotation.Y}, {_rotation.Z}" +
                $"Scale: {_scale.X}, {_scale.Y}, {_scale.Z}";
        }
    }
}
