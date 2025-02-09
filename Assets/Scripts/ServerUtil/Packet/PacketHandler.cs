﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

class PacketHandler
{
    #region Town

    public static void S_ConnectHandler(PacketSession session, IMessage packet)
    {
        S_Connect ConnectPacket = packet as S_Connect;
        if (ConnectPacket == null)
            return;

        TownManager.Instance.uiStart.ConfirmServer("127.0.0.1", "3000");

        if (!string.IsNullOrEmpty(GameManager.Instance.UserName) && !string.IsNullOrEmpty(GameManager.Instance.PassWord))
        {
            TownManager.Instance.uiStart.LoginServer(GameManager.Instance.UserName, GameManager.Instance.PassWord);
        }
    }

    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        S_Login LoginPacket = packet as S_Login;
        if (LoginPacket == null)
            return;

        if (LoginPacket.Success)
        {
            // true일 때 실행할 로직
            bool[] isUnlockedArray = LoginPacket.IsUnlocked.ToArray();
            TownManager.Instance.uiStart.InitializeCharacterInfos(isUnlockedArray);
            TownManager.Instance.coinDisplay.SetCoins(LoginPacket.Coin);
            TownManager.Instance.uiStart.SetCharacterSelectionUI(LoginPacket.Coin);
        }
        else
        {
            // false일 때 실행할 로직
            TownManager.Instance.uiStart.FailLoginServer();
        }
    }

    public static void S_RegisterHandler(PacketSession session, IMessage packet)
    {
        S_Register RegisterPacket = packet as S_Register;
        if (RegisterPacket == null)
            return;

        if (RegisterPacket.Success)
        {
            // true일 때 실행할 로직
            TownManager.Instance.uiStart.ConfirmRegister();
        }
        else
        {
            // false일 때 실행할 로직
            TownManager.Instance.uiStart.FailRegisterServer();
        }
    }

    public static void S_UnlockCharacterHandler (PacketSession session, IMessage packet)
	{
		S_UnlockCharacter playerUnlockPacket = packet as S_UnlockCharacter;
        if (playerUnlockPacket == null)
            return;

		TownManager.Instance.uiStart.UnlockCharacter(playerUnlockPacket.Idx, playerUnlockPacket.Coin);
    }

	public static void S_PlayerUpgradeHandler (PacketSession session, IMessage packet)
	{
		S_PlayerUpgrade playerUpgradePacket = packet as S_PlayerUpgrade;
        if (playerUpgradePacket == null)
            return;

        TownManager.Instance.uiShrine.UpdateStatOnServer(playerUpgradePacket);
    }

	public static void S_PlayerItemHandler(PacketSession session, IMessage packet)
	{
		S_PlayerItem playerItemPacket = packet as S_PlayerItem;
        if (playerItemPacket == null)
            return;
		
        TownManager.Instance.coinDisplay.SetCoins(playerItemPacket.Coin);
		TownManager.Instance.soulDisplay.SetSouls(playerItemPacket.Soul);
		TownManager.Instance.coinDisplay.UpdateCoinDisplay();
        TownManager.Instance.soulDisplay.UpdateSoulDisplay();
    }

	public static void S_EnterHandler(PacketSession session, IMessage packet)
	{
        S_Enter enterPacket = packet as S_Enter;
        if (enterPacket == null)
	        return;

		TownManager.Instance.Spawn(enterPacket.Player);
	}
	
	public static void S_LeaveHandler(PacketSession session, IMessage packet) {}
	
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		if (spawnPacket == null)
			return;
		
		var playerList = spawnPacket.Players;
		foreach (var playerInfo in playerList)
		{
			var tr = playerInfo.Transform;
        
			var player = TownManager.Instance.CreatePlayer(playerInfo, new Vector3(tr.PosX, tr.PosY, tr.PosZ));
			player.SetIsMine(false);
		}
	}
	
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		if (despawnPacket == null)
			return;
		
		foreach (var playerId in despawnPacket.PlayerIds)
		{
			TownManager.Instance.ReleasePlayer(playerId);
		}
	}
	
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		if (movePacket == null)
			return;
		
		var tr = movePacket.Transform;
		Vector3 move = new Vector3(tr.PosX, tr.PosY, tr.PosZ);
		Vector3 eRot = new Vector3(0, tr.Rot, 0);
	
		var player = TownManager.Instance.GetPlayerAvatarById(movePacket.PlayerId);
		if (player)
		{
			player.Move(move, Quaternion.Euler(eRot));
		}
	}



	public static void S_AnimationHandler(PacketSession session, IMessage packet)
	{
		S_Animation animationPacket = packet as S_Animation;
		if (animationPacket == null)
			return;
		
		var animCode = animationPacket.AnimCode;

		var player = TownManager.Instance.GetPlayerAvatarById(animationPacket.PlayerId);
		if (player)
		{
			player.Animation(animCode);
		}
	}
	
	public static void S_ChangeCostumeHandler(PacketSession session, IMessage packet) { }

	public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chatPacket = packet as S_Chat;
		if (chatPacket == null)
			return;
		
		var msg = chatPacket.ChatMsg;

		var player = TownManager.Instance.GetPlayerAvatarById(chatPacket.PlayerId);
		if (player)
		{
			player.RecvMessage(msg);
		}
	}

	#endregion


	#region Battle

	
	public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
	{
		S_EnterDungeon pkt = packet as S_EnterDungeon;
		if (pkt == null)
			return;
		
		Scene scene = SceneManager.GetActiveScene();
		
		if (scene.name == GameManager.BattleScene)
		{
			BattleManager.Instance.Set(pkt);
		}
		else
		{
			GameManager.Instance.Pkt = pkt;
			SceneManager.LoadScene(GameManager.BattleScene);
		}
	}
	
	public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
	{
		S_LeaveDungeon pkt = packet as S_LeaveDungeon;
		if (pkt == null)
			return;

		int ClassIdx = GameManager.Instance.ClassIdx - 1001;

		SceneManager.LoadScene(GameManager.TownScene);
        TownManager.Instance.uiStart.StartGame(false, ClassIdx);
    }
	
	public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
	{
		S_ScreenText pkt = packet as S_ScreenText;
		if (pkt == null)
			return;
		
		if (pkt.ScreenText != null)
		{
			var uiScreen = BattleManager.Instance.UiScreen;
			uiScreen.Set(pkt.ScreenText);
		}
	}
	
	public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
	{
		S_ScreenDone pkt = packet as S_ScreenDone;
		if (pkt == null)
			return;
		
		var uiScreen = BattleManager.Instance.UiScreen;
		uiScreen.gameObject.SetActive(false);
	}
	
	public static void S_BattleLogHandler(PacketSession session, IMessage packet)
	{
		S_BattleLog pkt = packet as S_BattleLog;
		if (pkt == null)
			return;
		
		if (pkt.BattleLog != null)
		{
			var uiBattleLog = BattleManager.Instance.UiBattleLog;
			uiBattleLog.Set(pkt.BattleLog);
		}
	}
	
	public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
	{
		S_SetPlayerHp pkt = packet as S_SetPlayerHp;
		if (pkt == null)
			return;
		
		var uiPlayer = BattleManager.Instance.UiPlayerInformation;
		uiPlayer.SetCurHP(pkt.Hp);
	}
	
	public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
	{
		S_SetPlayerMp pkt = packet as S_SetPlayerMp;
		if (pkt == null)
			return;
		
		var uiPlayer = BattleManager.Instance.UiPlayerInformation;
		uiPlayer.SetCurMP(pkt.Mp);
	}
	
	public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
	{
		S_SetMonsterHp pkt = packet as S_SetMonsterHp;
		if (pkt == null)
			return;
		
		BattleManager.Instance.SetMonsterHp(pkt.MonsterIdx, pkt.Hp);
	}
	
	public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
	{
		S_PlayerAction pkt = packet as S_PlayerAction;
		if (pkt == null)
			return;
        
        Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
		monster.Hit();

        BattleManager.Instance.PlayerAnim(pkt.ActionSet.AnimCode);
		EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
	}
	
	public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
	{
		S_MonsterAction pkt = packet as S_MonsterAction;
		if (pkt == null)
			return;
		
		Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
		monster.SetAnim(pkt.ActionSet.AnimCode);
		if (pkt.ActionSet != null && pkt.ActionSet.EffectCode != 0)
        {
            BattleManager.Instance.PlayerHit();
            EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
        }
	}

	#endregion
}

