using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorPlayHelper : MonoBehaviour
    {
        [Header("起始位置 百分比")]
        [Range(0,1)]
        public float NormalizedTime;
    }
}

