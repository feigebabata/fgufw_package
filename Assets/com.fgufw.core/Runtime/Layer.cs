namespace FGUFW
{
    /// <summary>
    /// 并行层概念 0或不相等为并行
    /// </summary>
    public struct Layer
    {
        public const int None = 0;

        public int Value;

        public override bool Equals(object obj)
        {
            return (Layer)obj==this;
        }

        public override int GetHashCode()
        {
            return Value;
        }


        public static implicit operator int(Layer exists)
        {
            return exists.Value;
        }

        public static implicit operator Layer(int exists)
        {
            return new Layer{Value=exists};
        }

        public static bool operator == (Layer l, Layer r)
        {
            if(l==None)return false;
            if(r==None)return false;
            return l.Value == r.Value;
        }

        public static bool operator != (Layer l, Layer r)
        {
            if(l==None)return true;
            if(r==None)return true;
            return l.Value != r.Value;
        }

    }
}