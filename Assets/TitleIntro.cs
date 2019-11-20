using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleIntro : MonoBehaviour {
    [SerializeField] private AnimationCurve IntroCurve;
    [SerializeField] private float DissolveTime;
    [SerializeField] private float SheenTime;
    [SerializeField] private float Delay;
    Material m;
    // Start is called before the first frame update
    void Start() {
        m = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move() {
        m.SetFloat("_Dissolve", 1);
        m.SetFloat("_Sheen", -1);
        yield return new WaitForSeconds(Delay);
        float time;
        float start = Time.timeSinceLevelLoad;
        do {
            time = Time.timeSinceLevelLoad;
            m.SetFloat("_Dissolve", IntroCurve.Evaluate(1 - ((time - start) / DissolveTime)));
            yield return null;
        }
        while (time < start + DissolveTime);
        m.SetFloat("_Dissolve", 0);

        start = Time.timeSinceLevelLoad;
        do {
            time = Time.timeSinceLevelLoad;
            m.SetFloat("_Sheen", Mathf.Lerp(-1f, 2f, IntroCurve.Evaluate(((time - start) / SheenTime))));
            yield return null;
        }
        while (time < start + SheenTime);
        m.SetFloat("_Sheen", 2f);
    }

    private void OnDestroy()
    {
        Destroy(m);
    }
}