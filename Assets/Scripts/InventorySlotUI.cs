using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    private int slotIndex;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Setup(int index)
    {
        slotIndex = index;
    }

    public void OnClick()
    {
        InventoryManager.Instance.OnSlotSelected(slotIndex);
    }
}