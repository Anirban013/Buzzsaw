using UnityEngine;
using System.Collections;

public interface IPlayerRespawnListener{

   void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player);
}
