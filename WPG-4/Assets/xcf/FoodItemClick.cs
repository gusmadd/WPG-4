using UnityEngine;

public class FoodItemClick : MonoBehaviour
{
    public GameObject detailedFoodPage;

    void Start()
    {
        if (detailedFoodPage != null)
        {
            SpriteRenderer sr = detailedFoodPage.GetComponent<SpriteRenderer>();
            sr.sortingOrder = -10; // sembunyi di belakang
        }
    }

    void OnMouseDown()
    {
        if (detailedFoodPage != null)
        {
            SpriteRenderer sr = detailedFoodPage.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 10; // pindah ke depan
        }

        Debug.Log("Open detail page");
    }
}