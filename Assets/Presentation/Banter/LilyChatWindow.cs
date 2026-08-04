using Ghost.Presentation.Common;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public sealed class LilyChatWindow : MonoBehaviour
    {
        private const string ChatCanvasName = "Lily Chat Canvas";
        private const string ChatWindowName = "Lily Chat Window";
        private const int ChatSortingOrder = 180;
        private const int MaxHistoryTurns = 10;

        private static LilyChatWindow activeWindow;

        private RectTransform panelRoot;
        private RectTransform messageContentRoot;
        private ScrollRect scrollRect;
        private InputField inputField;
        private Button sendButton;
        private Button closeButton;
        private Text sendButtonLabel;
        private string actId;
        private string stateSummary;
        private string trigger;
        private bool requestInFlight;

        private readonly List<ChatHistoryItem> history = new List<ChatHistoryItem>();

        public static void Open(
            string requestedActId,
            string openingLine,
            string requestedStateSummary = "",
            string requestedTrigger = "manual_ask_lily")
        {
            EnsureEventSystem();
            var window = EnsureWindow();
            window.OpenInternal(
                requestedActId,
                openingLine,
                requestedStateSummary,
                requestedTrigger);
        }

        private static LilyChatWindow EnsureWindow()
        {
            if (activeWindow != null)
            {
                return activeWindow;
            }

            var existing = GameObject.Find(ChatWindowName);
            if (existing != null)
            {
                activeWindow = existing.GetComponent<LilyChatWindow>();
                if (activeWindow != null)
                {
                    return activeWindow;
                }
            }

            var canvas = EnsureCanvas();
            return CreateWindow(canvas.transform);
        }

        private static Canvas EnsureCanvas()
        {
            var existingCanvasObject = GameObject.Find(ChatCanvasName);
            if (existingCanvasObject != null)
            {
                var existingCanvas = existingCanvasObject.GetComponent<Canvas>();
                if (existingCanvas != null)
                {
                    return existingCanvas;
                }
            }

            var canvasObject = new GameObject(ChatCanvasName, typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = ChatSortingOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static LilyChatWindow CreateWindow(Transform parent)
        {
            var root = new GameObject(ChatWindowName, typeof(RectTransform));
            root.transform.SetParent(parent, false);

            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(520f, 0f);
            rect.sizeDelta = new Vector2(560f, 520f);

            var image = GhostUITheme.Panel(root, new Color(1f, 0.98f, 0.93f, 0.98f));
            image.raycastTarget = true;

            var layout = root.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var window = root.AddComponent<LilyChatWindow>();
            window.panelRoot = rect;
            window.BuildUi(root.transform);
            root.SetActive(false);
            activeWindow = window;
            return window;
        }

        private void BuildUi(Transform parent)
        {
            CreateHeader(parent);
            CreateMessageScroll(parent);
            CreateInputRow(parent);
        }

        private void CreateHeader(Transform parent)
        {
            var header = new GameObject("Chat Header", typeof(RectTransform));
            header.transform.SetParent(parent, false);

            var headerLayoutElement = header.AddComponent<LayoutElement>();
            headerLayoutElement.minHeight = 42f;
            headerLayoutElement.preferredHeight = 42f;

            var headerImage = GhostUITheme.Card(header, new Color(1f, 1f, 1f, 0.01f));
            headerImage.raycastTarget = true;

            var dragHandle = header.AddComponent<FloatingWindowDragHandle>();
            dragHandle.Configure(panelRoot);

            var layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var title = GhostUITheme.Label(
                "Chat Title",
                header.transform,
                "Ask Lily",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            var titleLayout = title.gameObject.AddComponent<LayoutElement>();
            titleLayout.flexibleWidth = 1f;
            titleLayout.minHeight = 38f;

            closeButton = GhostUITheme.PushButton("Close Button", header.transform, "Close");
            var closeLayout = closeButton.gameObject.AddComponent<LayoutElement>();
            closeLayout.minWidth = 72f;
            closeLayout.preferredWidth = 72f;
            closeLayout.minHeight = 38f;
            closeLayout.preferredHeight = 38f;
            closeButton.onClick.AddListener(Close);
        }

        private void CreateMessageScroll(Transform parent)
        {
            var scrollRoot = new GameObject("Chat Scroll", typeof(RectTransform));
            scrollRoot.transform.SetParent(parent, false);
            GhostUITheme.Card(scrollRoot, new Color(1f, 0.995f, 0.96f, 0.92f));

            var scrollLayout = scrollRoot.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;
            scrollLayout.minHeight = 280f;

            scrollRect = scrollRoot.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;

            var viewport = new GameObject("Viewport", typeof(RectTransform));
            viewport.transform.SetParent(scrollRoot.transform, false);
            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(8f, 8f);
            viewportRect.offsetMax = new Vector2(-8f, -8f);

            var viewportImage = GhostUITheme.Card(viewport, new Color(1f, 1f, 1f, 0.01f));
            viewportImage.raycastTarget = true;

            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            var content = new GameObject("Messages", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            messageContentRoot = content.GetComponent<RectTransform>();
            messageContentRoot.anchorMin = new Vector2(0f, 1f);
            messageContentRoot.anchorMax = new Vector2(1f, 1f);
            messageContentRoot.pivot = new Vector2(0.5f, 1f);
            messageContentRoot.anchoredPosition = Vector2.zero;
            messageContentRoot.sizeDelta = Vector2.zero;

            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 6f;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = messageContentRoot;
        }

        private void CreateInputRow(Transform parent)
        {
            var row = new GameObject("Input Row", typeof(RectTransform));
            row.transform.SetParent(parent, false);

            var rowLayoutElement = row.AddComponent<LayoutElement>();
            rowLayoutElement.minHeight = 48f;
            rowLayoutElement.preferredHeight = 48f;

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            inputField = CreateInputField(row.transform);
            var inputLayout = inputField.gameObject.AddComponent<LayoutElement>();
            inputLayout.flexibleWidth = 1f;
            inputLayout.minHeight = 44f;
            inputLayout.preferredHeight = 44f;

            sendButton = GhostUITheme.PushButton("Send Button", row.transform, "Send");
            var sendLayout = sendButton.gameObject.AddComponent<LayoutElement>();
            sendLayout.minWidth = 84f;
            sendLayout.preferredWidth = 84f;
            sendLayout.minHeight = 44f;
            sendLayout.preferredHeight = 44f;
            sendButtonLabel = sendButton.GetComponentInChildren<Text>();
            sendButton.onClick.AddListener(SendCurrentMessage);
        }

        private void OpenInternal(
            string requestedActId,
            string openingLine,
            string requestedStateSummary,
            string requestedTrigger)
        {
            var normalizedActId = string.IsNullOrWhiteSpace(requestedActId)
                ? GhostNarrativeState.Act1Id
                : requestedActId;

            if (!string.Equals(actId, normalizedActId, System.StringComparison.Ordinal))
            {
                actId = normalizedActId;
                history.Clear();
                ClearMessages();
            }

            stateSummary = requestedStateSummary ?? string.Empty;
            trigger = string.IsNullOrWhiteSpace(requestedTrigger)
                ? "manual_ask_lily"
                : requestedTrigger;

            panelRoot.gameObject.SetActive(true);
            AmbientBanterPanel.ActivePanel?.PauseForChat();

            if (!string.IsNullOrWhiteSpace(openingLine) && ShouldAppendOpeningLine(openingLine))
            {
                AppendTurn("lily", ShellDialogueData.LilySpeakerName, openingLine, true);
            }

            SetRequestInFlight(false);
            inputField.text = string.Empty;
            inputField.ActivateInputField();
            ScrollToBottom();
        }

        private bool ShouldAppendOpeningLine(string openingLine)
        {
            if (history.Count == 0)
            {
                return true;
            }

            var last = history[history.Count - 1];
            return !string.Equals(last.text, openingLine, System.StringComparison.Ordinal);
        }

        private void Close()
        {
            panelRoot.gameObject.SetActive(false);
            requestInFlight = false;
            AmbientBanterPanel.ActivePanel?.ResumeAfterChat();
        }

        private void SendCurrentMessage()
        {
            if (requestInFlight || inputField == null)
            {
                return;
            }

            var message = (inputField.text ?? string.Empty).Trim();
            if (message.Length == 0)
            {
                return;
            }

            var requestHistory = CopyHistoryForRequest();
            inputField.text = string.Empty;
            AppendTurn("player", "You", message, true);
            var pendingText = AppendTurn("lily", ShellDialogueData.LilySpeakerName, "Um... let me think.", false);
            SetRequestInFlight(true);

            GhostBackendClient.PostChat(
                actId,
                message,
                requestHistory,
                GhostNarrativeState.PlayerName,
                stateSummary,
                trigger,
                response =>
                {
                    SetRequestInFlight(false);
                    var reply = response.Succeeded && response.Value != null && !string.IsNullOrWhiteSpace(response.Value.reply)
                        ? response.Value.reply
                        : BanterData.GetStaticChatReply(actId);

                    pendingText.text = FormatLine(ShellDialogueData.LilySpeakerName, reply);
                    AddHistory("lily", reply);
                    ScrollToBottom();
                });
        }

        private List<ChatHistoryItem> CopyHistoryForRequest()
        {
            var copy = new List<ChatHistoryItem>();
            var start = Mathf.Max(0, history.Count - MaxHistoryTurns);
            for (var i = start; i < history.Count; i++)
            {
                copy.Add(new ChatHistoryItem
                {
                    role = history[i].role,
                    text = history[i].text
                });
            }

            return copy;
        }

        private Text AppendTurn(string role, string label, string text, bool storeInHistory)
        {
            var line = GhostUITheme.Label(
                "Chat Line",
                messageContentRoot,
                FormatLine(label, text),
                GhostUITheme.SmallSize,
                role == "player" ? FontStyle.Bold : FontStyle.Normal,
                TextAnchor.UpperLeft,
                role == "player"
                    ? GhostUITheme.Ink
                    : GhostUITheme.InkSoft);

            line.horizontalOverflow = HorizontalWrapMode.Wrap;
            line.verticalOverflow = VerticalWrapMode.Overflow;

            var layout = line.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 28f;
            layout.preferredHeight = Mathf.Clamp(28f + (text ?? string.Empty).Length / 46 * 18f, 34f, 92f);

            if (storeInHistory)
            {
                AddHistory(role, text);
            }

            ScrollToBottom();
            return line;
        }

        private void AddHistory(string role, string text)
        {
            history.Add(new ChatHistoryItem
            {
                role = role ?? string.Empty,
                text = text ?? string.Empty
            });

            while (history.Count > MaxHistoryTurns)
            {
                history.RemoveAt(0);
            }
        }

        private void SetRequestInFlight(bool value)
        {
            requestInFlight = value;
            if (sendButton != null)
            {
                sendButton.interactable = !value;
            }

            if (sendButtonLabel != null)
            {
                sendButtonLabel.text = value ? "..." : "Send";
            }
        }

        private void ClearMessages()
        {
            for (var i = messageContentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(messageContentRoot.GetChild(i).gameObject);
            }
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageContentRoot);
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        private void OnDestroy()
        {
            if (activeWindow == this)
            {
                activeWindow = null;
            }
        }

        private static string FormatLine(string label, string text)
        {
            return (label ?? string.Empty) + ": " + (text ?? string.Empty).Replace("{playerName}", GhostNarrativeState.PlayerName);
        }


        private static InputField CreateInputField(Transform parent)
        {
            var root = new GameObject("Chat Input", typeof(RectTransform));
            root.transform.SetParent(parent, false);

            var image = GhostUITheme.Card(root, Color.white);
            image.raycastTarget = true;

            var input = root.AddComponent<InputField>();
            input.lineType = InputField.LineType.SingleLine;
            input.characterLimit = 240;

            var text = GhostUITheme.Label(
                "Input Text",
                root.transform,
                string.Empty,
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10f, 4f);
            textRect.offsetMax = new Vector2(-10f, -4f);

            var placeholder = GhostUITheme.Label(
                "Placeholder",
                root.transform,
                "Ask about this act...",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
            var placeholderRect = placeholder.rectTransform;
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10f, 4f);
            placeholderRect.offsetMax = new Vector2(-10f, -4f);

            input.textComponent = text;
            input.placeholder = placeholder;
            return input;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

    }
}
