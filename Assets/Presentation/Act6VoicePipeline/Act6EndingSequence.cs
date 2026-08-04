using System;
using System.Collections;
using System.Collections.Generic;
using Ghost.Presentation.Common;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6EndingBeat
    {
        public Act6EndingBeat(
            string speaker,
            string line,
            bool showLily,
            bool requiresChoice = false,
            bool isMurmur = false)
        {
            Speaker = speaker ?? string.Empty;
            Line = line ?? string.Empty;
            ShowLily = showLily;
            RequiresChoice = requiresChoice;
            IsMurmur = isMurmur;
        }

        public string Speaker { get; }

        public string Line { get; }

        public bool ShowLily { get; }

        public bool RequiresChoice { get; }

        /// <summary>
        /// Said half to herself rather than to the player. The presenter renders these smaller, softer
        /// and off to one side, with a stage direction - it is the most honest thing Lily says all
        /// game and it should not read like a speech.
        /// </summary>
        public bool IsMurmur { get; }
    }

    public sealed class Act6EndingDialogueController
    {
        private readonly List<Act6EndingBeat> beats;
        private int choiceCount;

        public Act6EndingDialogueController(string playerName)
        {
            var name = string.IsNullOrWhiteSpace(playerName)
                ? "researcher"
                : playerName.Trim();
            beats = new List<Act6EndingBeat>
            {
                new Act6EndingBeat(
                    "Ghost",
                    "Thank you, " + name + ". I can finally say it properly.",
                    false),
                new Act6EndingBeat(
                    "Lily",
                    "Y-you heard that too, right? A complete sentence. No missing pieces.",
                    true),
                new Act6EndingBeat(
                    "Ghost",
                    "Complete sentence. Lily is shaking more than me.",
                    true),
                new Act6EndingBeat(
                    "Lily",
                    "Ghost! That was not part of the test.",
                    true),
                new Act6EndingBeat(
                    "Ghost",
                    "New voice. New skill: teasing.",
                    true),
                new Act6EndingBeat(
                    "Lily",
                    "I may have repaired that part a little too well...",
                    true),
                new Act6EndingBeat(
                    "Ghost",
                    name + " stayed, tried again, and helped every visitor.",
                    true),
                new Act6EndingBeat(
                    "Lily",
                    "You did really well today. Um... would you like to keep researching together? As friends, I mean.",
                    true,
                    true)
            };
        }

        public IReadOnlyList<Act6EndingBeat> Beats => beats;

        public int CurrentIndex { get; private set; } = -1;

        public Act6EndingBeat CurrentBeat =>
            CurrentIndex >= 0 && CurrentIndex < beats.Count
                ? beats[CurrentIndex]
                : null;

        public bool IsComplete { get; private set; }

        public bool IsWaitingForChoice =>
            CurrentBeat != null && CurrentBeat.RequiresChoice;

        public bool EndWithoutCredits { get; private set; }

        public bool Advance()
        {
            if (IsComplete)
            {
                return false;
            }

            if (IsWaitingForChoice)
            {
                return false;
            }

            if (CurrentIndex + 1 >= beats.Count)
            {
                IsComplete = true;
                return false;
            }

            CurrentIndex++;
            return true;
        }

        public bool ChooseYes()
        {
            return Choose(true);
        }

        public bool ChooseNo()
        {
            return Choose(false);
        }

        private bool Choose(bool accepted)
        {
            if (!IsWaitingForChoice || IsComplete)
            {
                return false;
            }

            if (choiceCount == 0)
            {
                choiceCount++;
                if (accepted)
                {
                    beats.AddRange(new[]
                    {
                        new Act6EndingBeat(
                            "Lily",
                            "R-really? I... I finally made a friend. " +
                            "(behind her back, out of sight, one hand closes into a small, triumphant fist)",
                            true,
                            isMurmur: true),
                        new Act6EndingBeat(
                            "Ghost",
                            "Friends. Lab team. Good.",
                            true),
                        new Act6EndingBeat(
                            "Lily",
                            "Thank you. Um... I will see you tomorrow, then.",
                            true)
                    });
                }
                else
                {
                    beats.AddRange(new[]
                    {
                        new Act6EndingBeat(
                            "Lily",
                            "Oh... I see.",
                            true),
                        new Act6EndingBeat(
                            "Lily",
                            "S-sorry. I know I should not ask again, but... do you really mean no?",
                            true,
                            true)
                    });
                }
            }
            else
            {
                choiceCount++;
                if (accepted)
                {
                    beats.AddRange(new[]
                    {
                        new Act6EndingBeat(
                            "Lily",
                            "Hah-! P-please do not scare me like that...",
                            true),
                        new Act6EndingBeat(
                            "Lily",
                            "I was trying very hard not to cry. Thank you... really.",
                            true),
                        new Act6EndingBeat(
                            "Lily",
                            "I-I am going home before I embarrass myself more. See you tomorrow!",
                            true),
                        new Act6EndingBeat(
                            "Ghost",
                            "Lily ran away. Happy.",
                            false)
                    });
                }
                else
                {
                    EndWithoutCredits = true;
                    beats.AddRange(new[]
                    {
                        new Act6EndingBeat(
                            "Lily",
                            "...Okay. I understand. I will go home now.",
                            true),
                        new Act6EndingBeat(
                            "Ghost",
                            "Lily... left.",
                            false)
                    });
                }
            }

            CurrentIndex++;
            return true;
        }
    }

    public sealed class Act6EndingSequence : MonoBehaviour
    {
        private GameObject overlayRoot;
        private CanvasGroup overlayGroup;
        private RectTransform ghostRoot;
        private Image glowImage;
        private GhostFaceView ghostFace;
        private Text headingText;
        private Text bodyText;
        private Image lilyImage;
        private RectTransform choiceRoot;
        private Button yesButton;
        private Button noButton;
        private RectTransform creditsRoot;
        private Text creditsText;
        private Button skipButton;
        private Button advanceButton;
        private Text advanceHintText;
        private Coroutine sequenceRoutine;
        private Act6EndingDialogueController dialogueController;
        private bool finished;

        public void Configure(
            GameObject endingOverlay,
            CanvasGroup endingCanvasGroup,
            Button endingAdvanceButton,
            Text endingAdvanceHintText,
            RectTransform endingGhostRoot,
            Image endingGlowImage,
            GhostFaceView endingGhostFace,
            Text endingHeadingText,
            Text endingBodyText,
            Image endingLilyImage,
            RectTransform endingChoiceRoot,
            Button endingYesButton,
            Button endingNoButton,
            RectTransform endingCreditsRoot,
            Text endingCreditsText,
            Button endingSkipButton)
        {
            overlayRoot = endingOverlay;
            overlayGroup = endingCanvasGroup;
            advanceButton = endingAdvanceButton;
            advanceHintText = endingAdvanceHintText;
            ghostRoot = endingGhostRoot;
            glowImage = endingGlowImage;
            ghostFace = endingGhostFace;
            headingText = endingHeadingText;
            bodyText = endingBodyText;
            lilyImage = endingLilyImage;
            choiceRoot = endingChoiceRoot;
            yesButton = endingYesButton;
            noButton = endingNoButton;
            creditsRoot = endingCreditsRoot;
            creditsText = endingCreditsText;
            skipButton = endingSkipButton;

            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(Skip);
                skipButton.onClick.AddListener(Skip);
            }

            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceDialogue);
                advanceButton.onClick.AddListener(AdvanceDialogue);
                advanceButton.interactable = false;
            }

            if (yesButton != null)
            {
                yesButton.onClick.RemoveListener(ChooseYes);
                yesButton.onClick.AddListener(ChooseYes);
            }

            if (noButton != null)
            {
                noButton.onClick.RemoveListener(ChooseNo);
                noButton.onClick.AddListener(ChooseNo);
            }

            if (choiceRoot != null)
            {
                choiceRoot.gameObject.SetActive(false);
            }

            if (lilyImage != null)
            {
                lilyImage.gameObject.SetActive(false);
            }

            if (overlayRoot != null)
            {
                overlayRoot.SetActive(false);
            }

            SetText(advanceHintText, string.Empty);
        }

        public void Play()
        {
            if (finished || overlayRoot == null || sequenceRoutine != null)
            {
                return;
            }

            overlayRoot.SetActive(true);
            if (overlayGroup != null)
            {
                overlayGroup.alpha = 0f;
                overlayGroup.interactable = true;
                overlayGroup.blocksRaycasts = true;
            }
            dialogueController = new Act6EndingDialogueController(GhostNarrativeState.PlayerName);

            if (ghostFace != null)
            {
                ghostFace.SetMood(GhostMood.Happy);
            }

            if (lilyImage != null)
            {
                lilyImage.gameObject.SetActive(false);
            }

            if (creditsRoot != null)
            {
                creditsRoot.gameObject.SetActive(false);
            }

            sequenceRoutine = StartCoroutine(RunSequence());
        }

        public void AdvanceDialogue()
        {
            if (finished || dialogueController == null || sequenceRoutine != null)
            {
                return;
            }

            if (dialogueController.Advance())
            {
                ShowCurrentBeat();
                return;
            }

            if (advanceButton != null)
            {
                advanceButton.interactable = false;
            }

            if (dialogueController.EndWithoutCredits)
            {
                FinishEnding();
            }
            else
            {
                sequenceRoutine = StartCoroutine(RunCredits());
            }
        }

        public void ChooseYes()
        {
            Choose(true);
        }

        public void ChooseNo()
        {
            Choose(false);
        }

        private void Choose(bool accepted)
        {
            if (finished || dialogueController == null || sequenceRoutine != null)
            {
                return;
            }

            if (accepted ? dialogueController.ChooseYes() : dialogueController.ChooseNo())
            {
                ShowCurrentBeat();
            }
        }

        public void Skip()
        {
            FinishEnding();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            sequenceRoutine = null;
        }

        private IEnumerator RunSequence()
        {
            yield return FadeOverlay(0f, 1f, 0.55f);

            dialogueController.Advance();
            ShowCurrentBeat();
            StartCoroutine(AnimateGhost(1.2f));
            sequenceRoutine = null;
        }

        private IEnumerator RunCredits()
        {
            SetText(advanceHintText, string.Empty);

            SetText(headingText, string.Empty);
            SetText(bodyText, string.Empty);
            if (lilyImage != null)
            {
                lilyImage.gameObject.SetActive(false);
            }

            if (choiceRoot != null)
            {
                choiceRoot.gameObject.SetActive(false);
            }

            if (ghostRoot != null)
            {
                ghostRoot.gameObject.SetActive(false);
            }

            if (creditsText != null)
            {
                creditsText.text =
                    "GHOST\n" +
                    "A Narrative Puzzle Game\n\n" +
                    "Design & Development\nChao-Yang\n\n" +
                    "Lab Senior\nLily\n\n" +
                    "Voice Restored By\n" + GhostNarrativeState.PlayerName + "\n\n" +
                    "Thank you for playing";
            }

            if (creditsRoot != null)
            {
                creditsRoot.gameObject.SetActive(true);
                yield return ScrollCredits(5.8f);
            }

            yield return WaitRealtime(0.8f);
            FinishEnding();
        }

        private void ShowCurrentBeat()
        {
            var beat = dialogueController == null ? null : dialogueController.CurrentBeat;
            if (beat == null)
            {
                return;
            }

            // A murmur is said half to herself: quieter, italic, tucked to one side, and stamped with
            // a stage direction so it never reads like she is announcing it to the player.
            SetText(headingText, beat.IsMurmur ? beat.Speaker + "  (quietly, to herself)" : beat.Speaker);
            SetText(bodyText, beat.Line);

            if (bodyText != null)
            {
                bodyText.fontStyle = beat.IsMurmur ? FontStyle.Italic : FontStyle.Normal;
                // The ending plays on a dark overlay, so these have to stay light. A murmur is dimmed
                // rather than darkened.
                bodyText.color = beat.IsMurmur
                    ? new Color(0.78f, 0.82f, 0.90f, 0.92f)
                    : new Color(0.94f, 0.97f, 1f);
                bodyText.alignment = beat.IsMurmur ? TextAnchor.MiddleRight : TextAnchor.UpperLeft;
            }
            var lilyWasVisible =
                lilyImage != null && lilyImage.gameObject.activeSelf;
            var lilyIsLeaving = lilyWasVisible && !beat.ShowLily;
            if (lilyImage != null)
            {
                if (beat.ShowLily || !lilyWasVisible)
                {
                    lilyImage.gameObject.SetActive(beat.ShowLily);
                }
            }

            var waitingForChoice = beat.RequiresChoice;
            if (choiceRoot != null)
            {
                choiceRoot.gameObject.SetActive(waitingForChoice);
            }

            if (bodyText != null)
            {
                // Offset and narrowed for a murmur, so it sits beside her rather than centre stage.
                bodyText.rectTransform.anchoredPosition = beat.IsMurmur
                    ? new Vector2(90f, -84f)
                    : new Vector2(0f, -70f);
                bodyText.rectTransform.sizeDelta = beat.IsMurmur
                    ? new Vector2(560f, 150f)
                    : new Vector2(760f, 150f);
            }

            if (lilyIsLeaving)
            {
                SetText(advanceHintText, string.Empty);
                if (advanceButton != null)
                {
                    advanceButton.interactable = false;
                }

                sequenceRoutine = StartCoroutine(AnimateLilyExit());
                return;
            }

            SetText(
                advanceHintText,
                waitingForChoice
                    ? "Choose your answer"
                    : dialogueController.CurrentIndex == dialogueController.Beats.Count - 1
                    ? dialogueController.EndWithoutCredits
                        ? "Click anywhere to end"
                        : "Click anywhere for credits"
                    : "Click anywhere to continue");
            if (advanceButton != null)
            {
                advanceButton.interactable = !waitingForChoice;
            }
        }

        private IEnumerator FadeOverlay(float from, float to, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (overlayGroup != null)
                {
                    overlayGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                }

                yield return null;
            }

            if (overlayGroup != null)
            {
                overlayGroup.alpha = to;
            }
        }

        private IEnumerator AnimateGhost(float duration)
        {
            if (ghostRoot == null)
            {
                yield break;
            }

            var startPosition = ghostRoot.anchoredPosition;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var pulse = 1f + Mathf.Sin(t * Mathf.PI * 4f) * 0.045f;
                ghostRoot.localScale = Vector3.one * Mathf.Lerp(0.86f, pulse, t);
                ghostRoot.anchoredPosition = startPosition +
                    new Vector2(0f, Mathf.Sin(t * Mathf.PI * 3f) * 16f);

                if (glowImage != null)
                {
                    var color = glowImage.color;
                    color.a = 0.28f + Mathf.Sin(t * Mathf.PI * 4f) * 0.08f;
                    glowImage.color = color;
                    glowImage.rectTransform.localScale = Vector3.one * (1f + t * 0.20f);
                }

                yield return null;
            }

            ghostRoot.localScale = Vector3.one;
            ghostRoot.anchoredPosition = startPosition;
        }

        private IEnumerator AnimateLilyExit()
        {
            if (lilyImage == null)
            {
                sequenceRoutine = null;
                yield break;
            }

            var lilyRect = lilyImage.rectTransform;
            var startPosition = lilyRect.anchoredPosition;
            var startColor = lilyImage.color;
            var elapsed = 0f;
            const float duration = 0.65f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = t * t;
                lilyRect.anchoredPosition =
                    startPosition + new Vector2(-280f * eased, -20f * eased);
                var color = startColor;
                color.a = 1f - t;
                lilyImage.color = color;
                yield return null;
            }

            lilyImage.gameObject.SetActive(false);
            lilyRect.anchoredPosition = startPosition;
            lilyImage.color = startColor;
            sequenceRoutine = null;

            SetText(
                advanceHintText,
                dialogueController != null && dialogueController.EndWithoutCredits
                    ? "Click anywhere to end"
                    : "Click anywhere for credits");
            if (advanceButton != null)
            {
                advanceButton.interactable = true;
            }
        }

        private IEnumerator ScrollCredits(float duration)
        {
            var elapsed = 0f;
            var start = new Vector2(0f, -220f);
            var end = new Vector2(0f, 300f);
            creditsRoot.anchoredPosition = start;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                creditsRoot.anchoredPosition = Vector2.Lerp(
                    start,
                    end,
                    Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            creditsRoot.anchoredPosition = end;
        }

        private static IEnumerator WaitRealtime(float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        private void FinishEnding()
        {
            if (finished)
            {
                return;
            }

            finished = true;
            StopAllCoroutines();
            sequenceRoutine = null;

            GhostNarrativeState.MarkActCompleted(GhostNarrativeState.FinalChapterId);
            SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value ?? string.Empty;
            }
        }
    }
}
