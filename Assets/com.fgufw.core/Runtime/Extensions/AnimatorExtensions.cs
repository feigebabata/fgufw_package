using UnityEngine;
using System.Collections;

namespace FGUFW
{
    public static class AnimatorExtensions
    {
        public static void ReplaceClip(this Animator self,AnimationClip clip)
        {
            AnimatorOverrideController overrideController = null;
            if(self.runtimeAnimatorController is AnimatorOverrideController)
            {
                overrideController = self.runtimeAnimatorController as AnimatorOverrideController;
            }
            else
            {
                overrideController = new AnimatorOverrideController(self.runtimeAnimatorController);
                self.runtimeAnimatorController = overrideController;
            }
            var clipName = clip.name;
            overrideController[clipName] = clip;
        }
        
    }
}