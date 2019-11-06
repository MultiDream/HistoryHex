using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {
    [SerializeField] private Vector3 Offset;
    [SerializeField] private AnimationCurve MoveCurve;
    [SerializeField] private float MoveTime;
    [SerializeField] private float Delay;
    Vector3 startPos;
    Vector3 endPos;
    // Start is called before the first frame update
    void Start() {
        startPos = transform.localPosition + Offset;
        endPos = transform.localPosition;
        transform.localPosition = startPos;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move() {
        transform.localPosition = Vector3.Lerp(startPos, endPos, MoveCurve.Evaluate(0));
        yield return new WaitForSeconds(Delay);
        float time;
        float start = Time.timeSinceLevelLoad;
        do {
            time = Time.timeSinceLevelLoad;
            transform.localPosition = Vector3.Lerp(startPos, endPos, MoveCurve.Evaluate((time - start) / MoveTime));
            yield return null;
        }
        while (time < start + MoveTime);
        transform.localPosition = Vector3.Lerp(startPos, endPos, MoveCurve.Evaluate(1));
    }
}