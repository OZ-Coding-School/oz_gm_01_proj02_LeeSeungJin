using System.Collections;
using UnityEngine;

public class BubblePopEffect : MonoBehaviour
{
    private WaitForSeconds lifetime = new WaitForSeconds(0.2f);
    public GameObject PrefabReference { get; private set; }

    public static BubblePopEffect CreateFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab, position, rotation);
        BubblePopEffect effect = obj.GetComponent<BubblePopEffect>();
        effect.PrefabReference = prefab;
        return effect;
    }
    private void OnEnable()
    {
        StartCoroutine(ReturnPoolCo());
    }

    private IEnumerator ReturnPoolCo()
    {
        yield return lifetime;
        PoolManager.Instance.ReturnToPool(PrefabReference, gameObject);
    }
}
