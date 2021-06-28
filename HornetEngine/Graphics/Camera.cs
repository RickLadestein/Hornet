using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;
using HornetEngine.Ecs;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;

namespace HornetEngine.Graphics
{

    /// <summary>
    /// Struct that describes the camera viewing properties like lens specification, fov and clip distance
    /// </summary>
    public struct CameraViewSettings
    {
        /// <summary>
        /// The width of the lens
        /// </summary>
        public float Lens_width;

        /// <summary>
        /// The height of the lens
        /// </summary>
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
        /// <summary>
        /// The CameraViewSettings
        /// </summary>
        public CameraViewSettings ViewSettings {get; set; }

        /// <summary>
        /// The current position of the camera
        /// </summary>
        public vec3 Position { get; set; }

        /// <summary>
        /// Quaternion for describing the orientation of the camera; camera will always look at Z+ if orientation[0,0,0]
        /// </summary>
        public quat Orientation { get; set; }

        /// <summary>
        /// The current Rotation in degrees
        /// </summary>
        public vec3 Rotation
        {
            get
            {
                dvec3 _rot = glm.EulerAngles(Orientation);
                return new vec3((float)_rot.x, (float)_rot.y, (float)_rot.z);
            }
        }

        /// <summary>
        /// The framebuffer of the camera
        /// </summary>
        public FrameBuffer FrameBuffer { get; private set; }

        private MeshComponent Render_plane;
        private MaterialComponent Material;

        /// <summary>
        /// The foreward view
        /// </summary>
        public vec3 Foreward { get; private set; }

        /// <summary>
        /// The right view
        /// </summary>
        public vec3 Right { get; private set; }

        /// <summary>
        /// The up view
        /// </summary>
        public vec3 Up { get; private set; }

        public vec3 Target { get; private set; }

        /// <summary>
        /// The projection matrix
        /// </summary>
        public mat4 ProjectionMatrix { get; private set; }

        /// <summary>
        /// The view matrix
        /// </summary>
        public mat4 ViewMatrix { get; private set; }

        /// <summary>
        /// The primary camera
        /// </summary>
        public static Camera Primary { get; private set; }

        /// <summary>
        /// The constructor of the camera
        /// </summary>
        public Camera()
        {
            this.Position = new vec3(0.0f);
            this.Orientation = quat.Identity;
            this.Up = new vec3(0.0f, 1.0f, 0.0f);

            this.ViewSettings = new CameraViewSettings()
            {
                Lens_height = 480,
                Lens_width = 720,
                Fov = OpenTK.Mathematics.MathHelper.DegreesToRadians(45),
                clip_min = 1.0f,
                clip_max = 100.0f
            };
            InitCamRenderPlane();
            this.UpdateProjectionMatrix();
            
            
        }

        private void InitCamRenderPlane()
        {
            this.FrameBuffer = new FrameBuffer((uint)this.ViewSettings.Lens_width, (uint)this.ViewSettings.Lens_height);
            if (!MeshResourceManager.Instance.HasResource("camplane"))
            {
                Mesh m = new Mesh("camplane");
                FloatAttribute attrib = new FloatAttribute("position", 3);
                attrib.AddData(new GlmSharp.vec3(-1.0f, -1.0f, 0.0f));
                attrib.AddData(new GlmSharp.vec3(-1.0f, 1.0f, 0.0f));
                attrib.AddData(new GlmSharp.vec3(1.0f, 1.0f, 0.0f));

                attrib.AddData(new GlmSharp.vec3(-1.0f, -1.0f, 0.0f));
                attrib.AddData(new GlmSharp.vec3(1.0f, 1.0f, 0.0f));
                attrib.AddData(new GlmSharp.vec3(1.0f, -1.0f, 0.0f));
                m.Attributes.AddAttribute(attrib);
                m.BuildVertexBuffer();
                MeshResourceManager.Instance.AddResource("camplane", m);

                ShaderProgram prog1 = new ShaderProgram(new VertexShader("shaders", "deferred_post.vert"), new FragmentShader("shaders", "deferred_post.frag"));
                ShaderResourceManager.Instance.AddResource("deferred_post", prog1);
            }

            this.Render_plane = new MeshComponent();
            this.Render_plane.SetTargetMesh("camplane");

            this.Material = new MaterialComponent();
            this.Material.SetShaderFromId("deferred_post");
        }

        /// <summary>
        /// A function which can be used to register the primary camera 
        /// </summary>
        /// <param name="scene">The scene from which the primary camera can be pulled</param>
        public static void RegisterScenePrimaryCamera(Scene scene)
        {
            Primary = scene.PrimaryCam;
        }

        /// <summary>
        /// Sets the orientation
        /// </summary>
        /// <param name="roll">The roll used in the rotation</param>
        /// <param name="pitch">The pitch used in the rotation</param>
        /// <param name="yaw">The yaw used in the rotation</param>
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
        /// Rotate the camera
        /// </summary>
        /// <param name="rotation_quat">The rotation quat used for the rotation</param>
        public void Rotate(quat rotation_quat)
        {
            this.Orientation = this.Orientation * rotation_quat;
        }

        /// <summary>
        /// Rotate the camera
        /// </summary>
        /// <param name="axis_angle">The axis over which the camera should be rotated</param>
        /// <param name="degrees">The amount of degrees which the camera should be rotated</param>
        public void Rotate(vec3 axis_angle, float degrees)
        {
            this.Orientation = this.Orientation.Rotated(OpenTK.Mathematics.MathHelper.DegreesToRadians(degrees), axis_angle);
        }

        /// <summary>
        /// Update the view matrix
        /// </summary>
        public void UpdateViewMatrix()
        {
            //Translate the orientation to looking point
            vec4 _foreward = Orientation * new vec4(0.0f, 0.0f, 1.0f, 0.0f);
            this.Foreward = _foreward.xyz;
            this.Target = this.Foreward + this.Position;

            //apply the current orientation and calculate right vector, up vector and lookat matrix
            vec3 virt_cam_up = new vec3(0.0f, 1.0f, 0.0f);
            vec3 cam_dir = glm.Normalized(this.Position - this.Target);
            this.Right = glm.Normalized(glm.Cross(virt_cam_up, cam_dir));
            this.Up = glm.Normalized(glm.Cross(cam_dir, this.Right));
            this.ViewMatrix = mat4.LookAt(this.Position, this.Target, this.Up);
        }

        /// <summary>
        /// Update the projection matrix
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            float aspect = ViewSettings.Lens_width / ViewSettings.Lens_height;
            this.ProjectionMatrix = mat4.PerspectiveFov(ViewSettings.Fov, ViewSettings.Lens_width, ViewSettings.Lens_height, ViewSettings.clip_min, ViewSettings.clip_max);
        }
    }
}
