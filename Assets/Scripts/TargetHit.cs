using UnityEngine;

public class TargetHit : MonoBehaviour
{
    public int life = 1;
    public int scoreValue = 1;
    public bool destroyOnHit = false;
    public Color hitColor = Color.green;

    private bool alreadyScored = false;

    public void TakeHit(int damage)
    {
        if (alreadyScored)
        {
            return;
        }

        life -= damage;

        Debug.Log(gameObject.name + " foi atingido!");

        if (life <= 0)
        {
            alreadyScored = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }

            ChangeColor();

            if (destroyOnHit)
            {
                Destroy(gameObject, 0.2f);
            }
        }
    }

    void ChangeColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", hitColor);
                }
                else
                {
                    material.color = hitColor;
                }
            }
        }
    }
}