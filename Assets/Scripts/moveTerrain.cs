using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Scripts/Game/Brainwaves")]
public class moveTerrain : MonoBehaviour
{
    #region Subclasses

    ///<summary>
    ///	Defines a pattern for a specific phase
    ///</summary>
    [System.Serializable]
    private struct ESensePattern
    {
        [Range(0.0f, 100.0f)]
        [Tooltip("The value to reach")]
        public float target;

        [Range(0.0f, 100.0f)]
        [Tooltip("Target value will be reached in that given duration")]
        public float increaseDuration;

        [Range(0.0f, 100.0f)]
        [Tooltip("Defines a value to hold after the phase is ended")]
        public float valueAfterIncreaseEnd;
    }

    #endregion

    #region Attributes

    // Delegates

    // Defines a delegate for OnUpdateMindwaveData event
    public delegate void MindwaveDataDelegate(MindwaveDataModel _Data);

    // Triggered each frame
    public event MindwaveDataDelegate OnUpdateMindwaveData;

    // Settings

    [Header("Settings")]

    [SerializeField]
    [Tooltip("Defines a pattern for meditation phase")]
    private ESensePattern m_MeditationPattern;

    [SerializeField, Range(0.0f, 100.0f)]
    [Tooltip("Defines a \"sleep\" time between meditation and focus phases, before meditation goes to its valueAfterIncreaseEnd")]
    private float m_PauseBetweenPhases = 5.0f;

    [SerializeField]
    [Tooltip("Defines a pattern for attention phase")]
    private ESensePattern m_AttentionPattern;

    // Flow

    // The current meditation value
    private float m_Meditation = 0.0f;
    //The current attention value
    private float m_Attention = 0.0f;

    #endregion

    #region localVariables
    float smooth = 5.0f;
    float tiltAngle = 60.0f;
    float x, z;
    int frames;
    #endregion

    void Start()
    {
        StartCoroutine(StartTesting(m_MeditationPattern, m_AttentionPattern));
        frames = 0;
        x = Random.Range(1, 5);
        z = Random.Range(1, 5);
        

    }
    // Update is called once per frame
    void Update()
    {
        //MindwaveManager.Instance.Controller.
        //Debug.Log("att : "+MakeMindwaveData(m_Meditation, m_Attention).eSense.attention);
        //OnUpdateMindwaveData += OnUpdateMindwaveData;

        frames++;
        // Smoothly tilts a transform towards a target rotation.
        float tiltAroundZ = Input.GetAxis("Horizontal") * -tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;

        Quaternion target;
        if (frames > 50)
        {
            target = Quaternion.Euler(tiltAroundX + x, 0, tiltAroundZ + z);
            x *= 1.1f;
            z *= 1.1f;
            frames = 0;
        }
        else
        {
            target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
        }
        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

    }

    #region Private Methods

    /// <summary>
    /// Coroutine: Start increase meditation, pause the script, then increase attention.
    /// </summary>
    private IEnumerator StartTesting(ESensePattern _MeditationPattern, ESensePattern _AtrtentionPattern)
    {
        float timer = 0.0f;
        // Increase meditation during the meditation pattern's increaseDuration
        while (timer < _MeditationPattern.increaseDuration)
        {
            m_Meditation = Mathf.Lerp(0.0f, m_MeditationPattern.target, (timer / m_MeditationPattern.increaseDuration));
            m_Meditation = (m_Meditation > 100.0f) ? 100.0f : m_Meditation;

            // Trigger update event
            if (OnUpdateMindwaveData != null)
            {
                OnUpdateMindwaveData(MakeMindwaveData(m_Meditation, m_Attention));
            }

            timer += Time.deltaTime;
            yield return null;
        }

        m_Meditation = m_MeditationPattern.target;
        // Pause current phase
        yield return new WaitForSeconds(m_PauseBetweenPhases);
        // Reset meditation to its valueAfterIncreaseEnd
        m_Meditation = m_MeditationPattern.valueAfterIncreaseEnd;

        timer = 0.0f;
        // Increase attention during the attention pattern's increaseDuration
        while (timer < m_AttentionPattern.increaseDuration)
        {
            m_Attention = Mathf.Lerp(0.0f, m_AttentionPattern.target, (timer / m_AttentionPattern.increaseDuration));
            m_Attention = (m_Attention > 100.0f) ? 100.0f : m_Attention;

            // Trigger update event
            if (OnUpdateMindwaveData != null)
            {
                OnUpdateMindwaveData(MakeMindwaveData(m_Meditation, m_Attention));
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset attention to its valueAfterIncreaseEnd
        m_Attention = m_AttentionPattern.valueAfterIncreaseEnd;
    }

    /// <summary>
    /// Makes an instance of MindwaveDataModel with the given meditation and attention as eSense values.
    /// </summary>
    private MindwaveDataModel MakeMindwaveData(float _Meditation, float _Attention)
    {
        MindwaveDataModel data = new MindwaveDataModel();
        data.eSense.meditation = Mathf.FloorToInt(_Meditation);
        data.eSense.attention = Mathf.FloorToInt(_Attention);

        return data;
    }

    #endregion


}