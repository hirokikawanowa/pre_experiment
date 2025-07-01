# Audio実装まとめ（2024/06/27）

---

## ロケーション別 環境音一覧

| ロケーション | テーマ     | 環境音                 |
|:------------:|:----------|:----------------------|
| 01           | 和室      | カフェのざわめき       |
| 02           | 玄関      | 森の小鳥のさえずり     |
| 03           | 廊下      | 電車の走行音           |
| 04           | リビング  | 波の音                 |
| 05           | キッチン  | オフィスの環境音       |
| 06           | 洗面所    | ジャズの演奏           |
| 07           | ベランダ  | 地下鉄の駅の音         |
| 08           | 書斎      | 落ち葉を歩く音      |
| 09           | 子供部屋  | オーケストラの演奏     |
| 10           | 自室      | 花火の音|

---

## 1. 目的・全体方針
- 各ロケーション（部屋）ごとに異なる環境音（1分ループ・-28LUFS）を再生
- 空間オーディオ（HRTF/Steam Audio等）と非空間オーディオを切り替え可能
- 1周目・2周目で空間/非空間オーディオを切り替える（例：1周目=非空間、2周目=空間）
- ロケーション移動時に音声も自動で切り替え
- ユーザーの頭部周囲に音源を配置し、HRTF的な定位感を実現

---

## 2. システム設計・構成

### 2.1 AudioSourceの配置
- 各ロケーションの中心や適切な位置にAudioSourceを設置
- AudioClip（1分ループ環境音）を設定
- 音量は-28LUFS相当に調整
- spatialBlend=1.0（空間）/0.0（非空間）で切り替え
- Steam AudioやOculus Spatializer等のHRTFプラグインを有効化

### 2.2 AudioSettingsクラス例
```csharp
[System.Serializable]
public class AudioSettings {
    public bool isSpatial = true;
    public float spatialBlend = 1.0f;
    public float minDistance = 1.0f;
    public float maxDistance = 10.0f;
    public AnimationCurve falloffCurve;
}
```
- Inspectorから各種パラメータを調整可能に

### 2.3 AudioControllerの設計例
- 各ロケーションのAudioSourceを配列で管理
- 現在のロケーションのみ再生、他は停止
- 空間/非空間切り替えはAudioSourceのspatialBlendを一括変更
- 1周目Finish時に全AudioSourceのspatialBlendを切り替え

```csharp
public class AudioController : MonoBehaviour {
    public AudioSource[] locationAudioSources;
    public AudioSettings audioSettings;
    public int currentLocation = 0;

    public void PlayLocationAudio(int locationIndex) {
        for (int i = 0; i < locationAudioSources.Length; i++) {
            if (i == locationIndex) {
                locationAudioSources[i].spatialBlend = audioSettings.spatialBlend;
                locationAudioSources[i].Play();
            } else {
                locationAudioSources[i].Stop();
            }
        }
        currentLocation = locationIndex;
    }

    public void SetSpatialAudio(bool isSpatial) {
        audioSettings.isSpatial = isSpatial;
        audioSettings.spatialBlend = isSpatial ? 1.0f : 0.0f;
        foreach (var src in locationAudioSources) {
            src.spatialBlend = audioSettings.spatialBlend;
        }
    }
}
```

---

## 3. 実装手順

1. **各ロケーションにAudioSourceを設置**
    - AudioClip（環境音）をセット、ループON、音量調整
    - spatialBlend=1.0（空間）で初期化
2. **AudioControllerスクリプトを作成・配置**
    - locationAudioSources配列に各部屋のAudioSourceを登録
    - AudioSettingsをInspectorで調整
3. **ロケーション移動時の音声切り替え**
    - LocationWarpManagerやGameManagerからPlayLocationAudio(新index)を呼ぶ
    - 前の部屋の音声はStop、新しい部屋の音声をPlay
4. **空間/非空間オーディオの切り替え**
    - 1周目Finish時にSetSpatialAudio(false→true)などで一括切り替え
    - 必要に応じてUIやボタンで切り替えも可能
5. **HRTF/Steam Audioの有効化**
    - ProjectSettings→AudioでSpatializer PluginをSteam Audio等に設定
    - 各AudioSourceのSpatializeチェックON
    - 必要に応じてSteam Audio Sourceコンポーネントを追加
6. **テスト・デバッグ**
    - ロケーション間の音声干渉がないか確認
    - 空間/非空間切り替え・定位・減衰が意図通りか確認

---

## 4. 注意点・運用ガイド
- AudioSourceのSpatializeチェックを忘れずON
- Steam Audio等のプラグインは必ず有効化
- InspectorでAudioSource/AudioSettingsの参照切れに注意
- 1周目/2周目の切り替えはGameManagerやLocationWarpManagerから明示的に呼ぶ
- 音量・定位・減衰カーブは実機（Quest 3）で必ずテスト
- 音声ファイルは-28LUFSで正規化

---

## 5. サンプル運用フロー
1. シーン開始時：各部屋のAudioSourceはStop、現在地のみPlay
2. ワープ時：PlayLocationAudio(新index)で音声切り替え
3. 1周目Finish時：SetSpatialAudio(true)で空間オーディオに一括切り替え
4. 2周目も同様にロケーション移動ごとに音声切り替え

---

## 6. 参考・関連スクリプト
- AudioController.cs
- LocationWarpManager.cs
- GameManager.cs
- AudioSettingsクラス

---

## 7. 環境音（Ambient Sound）実装・運用の詳細

### 7.1 環境音の仕様・設計方針
- 各ロケーションごとに異なる環境音（Ambient Sound）を用意
- 例：和室=カフェのざわめき、玄関=森の小鳥のさえずり、廊下=電車の走行音 など
- 環境音は1分程度のループ音声（AudioClip.loop=true）
- 音声ファイルはWAV 48kHz/16bit、モノラル推奨
- 音量は-28 LUFSで正規化
- 各ロケーションの中心や適切な位置にAudioSourceを設置し、環境音を再生
- AudioSourceのspatialBlendで空間/非空間を切り替え
- AddressablesやInspectorでAudioClipを差し替え可能にしておくと運用が楽

### 7.2 環境音のカスタマイズ・管理
- LocationTemplateやScriptableObjectで各ロケーションの環境音を管理
- 例：
```csharp
[CreateAssetMenu(menuName="VRMoL/LocationTemplate")]
public class LocationTemplate : ScriptableObject {
    public string locationName;
    public AudioClip ambientSound;
    // ...他の設定...
}
```
- AudioControllerやGameManagerで、現在のロケーションに応じてambientSoundを切り替え

### 7.3 音漏れ・干渉防止の工夫
- 各部屋の壁・ドア・距離を十分に取り、隣接部屋の音が混ざらないように設計
- AudioSourceのminDistance/maxDistanceや減衰カーブを調整し、部屋外での音量を極力小さく
- 必要に応じてAudioMixerでロケーションごとにグループ分けし、ミュート/フェード制御も可能

### 7.4 運用・テストのポイント
- 実機（Quest 3）で環境音の定位・音量・音漏れを必ず確認
- ループ音声のつなぎ目が不自然でないかチェック
- 各ロケーションの雰囲気に合った環境音を選定
- 音声ファイルのLUFS・フォーマット・長さを統一

---

**このドキュメントは2024/06/27時点のAudio実装・設計・運用のまとめです。今後の拡張や運用時の参考にしてください。** 