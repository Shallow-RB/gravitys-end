using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Items")]
        [SerializeField]
        [Tooltip("The items that can be sold in the shop")]
        private List<GameObject> items = new List<GameObject>();

        [Header("Shop Settings")]
        [SerializeField]
        [Range(0, 3)]
        private int maxItems = 3;

        [SerializeField]
        private GameObject shopItemPrefab;

        [SerializeField]
        private GameObject container;

        [SerializeField]
        private GameObject errorHandler;

        private List<GameObject> shopItems = new List<GameObject>();

        public static ShopManager instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            // For each item in the shop, create a random item from the list of items
            for (int i = 0; i < maxItems; i++)
            {
                // Instantiate a new shop item using the shop item prefab and add it to the container
                GameObject shopItem = Instantiate(shopItemPrefab, container.transform);
                SpawnRandomItem(shopItem);
            }
        }

        private void SpawnRandomItem(GameObject shopItem)
        {
            // Get a random index from the list of items
            int index = Random.Range(0, items.Count);
            // Create a new shop item using the prefab and add it to the list of shop items
            CreateItem(shopItem, items[index]);
            // Remove the item from the list of items so it cannot be selected again
            items.RemoveAt(index);
        }

        private void CreateItem(GameObject shopItem, GameObject prefab)
        {
            // Instantiate a new item using the given prefab and add it to the shop item
            GameObject item = Instantiate(prefab);
            // Set the item for the shop item component
            shopItem.GetComponent<ShopItem>().SetItem(item);
            // Add the shop item to the list of shop items
            shopItems.Add(shopItem);
        }

        private GameObject FirstEmptyItem()
        {
            return shopItems.Find(item => item.GetComponent<ShopItem>().item == null);
        }

        public void ShowError(string message)
        {
            // Set the error message
            errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = message;
            // Show the error message for 2 seconds
            errorHandler.SetActive(true);
            Invoke("HideError", 2f);
        }

        private void HideError()
        {
            errorHandler.SetActive(false);
        }
    }
}
