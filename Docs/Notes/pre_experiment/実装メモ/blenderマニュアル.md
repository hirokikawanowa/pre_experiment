# Blender操作マニュアル

## 1. 基本設定

### 1.1 単位設定
- メニュー「Edit > Preferences > Scene」を開く
- Units: Metric
- Length: Meters
- Scale: 1.0
- これにより、Unityと同じ単位系（メートル）で作業可能

### 1.2 ビューポート設定
- 右クリックメニューから「Set Smooth」でスムーズシェーディング
- 表示モード切り替え：
  - タブキー：オブジェクト/エディットモード
  - Zキー：ワイヤーフレーム/ソリッド表示
  - Alt + Z：テクスチャ表示

### 1.3 ショートカット
- G: 移動
- R: 回転
- S: スケール
- Ctrl + A: 適用メニュー
- Ctrl + Z: 元に戻す
- Shift + A: 追加メニュー
- Tab: オブジェクト/エディットモード切り替え

## 2. モデリング手順

### 2.1 基本形状の作成
1. キューブを追加（Shift + A > Mesh > Cube）
2. スケールを調整（Sキー）
3. エッジを丸める（Modifier > Bevel）

### 2.2 UVマッピング
1. エディットモードに切り替え（Tab）
2. 面を選択
3. Uキーを押して「Unwrap」を選択
4. UVエディターで展開を確認
5. テクスチャを適用

### 2.3 マテリアル設定
1. マテリアルプロパティを開く
2. 「New」をクリック
3. 基本設定：
   - Base Color: テクスチャを設定
   - Metallic: 0.0
   - Roughness: 0.7
   - Normal: 必要に応じて設定

## 3. Unityへのエクスポート

### 3.1 エクスポート前の確認事項
- スケールが正しいか
- マテリアルが正しく設定されているか
- UVマッピングが正しいか
- 法線が正しい方向を向いているか

### 3.2 FBXエクスポート設定
1. File > Export > FBX
2. 以下の設定を確認：
   - Scale: 1.0
   - Apply Transform: チェック
   - Apply Modifiers: チェック
   - Include: 
     - Custom Properties
     - Armatures
     - Mesh
     - Materials
   - Geometry:
     - Smoothing: Face
     - Export Subdivision Surface: チェック

### 3.3 ファイル名と保存場所
- ファイル名：`[オブジェクト名].fbx`
- 保存場所：`Assets/VRMoL/Models/[カテゴリ]/`

## 4. Unityでのインポート設定

### 4.1 モデル設定
- Scale Factor: 1
- Import Blendshapes: チェック
- Generate Colliders: 必要に応じて
- Import Visibility: チェック
- Import Cameras: 不要
- Import Lights: 不要

### 4.2 マテリアル設定
- Location: Use External Materials
- Naming: From Model's Material
- Search: Recursive-Up

### 4.3 アニメーション設定
- Import Animation: 必要に応じて
- Import Constraints: 必要に応じて
- Import Curves: 必要に応じて

## 5. 注意点

### 5.1 モデリング時の注意
- ポリゴン数は必要最小限に
- エッジの処理は適度に
- スケールは常に確認
- 原点（Origin）の位置に注意

### 5.2 テクスチャ関連
- テクスチャは2の累乗の解像度（512x512, 1024x1024等）
- テクスチャのファイル名は英数字のみ
- テクスチャの保存形式はPNG推奨

### 5.3 エクスポート時の注意
- 不要なオブジェクトは削除
- マテリアル名は分かりやすく
- スケールは必ず1.0に
- 法線の向きを確認

## 6. トラブルシューティング

### 6.1 よくある問題
1. スケールがおかしい
   - エクスポート時のスケール設定を確認
   - 適用（Apply）を忘れていないか確認

2. テクスチャが表示されない
   - テクスチャのパスが正しいか確認
   - マテリアルの設定を確認
   - UVマッピングが正しいか確認

3. 法線が反転している
   - エディットモードで面を選択
   - Alt + Nで法線を再計算

### 6.2 パフォーマンス最適化
- 不要なポリゴンを削除
- テクスチャの解像度を適切に
- マテリアルは必要最小限に
- メッシュの最適化を検討

## 7. 参考リソース
- [Blender公式ドキュメント](https://docs.blender.org/)
- [Unity FBX Exporter](https://docs.unity3d.com/Manual/FBXExporter.html)
- [Blender to Unity workflow](https://docs.unity3d.com/Manual/Blender.html) 