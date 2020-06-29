﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roundbeargames
{
    [CreateAssetMenu(fileName = "New State", menuName = "Roundbeargames/AbilityData/InstantTransition")]
    public class InstantTransition : StateData
    {
        public Instant_Transition_States TransitionTo;
        public List<TransitionConditionType> transitionConditions = new List<TransitionConditionType>();
        public float CrossFade;
        public float Offset;

        public override void OnEnter(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {
            if (!Interfered(characterState.characterControl))
            {
                if (TransitionConditionChecker.MakeTransition(characterState.characterControl, transitionConditions))
                {
                    characterState.ANIMATION_DATA.InstantTransitionMade = true;
                    MakeInstantTransition(characterState.characterControl);
                }
            }
        }

        public override void OnExit(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {
            characterState.ANIMATION_DATA.InstantTransitionMade = false;
        }

        void MakeInstantTransition(CharacterControl control)
        {
            if (CrossFade <= 0f)
            {
                control.SkinnedMeshAnimator.Play(
                    HashManager.Instance.DicInstantTransitionStates[TransitionTo], 0);
            }
            else
            {
                if (Offset <= 0f)
                {
                    control.SkinnedMeshAnimator.CrossFade(
                        HashManager.Instance.DicInstantTransitionStates[TransitionTo],
                        CrossFade, 0);
                }
                else
                {
                    control.SkinnedMeshAnimator.CrossFade(
                        HashManager.Instance.DicInstantTransitionStates[TransitionTo],
                        CrossFade, 0, Offset);
                }
            }
        }

        bool Interfered(CharacterControl control)
        {
            if (control.animationProgress.LockTransition)
            {
                return true;
            }

            if (control.ANIMATION_DATA.InstantTransitionMade)
            {
                return true;
            }

            if (control.SkinnedMeshAnimator.GetInteger(
                HashManager.Instance.ArrMainParams[(int)MainParameterType.TransitionIndex]) != 0)
            {
                return true;
            }

            if (control.ANIMATION_DATA.IsRunning(typeof(Attack)))
            {
                return true;
            }

            AnimatorStateInfo nextInfo = control.SkinnedMeshAnimator.GetNextAnimatorStateInfo(0);

            if (nextInfo.shortNameHash == HashManager.Instance.DicInstantTransitionStates[TransitionTo])
            {
                return true;
            }
            
            return false;
        }
    }
}