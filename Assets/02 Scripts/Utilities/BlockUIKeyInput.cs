using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockUIKeyInput : Button
{
    public override void OnSubmit(BaseEventData eventData)
    {
        // 아무 것도 안 적음 → Space/Enter 입력 무시
    }
}

