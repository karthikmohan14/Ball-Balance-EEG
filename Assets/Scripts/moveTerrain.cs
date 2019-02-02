#region Headers

using System.Collections;
using System.IO;
using UnityEngine;

#endregion
// to start playing:
// 	get meditation over MinMeditation to start recording
// 	get meditation over m_RequiredMeditation to start movement along y
// 	get attention over m_RequiredMeditation to start movement along z
//		Note that this script use coroutines for each phase timers and callbacks triggering.

[AddComponentMenu("Scripts/Game/Platform")]
public class moveTerrain : MonoBehaviour
{

    public enum Phase
    {
        None,
        Meditation,
        Focus
    }
    #region Attributes
    [Header("Settings")]
    [SerializeField, Range(0.0f, 100.0f)]
    [Tooltip("If the meditation goes down this value, cancel movement")]
    private float m_MinMeditation = 30.0f;
    [SerializeField, Range(0.0f, 100.0f)]
    [Tooltip("If the meditation goes over this value, start the \"Meditation phase\"")]
    private float m_RequiredMeditation = 70.0f;
    [SerializeField, Range(0.0f, 30.0f)]
    [Tooltip("You must hold meditation value over MinMeditation during this value (in seconds)")]
    private float m_MeditationPhaseDuration = 5.0f;
    [SerializeField, Range(0.0f, 100.0f)]
    [Tooltip("If the player is in \"Meditation phase\", it must hold its attention over this value to initialize \"Focus phase\"")]
    private float m_RequiredAttention = 70.0f;
    [SerializeField, Range(0.0f, 30.0f)]
    [Tooltip("The player has to hold its attention over RequiredAttention during this value (in seconds)")]
    private float m_FocusPhaseDuration = 1.0f;

    private Coroutine m_Routine = null;
    private Phase m_CurrentPhase = Phase.None;
    private bool m_IsMovement = false;

    #endregion

    #region localVariables
    float smooth = 5.0f;
    float tiltAngle = 10.0f;
    float x, z;
    int frames;
    float tiltAroundZ, tiltAroundX;
    string path;
    #endregion
    #region Engine Methods

    float map(float x, float oFrom, float oTo, float nFrom, float nTo)
    {
        return x * ((nTo - nFrom) / (oTo - oFrom));
    }

    float sigmoid(float x) {
        return (2 / (1+Mathf.Exp(x)))-1;
    }
    private void Awake()
    {

    }

