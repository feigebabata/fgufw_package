using System;
using UnityEngine;

namespace FGUFW
{
    public static class MeshHelper
    {
        /// <summary>
        /// 创建2D的Mesh
        /// </summary>
        /// <param name="size">尺寸</param>
        /// <param name="pivot">轴点</param>
        /// <returns></returns>
        public static Mesh CreateQuad(Vector2 size,Vector2 pivot)
        {
            /*  
                23
                01
            */
            Vector3 v_offset = -VectorHelper.Multiply(size,pivot);
            Mesh mesh = new Mesh
            {
                vertices = new Vector3[]
                {
                    Vector3.zero+v_offset,
                    Vector3.right*size.x+v_offset,
                    Vector3.up*size.y+v_offset,
                    new Vector3(size.x,size.y)+v_offset
                },
                triangles = new int[]{0,3,1,3,0,2},
                normals = new Vector3[]{Vector3.back,Vector3.back,Vector3.back,Vector3.back},
                uv = new Vector2[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one},
            };

            return mesh;
        }

        /// <summary>
        /// 拖尾mesh 目型结构 轴点(0.5,0)
        /// </summary>
        /// <param name="layerCount"></param>
        /// <returns></returns>
        public static Mesh CreateTrail(int layerCount)
        {
            if(layerCount<1)layerCount=1;

            float offset_x = 0.5f;
            float offset_y = 1f/layerCount;
            int verticesLength = (layerCount+1)*2;

            var vertices = new Vector3[verticesLength];
            var normals = new Vector3[verticesLength];
            var uvs = new Vector2[verticesLength];
            var triangles = new int[layerCount*6];

            for (int layer = 0; layer <= layerCount; layer++)
            {
                int idx_l = layer*2;
                int idx_r = idx_l+1;

                vertices[idx_l] = new Vector3(-offset_x,layer*offset_y,0);
                vertices[idx_r] = new Vector3(offset_x,layer*offset_y,0);

                normals[idx_l] = Vector3.back;
                normals[idx_r] = Vector3.back;
                
                uvs[idx_l] = new Vector2(0,(float)layer/layerCount);
                uvs[idx_r] = new Vector2(1,(float)layer/layerCount);
            }

            for (int layer = 0; layer < layerCount; layer++)
            {
                int t_idx = layer*6;
                int v_idx = layer*2;
                triangles[t_idx++] = v_idx;
                triangles[t_idx++] = v_idx+3;
                triangles[t_idx++] = v_idx+1;
                triangles[t_idx++] = v_idx+3;
                triangles[t_idx++] = v_idx;
                triangles[t_idx++] = v_idx+2;
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
                normals = normals
            };

            return mesh;
        }

    }
}