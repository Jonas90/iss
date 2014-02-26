
// Hannes Helmholz
//
// 
using UnityEngine;
using System.Collections;

using Holoville.HOTween;

[RequireComponent (typeof(NetworkView))] // for RPC

//vordere Box vom Spieler, im Blickfeld sichtbar
//Gui object for equipment.cs
//setzt texturen
public class EquipmentAsGui : Equipment
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private CaveTextures Textures;
    private NetworkView NetView;
    private GUITexture GuiTex;
    private bool FirstTaken = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start()
    {
        base.Start();
     
        NetView = networkView;
        GuiTex = guiTexture;
     
        GuiTex.texture = ( Config.IsServer ) ? Textures.Standalone : GetTextureAsClient();
        if( AvailableAtStart )
        {
            foreach( Object tweenTarget in TweenTargets )
            {
                HOTween.PlayBackwards( tweenTarget );
            }
        }
    }
 
 
    void Update()
    {
        if( !Config.IsServer )
        {
            return;
        }
         
        if( Available && TriggerManual )
        {
            NetView.RPC( "RPCTrigger", RPCMode.AllBuffered );
            TriggerManual = false;
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public new void TriggerStored()
    {
        if( FirstTaken )
        {
            return;
        }
 
        FirstTaken = true;
        base.TriggerStored();
    }


    public new bool IsEquipped()
    {
        return Available;
    }
 
 
    private Texture2D GetTextureAsClient()
    {
        ArrayList angles = GetAllCameraAngles();
        int indexDiff = ( angles.Count/2 ) - angles.IndexOf( Config.OwnClientData.AngleOffset );

        if( indexDiff == 0 )
        {
            return Textures.Front;
        }
        else if( indexDiff == 1 )
		{
			return Textures.Left;
		}
		else if( indexDiff == -1 )
		{
			return Textures.Right;
		}
		else
		{
			return Textures.Outer;
		}
	}
 
 
    private ArrayList GetAllCameraAngles()
    {
        ArrayList angles = new ArrayList();
        for( int i = 1; i < Config.ClientData.Count; i++ )
        {
            float angle = ( Config.ClientData[i] as ClientCameraInfo ).AngleOffset;
            if( !angles.Contains( angle ) )
            {
                angles.Add( angle );
            }
        }
     
        angles.Sort();
        return angles;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCTrigger()
    {
        Trigger();
    }
    // =============================================================================
}

public class CaveTextures : System.Object
{
    public Texture2D Standalone;
    public Texture2D Left;
    public Texture2D Front;
    public Texture2D Right;
    public Texture2D Outer;
}