using HornetEngine.Ecs;
using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;
using HornetEngine.Util;
using System.Threading.Tasks;

namespace HornetEngine.Graphics
{
    public class Scene
    {
        private static Scene _scene;
        private static object lck = new object();

        /// <summary>
        /// The current instance of the Scene
        /// </summary>
        public static Scene Instance
        {
            get
            {
                lock(lck)
                {
                    if(_scene == null)
                    {
                        _scene = new Scene();
                    }
                }
                return _scene;
            }
        }

        private List<Entity> scene_content;
        private List<Entity> deferred_objects;
        private List<Entity> foreward_objects;
        private List<Entity> light_objects;

        private bool scene_content_changed;

        /// <summary>
        /// The Scene's primary camera that renders to the screen
        /// </summary>
        public Camera PrimaryCam { get; private set; }
        private Scene()
        {
            scene_content = new List<Entity>();
            foreward_objects = new List<Entity>();
            deferred_objects = new List<Entity>();
            light_objects = new List<Entity>();

            PrimaryCam = new Camera();
            Camera.RegisterScenePrimaryCamera(this);
            scene_content_changed = true;
            Initialize();
        }

        /// <summary>
        /// Resets and clears the scene to the starting setup
        /// </summary>
        public void Reset()
        {
            this.scene_content.Clear();
            this.deferred_objects.Clear();
            this.foreward_objects.Clear();
            light_objects.Clear();

            PrimaryCam.Position = vec3.Zero;
            PrimaryCam.UpdateProjectionMatrix();
            PrimaryCam.UpdateViewMatrix();

        }

        /// <summary>
        /// Finds Entity in the scene that matches the anem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity FindEntityById(String id)
        {
            Entity found = this.scene_content.Find(entity => {
                return entity.Name == id;
            });

            return found;
        }

        /// <summary>
        /// Finds all Entity's in the scene that matches the predicate
        /// </summary>
        /// <param name="match">The conditions that the entity must match for retrieval</param>
        /// <returns></returns>
        public List<Entity> FindEntity(Predicate<Entity> match)
        {
            List<Entity> found = this.scene_content.FindAll(match);
            return found;
        }

        /// <summary>
        /// Adds an Entity to the scene
        /// </summary>
        /// <param name="entity">The Entity to be added to the scene</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddEntity(Entity entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.scene_content.Add(entity);
            this.scene_content_changed = true;
        }

        /// <summary>
        /// Removes Entity with specified name from the scene
        /// </summary>
        /// <param name="name">The Entity name</param>
        public void RemoveEntity(String name)
        {
            Predicate<Entity> pred = new Predicate<Entity>((e) => {
                return e.Name == name;
            });
            RemoveEntity(pred);
            this.scene_content_changed = true;
        }

        /// <summary>
        /// Removes Entities from the scene that matches the predicate
        /// </summary>
        /// <param name="match">The conditions that the entity must match for removal</param>
        public void RemoveEntity(Predicate<Entity> match)
        {
            this.scene_content.RemoveAll(match);
            this.foreward_objects.RemoveAll(match);
            this.deferred_objects.RemoveAll(match);
            this.light_objects.RemoveAll(match);
            this.scene_content_changed = true;
        }

        /// <summary>
        /// Updates the scene and all entities by calling fixed update
        /// </summary>
        public void UpdateFixed()
        {
            foreach (Entity ex in this.scene_content)
            {
                Parallel.ForEach(ex.Scripts, (e) => {
                    if (e.start)
                    {
                        e.Start();
                    } else
                    {
                        e.FixedUpdate();
                    }
                });
            }
        }

        /// <summary>
        /// Updates the scene and all entities by calling update
        /// </summary>
        public void UpdateScene()
        {
            SortSceneContent();
            Camera.Primary.UpdateProjectionMatrix();
            Camera.Primary.UpdateViewMatrix();
            foreach(Entity ex in this.scene_content)
            {
                Parallel.ForEach(ex.Scripts, (e) => {
                    if (e.start)
                    {
                        e.Start();
                    }
                    else
                    {
                        e.Update();
                    }
                });
            }
        }

        /// <summary>
        /// A function which gets the refresh function
        /// </summary>
        /// <returns>The refresh function of the window</returns>
        public Window.WindowRefreshFunc GetRefreshFunc()
        {
            return RedrawFunc;
        }

