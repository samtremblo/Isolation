using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessHandler : MonoBehaviour
{
    public float speedSwitch = .5f;

    public enum GameStatus { awake, begining, PowerOut, Main, Death, Ending };

    [SerializeField] private bool ValueChanged;

    private  float CurrentExposure,lastExposure,NextExposure,t,exposure,saturation,contrast, LastContrast, LastSaturation;
    private GameStatus lastGameStatus = GameStatus.awake;
    public GameStatus gameStatus = GameStatus.begining;


    /*
    [ARRAYS INDEX MEANING]
    [0] = EXPOSURE 
    [1] = SATURATION
    [2] = CONTRAST
    [3] = GAMMA   
    [4] = GAIN   
    */

    [SerializeField] private float[] StartColorGrading = { 0, 0, 0 };
    [SerializeField] private float[] BeginingColorGrading = { 2,-7 , 20};
    [SerializeField] private float[] PowerOutColorGrading = { -6, -100, 20 };
    [SerializeField] private float[] MainColorGrading = { 1.37f, -76, 100 };
    [SerializeField] private float[] DeathColorGrading = { 0, 0, 0 };
    // [SerializeField] private float[] EndingColorGrading = { 0, 0, 0, 0, 0 };


    ColorGrading colorGradingLayer = null;
    Grain grainLayer = null;
    ChromaticAberration chromaLayer = null;

    void Start()
    {
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGradingLayer);
        volume.profile.TryGetSettings(out grainLayer);
        volume.profile.TryGetSettings(out chromaLayer);

        colorGradingLayer.postExposure.value = exposure;
        colorGradingLayer.contrast.value = contrast;
        colorGradingLayer.saturation.value = saturation;
     

        //---------------------------------Values that fade in to the begining ones -----------------
        exposure = 1;
        saturation = StartColorGrading[1];
        contrast = StartColorGrading[2];
        //-------------------------------------------------------------------------------------------



        StartCoroutine(PostProcessCoroutine());

    }

    IEnumerator PostProcessCoroutine()
    {
        //---------------------------------Setting Start Coroutine Values-----------------

        grainLayer.enabled.value = false;
        chromaLayer.enabled.value = false;
        colorGradingLayer.gamma.overrideState = false;
        colorGradingLayer.lift.overrideState = false;


        t = 0;

        lastExposure = exposure;
        LastSaturation = saturation;
        LastContrast = contrast;

        //---------------------------------Begining of the game Post process-----------------
        while (gameStatus == GameStatus.begining)
        {
            
            exposure = Mathf.Lerp(lastExposure, BeginingColorGrading[0], t);
            saturation = Mathf.Lerp(lastExposure, BeginingColorGrading[1], t);
            contrast = Mathf.Lerp(LastContrast, BeginingColorGrading[2], t);

            colorGradingLayer.postExposure.value = exposure;
            colorGradingLayer.contrast.value = contrast;
            colorGradingLayer.saturation.value = saturation;

            if(t<=1) t += Time.deltaTime * speedSwitch;

            yield return null;

        }
        t = 0;
        lastExposure = exposure;
        LastSaturation = saturation;
        LastContrast = contrast;
        yield return null;

        //---------------------------------PowerOut Post process-----------------
        speedSwitch = 3;

        while (gameStatus == GameStatus.PowerOut)
        {
            exposure = Mathf.Lerp(lastExposure, PowerOutColorGrading[0], t);
            saturation = Mathf.Lerp(LastSaturation, PowerOutColorGrading[1], t);
            contrast = Mathf.Lerp(LastContrast, PowerOutColorGrading[2], t);

            colorGradingLayer.postExposure.value = exposure;
            colorGradingLayer.contrast.value = contrast;
            colorGradingLayer.saturation.value = saturation;

            if (t <= 1) t += Time.deltaTime * speedSwitch;
            yield return null;


        }


        t = 0;

        lastExposure = exposure;
        LastSaturation = saturation;
        LastContrast = contrast;
        lastGameStatus = gameStatus;

        yield return null;
        //---------------------------------Main Post process-----------------
        speedSwitch = .5f;

        while (gameStatus == GameStatus.Main)
        {

            colorGradingLayer.gamma.overrideState = true;
            colorGradingLayer.lift.overrideState = true;

            grainLayer.enabled.value = true;
            chromaLayer.enabled.value = true;

            exposure = Mathf.Lerp(lastExposure, MainColorGrading[0], t);
            saturation = Mathf.Lerp(LastSaturation, MainColorGrading[1], t);
            contrast = Mathf.Lerp(LastContrast, MainColorGrading[2], t);

            colorGradingLayer.postExposure.value = exposure;
            colorGradingLayer.contrast.value = contrast;
            colorGradingLayer.saturation.value = saturation;

            if (t <= 1) t += Time.deltaTime * speedSwitch;
            yield return null;

        }



        t = 0;

        lastExposure = exposure;
        LastSaturation = saturation;
        LastContrast = contrast;
        lastGameStatus = gameStatus;


        yield return null;

     

    }
    

}
