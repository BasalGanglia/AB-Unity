using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TehUtils 
{

	public static float MovingEstimate(float avg_value, float new_value, float proportion)
	{
		if (proportion > 1) 
		{
			proportion = (proportion - 1) / proportion;

//			Debug.Log ("Proportion will be : " + proportion);
//
//			Debug.Log ("Prop * avgvalue : " + (proportion * avg_value) + " where avgvalue is " + avg_value + " and propo:" + proportion ) ;
//			Debug.Log ("The rest : " + ((1.0f- proportion)));
		return ((proportion * avg_value) + ((1.0f - proportion) * new_value));			
		} else
			return new_value;
			
	}

	public static float toBlendShapeValue(float old_value, float multiplier)
	{
		float new_value = old_value; // + 0.1f;  // get rid of the insanely small values affectiva gives (0.000005)
		new_value = new_value * multiplier; // scale up because the affectiva underestimates the emotions
		if (new_value > 100) // maybe there is max method somewhere.. anyway, cap to max 100
			return 100;
		else
			return new_value;
	}
	public static void test_print()
	{
		Debug.Log("testing my own method.");
	}
}	