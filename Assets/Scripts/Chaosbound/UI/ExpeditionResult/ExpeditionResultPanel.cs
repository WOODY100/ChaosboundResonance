using Chaosbound.Gameplay.ExpeditionRuntime.Result;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.UI.ExpeditionResult
{
    public sealed class ExpeditionResultPanel : MonoBehaviour
    {
        //==========================================================
        // References
        //==========================================================

        [Header("Header")]

        [SerializeField]
        private TMP_Text statusText;

        [SerializeField]
        private TMP_Text subtitleText;


        [Header("Summary")]

        [SerializeField]
        private TMP_Text survivalTimeValue;

        [SerializeField]
        private TMP_Text playerLevelValue;


        [Header("Build")]

        [SerializeField]
        private Image[] skillIcons;

        [SerializeField]
        private TMP_Text[] skillLevels;


        [Header("Combat")]

        [SerializeField]
        private TMP_Text enemiesDefeatedValue;


        [Header("Actions")]

        [SerializeField]
        private Button returnToSanctuaryButton;


        //==========================================================
        // Events
        //==========================================================

        public event Action ReturnToSanctuaryRequested;


        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            if (returnToSanctuaryButton != null)
            {
                returnToSanctuaryButton.onClick.AddListener(
                    HandleReturnToSanctuaryClicked);
            }
        }

        private void OnDestroy()
        {
            if (returnToSanctuaryButton != null)
            {
                returnToSanctuaryButton.onClick.RemoveListener(
                    HandleReturnToSanctuaryClicked);
            }
        }


        //==========================================================
        // Presentation
        //==========================================================

        public void Show(ExpeditionResultData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            SetStatus(data.Status);
            SetSurvivalTime(data.ElapsedTime);
            SetPlayerLevel(data.PlayerLevel);
            SetEnemiesDefeated(data.EnemiesDefeated);
            SetSkillIcons(data.Skills);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }


        //==========================================================
        // Status
        //==========================================================

        private void SetStatus(ExpeditionResultStatus status)
        {
            if (statusText == null)
                return;

            switch (status)
            {
                case ExpeditionResultStatus.Failed:
                    statusText.text = "EXPEDITION FAILED";

                    if (subtitleText != null)
                        subtitleText.text =
                            "The expedition has come to an end.";

                    break;

                case ExpeditionResultStatus.Completed:
                    statusText.text = "EXPEDITION COMPLETE";

                    if (subtitleText != null)
                        subtitleText.text =
                            "The expedition has been completed.";

                    break;
            }
        }


        //==========================================================
        // Summary
        //==========================================================

        private void SetSurvivalTime(float elapsedTime)
        {
            if (survivalTimeValue == null)
                return;

            elapsedTime = Mathf.Max(0f, elapsedTime);

            int totalSeconds = Mathf.FloorToInt(elapsedTime);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            survivalTimeValue.text =
                $"{minutes:00}:{seconds:00}";
        }

        private void SetPlayerLevel(int level)
        {
            if (playerLevelValue == null)
                return;

            playerLevelValue.text =
                Mathf.Max(0, level).ToString();
        }


        //==========================================================
        // Build
        //==========================================================

        private void SetSkillIcons(
    RuntimeSkill[] skills)
        {
            if (skillIcons == null)
                return;

            for (int i = 0; i < skillIcons.Length; i++)
            {
                RuntimeSkill skill =
                    skills != null &&
                    i < skills.Length
                        ? skills[i]
                        : null;

                //========================================
                // Skill Icon
                //========================================

                if (skillIcons[i] != null)
                {
                    Sprite skillIcon =
                        skill != null &&
                        skill.Definition != null
                            ? skill.Definition.Icon
                            : null;

                    skillIcons[i].sprite = skillIcon;
                    skillIcons[i].enabled = skillIcon != null;
                }

                //========================================
                // Skill Level
                //========================================

                if (skillLevels != null &&
                    i < skillLevels.Length &&
                    skillLevels[i] != null)
                {
                    if (skill != null)
                    {
                        skillLevels[i].gameObject.SetActive(true);
                        skillLevels[i].text =
                            $"Lv. {skill.Level}";
                    }
                    else
                    {
                        skillLevels[i].gameObject.SetActive(false);
                    }
                }
            }
        }


        //==========================================================
        // Combat
        //==========================================================

        private void SetEnemiesDefeated(int enemiesDefeated)
        {
            if (enemiesDefeatedValue == null)
                return;

            enemiesDefeatedValue.text =
                Mathf.Max(0, enemiesDefeated).ToString();
        }


        //==========================================================
        // Actions
        //==========================================================

        private void HandleReturnToSanctuaryClicked()
        {
            ReturnToSanctuaryRequested?.Invoke();
        }
    }
}