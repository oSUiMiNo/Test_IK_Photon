using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Test_IK : MonoBehaviour
{
    [SerializeField, Tooltip("IK�p�A�j���[�^�[")]
    private Animator p_Animator;

    [SerializeField, Tooltip("IK�̃^�[�Q�b�g")]
    private GameObject IKTarget;

    void Start()
    {
        // Animator�̎Q�Ƃ��擾����
        p_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        IKTarget.transform.position = GameObject.Find("J_Bip_R_Hand").transform.position;
        IKTarget.transform.rotation = GameObject.Find("J_Bip_R_Hand").transform.rotation;
    }

    // IK�X�V���ɌĂ΂��֐�
    // IKPass�Ƀ`�F�b�N����ꂽ�ꍇ�̂݌Ăяo�����
    void OnAnimatorIK()
    {
        if (IKTarget == null) return;

        // �E����IK��L��������(�d��:1.0)
        p_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);�@//�ʒu
        p_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);�@//��]

        // �E����IK�̃^�[�Q�b�g��ݒ肷��
        p_Animator.SetIKPosition(AvatarIKGoal.RightHand, IKTarget.transform.position);�@//�ʒu
        p_Animator.SetIKRotation(AvatarIKGoal.RightHand, IKTarget.transform.rotation);  //��]
    }
}