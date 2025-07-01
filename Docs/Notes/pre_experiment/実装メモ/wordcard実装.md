# WordCard実装まとめ

---

## 全体実装順序（改訂版）

1. **単語候補リストの管理**
   - 単語候補リスト（40語以上）をScriptableObjectに登録
   - 1周目・2周目で20単語ずつランダム抽出するロジックの設計

2. **WordCardListの作成・セットアップ**
   - ScriptableObjectで40単語＋画像を管理
   - 画像（Sprite）とマテリアルの準備・登録方法の指示

3. **WordCardプレハブの作成**
   - TextMeshPro＋Image＋コライダー＋Rigidbody＋XR Grab Interactable
   - 画像・マテリアルのアタッチ方法

4. **CardSpawnerの実装・配置**
   - 各ロケーションに2枚ずつカードをランダム配置
   - 1周目・2周目の切り替え制御

5. **CardControllerの実装（必要に応じて）**
   - 掴み・離しのイベントフック

6. **CardLoggerの実装**
   - 配置順・配置位置・回収タイミングの記録

7. **DataLoggerとの連携（CSV保存）**

8. **LocationManager/ワープシステムとの動作確認・調整**

9. **UI/進行状況表示の調整（必要なら）**

---

## 各フェーズでの指示内容
- コードはそのフェーズで必要なものだけ提示
- Unityエディタでの操作（画像・マテリアルの登録やPrefab作成など）も具体的に指示
- 画像を載せる場合は「Sprite（イメージ）」と「マテリアル」の両方の準備・設定方法を明記

---

## 方針・整合性
- 単語候補リストから1周目20単語、2周目20単語をランダム抽出し、合計40単語を使用
- 40単語＋画像ペアをScriptableObjectで一元管理
- 各ロケーション（10部屋）に2枚ずつランダム配置（合計20枚/回）
- Prefab化されたWordCard（TextMeshPro＋Image＋コライダー＋Rigidbody＋XR Grab Interactable）
- CardSpawnerが各部屋に2枚ずつカードを配置
- XR Interaction Toolkitで掴み・離し
- 配置順や配置位置をCardLoggerで記録し、DataLoggerと連携してCSV保存
- ワープ順（visitOrder）は既に固定されているため、LoggerやSpawnerはこの順に従う
- LocationManager/ワープシステムとの連携を意識する

---

## やるべきこと（タスク分解）
1. 単語候補リストの管理
2. WordCardListの作成・セットアップ（画像・マテリアルの準備）
3. WordCardプレハブの作成
4. CardSpawnerの実装・配置
5. CardControllerの実装（必要に応じて）
6. CardLoggerの実装
7. DataLoggerとの連携
8. LocationManager/ワープシステムとの連携
9. UI/進行状況表示の調整

---

## 実装順序（推奨）
1. 単語候補リストの管理
2. WordCardListの作成・セットアップ
3. WordCardプレハブの作成
4. CardSpawnerの実装・配置
5. CardControllerの実装
6. CardLoggerの実装
7. DataLoggerとの連携
8. LocationManager/ワープシステムとの動作確認・調整
9. UI/進行状況表示の調整

---

## 整合性チェックポイント
- ワープ順（visitOrder）は既に固定されている
- 各ロケーションにCardSpawnerを設置
- カード配置・記録のタイミング（ワープ直後にカードが正しく配置されること）
- データ記録の一貫性（CardLogger→DataLoggerの流れでCSV保存）

---

## 次のアクション
まずは「単語候補リストの管理」から着手。
準備ができたら「次へ」と伝える。
その後、WordCardListの作成・セットアップ（画像・マテリアルの準備と登録方法）の指示に進む。

---

## 今後の拡張案
- 画像付きカードのInspector差し替え
- 配置位置や回収タイミングの記録
- DataLoggerとの連携によるCSV出力

## 単語候補リスト

- Bolt
- Roast
- Ankle
- Canal
- Feast
- Gate
- Crowd
- Burial
- Riot
- Helmet
- Rose
- Hammer
- Drive
- Stable
- Beard
- Chapel
- Cave
- Twin
- Toilet
- Tank
- Museum
- Cigar
- Scarf
- Sponge
- Cart
- Barrel
- Basket
- Lace
- Tail
- Onion
- Flame
- Drum
- Deer
- Infant
- Meal
- Salt
- Tongue
- Button
- Card
- Crest
- Autumn
- Disc
- Wound
- Beam
- Bone
- Devil
- Essay
- Cherry
- Ladies 
- Apple