        public void LoadScene(string folder_id, string scene_file)
        {
            scene_content.Clear();
            string error = DirectoryManager.ImportSceneFromFile(folder_id, scene_file, out Assimp.Scene s);

            if (error != string.Empty)
            {
                throw new Exception($"Could not load scene: {error}");
            }

            for (int i = 0; i < s.MeshCount; i++)
            {
                Assimp.Mesh _amesh = s.Meshes[i];
                Assimp.Material mat = s.Materials[s.Meshes[i].MaterialIndex];

                Mesh tmp = Mesh.ImportMesh(_amesh, mat);
                if (tmp.Error.Length != 0)
                {
                    throw new Exception($"Mesh {tmp.Name} loading encountered an error: {tmp.Error}");
                }

                MeshResourceManager.Instance.AddResource(tmp.Name, tmp);

                Entity e = new Entity(tmp.Name);

                MeshComponent meshcomp = new MeshComponent();
                meshcomp.SetTargetMesh(tmp.Name);
                e.AddComponent(meshcomp);

                MaterialComponent matcomp = new MaterialComponent();
                e.AddComponent(matcomp);

                e.AddComponent(new MeshRenderComponent());
                AddEntity(e);


                matcomp.SetShaderFromId("deferred_pre");
                matcomp.Material = MaterialDescriptor.ParseFrom(mat);
                if (matcomp.Material.Ambient_map != string.Empty)
                {
                    if (!TextureResourceManager.Instance.HasResource(matcomp.Material.Ambient_map))
                    {
                        TextureResourceManager.Instance.ImportResource(matcomp.Material.Ambient_map, "textures", matcomp.Material.Ambient_map);
                    }
                    matcomp.SetTextureUnit(matcomp.Material.Ambient_map, HTextureUnit.Unit_0);
                }
                else
                {
                    matcomp.SetTextureUnit("black", HTextureUnit.Unit_0);
                }

                if (matcomp.Material.Specular_map != string.Empty)
                {
                    if (!TextureResourceManager.Instance.HasResource(matcomp.Material.Normal_map))
                    {
                        TextureResourceManager.Instance.ImportResource(matcomp.Material.Normal_map, "textures", matcomp.Material.Normal_map);
                    }
                    matcomp.SetTextureUnit(matcomp.Material.Normal_map, HTextureUnit.Unit_1);
                }
                else
                {
                    matcomp.SetTextureUnit("black", HTextureUnit.Unit_1);
                }

                if (matcomp.Material.Specular_map != string.Empty)
                {
                    if (!TextureResourceManager.Instance.HasResource(matcomp.Material.Specular_map))
                    {
                        TextureResourceManager.Instance.ImportResource(matcomp.Material.Specular_map, "textures", matcomp.Material.Specular_map);
                    }
                    matcomp.SetTextureUnit(matcomp.Material.Specular_map, HTextureUnit.Unit_2);
                }
                else
                {
                    matcomp.SetTextureUnit("black", HTextureUnit.Unit_2);
                }
            }

        }

        private void Initialize()
        {
            TextureResourceManager.Instance.ImportResource("black", "textures", "black.png");
            ShaderResourceManager.Instance.ImportResource("default_line", "shaders", "line_default.vert", "line_default.frag");
            ShaderResourceManager.Instance.ImportResource("default", "shaders", "default.vert", "default.frag");
            ShaderResourceManager.Instance.ImportResource("deferred_pre", "shaders", "deferred_pre.vert", "deferred_pre.frag");


            PrimaryCam.ViewSettings = new CameraViewSettings()
            {
                Lens_height = 1080,
                Lens_width = 1920,
                Fov = OpenTK.Mathematics.MathHelper.DegreesToRadians(45),
                clip_min = 1.0f,
                clip_max = 10000.0f
            };
            PrimaryCam.UpdateProjectionMatrix();
            PrimaryCam.UpdateViewMatrix();

        }

        private void SortSceneContent()
        {
            if (scene_content_changed)
            {
                scene_content_changed = false;
                deferred_objects.Clear();
                foreward_objects.Clear();
                light_objects.Clear();

                scene_content.ForEach((e) => {
                    if (e.HasComponent<RadialLightComponent>())
                        light_objects.Add(e);


                    if (e.HasComponent<MeshRenderComponent>())
                    {
                        deferred_objects.Add(e);
                    }

                    if (e.HasComponent<InterfaceRenderComponent>())
                    {
                        foreward_objects.Add(e);
                    }
                });
            }
        }

        private void RedrawFunc()
        {
            //PrimaryCam.FrameBuffer.Bind();
            //NativeWindow.GL.Clear((int)Silk.NET.OpenGL.GLEnum.ColorBufferBit);
            //NativeWindow.GL.Clear((int)Silk.NET.OpenGL.GLEnum.DepthBufferBit);
            //foreach (Entity en in deferred_objects)
            //{
            //    if (en.HasComponent<MeshComponent>())
            //    {
            //        MeshRenderComponent fwc = en.GetComponent<MeshRenderComponent>();
            //        fwc.Render(Camera.Primary);
            //    }
            //}
            //
            //Buffers.FrameBuffer.Unbind();
            //MeshRenderComponent.RenderOnCamPlane(this.PrimaryCam);
            //
            //PrimaryCam.FrameBuffer.Bind();
            //NativeWindow.GL.BlitFramebuffer(0, 0, (int)PrimaryCam.FrameBuffer.Width, (int)PrimaryCam.FrameBuffer.Height,
            //    0, 0, (int)PrimaryCam.FrameBuffer.Width, (int)PrimaryCam.FrameBuffer.Height,
            //    (uint)Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit, Silk.NET.OpenGL.BlitFramebufferFilter.Nearest);
            //Buffers.FrameBuffer.Unbind();
            foreach (Entity en in foreward_objects)
            {
                if (en.HasComponent<MeshComponent>())
                {
                    InterfaceRenderComponent fwc = en.GetComponent<InterfaceRenderComponent>();
                    fwc.Render(Camera.Primary);
                }
            }

            foreach (Entity en in scene_content)
            {
                if (en.HasComponent<LineRenderComponent>())
                {
                    LineRenderComponent linercomp = en.GetComponent<LineRenderComponent>();
                    if (linercomp != null)
                    {
                        linercomp.Render(Camera.Primary);
                    }
                }
            }
        }

        
    }
}
