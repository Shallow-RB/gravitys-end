using Core;
using TMPro;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField]
        private Image sprite;

        [SerializeField]
        private TextMeshProUGUI price;

        public GameObject item { get; private set; }

        public void SetItem(GameObject obj)
        {
            item = obj;
            var script = item.GetComponent<Item>();

            script.RenderItem(false);
            sprite.sprite = script.icon;
            SetTime(script.price);
            gameObject.SetActive(true);
        }

        public void BuyItem()
        {
            var script = item.GetComponent<Item>();
            var time = script.price * 60;
            if (time <= 0 || Timer.instance.time - time <= 0)
            {
                ShopManager.instance.ShowError("Not enough time!");
                return;
            }

            Timer.instance.time -= time;
            if (InventoryManager.instance.PickupItem(script))
            {
                Clear();
            }
        }

        private void SetTime(float time)
        {
            price.text = string.Format("{0:00}:{1:00}", (int)time, (int)(time * 60) % 60);
        }

        private void Clear()
        {
            item = null;
            sprite.sprite = null;
            price.text = "00:00";
            gameObject.SetActive(false);
        }
    }
}