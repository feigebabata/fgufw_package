using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    public static class ColorExtensions
    {
        public static string RichText(this Color color,string text)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }

        public static Color ToHSV(this Color self)
        {
            float h,s,v;
            Color.RGBToHSV(self,out h,out s,out v);
            return new Color(h,s,v,self.a);
        }

        public static Color HSV2RGB(this Color self)
        {
            var rgb = Color.HSVToRGB(self.r,self.g,self.b);
            rgb.a = self.a;
            return rgb;
        }

        public static Color32 ToHSV32(this Color32 self)
        {
            float h,s,v;
            Color.RGBToHSV(self,out h,out s,out v);
            return new Color(h,s,v,self.a);
        }

        public static int ToARGBInt(this Color32 self)
        {
            return (255 << 24) | ((self.r & 255) << 16) | ((self.g & 255) << 8) | (self.b & 255);
        }

        public static Color32 FromARGBInt(int argb)
        {
            var color = new Color32();
            color.a = (byte)(argb>>24);
            color.r = (byte)(argb>>16);
            color.g = (byte)(argb>>8);
            color.b = (byte)argb;
            return color;
        }

        public static Color Lerp3(Color s,Color e,float t)
        {
            Vector3 v3 = Vector3.Lerp(new Vector3(s.r,s.g,s.b),new Vector3(e.r,e.g,e.b),t);
            return new Color(v3.x,v3.y,v3.z,s.a);
        }

        public static Color Lerp(Color s,Color e,float t)
        {
            Vector4 v4 = Vector4.Lerp(new Vector4(s.r,s.g,s.b,s.a),new Vector4(e.r,e.g,e.b,e.a),t);
            return new Color(v4.x,v4.y,v4.z,v4.w);
        }

    }
}
