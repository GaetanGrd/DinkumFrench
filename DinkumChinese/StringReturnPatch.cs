﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using I2LocPatch;
using Mirror;

namespace DinkumChinese
{
    public static class StringReturnPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(AnimalHouseMenu), "fillData")]
        public static void AnimalHouseMenu_fillData_Patch(AnimalHouseMenu __instance)
        {
            __instance.eatenText.text = __instance.eatenText.text.Replace("Eaten", "Mangé");
            __instance.shelterText.text = __instance.shelterText.text.Replace("Shelter", "Abri");
            __instance.pettedText.text = __instance.pettedText.text.Replace("Petted", "Caressé");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(AnimalManager), "capitaliseFirstLetter")]
        public static bool AnimalManager_capitaliseFirstLetter_Patch(ref string __result, string toChange)
        {
            Debug.Log($"AnimalManager_capitaliseFirstLetter_Patch 1:{toChange}");
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, toChange);
            Debug.Log($"AnimalManager_capitaliseFirstLetter_Patch 2:{__result}");
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AnimalManager), "fillAnimalLocation")]
        public static void AnimalManager_fillAnimalLocation_Patch(ref string __result)
        {
            Debug.Log($"AnimalManager_fillAnimalLocation_Patch 1:{__result}");
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, __result);
            Debug.Log($"AnimalManager_fillAnimalLocation_Patch 2:{__result}");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(BullitenBoardPost), "getRequirementsNeededInPhoto")]
        public static bool BullitenBoardPost_getRequirementsNeededInPhoto_Patch(BullitenBoardPost __instance, int postId, ref string __result)
        {
            if (!BulletinBoard.board.attachedPosts[postId].isPhotoTask)
            {
                __result = "";
                return false;
            }
            List<string> list = new List<string>();
            List<int> list2 = new List<int>();
            string text = "";
            if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.getSubjectType() == PhotoChallengeManager.photoSubject.Animal)
            {
                for (int i = 0; i < BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.animalsRequiredInPhoto().Length; i++)
                {
                    string item = BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.animalsRequiredInPhoto()[i].animalName;
                    if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.animalsRequiredInPhoto()[i].animalId == 1)
                    {
                        item = AnimalManager.manage.allAnimals[1].GetComponent<FishType>().getFishInvItem().getInvItemName();
                    }
                    else if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.animalsRequiredInPhoto()[i].animalId == 2)
                    {
                        item = BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.animalsRequiredInPhoto()[i].GetComponent<BugTypes>().bugInvItem().getInvItemName();
                    }
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                        list2.Add(1);
                    }
                    else
                    {
                        List<int> list3 = list2;
                        int index = list.IndexOf(item);
                        list3[index]++;
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    if (list2[j] > 1)
                    {
                        text = text + list2[j].ToString() + " " + list[j];
                    }
                    else
                    {
                        text = text + "" + list[j];
                    }
                    if (j != list.Count - 1)
                    {
                        if (j == list.Count - 2 && list.Count > 1)
                        {
                            text += "et";
                        }
                        else
                        {
                            text += "，";
                        }
                    }
                }
            }
            else
            {
                if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.getSubjectType() == PhotoChallengeManager.photoSubject.Npc)
                {
                    __result = NPCManager.manage.NPCDetails[BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.returnSubjectId()[0]].NPCName;
                    return false;
                }
                if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.getSubjectType() == PhotoChallengeManager.photoSubject.Location)
                {
                    __result = "Prenez des photos aux points marqués sur la carte";
                    return false;
                }
                if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.getSubjectType() == PhotoChallengeManager.photoSubject.Carryable)
                {
                    string itemName;
                    if (SaveLoad.saveOrLoad.carryablePrefabs[BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.returnSubjectId()[0]].GetComponent<SellByWeight>())
                    {
                        itemName = SaveLoad.saveOrLoad.carryablePrefabs[BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.returnSubjectId()[0]].GetComponent<SellByWeight>().itemName;
                    }
                    else
                    {
                        itemName = SaveLoad.saveOrLoad.carryablePrefabs[BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.returnSubjectId()[0]].name;
                    }
                    __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, itemName);
                    return false;
                }
                else if (BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.getSubjectType() == PhotoChallengeManager.photoSubject.Biome)
                {
                    __result = "exister" + BulletinBoard.board.attachedPosts[postId].myPhotoChallenge.returnRequiredLocationBiomeName() + "Photographier";
                    return false;
                }
            }
            __result = text;
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(CustomNetworkManager), "refreshLobbyTypeButtons")]
        public static void CustomNetworkManager_refreshLobbyTypeButtons_Patch(CustomNetworkManager __instance)
        {
            __instance.friendGameText.text = __instance.friendGameText.text.Replace("Friends Only", "Réservé aux amis");
            __instance.inviteOnlyText.text = __instance.inviteOnlyText.text.Replace("InviteOnly", "inviter seulement");
            __instance.publicGameText.text = __instance.publicGameText.text.Replace("Public", "Public");
            __instance.lanGameText.text = __instance.lanGameText.text.Replace("LAN", "LAN");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GenerateMap), "getBiomeNameById")]
        public static void GenerateMap_getBiomeNameById_Patch(ref string __result, int id)
        {
            GenerateMap.biomNames biomNames = (GenerateMap.biomNames)id;
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, biomNames.ToString());
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GenerateMap), "getBiomeNameUnderMapCursor")]
        public static void GenerateMap_getBiomeNameUnderMapCursor_Patch(ref string __result)
        {
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, __result.StrToI2Str());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Inventory), "getExtraDetails")]
        public static bool Inventory_getExtraDetails_Patch(Inventory __instance, int itemId, ref string __result)
        {
            var _this = __instance;
            string text = "";
            if (_this.allItems[itemId].placeable && _this.allItems[itemId].placeable.tileObjectGrowthStages && !_this.allItems[itemId].consumeable)
            {
                string text2 = "";
                if (_this.allItems[itemId].placeable.tileObjectGrowthStages.growsInSummer && _this.allItems[itemId].placeable.tileObjectGrowthStages.growsInWinter && _this.allItems[itemId].placeable.tileObjectGrowthStages.growsInSpring && _this.allItems[itemId].placeable.tileObjectGrowthStages.growsInAutum)
                {
                    text2 = "en toutes saisons";
                }
                else
                {
                    text2 += "exister";
                    if (_this.allItems[itemId].placeable.tileObjectGrowthStages.growsInSummer)
                    {
                        text2 += "été";
                    }
                    if (_this.allItems[itemId].placeable.tileObjectGrowthStages.growsInAutum)
                    {
                        if (text2 != "exister" && text2 != "在")
                        {
                            text2 += "et";
                        }
                        text2 += "Automne";
                    }
                    if (_this.allItems[itemId].placeable.tileObjectGrowthStages.growsInWinter)
                    {
                        if (text2 != "exister" && text2 != "在")
                        {
                            text2 += "et";
                        }
                        text2 += "hiver";
                    }
                    if (_this.allItems[itemId].placeable.tileObjectGrowthStages.growsInSpring)
                    {
                        if (text2 != "exister" && text2 != "在")
                        {
                            text2 += "et";
                        }
                        text2 += "Printemps";
                    }
                }
                if (_this.allItems[itemId].placeable.tileObjectGrowthStages.needsTilledSoil)
                {
                    text = text + "Convient à" + text2 + "plante.";
                }
                if (_this.allItems[itemId].placeable.tileObjectGrowthStages.objectStages.Length != 0)
                {
                    if (_this.allItems[itemId].placeable.tileObjectGrowthStages.steamsOutInto)
                    {
                        text = string.Concat(new string[]
                        {
                        text,
                        "besoin d'espace autour car ils se noueront dans la position suivante<b>",
                        _this.allItems[itemId].placeable.tileObjectGrowthStages.steamsOutInto.tileObjectGrowthStages.harvestSpots.Length.ToString(),
                        "individuel",
                        _this.allItems[itemId].placeable.tileObjectGrowthStages.steamsOutInto.tileObjectGrowthStages.harvestDrop.getInvItemName(),
                        "</b>.La plante peut diviser jusqu'à 4 branches!"
                        });
                    }
                    else
                    {
                        text = string.Concat(new string[]
                        {
                        text,
                        "besoin",
                        _this.allItems[itemId].placeable.tileObjectGrowthStages.objectStages.Length.ToString(),
                        "jours pour grandir et récolter",
                        _this.allItems[itemId].placeable.tileObjectGrowthStages.harvestSpots.Length.ToString(),
                        _this.allItems[itemId].placeable.tileObjectGrowthStages.harvestDrop.getInvItemName(),
                        "."
                        });
                    }
                }
                if (!_this.allItems[itemId].placeable.tileObjectGrowthStages.diesOnHarvest && !_this.allItems[itemId].placeable.tileObjectGrowthStages.steamsOutInto)
                {
                    text = string.Concat(new string[]
                    {
                    text,
                    "tous les suivants",
                    Mathf.Abs(_this.allItems[itemId].placeable.tileObjectGrowthStages.takeOrAddFromStateOnHarvest).ToString(),
                    "jour de récolte",
                    _this.allItems[itemId].placeable.tileObjectGrowthStages.harvestSpots.Length.ToString(),
                    _this.allItems[itemId].placeable.tileObjectGrowthStages.harvestDrop.getInvItemName(),
                    "."
                    });
                }
                if (!WorldManager.manageWorld.allObjectSettings[_this.allItems[itemId].placeable.tileObjectId].walkable)
                {
                    text += "Oh, et cela nécessite également des supports de plantes à attacher pour pousser.";
                }
            }
            __result = text;
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Licence), "getConnectedSkillName")]
        public static void Licence_getConnectedSkillName_Patch(ref string __result)
        {
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, __result);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NetworkMapSharer), "UserCode_RpcDeliverAnimal")]
        public static bool NetworkMapSharer_UserCode_RpcDeliverAnimal_Patch(uint deliveredBy, int animalDelivered, int variationDelivered, int rewardToSend, int trapType)
        {
            CharMovement component = NetworkIdentity.spawned[deliveredBy].GetComponent<CharMovement>();
            var animal = AnimalManager.manage.allAnimals[animalDelivered];
            // 获取动物名字 (obtenir le nom de l'animal)
            string animalName = TextLocData.GetLoc(DinkumChinesePlugin.Inst.AnimalsTextLocList, animal.animalName);
            string str = animalName;
            if (variationDelivered != 0)
            {
                string variationName = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, animal.hasVariation.variationAdjective[variationDelivered]);
                str = variationName + animalName;
            }
            NotificationManager.manage.createChatNotification(component.GetComponent<EquipItemToChar>().playerName + "livré un" + str, false);
            if (component.isLocalPlayer)
            {
                if (animalDelivered == 29)
                {
                    MailManager.manage.sendAChrissyAnimalCapturedLetter(trapType);
                    return false;
                }
                MailManager.manage.sendAnAnimalCapturedLetter(rewardToSend, trapType);
            }
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NPCRequest), "setRandomBugNoAndLocation")]
        public static void NPCRequest_setRandomBugNoAndLocation_Patch(NPCRequest __instance)
        {
            __instance.itemFoundInLocation = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, __instance.itemFoundInLocation);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NPCRequest), "setRandomFishNoAndLocation")]
        public static void NPCRequest_setRandomFishNoAndLocation_Patch(NPCRequest __instance)
        {
            __instance.itemFoundInLocation = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, __instance.itemFoundInLocation);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PickUpNotification), "fillButtonPrompt", new Type[] { typeof(string), typeof(Sprite) })]
        public static void PickUpNotification_fillButtonPrompt_Patch(PickUpNotification __instance, string buttonPromptText)
        {
            string text = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, buttonPromptText);
            __instance.itemText.text = text;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PickUpNotification), "fillButtonPrompt", new Type[] { typeof(string), typeof(Input_Rebind.RebindType) })]
        public static void PickUpNotification_fillButtonPrompt_Patch2(PickUpNotification __instance, string buttonPromptText)
        {
            string text = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, buttonPromptText);
            __instance.itemText.text = text;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PostOnBoard), "getContentText")]
        public static void PostOnBoard_getContentText_Patch(PostOnBoard __instance, ref string __result, int postId)
        {
            string textOri = __instance.getPostPostsById().contentText.StrToI2Str();
            string text = TextLocData.GetLoc(DinkumChinesePlugin.Inst.PostTextLocList, textOri);
            __result = text.Replace("<boardRewardItem>",
                __instance.getPostPostsById().getBoardRewardItem(postId)).Replace("<boardHuntRequestAnimal>",
                __instance.getPostPostsById().getBoardHuntRequestAnimal(postId)).Replace("<getAnimalsInPhotoList>",
                __instance.getPostPostsById().getRequirementsNeededInPhoto(postId)).Replace("<boardRequestItem>",
                __instance.getPostPostsById().getBoardRequestItem(postId));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PostOnBoard), "getTitleText")]
        public static void PostOnBoard_getTitleText_Patch(PostOnBoard __instance, ref string __result, int postId)
        {
            string titleOri = __instance.getPostPostsById().title.StrToI2Str();
            string title = TextLocData.GetLoc(DinkumChinesePlugin.Inst.PostTextLocList, titleOri);
            __result = title.Replace("<boardRewardItem>",
                __instance.getPostPostsById().getBoardRewardItem(postId)).Replace("<boardHuntRequestAnimal>",
                __instance.getPostPostsById().getBoardHuntRequestAnimal(postId)).Replace("<boardRequestItem>",
                __instance.getPostPostsById().getBoardRequestItem(postId));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(QuestButton), "setUp")]
        public static void QuestButton_setUp_Patch(QuestButton __instance, int questNo)
        {
            if (__instance.isMainQuestButton)
            {
                string nameOri = QuestManager.manage.allQuests[questNo].QuestName.StrToI2Str();
                string name = TextLocData.GetLoc(DinkumChinesePlugin.Inst.QuestTextLocList, nameOri);
                __instance.buttonText.text = name;
            }
            else if (__instance.isQuestButton)
            {
            }
            else
            {
                __instance.buttonText.text = __instance.buttonText.text.Replace("Request for ", "Demande de") + "demande";
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(QuestNotification), "showQuest")]
        public static void QuestNotification_showQuest_Patch(QuestNotification __instance)
        {
            string nameOri = __instance.displayingQuest.QuestName.StrToI2Str();
            string name = TextLocData.GetLoc(DinkumChinesePlugin.Inst.QuestTextLocList, nameOri);
            __instance.QuestText.text = name;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(QuestTracker), "displayMainQuest")]
        public static void QuestTracker_displayMainQuest_Patch(QuestTracker __instance, int questNo)
        {
            string nameOri = QuestManager.manage.allQuests[questNo].QuestName.StrToI2Str();
            string name = TextLocData.GetLoc(DinkumChinesePlugin.Inst.QuestTextLocList, nameOri);
            string descOri = QuestManager.manage.allQuests[questNo].QuestDescription.StrToI2Str();
            string desc = TextLocData.GetLoc(DinkumChinesePlugin.Inst.QuestTextLocList, descOri).Replace("<IslandName>", Inventory.inv.islandName);
            __instance.questTitle.text = name;
            __instance.questDesc.text = desc;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(QuestTracker), "pinTheTask")]
        public static void QuestTracker_pinTheTask_Patch(QuestTracker __instance, QuestTracker.typeOfTask type, int id)
        {
            if (type == QuestTracker.typeOfTask.Quest)
            {
                if (!QuestManager.manage.isQuestCompleted[id])
                {
                    string nameOri = QuestManager.manage.allQuests[id].QuestName.StrToI2Str();
                    string name = TextLocData.GetLoc(DinkumChinesePlugin.Inst.QuestTextLocList, nameOri);
                    string pinText = __instance.pinMissionText.text.Replace(QuestManager.manage.allQuests[id].QuestName, name);
                    __instance.pinMissionText.text = pinText;
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SeasonAndTime), "capitaliseFirstLetter")]
        public static bool SeasonAndTime_capitaliseFirstLetter_Patch(ref string __result, string toChange)
        {
            Debug.Log($"SeasonAndTime_capitaliseFirstLetter_Patch 1:{toChange}");
            __result = TextLocData.GetLoc(DinkumChinesePlugin.Inst.DynamicTextLocList, toChange);
            Debug.Log($"SeasonAndTime_capitaliseFirstLetter_Patch 2:{__result}");
            return false;
        }
    }
}