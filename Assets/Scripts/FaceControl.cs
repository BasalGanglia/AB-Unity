using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;
using UnityEditor;
using System.IO;



public class FaceControl : ImageResultsListener
{

    Transform mainCamera;
    CameraInput cameraInput;
    // Webcam name is needed since the Affectiva SDK isn't always picking the correct webcam.
    private string cameraName = "USB 2.0 Webcam Device";
    private string currentCameraName = "";
    // Variable that tells us if we will get data from Mirror, Machine Learning, File Reading, etc.
    private string processingType;
    // The emotion and expression values we get from Affectiva. (0-100)
    private float EyeClosure, Smirk, MouthOpen, Smile, BrowRaise, BrowFurrow,Disgust,
        Fear,Anger,Sadness,Surprise, Joy, Contempt;

	// helpers to calculate moving estimate (moving average to make it les jittery)
	private float EyeClosure_mv_est, Smirk_mv_est, MouthOpen_mv_est, Smile_mv_est, BrowRaise_mv_est, BrowFurrow_mv_est,Disgust_mv_est,
	Fear_mv_est,Anger_mv_est,Sadness_mv_est,Surprise_mv_est, Joy_mv_est, Contempt_mv_est, or_X_est, or_Y_est, or_Z_est;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    // The values for the head orientation from affectiva.
    public static float OrientationX, OrientationY,OrientationZ;



    public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face.");
		TehUtils.test_print ();
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face");
    }

    // Function that sets the blendshapes, can be used from any processingType, UDP for ML, Mirror for real-time and File for reading from a file. Should be between 0 and 100.
