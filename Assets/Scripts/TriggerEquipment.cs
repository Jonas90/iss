
// Hannes Helmholzd
//
// 

using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NetworkView))] // for Transform
[RequireComponent (typeof(LayerSetterElement))]

public class TriggerEquipment : TriggerAbstract
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private Equipment LinkedEquipment;
    [SerializeField]    private float TakeAnimationLength = 2; // optional
 
    private PlayerMovement PlayerMov;
    //TODO Fix player rotation
	//private PlayerRotation PlayerRot;
    private LayerSetterElement Layer;
    private EquipmentManager EquipManager;
    // =============================================================================



    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        base.Awake ();
     
        PlayerMov = Player.GetComponent<PlayerMovement> ();
        //dan
		//PlayerRot = Player.GetComponent<PlayerRotation> ();
        Layer = gameObject.GetComponent<LayerSetterElement> ();
        EquipManager = GameObject.FindObjectOfType ( typeof(EquipmentManager) ) as EquipmentManager;
    }
 
 
    void OnMouseEnter ()
    {
        EquipmentAtMercy atMercy = LinkedEquipment as EquipmentAtMercy;
        if ( !LinkedEquipment.IsAvailable () && !( atMercy && atMercy.IsPotentionallyAvailable () ) )
        {
            base.OnMouseEnter ();
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    protected new void DoTrigger ()
    {
        foreach ( RendererData rend in Renderers )
            rend.SetMaterials ( false );
         
        StartCoroutine ( YieldDoTrigger () );
    }
 
 
    // this design is stupid, but calling "yield StartCoroutine(Animate())" in DoTrigger() doesn't work
    private IEnumerator YieldDoTrigger ()
    {
        if ( TakeAnimationLength > 0 )
        {
            //dan
            //TODO
            //yield StartCoroutine(Animate());
            yield return StartCoroutine(Animate());
        }
     
        EquipManager.Take ( LinkedEquipment );
        NetView.RPC ( "RPCDoTrigger", RPCMode.AllBuffered );
    }
 

    private void SetPlayerMotion ( bool status )
    {
        PlayerMov.enabled = status;
        //PlayerRot.TemporaryDisable = !status;
    }

 
    private IEnumerator Animate ()
    {
        if ( Layer.OldLayer != Layer.OtherLayer )
        {
            LayerSetter.SetAllLayerRecursively ( gameObject, Layer.OtherLayer );
        }
 
        SetPlayerMotion ( false );
     
        Transform tweenTarget = LinkedEquipment.transform;
        if ( LinkedEquipment as EquipmentAsGui )
        {
            tweenTarget = Player.transform;
        }
         
        TweenParms parms = new TweenParms ();
        parms.Prop ( "position", new PlugVector3 ( tweenTarget.position ) );
        parms.Prop ( "rotation", new PlugQuaternion ( tweenTarget.rotation ) );
        parms.Prop ( "localScale", new PlugVector3 ( tweenTarget.localScale ) );
        parms.Ease ( EaseType.EaseInOutSine );
     
        HOTween.To ( Trans, TakeAnimationLength, parms );

        //dan
        //TODO
        //yield WaitForSeconds(TakeAnimationLength);
        yield return new WaitForSeconds(TakeAnimationLength);

        SetPlayerMotion ( true );
     
        LayerSetter.SetAllLayerRecursively ( gameObject, Layer.OldLayer );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCDoTrigger ()
    {
        gameObject.SetActive ( false );
    }
    // =============================================================================
}