using UnityEngine;
using UnityEngine.UI;

public class ItemSorting : MonoBehaviour
{
    public Transform itemSpawnPoint; // Where new items spawn
    public GameObject currentItem; // Item currently held by the player
    public GameObject checkpointMarker; // Visual indicator to guide the player
    public Text interactionPrompt; // UI prompt for "Press E to drop item"

    private string currentItemTag; // The tag of the item currently held

    void Start()
    {
        // Initial setup
        interactionPrompt.gameObject.SetActive(false);
        SpawnNewItem();
    }

    void Update()
    {
        // Show guidance toward target area
        if (currentItem != null)
            ShowCheckpoint();

        // Drop item if near the correct area and player presses 'E'
        if (Input.GetKeyDown(KeyCode.E) && IsInCorrectArea())
        {
            DropOffItem();
        }
    }

    // Check if player is in the correct drop-off area
    private bool IsInCorrectArea()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            // Check if the tag of the area matches the item tag
            return hit.collider.CompareTag(currentItemTag + "Area");
        }
        return false;
    }

    // Drop item and spawn a new one
    private void DropOffItem()
    {
        Destroy(currentItem);
        interactionPrompt.gameObject.SetActive(false);
        SpawnNewItem();
    }

    // Spawn a new item for the player to hold
    private void SpawnNewItem()
    {
        // Instantiate a new item (can randomize here if needed)
        currentItem = Instantiate(GetRandomItemPrefab(), itemSpawnPoint.position, Quaternion.identity);
        currentItemTag = currentItem.tag; // Assign the tag of the new item
    }

    // Visual guidance to the drop-off area
    private void ShowCheckpoint()
    {
        GameObject targetArea = GameObject.FindWithTag(currentItemTag + "Area");
        if (targetArea != null)
        {
            checkpointMarker.transform.position = targetArea.transform.position;
            checkpointMarker.SetActive(true);

            if (Vector3.Distance(transform.position, targetArea.transform.position) < 2f)
            {
                interactionPrompt.gameObject.SetActive(true);
                interactionPrompt.text = "Press E to drop item";
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
    }

    // Placeholder for getting a random item prefab (adjust as needed)
    private GameObject GetRandomItemPrefab()
    {
        // Replace with actual item prefab selection logic
        return currentItem; // This should return a new prefab from a list of item prefabs
    }
}