    private void Start()
    {
        MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
        frames = 0;
        x = Random.Range(1, 5);
        z = Random.Range(1, 5);
        path = @"k:\MyTest.txt";
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("attention,meditation,delta,theta,lowAlpha,highAlpha,lowBeta,highBeta,lowGamma,highGamma");
            }
        }
        //    MindwaveManager.Instance.Controller.OnUpdateRawEEG += OnUpdateRawEEG;
        // MindwaveManager.Instance.Controller.OnUpdateBlink += OnUpdateBlink;
    }
    private void Update()
    {
        frames++;
        // Smoothly tilts a transform towards a target rotation.
        Quaternion target;
        //jerk
        if (frames > 50)
        {
            target = Quaternion.Euler(tiltAroundX + x, 0, tiltAroundZ + z);
            x *= 1.0f;
            z *= 1.0f;
            frames = 0;
        }
        else
        {
            target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
        }
        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

    }
    #endregion
    #region Public Methods
    /// esense can return attention and meditation values
    /// MindwaveDataEegPowerModel returns public int delta; theta;lowAlpha;highAlpha;lowBeta;highBeta;lowGamma;highGamma;
    /// accessing theta and so on values by _Data.eegPower.delta
    /// Called when the MindwaveController sends new values.
    public void OnUpdateMindwaveData(MindwaveDataModel _Data)
    {
        Debug.Log("call me maybe");
        //string entry = "" + _Data.eSense.attention + "," + _Data.eSense.meditation + "," + _Data.eegPower.delta + "," + _Data.eegPower.theta + "," + _Data.eegPower.lowAlpha + "," + _Data.eegPower.highAlpha + "," + _Data.eegPower.lowBeta + "," + _Data.eegPower.highBeta + "," + _Data.eegPower.lowGamma + "," + _Data.eegPower.highGamma;
        //System.IO.File.WriteAllText(@"C:\ThingQbator\output\out" + 1 + ".txt", entry);
        Debug.Log("" + _Data.eSense.attention + "," + _Data.eSense.meditation + "," + _Data.eegPower.delta + "," + _Data.eegPower.theta + "," + _Data.eegPower.lowAlpha + "," + _Data.eegPower.highAlpha + "," + _Data.eegPower.lowBeta + "," + _Data.eegPower.highBeta + "," + _Data.eegPower.lowGamma + "," + _Data.eegPower.highGamma);
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine("" + _Data.eSense.attention + "," + _Data.eSense.meditation + "," + _Data.eegPower.delta + "," + _Data.eegPower.theta + "," + _Data.eegPower.lowAlpha + "," + _Data.eegPower.highAlpha + "," + _Data.eegPower.lowBeta + "," + _Data.eegPower.highBeta + "," + _Data.eegPower.lowGamma + "," + _Data.eegPower.highGamma);
        }
        //tiltAroundZ = map(_Data.eSense.attention,0,100, -1.85f, 0.85f) * -tiltAngle;
        //tiltAroundX = map(_Data.eSense.meditation, 0, 100, -1.28f, 0.85f) * tiltAngle;
        tiltAroundZ = sigmoid(_Data.eSense.attention) * -tiltAngle;
        tiltAroundX = sigmoid(_Data.eSense.meditation) * tiltAngle;

        // If the player is not in a specific phase
        if (m_CurrentPhase == Phase.None)
        {
            // If its meditation is sufficient
            if (_Data.eSense.meditation >= m_RequiredMeditation)
            {
                
                // Start Meditation phase
                // MOVE THE DAMN PLATFORM  along x!!!!!!!!!
            }
        }

        // If the player is in Meditation phase
        else if (m_CurrentPhase == Phase.Meditation)
        {
            if (_Data.eSense.meditation <= m_MinMeditation)
            {
                CancelMeditationPhase();
            }
        }

        // If the player is in focus phase
        else if (m_CurrentPhase == Phase.Focus)
        {
            if (_Data.eSense.attention >= m_RequiredAttention)
            {
                // Make the bomb explode
                // MOVE THE DAMN PLATFORM  along x!!!!!!!!!
            }
        }
    }

    #endregion
    #region Private Methods

    private void LoadBlast()
    {
        m_CurrentPhase = Phase.Meditation;
        CancelRoutine();
        m_Routine = StartCoroutine(EnterMeditationPhase(m_MeditationPhaseDuration, m_FocusPhaseDuration));
    }

    private IEnumerator EnterMeditationPhase(float _MeditationPhaseDuration, float _FocusPhaseDuration)
    {
        OnEnterMeditationPhase();
        m_CurrentPhase = Phase.Meditation;
        yield return new WaitForSeconds(_MeditationPhaseDuration);
        OnExitMeditationPhase();
        m_Routine = StartCoroutine(EnterFocusPhase(_FocusPhaseDuration));
    }

    private IEnumerator EnterFocusPhase(float _FocusPhaseDuration)
    {
        OnEnterFocusPhase();
        m_CurrentPhase = Phase.Focus;
        yield return new WaitForSeconds(_FocusPhaseDuration);
        OnCancelFocusPhase();
        CancelRoutine();
    }
    #endregion
    #region debug functions
    private void OnEnterMeditationPhase()
    {
        Debug.Log("Enter meditation phase");
    }

    private void OnCancelMeditationPhase()
    {
        Debug.Log("Cancel meditation phase");
    }

    private void OnExitMeditationPhase()
    {
        Debug.Log("Exit meditation phase");
    }
    private void printEEG(MindwaveDataModel _Data)
    {
        // _Data.eegPower.delta delta,theta,low alpha,high alpha,low beta,high beta,low gamma,high gamma
        // _Data.eSense.meditation
        string entry = "" + _Data.eSense.attention + "," + _Data.eSense.meditation + "," + _Data.eegPower.delta + "," + _Data.eegPower.theta + "," + _Data.eegPower.lowAlpha + "," + _Data.eegPower.highAlpha + "," + _Data.eegPower.lowBeta + "," + _Data.eegPower.highBeta + "," + _Data.eegPower.lowGamma + "," + _Data.eegPower.highGamma;
        System.IO.File.WriteAllText(@"C:\ThingQbator\output\out" + 1 + ".txt", entry);
        Debug.Log(entry);
    }
    #endregion
    #region Phases		
    private void OnEnterFocusPhase()
    {
        // move platform 
    }

    private void OnCancelFocusPhase()
    {
        // stop movement of platform
    }

    private void OnExitFocusPhase()
    {

    }
    #endregion
    #region routines		
    private void CancelRoutine()
    {
        if (m_Routine != null)
        {
            StopCoroutine(m_Routine);
            m_Routine = null;
        }
    }

    /// Reset mindwave
    private void CancelMeditationPhase()
    {
        CancelRoutine();
        m_CurrentPhase = Phase.None;
        OnCancelMeditationPhase();
    }

    private void gameOverScore() // restart , basically when the ball falls down
    {
        CancelRoutine();
        m_CurrentPhase = Phase.None;
        OnExitFocusPhase();
        // print score here
    }

    #endregion

}