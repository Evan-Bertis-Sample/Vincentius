[System.Serializable]
public class CameraRequest<T>
{
    public T to;
    public float speed;
    public bool reset;

    public T lockValue;
    public bool requestLock;

    public bool global;
    public int priority;

    public CameraRequest(T to, float speed, bool reset, bool global = true, int priority = 1)
    {
        this.to = to;
        this.speed = speed;
        this.reset = reset;
        requestLock = false;
        this.global = global;
        this.priority = priority;
    }

    public void SetLock(T lockValue)
    {
        this.lockValue = lockValue;
        requestLock = true;
    }
}