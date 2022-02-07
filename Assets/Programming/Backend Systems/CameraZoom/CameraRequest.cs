[System.Serializable]
public class CameraRequest<T>
{
    public T to;
    public float speed;
    public bool reset;

    public T lockValue;
    public bool requestLock;

    public bool global;

    public CameraRequest(T to, float speed, bool reset, bool global = true)
    {
        this.to = to;
        this.speed = speed;
        this.reset = reset;
        requestLock = false;
        this.global = global;
    }

    public void SetLock(T lockValue)
    {
        this.lockValue = lockValue;
        requestLock = true;
    }
}