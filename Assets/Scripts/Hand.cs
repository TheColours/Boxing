using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private SphereCollider _collider;
    public Character Main;
    public void EnablePunch(bool IsOn)
    {
        _collider.enabled = IsOn;
    }
}
