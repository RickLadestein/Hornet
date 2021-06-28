using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Assimp;
using Assimp.Configs;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using OpenTK.Mathematics;
using GlmSharp;

namespace HornetEngine.Graphics
{
    public class Mesh : IDisposable
    {
        /// <summary>
        /// The status of the mesh
        /// </summary>
        public MeshStatus Status { get; private set; }

        /// <summary>
        /// The vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer { get; private set; }

        /// <summary>
        /// The material
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// The attributes
        /// </summary>
        public AttributeStorage Attributes { get; private set; }

        /// <summary>
        /// The name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// The error message
        /// </summary>
        public String Error { get; private set; }

        /// <summary>
        /// The constructor of the mesg
        /// </summary>
        /// <param name="name">The name of the mesh</param>
        public Mesh(String name)
        {
            this.Status = MeshStatus.INVALID;
            this.VertexBuffer = new VertexBuffer();
            Attributes = new AttributeStorage();
            this.Name = name;
            this.Error = string.Empty;
            this.Material = new Material();
        }

        /// <summary>
        /// A function which can be used to build the vertex buffers
        /// </summary>
        public void BuildVertexBuffer()
        {
            VertexBuffer.InitialiseBuffers();
            VertexBuffer.BufferData(Attributes, ElementType.TRIANGLES);
            this.Status = MeshStatus.READY;
        }

        /// <summary>
        /// A function which can be used to import a mesh
        /// </summary>
        /// <param name="name">The name of the mesh</param>
        /// <param name="folder_id">The ID of the folder</param>
        /// <param name="file">The name of the file</param>
        /// <returns>A mesh</returns>
        public static Mesh ImportMesh(String name, String folder_id, String file)
        {
            
            Mesh output = new Mesh(name);

            output.Status = MeshStatus.IMPORTING_DATA;
            output.Error = Mesh.ImportMeshFromFile(folder_id, file, out Assimp.Mesh mesh, out Material mat);
            if(output.Error != String.Empty)
            {
                return output;
            }
            output.Material = mat;
            

            output.Status = MeshStatus.PARSING_DATA;
            ParseMeshData(output, mesh);
            if(output.Attributes.Count == 0)
            {
                return output;
            }

            output.Status = MeshStatus.CREATING_BUFFER;
            output.BuildVertexBuffer();
            if (output.VertexBuffer.Error.Length > 0)
            {
                return output;
            }
            output.Status = MeshStatus.READY;
            return output;
        }

