
// Hannes Helmholzd
//
// 

using System.Collections;
using UnityEngine;


//hintere Box vom Spieler
//enthaelt schon aufgenommenes Equipment
public class EquipmentAtMercy : Equipment
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private enum DependenceType
    {
        OtherEquipmentNeededFirst,
        OtherEquipmentDepends
    }
 
    [SerializeField]    private DependenceType Dependence;
    [SerializeField]    private EquipmentAtMercy OtherEquipment;
    private bool PotentionallyAvailable = false;
    // =========================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    public bool IsPotentionallyAvailable ()
    {
        return PotentionallyAvailable;
    }


    public new void SetAvailable ( bool available )
    {
        if ( !available )
        {
            PotentionallyAvailable = false;
            base.SetAvailable ( false );
         
            return;
        }
     
        if ( Dependence == DependenceType.OtherEquipmentDepends )
        {
            PotentionallyAvailable = true;
            base.SetAvailable ( true );
         
            if ( OtherEquipment.IsPotentionallyAvailable () )
            {
                OtherEquipment.SetAvailable ( true );
            }
         
            return;
        }
     
        if ( Dependence == DependenceType.OtherEquipmentNeededFirst )
        {
            PotentionallyAvailable = true;
         
            if ( OtherEquipment.IsAvailable () )
            {
                base.SetAvailable ( true );
            }
             
            return;
        }       

    }
    // =========================================================================
}