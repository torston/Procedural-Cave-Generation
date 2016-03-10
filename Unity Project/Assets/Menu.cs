using TestApp.Mesh;
using TestApp.Player;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text bulletsCount;
    public Text nodesCount;

    public Button reset;

    public MeshGenerator meshGenerator;
    public Player player;

    private void Awake()
    {
        reset.onClick.AddListener(Reset);
    }

    private void Start()
    {
        meshGenerator.SegmentsCount.SubscribeToText(nodesCount);
        player.BulletsCount.SubscribeToText(bulletsCount);
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }
}