        /// <summary>
        /// A function which can be used to import a mesh
        /// </summary>
        /// <param name="mesh">The mesh</param>
        /// <param name="mat">The material</param>
        /// <returns>A mesh</returns>
        public static Mesh ImportMesh(Assimp.Mesh mesh, Assimp.Material mat)
        {
            if(mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            Mesh output = new Mesh(mesh.Name);
            output.Status = MeshStatus.PARSING_DATA;
            ParseMeshData(output, mesh);
            if (output.Attributes.Count == 0)
            {
                return output;
            }

            if (mat != null)
            {
                Assimp.Material _amat = mat;
                output.Material = new Material()
                {
                    Ambient = _amat.HasColorAmbient == true ? new vec3(_amat.ColorAmbient.R, _amat.ColorAmbient.G, _amat.ColorAmbient.B) : new vec3(0),
                    Diffuse = _amat.HasColorDiffuse == true ? new vec3(_amat.ColorDiffuse.R, _amat.ColorDiffuse.G, _amat.ColorDiffuse.B) : new vec3(0),
                    Specular = _amat.HasColorSpecular == true ? new vec3(_amat.ColorSpecular.R, _amat.ColorSpecular.G, _amat.ColorSpecular.B) : new vec3(0),
                    ShinyFactor = _amat.HasShininess == true ? _amat.Shininess : 0,

                    Ambient_Texture = _amat.HasTextureAmbient == true ? _amat.TextureAmbient.FilePath : "",
                    Diffuse_Texture = _amat.HasTextureDiffuse == true ? _amat.TextureDiffuse.FilePath : "",
                    Specular_Texture = _amat.HasTextureSpecular == true ? _amat.TextureSpecular.FilePath : ""
                };
            }


            output.Status = MeshStatus.CREATING_BUFFER;
            output.BuildVertexBuffer();
            if (output.VertexBuffer.Error.Length > 0)
            {
                return output;
            }
            output.Status = MeshStatus.READY;
            return output;
        }

        /// <summary>
        /// A function which can import a mesh from a file
        /// </summary>
        /// <param name="folder_id">The ID of the folder</param>
        /// <param name="file">The name of the file</param>
        /// <param name="mesh">The mesg</param>
        /// <param name="mat">The material</param>
        /// <returns></returns>
        private static String ImportMeshFromFile(String folder_id, String file, out Assimp.Mesh mesh, out Material mat)
        {
            string dir = DirectoryManager.GetResourceDir(folder_id);
            mesh = null;
            mat = new Material();

            if (dir == String.Empty)
            {
                return $"Directory with id [{folder_id}] was not found in DirectoryManager";
            }
            string path = DirectoryManager.ConcatDirFile(dir, file);

            try
            {
                Assimp.AssimpContext ac = new AssimpContext();
                ac.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                Assimp.Scene s = ac.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.CalculateTangentSpace);

                if (s.MeshCount == 0)
                {
                    return "No meshes were found in the file";
                }

                if(s.MeshCount > 1)
                {
                    return "Scene detected: please only load single mesh files";
                }
                mesh = s.Meshes[0];
                if (mesh.MaterialIndex != -1)
                {
                    Assimp.Material _amat = s.Materials[mesh.MaterialIndex];
                    mat = new Material()
                    {
                        Ambient = _amat.HasColorAmbient == true ? new vec3(_amat.ColorAmbient.R, _amat.ColorAmbient.G, _amat.ColorAmbient.B) : new vec3(0),
                        Diffuse = _amat.HasColorDiffuse == true ? new vec3(_amat.ColorDiffuse.R, _amat.ColorDiffuse.G, _amat.ColorDiffuse.B) : new vec3(0),
                        Specular = _amat.HasColorSpecular == true ? new vec3(_amat.ColorSpecular.R, _amat.ColorSpecular.G, _amat.ColorSpecular.B) : new vec3(0),
                        ShinyFactor = _amat.HasShininess == true ? _amat.Shininess : 0,

                        Ambient_Texture = _amat.HasTextureAmbient == true ? _amat.TextureAmbient.FilePath : "",
                        Diffuse_Texture = _amat.HasTextureDiffuse == true ? _amat.TextureDiffuse.FilePath : "",
                        Specular_Texture = _amat.HasTextureSpecular == true ? _amat.TextureSpecular.FilePath : ""
                    };
                }
                return String.Empty;
            } catch(Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// A function which can parse the mesh data
        /// </summary>
        /// <param name="obj">The mesh object</param>
        /// <param name="mesh">The mesh</param>
        private static void ParseMeshData(HornetEngine.Graphics.Mesh obj, Assimp.Mesh mesh)
        {
            obj.Attributes.ClearAttributes();
            List<Vector3> vertex_data = new List<Vector3>();
            List<Vector3> normal_data = new List<Vector3>();
            List<Vector3> texture_data = new List<Vector3>();
            List<Vector3> tangent_data = new List<Vector3>();
            List<Vector3> bitangent_data = new List<Vector3>();

            bool hastex = mesh.HasTextureCoords(0);

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3 v;
                v.X = mesh.Vertices[i].X;
                v.Y = mesh.Vertices[i].Y;
                v.Z = mesh.Vertices[i].Z;

                Vector3 n;
                n.X = mesh.Normals[i].X;
                n.Y = mesh.Normals[i].Y;
                n.Z = mesh.Normals[i].Z;

                Vector3 t;
                if (hastex)
                {
                    t.X = mesh.TextureCoordinateChannels[0][i].X;
                    t.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    t.Z = 0.0f;
                }
                else
                {
                    t.X = 0.0f;
                    t.Y = 0.0f;
                    t.Z = 0.0f;
                }

                Vector3 tangent = new Vector3();
                Vector3 bitangent = new Vector3();

                if(mesh.HasTangentBasis)
                {
                    tangent.X = mesh.Tangents[i].X;
                    tangent.Y = mesh.Tangents[i].Y;
                    tangent.Z = mesh.Tangents[i].Z;

                    bitangent.X = mesh.BiTangents[i].X;
                    bitangent.Y = mesh.BiTangents[i].Y;
                    bitangent.Z = mesh.BiTangents[i].Z;
                }

                vertex_data.Add(v);
                normal_data.Add(n);
                texture_data.Add(t);
                tangent_data.Add(tangent);
                bitangent_data.Add(bitangent);

            }

            FloatAttribute vertex_attrib = new FloatAttribute("Vertices", 3);
            vertex_attrib.AddData(vertex_data.ToArray());

            FloatAttribute normal_attrib = new FloatAttribute("Normals", 3);
            normal_attrib.AddData(normal_data.ToArray());

            FloatAttribute texture_attrib = new FloatAttribute("TexCoords", 3);
            texture_attrib.AddData(texture_data.ToArray());

            FloatAttribute tangent_attrib = new FloatAttribute("NormalTangents", 3);
            tangent_attrib.AddData(tangent_data.ToArray());

            FloatAttribute bitangent_attrib = new FloatAttribute("NormalBiTangents", 3);
            bitangent_attrib.AddData(bitangent_data.ToArray());

            obj.Attributes.AddAttribute(vertex_attrib);
            obj.Attributes.AddAttribute(normal_attrib);
            obj.Attributes.AddAttribute(texture_attrib);
            obj.Attributes.AddAttribute(tangent_attrib);
            obj.Attributes.AddAttribute(bitangent_attrib);
        }

        /// <summary>
        /// A function which can disppose of the attributes
        /// </summary>
        public void Dispose()
        {
            this.Attributes.ClearAttributes();
        }
    }

    public struct Material
    {
        public GlmSharp.vec3 Ambient;
        public GlmSharp.vec3 Diffuse;
        public GlmSharp.vec3 Specular;
        
        public String Ambient_Texture;
        public String Diffuse_Texture;
        public String Specular_Texture;
        
        public float ShinyFactor;
    }

    public enum MeshStatus
    {
        INVALID,
        IMPORTING_DATA,
        PARSING_DATA,
        CREATING_BUFFER,
        READY
    }
}
