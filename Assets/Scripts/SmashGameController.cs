using UnityEngine;
using System.Collections;

public class SmashGameController : MonoBehaviour {

    [System.Serializable]
    private class PlayerData {

        public PlayerController Character;
        public Transform Spawn;
        public Transform Respawn;
        public Sprite Identifier;
        public Color IdentifierColor = Color.white;
        [System.NonSerialized]
        public PlayerIdentifier IdentifierObject;

    }

    [SerializeField]
    private PlayerData[] playerData;

    private void Awake() {
        for (var i = 0; i < playerData.Length; i++) {
            if (playerData[i].Character == null)
                continue;
            
            PlayerIdentifier identifier = new GameObject("P" + (i + 1) + " Identifier").AddComponent<PlayerIdentifier>();
            identifier.Sprite = playerData[i].Identifier;
            identifier.Color = playerData[i].IdentifierColor;
            identifier.Attach(playerData[i].Character);
            playerData[i].IdentifierObject = identifier;

        }
    }

    void OnDestroy() {
        
    }

}
