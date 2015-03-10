using UnityEngine;

public class SyncObject : MonoBehaviour
{
    private float _lastSynchronizationTime;
    private float _syncDelay;
    private float _syncTime;
    private Vector3 _syncStartPosition = Vector3.zero;
    private Vector3 _syncEndPosition = Vector3.zero;
    private Quaternion _q = Quaternion.identity;

    void Start()
    {

    }

    void Update()
    {

    }


    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        var syncPosition = Vector3.zero;
        var syncVelocity = Vector3.zero;

        if (stream.isWriting)
        {
            syncPosition = GetComponent<Rigidbody>().position;
            stream.Serialize(ref syncPosition);

            syncVelocity = GetComponent<Rigidbody>().velocity;
            stream.Serialize(ref syncVelocity);

            _q = GetComponent<Rigidbody>().rotation;
            stream.Serialize(ref _q);
            Serialize(stream, info);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref _q);

            _syncTime = 0f;
            _syncDelay = Time.time - _lastSynchronizationTime;
            _lastSynchronizationTime = Time.time;

            _syncEndPosition = syncPosition + syncVelocity * _syncDelay;
            _syncStartPosition = GetComponent<Rigidbody>().position;
            Deserialize(stream, info);
        }
    }

    void FixedUpdate()
    {
        if (GetComponent<NetworkView>().isMine || Consts.IsSinglePlayer)
        {
            MyUpdate();
        }
        else
        {
            _syncTime += Time.deltaTime;
            GetComponent<Rigidbody>().position = Vector3.Lerp(_syncStartPosition, _syncEndPosition, _syncTime / _syncDelay);
            GetComponent<Rigidbody>().rotation = _q;
        }
    }

    public virtual void MyUpdate()
    {
    }

    public virtual void Serialize(BitStream stream, NetworkMessageInfo info)
    {
    }

    public virtual void Deserialize(BitStream stream, NetworkMessageInfo info)
    {
    }

}
