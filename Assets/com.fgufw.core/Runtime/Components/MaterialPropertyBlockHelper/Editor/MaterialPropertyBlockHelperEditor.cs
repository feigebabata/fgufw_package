#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering;

namespace FGUFW.Flipbook.Editor
{
    [CustomEditor(typeof(MaterialPropertyBlockHelper))]
    public class MaterialPropertyBlockHelperEditor : UnityEditor.Editor
    {
        private MaterialPropertyBlockHelper _target;

        private void OnEnable() 
        {
            _target = target as MaterialPropertyBlockHelper;
            _target.EditorApple();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(_target.Render.sharedMaterial==null || !(_target.Render.sharedMaterial is UnityEngine.Object))return;

            if(_target.EditorAutoApple)
            {
                _target.EditorApple();
            }

            if(GUILayout.Button("应用"))
            {
                _target.EditorApple();
            }

            if(GUILayout.Button("填充所有属性  (非\"unity_\"开头的)"))
            {
                setAllPropertyData();
            }

            if(GUILayout.Button("清空"))
            {
                clear();
            }

        }

        private void clearTargetPropertyList()
        {
            _target.Textures.Clear();
            _target.Colors.Clear();
            _target.HDRColors.Clear();
            _target.Floats.Clear();
            _target.Vectors.Clear();

            _target.EditorApple();
        }

        private void clear()
        {
            setAllPropertyData();
            clearTargetPropertyList();
        }

        private void setAllPropertyData()
        {
            clearTargetPropertyList();

            var mat = _target.Render.sharedMaterial;
            var shader = mat.shader;
            var length = shader.GetPropertyCount();

            for (int i = 0; i < length; i++)
            {
                var name = shader.GetPropertyName(i);

                //过滤非自定义属性
                if(name.IndexOf("unity_")==0)
                {
                    continue;
                }

                var propertyType = shader.GetPropertyType(i);

                switch (propertyType)
                {
                    case ShaderPropertyType.Int:
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                    {
                        _target.Floats.Add(new MaterialPropertyBlockHelper.FloatProperty
                        {
                            PropertyName = name,
                            Value = mat.GetFloat(name)
                        });
                    }
                    break;
                    case ShaderPropertyType.Texture:
                    {
                        _target.Textures.Add(new MaterialPropertyBlockHelper.TextureProperty
                        {
                            PropertyName = name,
                            Value = mat.GetTexture(name)
                        });
                    }
                    break;
                    case ShaderPropertyType.Color:
                    {
                        _target.Colors.Add(new MaterialPropertyBlockHelper.ColorProperty
                        {
                            PropertyName = name,
                            Value = mat.GetColor(name)
                        });
                    }
                    break;
                    case ShaderPropertyType.Vector:
                    {
                        _target.Vectors.Add(new MaterialPropertyBlockHelper.VectorProperty
                        {
                            PropertyName = name,
                            Value = mat.GetVector(name)
                        });
                    }
                    break;
                }
            }

            _target.EditorApple();
        }
    }
}
#endif