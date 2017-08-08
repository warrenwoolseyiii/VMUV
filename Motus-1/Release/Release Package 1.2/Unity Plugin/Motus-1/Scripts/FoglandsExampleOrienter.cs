using UnityEngine;
using Motus_1_Plugin;

public class FoglandsExampleOrienter : MonoBehaviour {

    public SteamVR_TrackedController _controller;
    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.MenuButtonClicked += HandleMenuButtonClicked;
        _controller.PadClicked += HandlePadClicked;

    }

    private void OnDisable()
    {
        _controller.MenuButtonClicked -= HandleMenuButtonClicked;
        _controller.PadClicked -= HandlePadClicked;
    }

    private void HandleMenuButtonClicked(object sender, ClickedEventArgs e)
    {
        PluginInterface.orientMotus = true;
    }

    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {

    }
}
