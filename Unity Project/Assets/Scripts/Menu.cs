using TestApp.Mesh;
using TestApp.Player;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour, IInitializable
{
    [SerializeField]
    private Text bulletsCount;
    [SerializeField]
    private Text nodesCount;
    [SerializeField]
    private Button reset;
    [Inject]
    private MeshGenerator meshGenerator;
    [Inject]
    private Player player;

    public void Initialize()
    {
        reset.onClick.AddListener(Reset);

        meshGenerator.SegmentsCount.SubscribeToText(nodesCount);
        player.BulletsCount.SubscribeToText(bulletsCount);
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }
}
