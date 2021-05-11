using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Assimp;
using Assimp.Configs;
using System.Numerics;

namespace HornetEngine.Graphics
{
    public class Mesh : IDisposable
    {
        public MeshStatus status { get; private set; }
        public Mesh()
        {
            this.status = MeshStatus.INVALID;

        }

        public void ImportFromFile(Uri location)
        {
            Assimp.AssimpContext ac = new AssimpContext();
            ac.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene s = ac.ImportFile(location.AbsolutePath, PostProcessSteps.Triangulate);
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
        IMPORT_FAILED,
        PARSING_DATA,
        PARSING_FAILED,
        CREATING_BUFFER,
        BUFFERING_FAILED,
        READY
    }
}
