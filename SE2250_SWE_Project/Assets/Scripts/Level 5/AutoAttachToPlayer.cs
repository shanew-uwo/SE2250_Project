using UnityEngine;

public class AutoAttachToPlayer : MonoBehaviour
{
    [Tooltip("Attach any script components here with configured values.")]
    public MonoBehaviour[] scriptTemplates;

    [Tooltip("Time in seconds between checks.")]
    public float checkInterval = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(AttachScriptsToPlayer), 0f, checkInterval);
    }

    private void AttachScriptsToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[AutoAttach] No Player tagged GameObject found.");
            return;
        }

        foreach (MonoBehaviour template in scriptTemplates)
        {
            if (template == null) continue;

            System.Type scriptType = template.GetType();

            if (player.GetComponent(scriptType) == null)
            {
                // Add the component and copy serialized fields
                Component newComp = player.AddComponent(scriptType);
                CopyFields(template, newComp);

                Debug.Log($"[AutoAttach] ✅ Attached and configured: {scriptType.Name}");
            }
            else
            {
                Debug.Log($"[AutoAttach] Script '{scriptType.Name}' already exists on Player.");
            }
        }
    }

    private void CopyFields(Component source, Component destination)
    {
        var fields = source.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            object value = field.GetValue(source);
            field.SetValue(destination, value);
        }
    }
}