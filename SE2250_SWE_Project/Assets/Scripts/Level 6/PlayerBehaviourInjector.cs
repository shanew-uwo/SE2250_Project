using UnityEngine;

public class PlayerBehaviorInjector : MonoBehaviour
{
    public Gradient greenGradient;
    public Gradient orangeGradient;
    public Gradient purpleGradient;
    public Gradient blueGradient;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PlayerAOEAttack attack = player.GetComponent<PlayerAOEAttack>();
            if (attack == null)
            {
                attack = player.AddComponent<PlayerAOEAttack>();
            }

            // Assign gradients
            attack.aoeColors = new Gradient[4];
            attack.aoeColors[0] = greenGradient;
            attack.aoeColors[1] = orangeGradient;
            attack.aoeColors[2] = purpleGradient;
            attack.aoeColors[3] = blueGradient;
            
        }
    }
}