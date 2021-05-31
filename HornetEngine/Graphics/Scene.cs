﻿using HornetEngine.Ecs;
using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;

namespace HornetEngine.Graphics
{
    public class Scene
    {
        public List<Entity> scene_content;
        private List<Entity> normal_objects;
        private List<Entity> transperant_objects;

        public List<Entity> SortByDistanceToCam(Camera cam)
        {
            Entity[] entities = transperant_objects.ToArray();
            Array.Sort(entities, (item1, item2) => {
                float distance_to_item1 = glm.DistanceSqr(cam.Position, item1.Transform.Position);
                float distance_to_item2 = glm.DistanceSqr(cam.Position, item2.Transform.Position);

                if(distance_to_item1 > distance_to_item2)
                {
                    return 1;
                } else if(distance_to_item1 < distance_to_item2)
                {
                    return -1;
                } else
                {
                    return 0;
                }
            });
            return new List<Entity>(entities);
        }
    }
}