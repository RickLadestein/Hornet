﻿using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;
using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using Silk.NET.OpenGL;

namespace HornetEngine.Ecs
{
    public class LineRenderComponent : Component
    {
        public List<vec3> Points { get; private set; }
        public vec4 Base_Color { get; set; }
        public float Line_width { get; set; }

        private AttributeStorage ats;
        private FloatAttribute points;


        private VertexBuffer vbuf;

        public LineRenderComponent()
        {
            Line_width = 1;
            Points = new List<vec3>();
            Base_Color = new vec4(1.0f, 1.0f, 1.0f, 0.75f);
            vbuf = new VertexBuffer();
            vbuf.SetPrimitiveType(ElementType.LINES);

            ats = new AttributeStorage();
            points = new FloatAttribute("Points", 3);
            ats.AddAttribute(points);
        }

        public void AddLine(vec3 start, vec3 end)
        {
            this.Points.Add(start);
            this.Points.Add(end);
        }

        public void ClearPoints()
        {
            this.Points.Clear();
        }

        public void BuildBuffer()
        {
            if(this.Points.Count % 2 != 0)
            {
                throw new Exception("Could not builf VertexBuffer for LineRenderer: ");
            }
            vbuf.DestroyBuffers();
            points.ClearData();


            points.AddData(Points.ToArray());
            vbuf.InitialiseBuffers();
            vbuf.BufferData(ats, ElementType.LINES);
        }

        public void Render(Camera cam)
        {
            ShaderProgram sh = ShaderResourceManager.GetInstance().GetResource("default_line");

            if (sh == null)
            {
                throw new Exception($"Could not render mesh for entity {this.parent.Id}: Shader was null");
            }

            if(sh.Status != ShaderProgramStatus.READY)
            {
                throw new Exception($"Could not render mesh for entity {this.parent.Id}: Shader was not ready");
            }

            if(vbuf.VertexCount == 0)
            {
                return;
            }

            NativeWindow.GL.LineWidth(MathF.Abs(Line_width));
            vbuf.Bind();

            sh.Bind();
            sh.SetUniform("model", parent.Transform.ModelMat);
            sh.SetUniform("normal_mat", parent.Transform.NormalMat);
            sh.SetUniform("projection", cam.ProjectionMatrix);
            sh.SetUniform("view", cam.ViewMatrix);
            sh.SetUniform("camera_position", cam.Position);
            sh.SetUniform("camera_target", cam.Target);
            sh.SetUniform("time", (float)NativeWindow.GLFW.GetTime());
            sh.SetUniform("base_color", Base_Color);

            NativeWindow.GL.DrawArrays((GLEnum)vbuf.PrimitiveType, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();
            NativeWindow.GL.LineWidth(1);
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