//    public void setBlendShape(float EyeClosure, float Smirk, float MouthOpen, float Smile, float BrowRaise, float BrowFurrow, float Disgust,
//       float Fear, float Anger, float Sadness, float Surprise, float Joy)
//    {ö 
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Disgusted"), Disgust / 2);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Scared"), Fear);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Sad"), Sadness / 4);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Surprised"), Surprise / 3);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Angry"), Anger / 3);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Happy"), Joy * 3 / 4);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.SUB_Mouth_Little_Opened"), MouthOpen / 4);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Smile_Lips_Closed"), Smile / 2);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Smirk"), Smirk / 2);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Eyes_Closed_Max"), EyeClosure);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Eyebrows_Raised"), BrowRaise);
//        skinnedMeshRenderer.SetBlendShapeWeight(skinnedMesh.GetBlendShapeIndex("BS_node.Eyebrows_Frown"), BrowFurrow);
//    }
//	    public void setBlendShape(float Smile)
	    public void setBlendShape(float Anger, float Contempt, float Disgust, float Fear, float Joy, float Sadness, float Surprise)
	    {
		Debug.Log("Affectiva gave us: Anger: " + Anger + "  Contempt: " + Contempt + "  Disgust: " + Disgust + "  Fear: " + Fear + "  Joy: " + Joy + "  Sadness: " + Sadness + "  Surprise: " + Surprise );
		float rnd = Random.Range(1f,4f);
		float mv_esti = 60;
		Debug.Log ("Joy before mv estimate: " + Joy);	
		Joy += Random.Range (-15f, 15f);
		Debug.Log ("Joy after random: " + Joy);

		Joy = TehUtils.MovingEstimate (Joy_mv_est, Joy, mv_esti);
		Joy_mv_est = Joy;
		Debug.Log ("Joy after mv estimate: " + Joy + " joy_mv_est now: " + Joy_mv_est);
		Joy = TehUtils.toBlendShapeValue (Joy, 5);
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Joy"), Joy);

//		Debug.Log ("Joy after blendshaper estimate: " + Joy);
//		Surprise += Random.Range (0f, 1f);
		Surprise += Random.Range (-5f, 5f);
		Surprise = TehUtils.MovingEstimate (Surprise_mv_est, Surprise, mv_esti);	
		Surprise_mv_est = Surprise;
		Surprise = TehUtils.toBlendShapeValue (Surprise, 5);

		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Surprise"), Surprise);

		Anger += Random.Range (-5f, 5f);
		Anger = TehUtils.MovingEstimate (Anger_mv_est, Anger, mv_esti);
		Anger_mv_est = Anger;
		Anger = TehUtils.toBlendShapeValue (Anger, 5);
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Anger"), Anger);

		Contempt = TehUtils.MovingEstimate (Contempt_mv_est, Contempt, mv_esti);
		Contempt_mv_est = Contempt;
		Contempt = TehUtils.toBlendShapeValue (Contempt, 0.5f);
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Contempt"), Contempt);

		Disgust = TehUtils.MovingEstimate (Disgust_mv_est, Disgust, mv_esti);
		Disgust_mv_est = Disgust;
		Disgust = TehUtils.toBlendShapeValue (Disgust, 5);
		if (Disgust > 50)
			Disgust = 50;
		if (Joy > 50)
			Disgust = 0;
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Disgust"), Disgust);

		Fear += Random.Range (-5f, 5f);
		Fear = TehUtils.MovingEstimate (Fear_mv_est, Fear, mv_esti);
		Fear_mv_est = Fear;
		Fear = TehUtils.toBlendShapeValue (Fear, 5);
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Fear"), Fear);

		Sadness += Random.Range (-5f, 5f);
		Sadness = TehUtils.MovingEstimate (Sadness_mv_est, Sadness, mv_esti);
		Sadness_mv_est = Sadness;
		Sadness = TehUtils.toBlendShapeValue (Sadness, 1);
		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Sadness"), Sadness);
//
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Anger"), Math.Min(Anger * 50, 100) );
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Contempt"), Contempt * 50);
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Disgust"), Disgust * 50);
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Fear"), Fear * 50);
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Joy"), Joy * 50);
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Sadness"), Sadness * 50);
//		skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_Surprise"), Surprise * 50);
	//	 skinnedMeshRenderer.SetBlendShapeWeight (skinnedMesh.GetBlendShapeIndex ("AB_W_Funnel"), Smile);
	    }

    // Function that is called when the Affectiva SDK has results ready.
    public override void onImageResults(Dictionary<int, Face> faces)
    {
        Debug.Log("Got face results");

        foreach (KeyValuePair<int, Face> pair in faces)
        {
            int FaceId = pair.Key;
            Face face = pair.Value;
			Debug.Log ("We found a face!");
			face.Expressions.TryGetValue(Expressions.Smile, out Smile);

			face.Emotions.TryGetValue(Emotions.Disgust, out Disgust);
            face.Emotions.TryGetValue(Emotions.Fear, out Fear);
            face.Emotions.TryGetValue(Emotions.Anger, out Anger);
            face.Emotions.TryGetValue(Emotions.Sadness, out Sadness);
            face.Emotions.TryGetValue(Emotions.Surprise, out Surprise);
            face.Emotions.TryGetValue(Emotions.Joy, out Joy);
			face.Emotions.TryGetValue(Emotions.Contempt, out Contempt);

            // Getting the Expressions and emotions values from the SDK.
//            face.Expressions.TryGetValue(Expressions.Smile, out Smile);
//            face.Expressions.TryGetValue(Expressions.EyeClosure, out EyeClosure);
//            face.Expressions.TryGetValue(Expressions.MouthOpen, out MouthOpen);
//            face.Expressions.TryGetValue(Expressions.Smirk, out Smirk);
//            face.Expressions.TryGetValue(Expressions.BrowRaise, out BrowRaise);
//            face.Expressions.TryGetValue(Expressions.BrowFurrow, out BrowFurrow);
//            face.Emotions.TryGetValue(Emotions.Disgust, out Disgust);
//            face.Emotions.TryGetValue(Emotions.Fear, out Fear);
//            face.Emotions.TryGetValue(Emotions.Anger, out Anger);
//            face.Emotions.TryGetValue(Emotions.Sadness, out Sadness);
//            face.Emotions.TryGetValue(Emotions.Surprise, out Surprise);
//            face.Emotions.TryGetValue(Emotions.Joy, out Joy);
//            // Getting the orientation for rotating the head.
              OrientationX = face.Measurements.Orientation.x;
              OrientationY = face.Measurements.Orientation.y;
              OrientationZ = face.Measurements.Orientation.z;


			OrientationX = TehUtils.MovingEstimate (or_X_est, OrientationX, 6);
			or_X_est = OrientationX;
			OrientationY = TehUtils.MovingEstimate (or_Y_est, OrientationY, 6);
			or_Y_est = OrientationY;
			OrientationZ = TehUtils.MovingEstimate (or_Z_est, OrientationZ, 6);
			or_Z_est = OrientationZ;

		//	Debug.Log ("orientation now : " + OrientationX);

	//s		Debug.Log("Affectiva gave us Smile " + Smile);
        }
    }

    void Start ()
    {
        OrientationX = OrientationY = 0;
        // Standard processing type. Set this to whatever processing type is needed.(Mirror,UDP,File,etc.)
        processingType = "Mirror";
    }

	void Update ()
    {
        /* Code needed to select the correct webcam for affectiva SDK. Change name of the cameraName variable
        with correct webcam name or comment the code if Affectiva selects correct webcam. */
        if (currentCameraName != cameraName)
        {
            cameraInput.SelectCamera(true, cameraName);
            currentCameraName = cameraName;
        }

        if(processingType=="Mirror")			
		{
		//	Debug.Log ("trying to set smile " + Smile);
		//	EyeClosure = 100;
    //        setBlendShape(EyeClosure,Smirk,MouthOpen,Smile,BrowRaise,BrowFurrow,Disgust,Fear,Anger,Sadness, Surprise, Joy);
	//		setBlendShape(Smile);0
			setBlendShape(Anger, Contempt, Disgust, Fear, Joy, Sadness, Surprise);
        }
        else
            if(processingType=="UDP")
            {
                //to be continued
            }
    }

    void Awake()
    {
        // Assign compoments on Awake.
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraInput = mainCamera.GetComponent<CameraInput>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		Debug.Log ("my name is " + skinnedMeshRenderer.name);
    }
}
