using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Assimp;
using Assimp.Configs;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using OpenTK.Mathematics;

namespace HornetEngine.Graphics
{
    public class Mesh : IDisposable
    {
        public MeshStatus Status { get; private set; }

        public VertexBuffer VertexBuffer { get; private set; }

        public AttributeStorage Attributes { get; private set; }

        public String Name { get; private set; }

        public String Error { get; private set; }
        public Mesh(String name)
        {
            this.Status = MeshStatus.INVALID;
            this.VertexBuffer = new VertexBuffer();
            Attributes = new AttributeStorage();
            this.Name = name;
            this.Error = string.Empty;
        }

        public void BuildVertexBuffer()
        {
            VertexBuffer.InitialiseBuffers();
            VertexBuffer.BufferData(Attributes, ElementType.TRIANGLES);
        }


        public static Mesh ImportMesh(String name, String folder_id, String file)
        {
            
            Mesh output = new Mesh(name);

            output.Status = MeshStatus.IMPORTING_DATA;
            output.Error = Mesh.ImportMeshFromFile(folder_id, file, out Assimp.Mesh mesh);
            if(output.Error != String.Empty)
            {
                return output;
            }

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

        public static Mesh ImportMesh(Assimp.Mesh mesh)
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

            output.Status = MeshStatus.CREATING_BUFFER;
            output.BuildVertexBuffer();
            if (output.VertexBuffer.Error.Length > 0)
            {
                return output;
            }
            output.Status = MeshStatus.READY;
            return output;
        }

        private static String ImportMeshFromFile(String folder_id, String file, out Assimp.Mesh mesh)
        {
            string dir = DirectoryManager.GetResourceDir(folder_id);
            mesh = null;
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
                return String.Empty;
            } catch(Exception ex)
            {
                return ex.Message;
            }
        }

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
        public void Dispose()
        {
            this.Attributes.ClearAttributes();
        }
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
