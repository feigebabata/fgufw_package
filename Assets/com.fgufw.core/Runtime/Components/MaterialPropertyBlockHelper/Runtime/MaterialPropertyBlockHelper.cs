using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FGUFW.Flipbook
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class MaterialPropertyBlockHelper : MonoBehaviour
    {
        private Renderer _renderer;
        public Renderer Render
        {
            get
            {
                if(!(_renderer is UnityEngine.Object) || _renderer==null)
                {
                    _renderer = GetComponent<Renderer>();
                }
                return _renderer;
            }
        }

        private MaterialPropertyBlock _materialPropertyBlock;
        public MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if(_materialPropertyBlock==null)
                {
                    _materialPropertyBlock = new MaterialPropertyBlock();
                }
                return _materialPropertyBlock;
            }
        }

        
        [Header("属性名 例:_MainTex")]

        [Header("纹理属性:")]
        public List<TextureProperty> Textures = new List<TextureProperty>();

        [Header("颜色属性: 自动填充会把HDR的颜色也放过来 需手动修正")]
        public List<ColorProperty> Colors = new List<ColorProperty>();

        [Header("HDR颜色属性:")]
        public List<HDRColorProperty> HDRColors = new List<HDRColorProperty>();

        [Header("数值属性: bool类型整型也用这个")]
        public List<FloatProperty> Floats = new List<FloatProperty>();

        [Header("向量属性: 二维三维四维都用这个")]
        public List<VectorProperty> Vectors = new List<VectorProperty>();



        #if UNITY_EDITOR
        [Header("----------------------")]
        [Header("修改参数后自动应用 可能会导致卡顿!")]
        public bool EditorAutoApple = false;
        #endif

        private void Reset() 
        {
            Apply();
        }

        private void Awake() 
        {
            Apply();
        }

        public void EditorApple()
        {
            GeneratePropertyID();
            Apply();
        }

        public void GeneratePropertyID()
        {
            foreach (var item in Textures)
            {
                item.PropertyID = Shader.PropertyToID(item.PropertyName);
            }
            foreach (var item in Colors)
            {
                item.PropertyID = Shader.PropertyToID(item.PropertyName);
            }
            foreach (var item in Floats)
            {
                item.PropertyID = Shader.PropertyToID(item.PropertyName);
            }
            foreach (var item in Vectors)
            {
                item.PropertyID = Shader.PropertyToID(item.PropertyName);
            }
            foreach (var item in HDRColors)
            {
                item.PropertyID = Shader.PropertyToID(item.PropertyName);
            }
        }


        /// <summary>
        /// 如果修改或增加PropertyName 需要调用GeneratePropertyID
        /// </summary>
        public void Apply()
        {
            Render.GetPropertyBlock(PropertyBlock);

            foreach (var item in Textures)
            {
                if(item.Value==null || !(item.Value is UnityEngine.Object))continue;
                PropertyBlock.SetTexture(item.PropertyID,item.Value);
            }

            foreach (var item in Colors)
            {
                PropertyBlock.SetColor(item.PropertyID,item.Value);
            }

            foreach (var item in HDRColors)
            {
                PropertyBlock.SetColor(item.PropertyID,item.Value);
            }

            foreach (var item in Floats)
            {
                PropertyBlock.SetFloat(item.PropertyID,item.Value);
            }

            foreach (var item in Vectors)
            {
                PropertyBlock.SetVector(item.PropertyID,item.Value);
            }

            Render.SetPropertyBlock(PropertyBlock);

        }

        [Serializable]
        public class TextureProperty
        {
            // //[HideInInspector]
            public int PropertyID;

            public string PropertyName;
            public Texture Value;
        }

        [Serializable]
        public class FloatProperty
        {
            //[HideInInspector]
            public int PropertyID;
            
            public string PropertyName;

            public float Value;
        }

        [Serializable]
        public class ColorProperty
        {
            //[HideInInspector]
            public int PropertyID;
            
            public string PropertyName;
        
            public Color Value;
        }

        [Serializable]
        public class HDRColorProperty
        {
            //[HideInInspector]
            public int PropertyID;
            
            public string PropertyName;

            [ColorUsage(true,true)]            
            public Color Value;
        }

        [Serializable]
        public class VectorProperty
        {
            //[HideInInspector]
            public int PropertyID;
            
            public string PropertyName;
            public Vector4 Value;
        }

    }
}
