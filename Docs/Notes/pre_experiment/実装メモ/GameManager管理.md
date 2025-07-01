# GameManager管理まとめ（2024/06/27時点）

---

## 1. GameManagerの役割

- WordCardシステム全体の「単語リスト管理」「カード生成・削除」「進行状態の同期」を一元管理する中枢クラス
- 1周目/2周目の単語リスト（各20単語）を50単語からランダム抽選し、各ロケーションに2枚ずつカードを割り当て
- LocationWarpManagerの進行状態（ラウンド・ロケーション・visitCount）を「正」として参照し、カード管理を完全同期
- 各ロケーション（location1〜location10）にアタッチされたCardSpawner[]をInspectorで配列として管理し、現在のロケーションのCardSpawnerにのみカード生成/削除を指示
- LoggerやUI、他システムとの連携のハブとしても機能

---

## 2. 構造・主要プロパティ

- `WordCardList wordCardList`：ScriptableObject。50単語＋画像のリストを保持
- `CardSpawner[] cardSpawners`：各ロケーションのCardSpawnerをInspectorで配列としてセット
- `LocationWarpManager warpManager`：進行・ワープ・ラウンド管理の本体。GameManagerはこの状態を参照
- `List<WordCardData> round1Words, round2Words`：1周目/2周目の単語リスト（ランダム抽選でセット）
- `const int LOCATIONS = 10, CARDS_PER_LOCATION = 2`：ロケーション数・1部屋あたりのカード数

---

## 3. Inspector設定手順

1. GameManagerオブジェクトをシーンに1つ配置
2. Inspectorで
   - `WordCardList`（ScriptableObjectアセット）
   - `CardSpawners`（location1〜location10の各CardSpawnerを順番に配列でセット）
   - `WarpManager`（LocationWarpManagerオブジェクト）
   を必ずセット
3. 参照が「None」や「Missing」になっていないか要確認

---

## 4. 進行・カード生成/削除の流れ

- LocationWarpManagerの進行（OnButtonPressed等）でvisitCountやCurrentStateが更新される
- GameManagerはLocationWarpManagerの状態を参照し、
  - 現在のラウンド・ロケーションに該当するCardSpawnerだけにカード生成/削除を指示
  - 他のCardSpawnerは必ずClearAllCards()でリセット
- 1周目→2周目の切り替え時も、カードの混在や消し忘れが起きない
- NextLocation()はカード生成のみ担当し、進行自体はLocationWarpManagerが管理

---

## 5. 他コンポーネントとの連携

- **LocationWarpManager**：進行状態（ラウンド・ロケーション・visitCount）の「正」。GameManagerはこの状態を参照
- **CardSpawner**：各ロケーションにアタッチ。GameManagerからSpawnCards/ClearAllCardsで制御
- **Logger**：カード配置時にCardSpawnerからLogCardPlacementを呼び出し、配置位置・タイミングを記録
- **UI（ProgressMenuUI等）**：進行表示・ボタン制御はLocationWarpManagerの状態を参照

---

## 6. 設計方針・注意点

- 進行・状態管理の「正」はLocationWarpManager。GameManagerはカード管理・単語リスト管理に特化
- CardSpawnerは必ず各ロケーションに1つずつアタッチし、Inspectorで配列順にセット
- 参照切れ・Inspector未設定があるとNullReferenceExceptionが発生するので注意
- 進行・UI・Loggerとの連携は「LocationWarpManagerの状態を参照」が原則
- 今後LoggerやUIの拡張が必要な場合も、GameManagerは「カード管理の中枢」として拡張しやすい構造

---

## 7. 今後の拡張・運用例

- Loggerの拡張（カード回収・リセット・新イベント記録など）は必要になったタイミングで追加
- UIやカードの微調整、最終テスト・デバッグも随時対応可能
- ロケーション追加・削除もCardSpawner[]の要素を増減するだけで柔軟に対応

---

**このドキュメントは2024/06/27時点のGameManagerの構造・運用・設計方針のまとめです。今後の拡張や運用時の参考にしてください。** 