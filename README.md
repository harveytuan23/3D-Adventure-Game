# 🎂 Birthday Project - 生日回憶收集遊戲

一個溫馨的 Unity 3D 互動式遊戲，讓玩家通過收集記憶碎片來重現特別的生日回憶。

## 🎮 遊戲簡介

這是一個以生日為主題的情感向互動遊戲。玩家將在一個美麗的 3D 世界中探索，與 NPC 互動，收集重要的記憶碎片，最終喚醒與「段」相關的珍貴生日回憶。

### 遊戲特色
- **溫馨的故事敘事**：包含開場、主要遊戲和結局三個完整場景
- **記憶收集機制**：需要收集 3 個重要的回憶物品
- **NPC 互動系統**：與遊戲中的角色對話獲得指引
- **美麗的視覺風格**：低多邊形美術風格，亞洲風格的環境設計
- **沉浸式音效**：豐富的背景音樂和音效系統

## 🚀 如何運行

### 系統需求
- Unity 2022.3 LTS 或更新版本
- macOS 或 Windows 作業系統
- 至少 4GB RAM
- 支援 DirectX 11 或 OpenGL 4.1 的顯示卡

### 安裝步驟
1. 克隆此專案到本地：
   ```bash
   git clone https://github.com/your-username/birthday-project.git
   cd birthday-project
   ```

2. 使用 Unity Hub 開啟專案：
   - 開啟 Unity Hub
   - 點擊 "Open" → "Add"
   - 選擇專案資料夾
   - 等待 Unity 載入專案

3. 運行遊戲：
   - 在 Unity 編輯器中，確保 `Scene_Intro` 是第一個場景
   - 點擊 Play 按鈕開始遊戲

## 🎯 遊戲玩法

### 主要目標
收集 3 個重要的記憶碎片，完成與 NPC 的互動，最終喚醒完整的生日回憶。

### 操作方式
- **移動**：WASD 鍵控制角色移動
- **互動**：E 鍵與 NPC 對話或收集物品
- **相機**：滑鼠控制視角

### 遊戲流程
1. **開場故事**：觀看溫馨的開場劇情
2. **探索世界**：在 3D 世界中自由探索
3. **收集回憶**：找到並收集 3 個重要的回憶物品
4. **與 NPC 互動**：與遊戲中的角色對話獲得指引
5. **完成任務**：將收集到的回憶交給 NPC
6. **結局**：觀看溫馨的結局故事

## 📁 專案結構

```
Assets/
├── Scripts/           # C# 腳本文件
│   ├── IntroStoryManager.cs      # 開場故事管理
│   ├── EndingStoryManager.cs     # 結局故事管理
│   ├── NPCInteraction.cs         # NPC 互動系統
│   ├── PlayerCarry.cs            # 玩家攜帶物品
│   └── ...
├── Scenes/            # 遊戲場景
│   ├── Scene_Intro.unity         # 開場場景
│   ├── Scene_MainMemory.unity    # 主要遊戲場景
│   └── Scene_Ending.unity        # 結局場景
├── Animations/        # 角色動畫
├── SoundEffects/      # 音效和音樂
├── Intro_Image/       # 開場圖片
├── EndingImage/       # 結局圖片
└── ...
```

## 🛠️ 技術細節

### 使用的技術
- **Unity 2022.3 LTS**：遊戲引擎
- **C#**：程式語言
- **TextMesh Pro**：文字渲染
- **NavMesh**：AI 導航系統
- **Animation System**：角色動畫

### 主要腳本說明
- `IntroStoryManager.cs`：管理開場故事的播放，包含打字機效果和音樂淡入淡出
- `EndingStoryManager.cs`：管理結局故事的播放
- `NPCInteraction.cs`：處理玩家與 NPC 的互動，包含對話系統和記憶收集
- `PlayerCarry.cs`：管理玩家攜帶物品的系統
- `CatAIController.cs`：貓咪 AI 行為控制

## 🎨 美術資源

### 使用的資源包
- **Low Poly Fruits**：低多邊形水果模型
- **Asian Style Trees**：亞洲風格樹木
- **PolygonStarter**：低多邊形環境資源
- **Original Wood Textures**：木質紋理

### 自製內容
- 開場和結局的圖片素材
- 遊戲音效和背景音樂
- 自定義腳本和遊戲邏輯

## 🤝 貢獻指南

歡迎貢獻這個專案！請遵循以下步驟：

1. Fork 此專案
2. 創建你的功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交你的更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟一個 Pull Request

## 📄 授權條款

此專案採用 MIT 授權條款 - 查看 [LICENSE](LICENSE) 文件了解詳情。

## 🙏 致謝

- Unity Technologies 提供的遊戲引擎
- 所有開源資源的創作者
- 特別感謝「段」帶來的靈感

## 📞 聯絡方式

如有任何問題或建議，請透過以下方式聯絡：
- 開啟 GitHub Issue
- 發送 Email 至：[your-email@example.com]

---

**注意**：這是一個個人專案，僅供學習和娛樂使用。請尊重原創內容，不要用於商業用途。 