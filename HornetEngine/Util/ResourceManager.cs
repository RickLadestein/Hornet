using HornetEngine.Graphics;
using HornetEngine.Sound;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace HornetEngine.Util
{
    /// <summary>
    /// Manager base class for managing and storing resources
    /// </summary>
    public abstract class ResourceManager<T> where T : class
    {
        private Dictionary<String, T> resources;
        private Mutex access_mutex;

        /// <summary>
        /// Instantiates a new instance of ResourceManager with default attributes
        /// </summary>
        public ResourceManager()
        {
            resources = new Dictionary<string, T>();
            access_mutex = new Mutex();
        }

        /// <summary>
        /// Checks if the collection contains a resource with given identifier
        /// </summary>
        /// <param name="identifier">The resource identifier</param>
        /// <returns></returns>
        public bool HasResource(string identifier)
        {
            access_mutex.WaitOne();
            bool found = resources.ContainsKey(identifier);
            access_mutex.ReleaseMutex();
            return found;
        }

        /// <summary>
        /// Adds a new resource with matching identifier to the collection
        /// </summary>
        /// <param name="identifier">The identifier for the resource</param>
        /// <param name="resource">The resource</param>
        /// <returns>true if resource was added, false if resource already existed under that specific identifier</returns>
        public bool AddResource(String identifier, T resource)
        {
            if(resource == null)
            {
                throw new ArgumentException("Resource cannot be null");
            }

            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool succes = resources.TryAdd(identifier, resource);
            access_mutex.ReleaseMutex();
            if (succes)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Gets a resource at specified identifier
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <returns>Resource if found, returns null if not found</returns>
        public T GetResource(String identifier)
        {
            if(identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool succes = resources.TryGetValue(identifier, out T res);
            if (succes)
            {
                access_mutex.ReleaseMutex();
                return res;
            } else
            {
                access_mutex.ReleaseMutex();
                return null;
            }
        }

        /// <summary>
        /// Sets a the resource at specified identifier if the collection already contains the identifier
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <param name="resource">The resource to be set at identifier</param>
        /// <returns>true if resource was set, false if the identifier is foreign to the collection</returns>
        public bool SetResource(String identifier, T resource)
        {
            if (resource == null)
            {
                throw new ArgumentException("Resource cannot be null");
            }

            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool found = resources.ContainsKey(identifier);
            if(found)
            {
                access_mutex.ReleaseMutex();
                resources[identifier] = resource;
                return true;
            } else
            {
                access_mutex.ReleaseMutex();
                throw new Exception("Could not add resource to identifier because manager does not contain identifier");
            }
        }

        /// <summary>
        /// Deletes resource with specified identifier from the collection
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <returns>true if resource was deleted, false if the resource was not found within the list</returns>
        public bool DeleteResource(String identifier)
        {
            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }
            access_mutex.WaitOne();
            bool success = resources.Remove(identifier);
            access_mutex.ReleaseMutex();
            return success;
        }
    }

    /// <summary>
    /// Manager class for managing and storing Mesh resources
    /// </summary>
    public class MeshResourceManager : ResourceManager<Mesh>
    {
        private static MeshResourceManager instance;
        private static object _lck = new object();

        /// <summary>
        /// The current instance of MeshResourceManager
        /// </summary>
        public static MeshResourceManager Instance 
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new MeshResourceManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Adds Mesh resource to the manager
        /// </summary>
        /// <param name="identifier">The identifier the Mesh is stored under</param>
        /// <param name="resource">The resource to be managed and stored</param>
        public new void AddResource(String identifier, Mesh resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException("Mesh");
            }
            if(resource.Status != MeshStatus.READY)
            {
                throw new Exception($"Could not add resource: Mesh creation failed or is not done yet! \n Error_str: {resource.Error}");
            }
            base.AddResource(identifier, resource);
        }


        /// <summary>
        /// Tries to import a Mesh resource
        /// </summary>
        /// <param name="identifier">The identifier that the mesh is stored under</param>
        /// <param name="folder_id">The folder id of the folder containing the mesh</param>
        /// <param name="object_file">The mesh resource file</param>
        public void ImportResource(string identifier, string folder_id, string object_file)
        {
            Mesh m = Mesh.ImportMesh(identifier, folder_id, object_file);
            if(m.Error.Length != 0 || m.Status != MeshStatus.READY)
            {
                throw new Exception($"Failed to import mesh resource. status: {m.Status}. error: {m.Error}");
            } else
            {
                this.AddResource(identifier, m);
            }
        }

        private MeshResourceManager() : base() { }
    }

    /// <summary>
    /// Manager class for managing and storing Shader resources
    /// </summary>
    public class ShaderResourceManager : ResourceManager<ShaderProgram>
    {
        private static ShaderResourceManager instance;
        private static object _lck = new object();

        /// <summary>
        /// The current instance of ShaderResourceManager
        /// </summary>
        public static ShaderResourceManager Instance
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new ShaderResourceManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Tries to import a Shader resource
        /// </summary>
        /// <param name="identifier">The identifier that the shader program is stored under</param>
        /// <param name="folder_id">The folder id of the folder containing the shaders</param>
        /// <param name="vert_file">The vertex shader file</param>
        /// <param name="frag_file">The fragment shader file</param>
        public void ImportResource(string identifier, string folder_id, string vert_file, string frag_file)
        {
            VertexShader vs = new VertexShader(folder_id, vert_file);
            FragmentShader fs = new FragmentShader(folder_id, frag_file);
            if (vs.Error.Length != 0)
            {
                throw new Exception($"Failed to import vertex shader resource: {vs.Error}");
            }

            if(fs.Error.Length != 0)
            {
                throw new Exception($"Failed to import fragment shader resource: {vs.Error}");
            }

            ShaderProgram shp = new ShaderProgram(vs, fs);
            if(shp.Status != ShaderProgramStatus.READY)
            {
                throw new Exception($"ShaderProgram error status: {shp.Status}");
            }
            this.AddResource(identifier, shp);
        }

        /// <summary>
        /// Tries to import a Shader resource
        /// </summary>
        /// <param name="identifier">The identifier that the shader program is stored under</param>
        /// <param name="folder_id">The folder id of the folder containing the shaders</param>
        /// <param name="vert_file">The vertex shader file</param>
        /// <param name="geo_file">The geometry shader file</param>
        /// <param name="frag_file">The fragment shader file</param>
        public void ImportResource(string identifier, string folder_id, string vert_file, string geo_file, string frag_file)
        {
            VertexShader vs = new VertexShader(folder_id, vert_file);
            GeometryShader gs = new GeometryShader(folder_id, geo_file);
            FragmentShader fs = new FragmentShader(folder_id, frag_file);
            if (vs.Error.Length != 0)
            {
                throw new Exception($"Failed to import vertex shader resource: {vs.Error}");
            }

            if (gs.Error.Length != 0)
            {
                throw new Exception($"Failed to import geometry shader resource: {vs.Error}");
            }

            if (fs.Error.Length != 0)
            {
                throw new Exception($"Failed to import fragment shader resource: {vs.Error}");
            }

            ShaderProgram shp = new ShaderProgram(vs, gs, fs);
            if (shp.Status != ShaderProgramStatus.READY)
            {
                throw new Exception($"ShaderProgram error status: {shp.Status}");
            }
            this.AddResource(identifier, shp);
        }
        private ShaderResourceManager() : base() { }
    }

    /// <summary>
    /// Manager class for managing and storing Texture resources
    /// </summary>
    public class TextureResourceManager : ResourceManager<Texture>
    {
        private static TextureResourceManager instance;
        private static object _lck = new object();

        /// <summary>
        /// The current instance of TextureResourceManager
        /// </summary>
        public static TextureResourceManager Instance
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new TextureResourceManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Tries to import a Texture resource
        /// </summary>
        /// <param name="identifier">The identifier that the texture is stored under</param>
        /// <param name="folder_id">The folder id of the folder containing the texture image</param>
        /// <param name="tex_file">The texture image file</param>
        public void ImportResource(string identifier, string folder_id, string tex_file)
        {
            Texture tex = new Texture(folder_id, tex_file, false);
            if(tex.Error.Length != 0 || tex.Status != TextureStatus.READY)
            {
                throw new Exception($"Failed to import texture resource:{tex.Error}");
            }
            this.AddResource(identifier, tex);
        }

        private TextureResourceManager() : base() { }
    }


    /// <summary>
    /// Manager class for managing and storing Sound resources
    /// </summary>
    public class SoundResourceManager : ResourceManager<Sample>
    {
        private static SoundResourceManager instance;
        private static object _lck = new object();

        /// <summary>
        /// The current instance of TextureResourceManager
        /// </summary>
        public static SoundResourceManager Instance
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new SoundResourceManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Instantiates a new instance of SoundResourceManager with default attributes
        /// </summary>
        private unsafe SoundResourceManager()
        {
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int*)null);
            ALC.MakeContextCurrent(context);
        }

        /// <summary>
        /// Tries to import a sound resource
        /// </summary>
        /// <param name="identifier">The identifier that the sound sample is stored under</param>
        /// <param name="folder_id">The folder id of the folder containing the sound sample</param>
        /// <param name="sound_file">The sound file</param>
        public void ImportResource(string identifier, string folder_id, string sound_file)
        {
            Sample samp = new Sample(folder_id, sound_file);
            this.AddResource(identifier, samp);
        }

        
    }
}
