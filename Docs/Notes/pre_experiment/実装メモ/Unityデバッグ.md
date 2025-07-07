# Meta Quest 3 で Unityアプリのログを取得する方法（Windows）

---

## 1. 準備

- Google公式から [platform-tools](https://developer.android.com/studio/releases/platform-tools) をダウンロード＆解凍
- Quest 3 で「開発者モード」を有効化し、USBデバッグを許可
- データ転送対応USBケーブルでPCと接続

---

## 2. コマンドプロンプトで platform-tools フォルダに移動

```sh
cd C:\Users\（あなたのユーザー名）\Downloads\platform-tools
```
※「platform-tools」を解凍した場所に合わせてパスを変更してください

---

## 3. adbコマンドでUnityログだけをリアルタイム表示

### その1: Unityログだけを表示する
```sh
adb logcat -s Unity
```

### その2: 全ログからUnity関連の行だけを抽出（Windows限定）
```sh
adb logcat | findstr Unity
```

---

## 4. （応用）アプリのパッケージ名でプロセスIDを調べて、そのアプリのログだけ見る

1. プロセスID（PID）を調べる
```sh
adb shell pidof com.あなたのアプリのパッケージ名
```

2. そのPIDのログだけを表示
```sh
adb logcat --pid=ここにPID番号
```

---

## 5. 補足・参考

- UnityエンジンのDebug.Log/Warning/Errorは全て「Unity」タグで出力されます。
- ログの確認には `adb logcat -s Unity` が最も簡単です。
- パッケージ名の調べ方：
  - Unityエディタの「Build Settings > Player Settings > Other Settings > Package Name」
  - またはQuest内の「設定 > アプリ > アプリ情報」などで確認可能

---

必要に応じて追加情報（パッケージ名の調べ方、ログの保存方法など）も追記できます！ 