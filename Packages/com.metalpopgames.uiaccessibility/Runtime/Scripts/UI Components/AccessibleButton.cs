using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UAP.Entities;

namespace UAP
{
	[AddComponentMenu("Accessibility/UI/Accessible Button")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public class AccessibleButton : UAP_BaseElement, ISelectHandler, IDeselectHandler
	{
		
		protected Text frameSelectedText;
		protected Text buttonText;
		protected TextValues originTextValues = new();
		protected Button btn;

		//////////////////////////////////////////////////////////////////////////

		AccessibleButton()
		{
			m_Type = AccessibleUIGroupRoot.EUIElement.EButton;
		}

		//////////////////////////////////////////////////////////////////////////

		#region Unity Events
		
		protected virtual void Awake()
		{
			InitComponents();
		}

		private void OnDisable()
		{
			RestoreTextColor();
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			// --- Change Text style ---
			if (frameSelectedText == null)
			{
				return;
			}
			
			buttonText = GetText(eventData);
			if (buttonText != null)
			{
				if (originTextValues.Color == Color.clear)
				{
					originTextValues.Color = buttonText.color;
				}
			}
			
			buttonText.color = frameSelectedText.color;
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			RestoreTextColor(eventData);
		}

		#endregion
		
		protected override void OnInteract()
		{
			// Press button (works for UGUI and TMP)
			Button button = GetButton(forceFetchInactive: true);
			if (button != null)
			{
				var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
				
				button.OnPointerClick(pointer);
				
				// PS: That "IDeselectHandler.OnDeselect()" event could be triggered in a override method of "base.OnInteractEnd()"
				// but won't be called if inside of any click handler of this button there is at least an action
				// that disables this gameObject :(
				OnDeselect(pointer);
				return;
			}

	#if ACCESS_NGUI
			UIButton nGUIButton = GetNGUIButton();
			if (nGUIButton != null)
			{
				nGUIButton.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
				nGUIButton.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
				nGUIButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				return;
			}
			else
			{
				UIEventTrigger nGUITrigger = GetNGUIEventTrigger();
				if (nGUITrigger != null)
				{
					nGUITrigger.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
					nGUITrigger.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
					nGUITrigger.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
					return;
				}
			}
	#endif
		}

		//////////////////////////////////////////////////////////////////////////

		public override bool IsElementActive()
		{
			// Return whether this button is usable, and visible
			if (!base.IsElementActive())
				return false;

			if (m_ReferenceElement != null)
				if (!m_ReferenceElement.gameObject.activeInHierarchy)
					return false;

			if (m_SkipIfDisabled && !IsInteractable())
				return false;

			if (!UAP_AccessibilityManager.GetSpeakDisabledInteractables())
				if (!IsInteractable())
					return false;

			return true;
		}

		//////////////////////////////////////////////////////////////////////////

		protected virtual void InitComponents()
		{
			buttonText = GetComponentInChildren<Text>();
			
			// Cache Button component in "btn" field
			GetButton();
		}
		
		protected virtual void RestoreTextColor(BaseEventData eventData = null)
		{
			buttonText = GetText(eventData);
			if (buttonText != null && originTextValues.Color != Color.clear)
			{
				buttonText.color = originTextValues.Color;
			}
		}
		
		protected virtual Text GetText(BaseEventData eventData = null)
		{
			if (buttonText != null)
			{
				return buttonText;
			}
			
			var currentObj = eventData != null ? eventData.selectedObject : gameObject;
			buttonText = buttonText == null ? currentObj.GetComponentInChildren<Text>() : buttonText;
			
			return buttonText;
		}
		
		private Button GetButton(bool forceFetchInactive = false)
		{
			if (!forceFetchInactive && btn != null)
			{
				return btn;
			}
			
			if (m_ReferenceElement != null)
				btn = forceFetchInactive && m_ReferenceElement.activeInHierarchy ? m_ReferenceElement.GetComponentInChildren<Button>(true) : m_ReferenceElement.GetComponent<Button>();
			if (btn == null)
				btn = forceFetchInactive && gameObject.activeInHierarchy ? gameObject.GetComponentInChildren<Button>(true) : gameObject.GetComponent<Button>();

			return btn;
		}


		//////////////////////////////////////////////////////////////////////////

	#if ACCESS_NGUI
		private UIButton GetNGUIButton()
		{
			UIButton refButton = null;
			if (m_ReferenceElement != null)
				refButton = m_ReferenceElement.GetComponent<UIButton>();
			if (refButton == null)
				refButton = GetComponent<UIButton>();

			return refButton;
		}

		private UIEventTrigger GetNGUIEventTrigger()
		{
			UIEventTrigger refButton = null;
			if (m_ReferenceElement != null)
				refButton = m_ReferenceElement.GetComponent<UIEventTrigger>();
			if (refButton == null)
				refButton = GetComponent<UIEventTrigger>();

			return refButton;
		}
	#endif

		//////////////////////////////////////////////////////////////////////////

		public override bool IsInteractable()
		{
			Button buttonComponent = GetButton();
			if (buttonComponent != null)
			{
				if (buttonComponent.enabled == false || buttonComponent.IsInteractable() == false)
					return false;
				else
					return true;
			}

			// NGUI
	#if ACCESS_NGUI
			UIButton nGUIButtonComponent = GetNGUIButton();
			if (nGUIButtonComponent != null)
			{
				if (nGUIButtonComponent.enabled == false || nGUIButtonComponent.isEnabled == false)
					return false;
				else
					return true;
			}
			else
			{
				// There might be an event trigger on this instead of a regular UI button
				UIEventTrigger nGUIEventTrigger = GetNGUIEventTrigger();
				if (nGUIEventTrigger != null)
				{
					//Debug.Log("Found Event Trigger");
					if (nGUIEventTrigger.enabled && nGUIEventTrigger.isActiveAndEnabled)
						return true;
					else
						return false;
				}
			}

	#endif

			// We couldn't find any buttons...
			return false;
		}

		//////////////////////////////////////////////////////////////////////////

		public override bool AutoFillTextLabel()
		{
			if (base.AutoFillTextLabel())
				return true;

			bool found = false;

			// Unity UI
			//////////////////////////////////////////////////////////////////////////
			{
				// Try to find a label in the button's children
				Transform textLabel = gameObject.transform.Find("Text");
				if (textLabel != null)
				{
					Text label = textLabel.gameObject.GetComponent<Text>();
					if (label != null)
					{
						m_Text = label.text;
						found = true;
					}
				}

				if (!found)
				{
					Text label = gameObject.GetComponentInChildren<Text>();
					if (label != null)
					{
						m_Text = label.text;
						found = true;
					}
				}
			}

			// TextMesh Pro
			//////////////////////////////////////////////////////////////////////////
			if (!found)
			{
				var TMP_Label = GetTextMeshProLabelInChildren();
				if (TMP_Label != null)
				{
					m_Text = GetTextFromTextMeshPro(TMP_Label);
					found = true;
				}
			}

	#if ACCESS_NGUI
			// NGUI
			//////////////////////////////////////////////////////////////////////////
			{
				Transform textLabel = gameObject.transform.Find("Label");
				if (textLabel != null)
				{
					UILabel label = textLabel.gameObject.GetComponent<UILabel>();
					if (label != null)
					{
						m_Text = label.text;
						found = true;
					}
				}

				if (!found)
				{
					UILabel label = gameObject.GetComponentInChildren<UILabel>();
					if (label != null)
					{
						m_Text = label.text;
						found = true;
					}
				}
			}
	#endif

			// if nothing, use the GameObject name
			if (!found)
				m_Text = gameObject.name;

			return found;
		}

		//////////////////////////////////////////////////////////////////////////

		protected override void AutoInitialize()
		{
			if (m_TryToReadLabel)
			{
				bool found = false;

				// Unity UI
				//////////////////////////////////////////////////////////////////////////
				{
					// Try to find a label in the button's children
					Transform textLabel = gameObject.transform.Find("Text");
					if (textLabel != null)
					{
						Text label = textLabel.gameObject.GetComponent<Text>();
						if (label != null)
						{
							m_NameLabel = label.gameObject;
							found = true;
						}
					}

					if (!found)
					{
						Text label = gameObject.GetComponentInChildren<Text>();
						if (label != null)
						{
							m_NameLabel = label.gameObject;
							found = true;
						}
					}
				}

				// TextMesh Pro
				//////////////////////////////////////////////////////////////////////////
				if (!found)
				{
					var TMP_Label = GetTextMeshProLabelInChildren();
					if (TMP_Label != null)
					{
						m_NameLabel = TMP_Label.gameObject;
						found = true;
					}
				}


	#if ACCESS_NGUI
				// NGUI
				//////////////////////////////////////////////////////////////////////////
				{
					Transform textLabel = gameObject.transform.Find("Label");
					if (textLabel != null)
					{
						UILabel label = textLabel.gameObject.GetComponent<UILabel>();
						if (label != null)
						{
							m_NameLabel = label.gameObject;
							found = true;
						}
					}

					if (!found)
					{
						UILabel label = gameObject.GetComponentInChildren<UILabel>();
						if (label != null)
						{
							m_NameLabel = label.gameObject;
							found = true;
						}
					}
				}
	#endif
			}
			else
			{
				m_NameLabel = null;
			}
		}

		//////////////////////////////////////////////////////////////////////////

		protected override void OnHoverHighlight(bool enable)
		{
	#if ACCESS_NGUI
			UIButton nGUIButton = GetNGUIButton();
			if (nGUIButton != null)
			{
				if (enable)
					nGUIButton.SetState(IsInteractable() ? UIButtonColor.State.Hover : UIButtonColor.State.Disabled, false);
				else
					nGUIButton.SetState(IsInteractable() ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, false);
			}
	#else
			Button button = GetButton();
			if (button != null)
			{
				var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
				if (enable)
					button.OnPointerEnter(pointer);
				else
					button.OnPointerExit(pointer);
			}
	#endif
		}

		//////////////////////////////////////////////////////////////////////////

		public override void UpdateElementFrame(RectTransform frameSelected, RectTransform elementRect)
		{
			var frameSpriteRenderer = frameSelected.GetComponentInChildren<SpriteRenderer>();
			var frameButton = frameSelected.GetComponentInChildren<Button>(true);
			frameSelectedText = frameSelected.GetComponentInChildren<Text>(true);
			
			var button = GetButton();
			
			if (button == null)
			{
				return;
			}
			
			var spriteState = button.spriteState;
			ColorBlock colors = new()
			{
				selectedColor = Color.clear
			};
			
			// --- Change Button: Sprite or Color ---
			if (frameButton != null)
			{
				switch (frameButton.transition)
				{
					case Selectable.Transition.SpriteSwap:
						spriteState.selectedSprite = frameButton.spriteState.selectedSprite;
						break;
					case Selectable.Transition.ColorTint:
						colors = frameButton.colors;
						break;
				}
			}
			else if (frameSpriteRenderer != null || frameSpriteRenderer.sprite != null)
			{
				spriteState.selectedSprite = frameSpriteRenderer.sprite;
			}

			if (spriteState.selectedSprite != null)
			{
				if (button.transition != Selectable.Transition.SpriteSwap)
				{
					button.transition = Selectable.Transition.SpriteSwap;
				}
				
				button.spriteState = spriteState;
			} else if (colors.selectedColor != Color.clear)
			{
				var buttonColors = button.colors;
				buttonColors.selectedColor = colors.selectedColor;

				button.colors = buttonColors;
			}
			
			button.Select();
		}
	}
}
