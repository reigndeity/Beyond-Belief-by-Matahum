using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("instance", "m_uiGame", "isTutorialDone", "playerMovement", "inventory", "agimatPanel", "pamanaPanel", "characterDetailsButton", "inventoryButton", "archiveButton", "questButton", "minimap", "normalSkill", "ultimateSkill", "agimatOne", "agimatTwo", "health", "temporaryCollider", "tupasHouseStairs", "tupasHouseDoor", "cutsceneTriggerOne", "cutsceneBakalNPC", "lewenriSacredStatue", "tutorial_canMovementToggle", "tutorial_canJump", "tutorial_canCameraZoom", "tutorial_canCameraDirection", "tutorial_canAttack", "tutorial_canSprintAndDash", "tutorial_canNormalSkill", "tutorial_canUltimateSkill", "tutorial_canOpenMap", "tutorial_canToggleMouse", "tutorial_canArchives", "tutorial_isFirstStatueInteract", "tutorialFadeImage", "m_questButtonManager", "questJournalTutorial", "mainQuestViewportTH", "questSelectionPanelTH", "questDetailsPanelTH", "questButtonFiltersTH", "claimQuestButtonTH", "closeQuestJournalButtonTH", "claimQuestButton", "closeQuestButton", "questJournalTextTutorial", "nextJournalTutorialButton", "nextJournalTutorialCanvasGroup", "nextJournalTutorialTH", "nonInteractablePanel", "currentQuestJournalTutorial", "claimThisTextHelper", "weaponButtonTH", "closeCharacterDetailButton", "confirmSwitchButton", "confirmTextTH", "agimatButtonTH", "agimatTutorial", "agimatOneTH", "agimatTwoTH", "unequipAgimatButtonTH", "equipAgimatButtonTH", "agimatInventoryTH", "agimatItemImageTH", "agimatItemDescriptionTH", "currentAgimatTutorial", "agimatTutorialText", "firstAgimatSlot", "nextAgimatTutorialButton", "pamanaButtonTH", "attributesButtonTH", "currentPamanaTutorial", "firstPamanaSlot", "pamanaTutorialText", "pamanaTutorial", "diwataSlotButtonTH", "pamanaInventoryTH", "pamanaItemImageTH", "pamanaItemDescriptionTH", "equipPamanaButtonTH", "nextPamanaTutorialButtonTH", "attributeBackgroundTH", "inventoryTutorial", "inventoryTutorialText", "nonInteractableInventory", "sortButtonsTH", "currentFilterTextTH", "inventorySlotTH", "nextInventoryTutorial", "currentInventoryTutorial", "inventoryItemImageTH", "inventoryDescriptionTH", "closeInventoryButtonTH")]
	public class ES3UserType_TutorialManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TutorialManager() : base(typeof(TutorialManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (TutorialManager)obj;
			
			writer.WritePropertyByRef("instance", TutorialManager.instance);
			writer.WritePrivateFieldByRef("m_uiGame", instance);
			writer.WriteProperty("isTutorialDone", instance.isTutorialDone, ES3Type_bool.Instance);
			writer.WritePrivateFieldByRef("playerMovement", instance);
			writer.WritePrivateFieldByRef("inventory", instance);
			writer.WritePrivateFieldByRef("agimatPanel", instance);
			writer.WritePrivateFieldByRef("pamanaPanel", instance);
			writer.WritePropertyByRef("characterDetailsButton", instance.characterDetailsButton);
			writer.WritePropertyByRef("inventoryButton", instance.inventoryButton);
			writer.WritePropertyByRef("archiveButton", instance.archiveButton);
			writer.WritePropertyByRef("questButton", instance.questButton);
			writer.WritePropertyByRef("minimap", instance.minimap);
			writer.WritePropertyByRef("normalSkill", instance.normalSkill);
			writer.WritePropertyByRef("ultimateSkill", instance.ultimateSkill);
			writer.WritePropertyByRef("agimatOne", instance.agimatOne);
			writer.WritePropertyByRef("agimatTwo", instance.agimatTwo);
			writer.WritePropertyByRef("health", instance.health);
			writer.WritePropertyByRef("temporaryCollider", instance.temporaryCollider);
			writer.WritePropertyByRef("tupasHouseStairs", instance.tupasHouseStairs);
			writer.WritePropertyByRef("tupasHouseDoor", instance.tupasHouseDoor);
			writer.WritePropertyByRef("cutsceneTriggerOne", instance.cutsceneTriggerOne);
			writer.WritePropertyByRef("cutsceneBakalNPC", instance.cutsceneBakalNPC);
			writer.WritePropertyByRef("lewenriSacredStatue", instance.lewenriSacredStatue);
			writer.WriteProperty("tutorial_canMovementToggle", instance.tutorial_canMovementToggle, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canJump", instance.tutorial_canJump, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canCameraZoom", instance.tutorial_canCameraZoom, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canCameraDirection", instance.tutorial_canCameraDirection, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canAttack", instance.tutorial_canAttack, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canSprintAndDash", instance.tutorial_canSprintAndDash, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canNormalSkill", instance.tutorial_canNormalSkill, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canUltimateSkill", instance.tutorial_canUltimateSkill, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canOpenMap", instance.tutorial_canOpenMap, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canToggleMouse", instance.tutorial_canToggleMouse, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_canArchives", instance.tutorial_canArchives, ES3Type_bool.Instance);
			writer.WriteProperty("tutorial_isFirstStatueInteract", instance.tutorial_isFirstStatueInteract, ES3Type_bool.Instance);
			writer.WritePropertyByRef("tutorialFadeImage", instance.tutorialFadeImage);
			writer.WritePrivateFieldByRef("m_questButtonManager", instance);
			writer.WritePropertyByRef("questJournalTutorial", instance.questJournalTutorial);
			writer.WritePropertyByRef("mainQuestViewportTH", instance.mainQuestViewportTH);
			writer.WritePropertyByRef("questSelectionPanelTH", instance.questSelectionPanelTH);
			writer.WritePropertyByRef("questDetailsPanelTH", instance.questDetailsPanelTH);
			writer.WritePropertyByRef("questButtonFiltersTH", instance.questButtonFiltersTH);
			writer.WritePropertyByRef("claimQuestButtonTH", instance.claimQuestButtonTH);
			writer.WritePropertyByRef("closeQuestJournalButtonTH", instance.closeQuestJournalButtonTH);
			writer.WritePropertyByRef("claimQuestButton", instance.claimQuestButton);
			writer.WritePropertyByRef("closeQuestButton", instance.closeQuestButton);
			writer.WritePropertyByRef("questJournalTextTutorial", instance.questJournalTextTutorial);
			writer.WritePropertyByRef("nextJournalTutorialButton", instance.nextJournalTutorialButton);
			writer.WritePropertyByRef("nextJournalTutorialCanvasGroup", instance.nextJournalTutorialCanvasGroup);
			writer.WritePropertyByRef("nextJournalTutorialTH", instance.nextJournalTutorialTH);
			writer.WritePropertyByRef("nonInteractablePanel", instance.nonInteractablePanel);
			writer.WriteProperty("currentQuestJournalTutorial", instance.currentQuestJournalTutorial, ES3Type_int.Instance);
			writer.WritePropertyByRef("claimThisTextHelper", instance.claimThisTextHelper);
			writer.WritePropertyByRef("weaponButtonTH", instance.weaponButtonTH);
			writer.WritePropertyByRef("closeCharacterDetailButton", instance.closeCharacterDetailButton);
			writer.WritePropertyByRef("confirmSwitchButton", instance.confirmSwitchButton);
			writer.WritePropertyByRef("confirmTextTH", instance.confirmTextTH);
			writer.WritePropertyByRef("agimatButtonTH", instance.agimatButtonTH);
			writer.WritePropertyByRef("agimatTutorial", instance.agimatTutorial);
			writer.WritePropertyByRef("agimatOneTH", instance.agimatOneTH);
			writer.WritePropertyByRef("agimatTwoTH", instance.agimatTwoTH);
			writer.WritePropertyByRef("unequipAgimatButtonTH", instance.unequipAgimatButtonTH);
			writer.WritePropertyByRef("equipAgimatButtonTH", instance.equipAgimatButtonTH);
			writer.WritePropertyByRef("agimatInventoryTH", instance.agimatInventoryTH);
			writer.WritePropertyByRef("agimatItemImageTH", instance.agimatItemImageTH);
			writer.WritePropertyByRef("agimatItemDescriptionTH", instance.agimatItemDescriptionTH);
			writer.WriteProperty("currentAgimatTutorial", instance.currentAgimatTutorial, ES3Type_int.Instance);
			writer.WritePropertyByRef("agimatTutorialText", instance.agimatTutorialText);
			writer.WritePropertyByRef("firstAgimatSlot", instance.firstAgimatSlot);
			writer.WritePropertyByRef("nextAgimatTutorialButton", instance.nextAgimatTutorialButton);
			writer.WritePropertyByRef("pamanaButtonTH", instance.pamanaButtonTH);
			writer.WritePropertyByRef("attributesButtonTH", instance.attributesButtonTH);
			writer.WriteProperty("currentPamanaTutorial", instance.currentPamanaTutorial, ES3Type_int.Instance);
			writer.WritePropertyByRef("firstPamanaSlot", instance.firstPamanaSlot);
			writer.WritePropertyByRef("pamanaTutorialText", instance.pamanaTutorialText);
			writer.WritePropertyByRef("pamanaTutorial", instance.pamanaTutorial);
			writer.WritePropertyByRef("diwataSlotButtonTH", instance.diwataSlotButtonTH);
			writer.WritePropertyByRef("pamanaInventoryTH", instance.pamanaInventoryTH);
			writer.WritePropertyByRef("pamanaItemImageTH", instance.pamanaItemImageTH);
			writer.WritePropertyByRef("pamanaItemDescriptionTH", instance.pamanaItemDescriptionTH);
			writer.WritePropertyByRef("equipPamanaButtonTH", instance.equipPamanaButtonTH);
			writer.WritePropertyByRef("nextPamanaTutorialButtonTH", instance.nextPamanaTutorialButtonTH);
			writer.WritePropertyByRef("attributeBackgroundTH", instance.attributeBackgroundTH);
			writer.WritePropertyByRef("inventoryTutorial", instance.inventoryTutorial);
			writer.WritePropertyByRef("inventoryTutorialText", instance.inventoryTutorialText);
			writer.WritePropertyByRef("nonInteractableInventory", instance.nonInteractableInventory);
			writer.WritePropertyByRef("sortButtonsTH", instance.sortButtonsTH);
			writer.WritePropertyByRef("currentFilterTextTH", instance.currentFilterTextTH);
			writer.WritePropertyByRef("inventorySlotTH", instance.inventorySlotTH);
			writer.WritePropertyByRef("nextInventoryTutorial", instance.nextInventoryTutorial);
			writer.WriteProperty("currentInventoryTutorial", instance.currentInventoryTutorial, ES3Type_int.Instance);
			writer.WritePropertyByRef("inventoryItemImageTH", instance.inventoryItemImageTH);
			writer.WritePropertyByRef("inventoryDescriptionTH", instance.inventoryDescriptionTH);
			writer.WritePropertyByRef("closeInventoryButtonTH", instance.closeInventoryButtonTH);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (TutorialManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "instance":
						TutorialManager.instance = reader.Read<TutorialManager>();
						break;
					case "m_uiGame":
					instance = (TutorialManager)reader.SetPrivateField("m_uiGame", reader.Read<UI_Game>(), instance);
					break;
					case "isTutorialDone":
						instance.isTutorialDone = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "playerMovement":
					instance = (TutorialManager)reader.SetPrivateField("playerMovement", reader.Read<PlayerMovement>(), instance);
					break;
					case "inventory":
					instance = (TutorialManager)reader.SetPrivateField("inventory", reader.Read<R_Inventory>(), instance);
					break;
					case "agimatPanel":
					instance = (TutorialManager)reader.SetPrivateField("agimatPanel", reader.Read<R_AgimatPanel>(), instance);
					break;
					case "pamanaPanel":
					instance = (TutorialManager)reader.SetPrivateField("pamanaPanel", reader.Read<R_PamanaPanel>(), instance);
					break;
					case "characterDetailsButton":
						instance.characterDetailsButton = reader.Read<UI_CanvasGroup>();
						break;
					case "inventoryButton":
						instance.inventoryButton = reader.Read<UI_CanvasGroup>();
						break;
					case "archiveButton":
						instance.archiveButton = reader.Read<UI_CanvasGroup>();
						break;
					case "questButton":
						instance.questButton = reader.Read<UI_CanvasGroup>();
						break;
					case "minimap":
						instance.minimap = reader.Read<UI_CanvasGroup>();
						break;
					case "normalSkill":
						instance.normalSkill = reader.Read<UI_CanvasGroup>();
						break;
					case "ultimateSkill":
						instance.ultimateSkill = reader.Read<UI_CanvasGroup>();
						break;
					case "agimatOne":
						instance.agimatOne = reader.Read<UI_CanvasGroup>();
						break;
					case "agimatTwo":
						instance.agimatTwo = reader.Read<UI_CanvasGroup>();
						break;
					case "health":
						instance.health = reader.Read<UI_CanvasGroup>();
						break;
					case "temporaryCollider":
						instance.temporaryCollider = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "tupasHouseStairs":
						instance.tupasHouseStairs = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "tupasHouseDoor":
						instance.tupasHouseDoor = reader.Read<DoorInteractable>();
						break;
					case "cutsceneTriggerOne":
						instance.cutsceneTriggerOne = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "cutsceneBakalNPC":
						instance.cutsceneBakalNPC = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "lewenriSacredStatue":
						instance.lewenriSacredStatue = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "tutorial_canMovementToggle":
						instance.tutorial_canMovementToggle = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canJump":
						instance.tutorial_canJump = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canCameraZoom":
						instance.tutorial_canCameraZoom = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canCameraDirection":
						instance.tutorial_canCameraDirection = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canAttack":
						instance.tutorial_canAttack = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canSprintAndDash":
						instance.tutorial_canSprintAndDash = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canNormalSkill":
						instance.tutorial_canNormalSkill = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canUltimateSkill":
						instance.tutorial_canUltimateSkill = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canOpenMap":
						instance.tutorial_canOpenMap = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canToggleMouse":
						instance.tutorial_canToggleMouse = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_canArchives":
						instance.tutorial_canArchives = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorial_isFirstStatueInteract":
						instance.tutorial_isFirstStatueInteract = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "tutorialFadeImage":
						instance.tutorialFadeImage = reader.Read<Abu.TutorialFadeImage>();
						break;
					case "m_questButtonManager":
					instance = (TutorialManager)reader.SetPrivateField("m_questButtonManager", reader.Read<BB_Quest_ButtonManager>(), instance);
					break;
					case "questJournalTutorial":
						instance.questJournalTutorial = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "mainQuestViewportTH":
						instance.mainQuestViewportTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "questSelectionPanelTH":
						instance.questSelectionPanelTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "questDetailsPanelTH":
						instance.questDetailsPanelTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "questButtonFiltersTH":
						instance.questButtonFiltersTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "claimQuestButtonTH":
						instance.claimQuestButtonTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "closeQuestJournalButtonTH":
						instance.closeQuestJournalButtonTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "claimQuestButton":
						instance.claimQuestButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "closeQuestButton":
						instance.closeQuestButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "questJournalTextTutorial":
						instance.questJournalTextTutorial = reader.Read<TMPro.TextMeshProUGUI>();
						break;
					case "nextJournalTutorialButton":
						instance.nextJournalTutorialButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "nextJournalTutorialCanvasGroup":
						instance.nextJournalTutorialCanvasGroup = reader.Read<UI_CanvasGroup>();
						break;
					case "nextJournalTutorialTH":
						instance.nextJournalTutorialTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "nonInteractablePanel":
						instance.nonInteractablePanel = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "currentQuestJournalTutorial":
						instance.currentQuestJournalTutorial = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "claimThisTextHelper":
						instance.claimThisTextHelper = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "weaponButtonTH":
						instance.weaponButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "closeCharacterDetailButton":
						instance.closeCharacterDetailButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "confirmSwitchButton":
						instance.confirmSwitchButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "confirmTextTH":
						instance.confirmTextTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "agimatButtonTH":
						instance.agimatButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "agimatTutorial":
						instance.agimatTutorial = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "agimatOneTH":
						instance.agimatOneTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "agimatTwoTH":
						instance.agimatTwoTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "unequipAgimatButtonTH":
						instance.unequipAgimatButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "equipAgimatButtonTH":
						instance.equipAgimatButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "agimatInventoryTH":
						instance.agimatInventoryTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "agimatItemImageTH":
						instance.agimatItemImageTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "agimatItemDescriptionTH":
						instance.agimatItemDescriptionTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "currentAgimatTutorial":
						instance.currentAgimatTutorial = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "agimatTutorialText":
						instance.agimatTutorialText = reader.Read<TMPro.TextMeshProUGUI>();
						break;
					case "firstAgimatSlot":
						instance.firstAgimatSlot = reader.Read<UnityEngine.UI.Button>();
						break;
					case "nextAgimatTutorialButton":
						instance.nextAgimatTutorialButton = reader.Read<UnityEngine.UI.Button>();
						break;
					case "pamanaButtonTH":
						instance.pamanaButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "attributesButtonTH":
						instance.attributesButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "currentPamanaTutorial":
						instance.currentPamanaTutorial = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "firstPamanaSlot":
						instance.firstPamanaSlot = reader.Read<UnityEngine.UI.Button>();
						break;
					case "pamanaTutorialText":
						instance.pamanaTutorialText = reader.Read<TMPro.TextMeshProUGUI>();
						break;
					case "pamanaTutorial":
						instance.pamanaTutorial = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "diwataSlotButtonTH":
						instance.diwataSlotButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "pamanaInventoryTH":
						instance.pamanaInventoryTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "pamanaItemImageTH":
						instance.pamanaItemImageTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "pamanaItemDescriptionTH":
						instance.pamanaItemDescriptionTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "equipPamanaButtonTH":
						instance.equipPamanaButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "nextPamanaTutorialButtonTH":
						instance.nextPamanaTutorialButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					case "attributeBackgroundTH":
						instance.attributeBackgroundTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "inventoryTutorial":
						instance.inventoryTutorial = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "inventoryTutorialText":
						instance.inventoryTutorialText = reader.Read<TMPro.TextMeshProUGUI>();
						break;
					case "nonInteractableInventory":
						instance.nonInteractableInventory = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "sortButtonsTH":
						instance.sortButtonsTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "currentFilterTextTH":
						instance.currentFilterTextTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "inventorySlotTH":
						instance.inventorySlotTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "nextInventoryTutorial":
						instance.nextInventoryTutorial = reader.Read<UnityEngine.UI.Button>();
						break;
					case "currentInventoryTutorial":
						instance.currentInventoryTutorial = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "inventoryItemImageTH":
						instance.inventoryItemImageTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "inventoryDescriptionTH":
						instance.inventoryDescriptionTH = reader.Read<Abu.TutorialHighlight>();
						break;
					case "closeInventoryButtonTH":
						instance.closeInventoryButtonTH = reader.Read<UnityEngine.UI.Button>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TutorialManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TutorialManagerArray() : base(typeof(TutorialManager[]), ES3UserType_TutorialManager.Instance)
		{
			Instance = this;
		}
	}
}