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

        private AttributeStorage attributes;
        public Mesh()
        {
            this.Status = MeshStatus.INVALID;
            attributes = new AttributeStorage();

        }

        public void ImportFromFile(String folder_id, String file)
        {
            this.Status = MeshStatus.IMPORTING_DATA;
            string dir = DirectoryManager.GetResourceDir(folder_id);
            if(dir == String.Empty)
            {
                throw new Exception($"Directory with id [{folder_id}] was not found in DirectoryManager");
            }
            string path = DirectoryManager.ConcatDirFile(dir, file);

            Assimp.AssimpContext ac = new AssimpContext();
            ac.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene s = ac.ImportFile(path, PostProcessSteps.Triangulate);
            
            if(s.MeshCount == 0)
            {
                throw new Exception("No meshes were found in the scene");
            }

            this.Status = MeshStatus.PARSING_DATA;
            Assimp.Mesh mesh = s.Meshes[0];
            List<Vector3> vertex_data = new List<Vector3>();
            List<Vector3> normal_data = new List<Vector3>();
            List<Vector3> texture_data = new List<Vector3>();

            for(int i = 0; i < mesh.VertexCount; i++)
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
                t.X = mesh.TextureCoordinateChannels[0][i].X;
                t.Y = mesh.TextureCoordinateChannels[0][i].Y;
                t.Z = mesh.TextureCoordinateChannels[0][i].Z;

                vertex_data.Add(v);
                normal_data.Add(n);
                texture_data.Add(t);
            }
            FloatAttribute vertex_attrib = new FloatAttribute("Vertices", 3);
            vertex_attrib.AddData(vertex_data.ToArray());

            FloatAttribute normal_attrib = new FloatAttribute("Normals", 3);
            normal_attrib.AddData(normal_data.ToArray());

            FloatAttribute texture_attrib = new FloatAttribute("TexCoords", 3);
            texture_attrib.AddData(texture_data.ToArray());

            this.attributes.AddAttribute(vertex_attrib);
            this.attributes.AddAttribute(normal_attrib);
            this.attributes.AddAttribute(texture_attrib);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
