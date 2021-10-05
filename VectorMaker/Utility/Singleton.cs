namespace VectorMaker
{
    internal interface Singleton<T>
    {
        T Instance {
            get;
            set;
        }
    }
}