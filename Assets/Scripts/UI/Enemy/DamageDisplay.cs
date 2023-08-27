using TMPro;
using UnityEngine;

namespace UI.Enemy
{
    public class DamageDisplay : MonoBehaviour
    {
        public void Show(string damage, GameObject display, Canvas canvas, Color color)
        {
            var damageDisplay = Instantiate(display, canvas.transform.TransformPoint(0, 1, 0), Quaternion.identity,
                canvas.transform);

            TextMeshProUGUI textUI = damageDisplay.GetComponentInChildren<TextMeshProUGUI>();
            textUI.text = damage;
            textUI.color = color;

            Destroy(damageDisplay, 1f);
        }
    }
}
