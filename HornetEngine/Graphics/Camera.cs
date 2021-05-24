using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HornetEngine.Graphics
{

    /// <summary>
    /// Struct that describes the camera viewing properties like lens specification, fov and clip distance
    /// </summary>
    public struct CameraViewSettings
    {
        public float Lens_width;

        public float Lens_height;

        /// <summary>
        /// The field of view in degrees
        /// </summary>
        public float Fov;

        /// <summary>
        /// The minimum distance in digital units at which renderable objects should be drawn
        /// </summary>
        public float clip_min;

        /// <summary>
        /// The minimum distance in digital units at which renderable objects should be drawn
        /// </summary>
        public float clip_max;
    }

    public class Camera
    {
        public CameraViewSettings ViewSettings {get; set; }

        public Vector3 Position { get; set; }

        /// <summary>
        /// Vector for describing the orientation of the camera in euler angles; camera will always look at Z+ if orientation[0,0,0]
        /// </summary>
        public Vector3 Orientation { get; set; }


        public Vector3 Target { get; private set; }
        public Vector3 Foreward { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }
        public Matrix4x4 ViewMatrix { get; private set; }
        public Camera()
        {
            this.Position = new Vector3(0.0f);
            this.Orientation = new Vector3(0.0f);
        }

        public void UpdateViewMatrix()
        {
            //Translate the orientation to looking point
            Quaternion x_quat = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.X));
            Quaternion y_quat = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.Y));
            Quaternion z_quat = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.Z));
            Quaternion orient_quat = x_quat * y_quat * z_quat;
            Vector4 _foreward = Vector4.Transform(new Vector4(0.0f, 0.0f, 1.0f, 0.0f), orient_quat);
            this.Foreward = new Vector3(_foreward.X, _foreward.Y, _foreward.Z);
            this.Target = this.Foreward + this.Position;

            //apply the current orientation and calculate right vector, up vector and lookat matrix
            Vector3 virt_cam_up = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 cam_dir = Vector3.Normalize(this.Position - this.Target);
            this.Right = Vector3.Normalize(Vector3.Cross(virt_cam_up, cam_dir));
            this.Up = Vector3.Normalize(Vector3.Cross(cam_dir, this.Right));
            this.ViewMatrix = Matrix4x4.CreateLookAt(this.Position, this.Target, this.Up);
        }

        public void UpdateProjectionMatrix()
        {
            float aspect = ViewSettings.Lens_width / ViewSettings.Lens_height;
            this.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(ViewSettings.Fov, aspect, ViewSettings.clip_min, ViewSettings.clip_max);
        }


    }
}
