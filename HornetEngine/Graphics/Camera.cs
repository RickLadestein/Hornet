using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;

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

        public vec3 Position { get; set; }

        /// <summary>
        /// Vector for describing the orientation of the camera in euler angles; camera will always look at Z+ if orientation[0,0,0]
        /// </summary>
        public vec3 Orientation { get; set; }


        public vec3 Target { get; private set; }
        public vec3 Foreward { get; private set; }
        public vec3 Right { get; private set; }
        public vec3 Up { get; private set; }

        public mat4 ProjectionMatrix { get; private set; }
        public mat4 ViewMatrix { get; private set; }
        public Camera()
        {
            this.Position = new vec3(0.0f);
            this.Orientation = new vec3(0.0f);
        }

        public void UpdateViewMatrix()
        {
            //Translate the orientation to looking point
            quat x_quat = quat.FromAxisAngle(OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.x), new vec3(1, 0, 0));
            quat y_quat = quat.FromAxisAngle(OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.y), new vec3(0, 1, 0));
            quat z_quat = quat.FromAxisAngle(OpenTK.Mathematics.MathHelper.DegreesToRadians(Orientation.z), new vec3(0, 0, 1));
            quat orient_quat = x_quat * y_quat * z_quat;
            vec4 _foreward = orient_quat * new vec4(0.0f, 0.0f, 1.0f, 0.0f);
            this.Foreward = _foreward.xyz;
            this.Target = this.Foreward + this.Position;

            //apply the current orientation and calculate right vector, up vector and lookat matrix
            vec3 virt_cam_up = new vec3(0.0f, 1.0f, 0.0f);
            vec3 cam_dir = glm.Normalized(this.Position - this.Target);
            this.Right = glm.Normalized(glm.Cross(virt_cam_up, cam_dir));
            this.Up = glm.Normalized(glm.Cross(cam_dir, this.Right)); ;
            this.ViewMatrix = mat4.LookAt(this.Position, this.Target, this.Up);
        }

        public void UpdateProjectionMatrix()
        {
            float aspect = ViewSettings.Lens_width / ViewSettings.Lens_height;
            this.ProjectionMatrix = mat4.PerspectiveFov(ViewSettings.Fov, ViewSettings.Lens_width, ViewSettings.Lens_height, ViewSettings.clip_min, ViewSettings.clip_max);
        }


    }
}
