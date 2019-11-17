using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimaControllerLikelihoodPair
{
    public RuntimeAnimatorController anim;
    [Range(0.0f, 1.0f)]
    public float likelihood;
}

[RequireComponent(typeof(Animator))]
public class UnitSprite : MonoBehaviour
{
    public float idlePoseLoopTime = 1.0f;
    public float poseRepeatRangeMin = 2.0f;
    public float poseRepeatRangeMax = 4.0f;
    float poseRepeatCountdown;
    bool isPosing;

    Animator anim;

    public AnimaControllerLikelihoodPair[] animatorsAndLikelihoods;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        poseRepeatCountdown = Random.Range(0.0f, 1.0f);

        // pick a random animator
        bool pickedAnimator = false;
        RuntimeAnimatorController animController;
        int iterations = 0;
        while (!pickedAnimator && iterations++ < 10)
        {
            int randAnimator = Random.Range(0, animatorsAndLikelihoods.Length);
            
            if (Random.Range(0.0f, 1.0f) <= animatorsAndLikelihoods[randAnimator].likelihood)
            {
                pickedAnimator = true;
                animController = animatorsAndLikelihoods[randAnimator].anim;

                anim.runtimeAnimatorController = animController;
            }
        }
        if (!pickedAnimator)
        {

            pickedAnimator = true;
            animController = animatorsAndLikelihoods[0].anim;

            anim.runtimeAnimatorController = animController;
        }

        StartPosing();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePose();
    }

    void UpdatePose()
    {
        poseRepeatCountdown -= Time.deltaTime;
        if (poseRepeatCountdown < 0)
        {
            poseRepeatCountdown = Random.Range(poseRepeatRangeMin, poseRepeatRangeMax);

            // play pose animation
            anim.SetTrigger("SinglePose");
        }
    }

    public void StartPosing()
    {
        isPosing = true;
        anim.SetBool("IsSelected", true);
    }

    public void StopPosing()
    {
        isPosing = false;
        anim.SetBool("IsSelected", false);
    }
}
