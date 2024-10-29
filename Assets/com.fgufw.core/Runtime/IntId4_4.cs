namespace FGUFW
{
    public struct IntId4_4
    {
        public const int NoneValue = 0;
        public int Value{get;private set;}

        public ushort Group{get;private set;}

        public ushort Item{get;private set;}

        public IntId4_4(int value)
        {
            if(!IsId(value))
            {
                throw new System.Exception($"value:{value}必须大于1000_0000且小于1_0000_0000");
            }
            Value = value;
            Group = (ushort)(value/1_0000);
            Item = (ushort)(value-Group*1_0000);
        }
        
        public IntId4_4(int group,int item)
        {
            Value = group*1_0000+item;
            if(!IsId(Value))
            {
                throw new System.Exception($"value:{Value}必须大于1000_0000且小于1_0000_0000");
            }
            Group = (ushort)group;
            Item = (ushort)item;
        }

        public IntId4_4(string value)
        {
            if(!IsId(value))
            {
                throw new System.Exception($"value:{value}必须为8位正整数组成");
            }
            Value = int.Parse(value);
            Group = (ushort)(Value/1_0000);
            Item = (ushort)(Value-Group*1_0000);
        }

        public static implicit operator int(IntId4_4 exists)
        {
            return exists.Value;
        }

        public static implicit operator IntId4_4(int exists)
        {
            return new IntId4_4(exists);
        }

        public static bool IsId(float val)
        {
            return val%1f==0 && val>1000_0000 && val<1_0000_0000;
        }

        public static bool IsId(int val)
        {
            return val>1000_0000 && val<1_0000_0000;
        }

        public static bool IsId(string val)
        {
            if(val==null || val.Length!=8)return false;
            return int.TryParse(val,out _);
        }

        public override string ToString()
        {
            return $"{Group:D4}{Item:D4}";
        }

    }
}