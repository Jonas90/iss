//6.E

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkObserver : MonoBehaviour
{
    public CamManager camManager;
    public Restarter restarter;

    /// <summary>
    /// Sends the rotate from the Player.
    /// </summary>
    /// <param name='playerRotation'>
    /// Player rotation.
    /// </param>
    /// <param name='iAmServer'>
    /// who send
    /// </param>
    public void SendRotate ( Quaternion playerRotation, bool iAmServer )
    {
        networkView.RPC ( "ReceiveRotate", RPCMode.Others, playerRotation, iAmServer );
    }

 
    [RPC]
    void ReceiveRotate ( Quaternion playerRotation, bool sentByServer )
    {
        if ( sentByServer && Config.Instance.IsStandalone )
        {
            // Wir sind Standalone-Telepräsenzclient. Server Sendet uns seine Daten
            GameController.Instance.Client.updateRota ( playerRotation );
        }
        else if ( GameController.Instance.IsServer )
            {
                // Wir sind Server und haben vom Gegenspieler Daten bekommen
                GameController.Instance.Server.updateRota ( playerRotation );
            }
    }

    /// <summary>
    /// Sends the position.
    /// </summary>
    /// <param name='playerPosition'>
    /// Player position.
    /// </param>
    /// <param name='iAmServer'>
    /// who send
    /// </param>
    public void SendPosition ( Vector3 playerPosition, bool iAmServer )
    {
        networkView.RPC ( "ReceivePosition", RPCMode.Others, playerPosition, iAmServer );
    }

 
    [RPC]
    void ReceivePosition ( Vector3 playerPosition, bool sentByServer )
    {
        if ( sentByServer && Config.Instance.IsStandalone )
        {
            // Wir sind Standalone-Telepräsenzclient. Server Sendet uns seine Daten
            GameController.Instance.Client.updatePosition ( playerPosition );
        }
        else if ( GameController.Instance.IsServer )
            {
                // Wir sind Server und haben vom Gegenspieler Daten bekommen
                GameController.Instance.Server.updatePosition ( playerPosition );
            }
    }
 
 
    /* dan
 /// <summary>
 /// GameClient meldet "Wurftaste gedrückt"
 /// </summary>
 /// <param name='playerPosition'>
 /// Player position.
 /// </param>
 /// <param name='Force'>
 /// Force.
 /// </param>
 public void SendWillShoot (Vector3 playerPosition, Vector3 Force)
 {
     networkView.RPC("ReceiveWillShoot", RPCMode.Others, playerPosition, Force);
 }

 [RPC]
 void ReceiveWillShoot(Vector3 playerPosition, Vector3 Force)
 {
     if (Network.peerType == NetworkPeerType.Server)
     {
         GameController.Instance.Server.shoot(playerPosition, true, Force);
     }
 }
 
 
 /// <summary>
 /// GameClient meldet "Lichttaste gedrückt"
 /// </summary>
 public void SendWillSwitchGlobalLight()
 {
     networkView.RPC("ReceiveWillSwitchGlobalLight", RPCMode.Others);
 }
 [RPC]
 void ReceiveWillSwitchGlobalLight()
 {
     if (Network.peerType == NetworkPeerType.Server)
     {
         GameController.Instance.Server.switchGlobalLight();
     }
 }
 /// <summary>
 /// Sends the shot. (from Server)
 /// </summary>
 /// <param name='startPos'>
 /// Start position.
 /// </param>
 /// <param name='Force'>
 /// Force.
 /// </param>
 public void SendShot(Vector3 startPos, Vector3 Force)
 {
     networkView.RPC("ReceiveShot", RPCMode.Others, startPos, Force);
 }
 [RPC]
 void ReceiveShot(Vector3 startPos, Vector3 Force)
 {
     GameController.Instance.Client.spawnShot(startPos, Force);
 }
 */


    public void SendEyeSeperation ( float value )
    {
        networkView.RPC ( "ReceiveEyeSeparation", RPCMode.All, value );
    }


    [RPC]
    void ReceiveEyeSeparation ( float separation )
    {
        camManager.SetEyeSeparation ( separation );
    }


    public void SendSwitchProjectionMode ( int mode )
    {
        networkView.RPC ( "SwitchProjectionMode", RPCMode.All, mode );
    }


    [RPC]
    void ReceiveSwitchProjectionMode ( int mode )
    {
        camManager.SwitchProjectionMode ( mode );
    }

    /// <summary>
    /// Sends the physic data. (from Server)
    /// </summary>
    /// <param name='modelName'>
    /// Model name.
    /// </param>
    /// <param name='modelPosition'>
    /// Model position.
    /// </param>
    /// <param name='modelRotation'>
    /// Model rotation.
    /// </param>
    public void SendPhysicData ( string modelName, Vector3 modelPosition, Quaternion modelRotation )
    {
        networkView.RPC ( "ReceivePhysicData", RPCMode.Others, modelName, modelPosition, modelRotation );
    }


    [RPC]
    void ReceivePhysicData ( string modelName, Vector3 modelPosition, Quaternion modelRotation )
    {

        GameController.Instance.Client.receivePhysicData ( modelName, modelPosition, modelRotation );
    }
 
    /* dan
 /// <summary>
 /// Sends the kill shot. (from Server)
 /// </summary>
 /// <param name='name'>
 /// Ballname
 /// </param>
 public void SendKillShot(string name)
 {
     networkView.RPC("ReceiveKillShot", RPCMode.Others, name);
 }
 [RPC]
 void ReceiveKillShot(string name)
 {
     GameController.Instance.Client.killShots(name);
 }

 /// <summary>
 /// Sends the light toggle.(from Server)
 /// </summary>
 /// <param name='roomLight'>
 /// Room light.
 /// </param>
 /// <param name='globalLight'>
 /// Global light.
 /// </param>
 public void SendLightToggle(bool roomLight, bool globalLight)
 {
     networkView.RPC("ReceiveLightToggle", RPCMode.Others, roomLight, globalLight);
 }
 [RPC]
 void ReceiveLightToggle(bool roomLight, bool globalLight)
 {
     GameController.Instance.Client.actLightState(roomLight, globalLight);
 }
 */

    public void SendRestartGame ()
    {
        networkView.RPC ( "ReceiveRestartGame", RPCMode.Others );
    }


    [RPC]
    void ReceiveRestartGame ()
    {
        restarter.restart ();
    }

    /* dan
 #region Handling hit enemy
 /// <summary>
 /// Sends the hit enemy.
 /// </summary>
 /// <param name='enemy'>
 /// Enemy.
 /// </param>
 public void SendHitEnemy(string enemy)
 {
     networkView.RPC("ReceiveHitEnemy", RPCMode.Others, enemy);
 }

 /// <summary>
 /// Receives the hit enemy.
 /// </summary>
 /// <param name='enemyName'>
 /// Enemy name.
 /// </param>
 [RPC]
 void ReceiveHitEnemy(string enemyName)
 {
     GameController.Instance.Client.hitEnemy(enemyName);
 }

 /// <summary>
 /// Sends the hit enemy sound.
 /// </summary>
 public void SendHitEnemySound()
 {
     networkView.RPC("ReceiveHitEnemySound", RPCMode.Others);
 }

 /// <summary>
 /// Receives the hit enemy sound.
 /// </summary>
 [RPC]
 void ReceiveHitEnemySound()
 {
     GameController.Instance.Client.hitEnemySound();
 }
 #endregion


 #region Open door sound handling
 /// <summary>
 ///     Sends the sound for opening a door.
 /// </summary>
 public void SendOpenDoor()
 {
     networkView.RPC("ReceiveOpenDoor", RPCMode.Others);
 }

 /// <summary>
 ///     Receives the sound for opening a door.
 /// </summary>
 [RPC]
 void ReceiveOpenDoor()
 {
     GameController.Instance.Client.openDoor();
 }
 #endregion


 #region Close door sound handling
 /// <summary>
 ///     Sends the sound for closing a door.
 /// </summary>
 public void SendCloseDoor()
 {
     networkView.RPC("ReceiveCloseDoor", RPCMode.Others);
 }

 /// <summary>
 ///     Receives the sound for closing a door.
 /// </summary>
 [RPC]
 void ReceiveCloseDoor()
 {
     GameController.Instance.Client.closeDoor();
 }
 #endregion


 #region Wall sound handling
 /// <summary>
 ///     Sends the sound for the wall in the cave.
 /// </summary>
 public void SendWallSound()
 {
     networkView.RPC("ReceiveWallSound", RPCMode.Others);
 }

 /// <summary>
 ///     Receives the sound for the wall in the cave.
 /// </summary>
 [RPC]
 void ReceiveWallSound()
 {
     GameController.Instance.Client.wallSound();
 }
 #endregion
 */
 
    public void SendSkeletonEnabled ( bool enabled )
    {
        networkView.RPC ( "ReceiveSkeletonEnabled", RPCMode.Others, enabled );
    }


    [RPC]
    void ReceiveSkeletonEnabled ( bool enabled )
    {
        ( (Skeleton) GameObject.FindObjectOfType ( typeof(Skeleton) ) ).Enabled = enabled;
    }

}