public struct CompositeKey<T1, T2>
{
    public T1 Item1;
    public T2 Item2;
    public CompositeKey(T1 item1, T2 item2){
        this.Item1 = item1;
        this.Item2 = item2;
    }
    public bool IsNull(){
        return this.Item1 == null || this.Item2 == null;
    }
}