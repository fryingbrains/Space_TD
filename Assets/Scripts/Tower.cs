using UnityEngine;

public class Tower : MonoBehaviour
{
    private int _damage;
    private int _range;
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    public int Range
    {
        get { return _range; }
        set { _range = value; }
    }

}
