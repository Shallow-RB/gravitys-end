using UnityEngine;

namespace UI.Shop
{
    public class ShopArea : MonoBehaviour
    {
        private ShopManager _shopManager;

        private void Awake()
        {
            _shopManager = FindObjectOfType<ShopManager>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _shopManager.gameObject.SetActive(false);
            }
        }
    }
}