using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab; // カードのプレハブ
    public Transform[] spawnPoints; // 配置ポイント（2か所）
    public WordCardList wordCardList; // 単語＋画像リスト

    private int currentCardIndex = 0;
    private WordCardData[] selectedCards;
    private GameObject[] spawnedCards = new GameObject[2];
    private VRMoL.UI.Logger logger;

    private void Start()
    {
        logger = FindObjectOfType<VRMoL.UI.Logger>();
        // Start()ではカードを自動生成しない
    }

    void SpawnNextCard()
    {
        if (currentCardIndex >= spawnPoints.Length || currentCardIndex >= selectedCards.Length)
        {
            Debug.Log("全カード配置完了");
            // 必要ならここで「配置完了」コールバックやUI表示
            return;
        }
        // カード生成
        var card = Instantiate(cardPrefab, spawnPoints[currentCardIndex].position, spawnPoints[currentCardIndex].rotation);
        spawnedCards[currentCardIndex] = card;

        // すべての子孫からTextMeshProを取得
        var wordText = card.GetComponentInChildren<TextMeshPro>();
        if (wordText == null)
        {
            Debug.LogWarning("WordText (TextMeshPro) が見つかりません: " + card.name);
        }
        else
        {
            wordText.text = selectedCards[currentCardIndex].word;
            Debug.Log($"Set word: {selectedCards[currentCardIndex].word} to {wordText.name}");
        }

        // ImageQuadのMeshRendererも同様に
        var imageQuad = card.GetComponentsInChildren<MeshRenderer>()
            .FirstOrDefault(r => r.gameObject.name == "ImageQuad");
        if (imageQuad == null)
        {
            Debug.LogWarning("ImageQuad (MeshRenderer) が見つかりません: " + card.name);
        }
        else if (selectedCards[currentCardIndex].image != null)
        {
            var mat = new Material(imageQuad.sharedMaterial);
            mat.mainTexture = selectedCards[currentCardIndex].image.texture;
            imageQuad.material = mat;
        }

        // XR Grab InteractableのOnSelectExitedイベントにコールバック登録
        var grab = card.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectExited.AddListener(OnCardReleased);
        }
        else
        {
            Debug.LogWarning("XRGrabInteractableが見つかりません: " + card.name);
        }
    }

    void OnCardReleased(SelectExitEventArgs args)
    {
        // カードが離されたら次のカードを出現
        // ログ記録
        if (logger != null && currentCardIndex < selectedCards.Length && spawnedCards[currentCardIndex] != null)
        {
            var cardObj = spawnedCards[currentCardIndex];
            var cardData = selectedCards[currentCardIndex];
            var gm = VRMoL.Core.GameManager.Instance;
            int round = gm != null ? gm.GetCurrentRound() : 1;
            int locationIndex = gm != null ? gm.GetCurrentLocationIndex() : 0;
            string location = $"Location{locationIndex + 1}";
            int wordIndex = currentCardIndex;
            int orderInLocation = currentCardIndex + 1;

            logger.LogCardPlacement(
                round,
                location,
                cardData.word,
                wordIndex,
                orderInLocation,
                cardObj.transform.position,
                cardObj.transform.rotation
            );
        }
        currentCardIndex++;
        SpawnNextCard();
    }

    // 既存カードを全削除
    public void ClearAllCards()
    {
        foreach (var obj in spawnedCards)
        {
            if (obj != null) Destroy(obj);
        }
        currentCardIndex = 0;
        spawnedCards = new GameObject[2];
    }

    // 外部から単語リストを受け取ってカードを生成
    public void SpawnCards(WordCardData[] cards)
    {
        ClearAllCards();
        selectedCards = cards;
        SpawnNextCard();
    }

    // 今後、ランダム抽出や画像・単語差し替えロジックをここに追加
} 