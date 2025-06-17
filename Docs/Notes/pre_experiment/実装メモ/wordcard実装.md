# WordCard実装まとめ

## 1. 機能概要
- VRコントローラーで単語カードを掴み、好きな場所に配置できる
- 40単語＋画像のペアをリストで管理し、各部屋に2枚ずつランダム配置
- ユーザーがカードを配置した順番を記録し、データログとして保存

## 2. 単語＋画像リストの管理
```csharp
[System.Serializable]
public class WordCardData {
    public string word;
    public Sprite image;
}

// ScriptableObject例
[CreateAssetMenu(menuName="VRMoL/WordCardList")]
public class WordCardList : ScriptableObject {
    public List<WordCardData> cards;
}
```

## 3. Prefab化
- 1枚のWordCard（TextMeshPro＋Image＋コライダー＋Rigidbody＋XR Grab Interactable）をPrefab化
- スクリプトで単語・画像を差し替え

## 4. ランダム配置スクリプト例
```csharp
public class CardSpawner : MonoBehaviour {
    public GameObject cardPrefab;
    public Transform[] spawnPoints; // 部屋内の2か所
    public WordCardList wordCardList;

    void Start() {
        var selected = wordCardList.cards.OrderBy(x => Random.value).Take(2).ToArray();
        for (int i = 0; i < 2; i++) {
            var card = Instantiate(cardPrefab, spawnPoints[i].position, Quaternion.identity);
            card.GetComponentInChildren<TextMeshProUGUI>().text = selected[i].word;
            card.GetComponentInChildren<Image>().sprite = selected[i].image;
        }
    }
}
```

## 5. 掴み・離しの実装
- XR Interaction ToolkitのXR Grab Interactableを利用
- 独自拡張したい場合はCardControllerを継承

## 6. 配置順の記録とログ保存
```csharp
public class CardLogger : MonoBehaviour {
    public List<string> placedOrder = new List<string>();
    public void OnCardPlaced(string word) {
        placedOrder.Add(word);
        // 必要に応じてDataLoggerでCSV保存
    }
}
```

## 7. 今後の拡張
- 画像付きカードのInspector差し替え
- 配置位置や回収タイミングの記録
- DataLoggerとの連携によるCSV出力 