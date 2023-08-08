using UnityEngine;

public class HexNodeDisplay : MonoBehaviour
{

    public void SetState(HexState state)
    {
        var ratio = (float)state / (float)HexState.__COUNT__;
        var hex = GetComponent<HexRenderer>();
        
        switch (state)
        {
            case HexState.None:
                {
                    gameObject.SetActive(false);
                    break;
                }
            case HexState.Hover:
                {
                    hex.Material.color = Color.green; ;
                    gameObject.SetActive(true);
                break;
                }
            case HexState.Selected:
                {
                    hex.Material.color = Color.blue;
                    gameObject.SetActive(true);
                    break;
                }
            case HexState.Occupied:
                {
                    hex.Material.color = Color.white;
                    gameObject.SetActive(true);
                    break;
                }
            case HexState.Occupied_Hover:
                {
                    hex.Material.color = Color.green; ;
                    gameObject.SetActive(true);
                    break;
                }
            case HexState.Occupied_Selected:
                {
                    hex.Material.color = Color.blue;
                    gameObject.SetActive(true);
                    break;
                }

        }

    }
}
