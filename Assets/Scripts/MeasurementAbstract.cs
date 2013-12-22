
// Hannes Helmholz
//
// 

using System;
using UnityEngine;

public class MeasurementValues : System.Object
{
 
    public float O2;
    public float CO;
    public float HCN;
    public float HCL;

 
    public MeasurementValues ( float o2, float co, float hcn, float hcl )
    {
        this.O2 = o2;
        this.CO = co;
        this.HCN = hcn;
        this.HCL = hcl;
    }
 
 
    public new string ToString ()
    {
        return O2 + " O2  " + CO + " CO  " + HCN + " HCN  " + HCL + " HCL";
    }

    public bool isNull()
    {
        if( O2 == 0f && CO == 0f && HCN == 0f && HCL == 0f) return true;
        return false;
    }

}

public /*abstract*/ class MeasurementAbstract : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private string DataFile = "*.conf";
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public MeasurementValues GetValues ( string name )
    {
        MeasurementValues values = new MeasurementValues(0f, 0f, 0f, 0f);;

        if ( name.Equals("") || name.Length == 0 )
        {
            return values;
        }

        System.IO.StreamReader sr = new System.IO.StreamReader ( DataFile );
        try
        {

            bool found = false;
            string line = sr.ReadLine ();
            while ( line != null && !found )
            {
                line = line.Split ( "|"[0] )[0]; // without comments
             
                if ( line.Length <= 0 )
                {
                    line = sr.ReadLine ();
                    continue;
                }
             
                String[] parts = line.Split ( ";"[0] );
                for ( int i = 0; i< parts.Length; i++ )
                {
                    parts[i] = parts[i].Replace ( " ", "" );  // delete all spaces
                }

                if ( parts.Length < 5 )
                {
                    line = sr.ReadLine ();
                    continue;
                }
             
                if ( parts[0].Equals ( name ) )
                {
                    values = new MeasurementValues ( float.Parse ( parts[1] ), float.Parse ( parts[2] ), float.Parse ( parts[3] ), float.Parse ( parts[4] ) );
                    found = true;
                }

                line = sr.ReadLine ();
            }
        }
        catch ( Exception e )
        {
            ConfigClass.Log ( e.ToString (), true );
        }
        finally
        {
            sr.Close ();
        }

        return values;
    }
    // =============================================================================
}