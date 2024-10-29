#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace FGUFW
{
    [CustomEditor(typeof(AnimatorPlayHelper))]
    public class AnimatorPlayHelperEditor : Editor
    {
        private Animator _animator;
        private AnimatorPlayHelper _animatorPlayHelper;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            if(_animatorPlayHelper==default)
            {
                _animatorPlayHelper = target as AnimatorPlayHelper;
                _animator = _animatorPlayHelper.GetComponent<Animator>();
            }

            drawAnimPlayBtn();
        }

        private void drawAnimPlayBtn()
        {
            if(_animator.runtimeAnimatorController==default)return;

            var anims = _animator.runtimeAnimatorController.animationClips;

            foreach (var item in anims)
            {
                if(EditorApplication.isPlaying)
                {
                    if(GUILayout.Button(item.name))
                    {
                        _animator.Play(item.name,0,_animatorPlayHelper.NormalizedTime);
                    }
                }
                else
                {
                    GUILayout.Label(item.name);
                }
            }
        }
    }
}
#endif