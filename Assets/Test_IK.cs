using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Test_IK : MonoBehaviour
{
    [SerializeField, Tooltip("IK用アニメーター")]
    private Animator p_Animator;

    [SerializeField, Tooltip("IKのターゲット")]
    private GameObject IKTarget;

    void Start()
    {
        // Animatorの参照を取得する
        p_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        IKTarget.transform.position = GameObject.Find("J_Bip_R_Hand").transform.position;
        IKTarget.transform.rotation = GameObject.Find("J_Bip_R_Hand").transform.rotation;
    }

    // IK更新時に呼ばれる関数
    // IKPassにチェックを入れた場合のみ呼び出される
    void OnAnimatorIK()
    {
        if (IKTarget == null) return;

        // 右足のIKを有効化する(重み:1.0)
        p_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);　//位置
        p_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);　//回転

        // 右足のIKのターゲットを設定する
        p_Animator.SetIKPosition(AvatarIKGoal.RightHand, IKTarget.transform.position);　//位置
        p_Animator.SetIKRotation(AvatarIKGoal.RightHand, IKTarget.transform.rotation);  //回転
    }
}